﻿using Terraria;
using Terraria.Localization;

namespace Coralite
{
    /// <summary>
    /// 用于处理各类“重要事项”，比如版本更新时的进入世界跳字等<br></br>
    /// 在不同版本此类中的内容可能有较大程度的变化
    /// </summary>
    public class CoralteSystem : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "Important";

        //v0.2.1 用于提醒玩家魔能的故障
        public static LocalizedText OnEnterWorld { get; set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                OnEnterWorld = this.GetLocalization(nameof(OnEnterWorld));
            }
        }

        public override void Unload()
        {
            OnEnterWorld = null;
        }
    }
}
