﻿using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyItem : ModItem, IFairyItem
    {
        /// <summary>
        /// 仙灵的个体数据，用于存放各类增幅
        /// </summary>
        protected FairyData fairyData;

        /// <summary>
        /// 仙灵的实际血量
        /// </summary>
        public int life;
        /// <summary>
        /// 仙灵是否存活
        /// </summary>
        public bool dead;

        public abstract int GetFairyType();

        /// <summary>
        /// 受到个体值加成过的仙灵自身的伤害
        /// </summary>
        public float FairyDamage => fairyData.damageBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseDamage);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的大小
        /// </summary>
        public float FairyScale
        {
            get
            {
                float scale = fairyData.scaleBonus * Item.GetGlobalItem<FairyGlobalItem>().baseScale;
                scale = Math.Clamp(scale, 0.5f, 2.5f);
                return scale;
            }
        }
        /// <summary>
        /// 受到个体值加成过的仙灵自身的防御
        /// </summary>
        public float FairyDefence => fairyData.defenceBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseDefence);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的生命值上限
        /// </summary>
        public float FairyLifeMax => fairyData.lifeMaxBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseLifeMax);

        public FairyData IV  { get => fairyData; set => fairyData = value; }
        public bool IsDead => dead;

        public virtual void Hurt()
        {

        }

        /// <summary>
        /// 将生命值限制在0-最大值之间
        /// </summary>
        public void LimitLife()
        {
            life = Math.Clamp(life, 0, (int)FairyLifeMax);
        }

        /// <summary>
        /// 将仙灵发射出去
        /// </summary>
        /// <returns></returns>
        public virtual bool ShootFairy(Vector2 position, Vector2 velocity)
        {
            if (dead)
                return false;

            //生成仙灵弹幕

            //将弹幕的item赋值为自身

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
        }

        public void GetCurrentLife()
        {
        }

        public virtual void ShootFairy()
        {
        }
    }

    public interface IFairyItem
    {
        public bool IsDead { get; }
        public void GetCurrentLife();
        public void Hurt();

        public FairyData IV { get; set; }

        public void ShootFairy();
    }
}
