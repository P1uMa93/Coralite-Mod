﻿using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeLinerSender : Component
    {
        public override int ID => MagikeComponentID.MagikeSender;

        /// <summary> 基础连接数量 </summary>
        public int MaxConnectBase { get; private set; }
        /// <summary> 额外连接数量 </summary>
        public int MaxConnectExtra { get; set; }

        /// <summary> 可连接数量 </summary>
        public int MaxConnect { get => MaxConnectBase + MaxConnectExtra; }

        /// <summary> 基础连接距离 </summary>
        public int ConnectLengthBase { get; private set; }
        /// <summary> 额外连接距离 </summary>
        public int ConnectLengthExtra { get; set; }

        /// <summary> 连接距离 </summary>
        public int ConnectLength { get => ConnectLengthBase + ConnectLengthExtra; }

        /// <summary> 基础单次发送量 </summary>
        public int UnitDeliveryBase { get; private set; }
        /// <summary> 额外单次发送量 </summary>
        public int UnitDeliveryExtra { get; set; }

        /// <summary> 单次发送量 </summary>
        public int UnitDelivery { get => UnitDeliveryBase + UnitDeliveryExtra; }

        /// <summary> 基础发送时间 </summary>
        public int SendDelayBase { get; private set; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float SendDelayBonus { get; set; } = 1f;

        /// <summary> 发送时间 </summary>
        public int SendDelay { get => Math.Clamp((int)(SendDelayBase * SendDelayBonus), 1, int.MaxValue); }

        /// <summary> 当前连接者 </summary>
        public int CurrentConnector => Receivers.Count;

        /// <summary> 发送魔能的计时器 </summary>
        private int _sendTimer;

        public List<Point16> Receivers = new List<Point16>();

        public MagikeLinerSender()
        {
            Receivers = new List<Point16>(MaxConnect);
        }

        #region 发送工作相关

        public override void Update(IEntity entity)
        {
            //发送时间限制
            if (!CanSend())
                return;

            //获取魔能容器并检测能否发送魔能
            MagikeContainer container = Entity.GetMagikeContainer();
            if (!GetSendAmount(container, out int amount))
                return;

            //直接发送
            for (int i = 0; i < Receivers.Count; i++)
            {
                if (!container.HasMagike)//自身没魔能了就跳出
                    break;
                Send(container, Receivers[i], amount);
            }
        }

        public bool CanSend()
        {
            _sendTimer--;
            if (_sendTimer == 0)
            {
                _sendTimer = SendDelay;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取具体发送多少，最少为剩余量除以所有连接数量
        /// </summary>
        /// <param name="container"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool GetSendAmount(MagikeContainer container, out int amount)
        {
            amount = 0;
            //没有魔能直接返回
            if (!container.HasMagike)
                return false;

            int currentMagike = container.Magike;

            //设置初始发送量
            amount = UnitDelivery;

            //如果魔能量不够挨个发一份那么就把当前剩余的魔能挨个发一份
            if (currentMagike < amount * CurrentConnector)
                amount = currentMagike / CurrentConnector;

            //防止小于1
            if (amount < 1)
                amount = 1;

            return true;
        }

        /// <summary>
        /// 发送魔能
        /// </summary>
        public void Send(MagikeContainer selfMagikeContainer, Point16 position, int amount)
        {
            //如果无法获取物块实体就移除
            if (MagikeHelper.TryGetEntity(position, out IEntity receiverEntity))
                goto remove;

            //如果不是魔能容器那么就丢掉喽
            if (!receiverEntity.IsMagikeContainer())
                goto remove;

            /*
             * 对于传入的魔能数量，分为以下几种情况
             *  - 接收者能接受，就全额发送
             *  - 接收者无法接受全部，就发送接收者能接受的数量
             */

            MagikeContainer receiver = receiverEntity.GetMagikeContainer();
            if (receiver.FullMagike)
                return;

            //限制不溢出
            receiver.LimitReceiveOverflow(ref amount);

#pragma warning disable IDE0059 // 不需要赋值
            receiver += amount;
            selfMagikeContainer -= amount;
#pragma warning restore IDE0059 // 不需要赋值

            return;
        remove:
            RemoveReceiver(position);
        }

        #endregion

        #region 连接相关

        public void Connect(Point16 receiverPoint)
        {
            Receivers.Add(receiverPoint);
        }

        /// <summary>
        /// 移除接收者
        /// </summary>
        /// <param name="receiverPoint"></param>
        public void RemoveReceiver(Point16 receiverPoint) => Receivers.Remove(receiverPoint);

        #endregion

        /// <summary>
        /// 获取魔能量，一定得有唯一的魔能容器才行，没有的话我给你一拳
        /// </summary>
        /// <returns></returns>
        public int GetMagikeAmount()
            => Entity.GetMagikeContainer().Magike;

        /// <summary>
        /// 是否已经装满
        /// </summary>
        /// <returns></returns>
        public bool FillUp() => MaxConnect == Receivers.Count;

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(MaxConnectBase), MaxConnectBase);
            tag.Add(preName + nameof(MaxConnectExtra), MaxConnectExtra);

            tag.Add(preName + nameof(ConnectLengthBase), ConnectLengthBase);
            tag.Add(preName + nameof(ConnectLengthExtra), ConnectLengthExtra);

            tag.Add(preName + nameof(UnitDeliveryBase), UnitDeliveryBase);
            tag.Add(preName + nameof(UnitDeliveryExtra), UnitDeliveryExtra);

            tag.Add(preName + nameof(SendDelayBase), SendDelayBase);
            tag.Add(preName + nameof(SendDelayBonus), SendDelayBonus);

            for (int i = 0; i < Receivers.Count; i++)
                tag.Add(preName + nameof(Receivers) + i, Receivers[i]);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            MaxConnectBase = tag.GetInt(preName + nameof(MaxConnectBase));
            MaxConnectExtra = tag.GetInt(preName + nameof(MaxConnectExtra));

            ConnectLengthBase = tag.GetInt(preName + nameof(ConnectLengthBase));
            ConnectLengthExtra = tag.GetInt(preName + nameof(ConnectLengthExtra));

            UnitDeliveryBase = tag.GetInt(preName + nameof(UnitDeliveryBase));
            UnitDeliveryExtra = tag.GetInt(preName + nameof(UnitDeliveryExtra));

            SendDelayBase = tag.GetInt(preName + nameof(SendDelayBase));
            SendDelayBonus = tag.GetFloat(preName + nameof(SendDelayBonus));

            int i = 0;

            while (tag.TryGet(preName + nameof(Receivers) + i, out Point16 position))
            {
                Receivers.Add(position);
                i++;
            }
        }
    }
}
