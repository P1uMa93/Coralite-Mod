﻿using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class VampiresFang : BaseFlyingShieldItem<VampiresFangGuard>
    {
        public VampiresFang() : base(Item.sellPrice(0, 3, 50), ItemRarityID.Lime, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<VampiresFangProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 17;
            Item.damage = 37;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.MaxFlyingShield++;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BatfangShield>()
                .AddIngredient(ItemID.BrokenBatWing, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class VampiresFangProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "VampiresFang";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 44;
        }

        public override void SetOtherValues()
        {
            flyingTime = 20;
            backTime = 22;
            backSpeed = 18;
            trailCachesLength = 9;
            trailWidth = 16 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.moonLeech && !target.immortal && State == (int)FlyingShieldStates.Shooting)
            {
                float num = damageDone * 0.035f;
                if ((int)num != 0 && !(Owner.lifeSteal <= 0f))
                {
                    Owner.lifeSteal -= num * 1.5f;
                    int num2 = Projectile.owner;
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, 305, 0, 0f, Projectile.owner, num2, num);
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override Color GetColor(float factor)
        {
            return Color.Red * factor;
        }
    }

    public class VampiresFangGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "VampiresFang";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 55;
            Projectile.height = 44;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.05f;
            scalePercent = 1.4f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Fleshy_NPCHit1, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2;
        }
    }
}
