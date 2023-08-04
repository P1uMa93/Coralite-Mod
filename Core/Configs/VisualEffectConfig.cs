﻿using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Coralite.Core.Configs
{
    public class VisualEffectConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("HitEffects")]
        [SeparatePage]
        [DefaultValue(true)] 
        public bool HitEffect_Dusts;

        [DefaultValue(true)]
        public bool HitEffect_ScreenShaking;

        [DefaultValue(true)]
        public bool HitEffect_Lightning;

        [DefaultValue(true)]
        public bool HitEffect_SpecialParticles;

        [DefaultValue(true)]
        public bool HitEffect_HitFreeze;

        [Header("OtherVisualEffects")]
        [SeparatePage]
        [DefaultValue(true)]
        public bool DrawKniefLight;

        [DefaultValue(true)]
        public bool DrawWarp;

        [DefaultValue(true)]
        public bool DrawTrail;

        public override void OnChanged()
        {
            SetValues();
        }

        public override void OnLoaded()
        {
            SetValues();
        }

        public void SetValues()
        {
            VisualEffectSystem.HitEffect_Dusts = HitEffect_Dusts;
            VisualEffectSystem.HitEffect_ScreenShaking = HitEffect_ScreenShaking;
            VisualEffectSystem.HitEffect_Lightning = HitEffect_Lightning;
            VisualEffectSystem.HitEffect_SpecialParticles = HitEffect_SpecialParticles;
            VisualEffectSystem.HitEffect_HitFreeze = HitEffect_HitFreeze;
            VisualEffectSystem.DrawKniefLight = DrawKniefLight;
            VisualEffectSystem.DrawWarp = DrawWarp;
            VisualEffectSystem.DrawTrail = DrawTrail;
        }
    }

    public class VisualEffectSystem
    {
        public static bool HitEffect_Dusts=true;
        public static bool HitEffect_ScreenShaking = true;
        public static bool HitEffect_Lightning = true;
        public static bool HitEffect_SpecialParticles = true;

        public static bool HitEffect_HitFreeze = true;
        public static bool DrawKniefLight = true;
        public static bool DrawWarp = true;
        public static bool DrawTrail=true;
    }
}
