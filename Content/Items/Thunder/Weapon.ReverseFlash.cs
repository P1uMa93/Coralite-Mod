﻿using Coralite.Content.Items.ThyphionSeries;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using rail;
using System;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ReverseFlash : ModItem,IDashable
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(68, 7f, 10);
            Item.DefaultToRangedWeapon(ProjectileType<ReverseFlashProj>(), AmmoID.Arrow
                , 22, 15f,true);

            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 2, 50);

            Item.noUseGraphic = true;
            Item.useTurn = false;

            Item.UseSound = CoraliteSoundID.Bow2_Item102;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.Zero) * 16, ProjectileType<ReverseFlashProj>(),
                damage, knockback, player.whoAmI);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(3)
                .AddIngredient<ElectrificationWing>(2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 48;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 3;
            Player.AddImmuneTime(ImmunityCooldownID.General, 5);

            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<ReverseFlashHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕

                int damage = Player.GetWeaponDamage(Player.HeldItem);

                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                   damage, Player.HeldItem.knockBack, Player.whoAmI, 10, DashDir>0?0:3.141f, ai2: 5);

                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ReverseFlashHeldProj>(),
                        damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 20);
            }

            return true;
        }
    }

    public class ReverseFlashHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThunderItems + "ReverseFlash";

        public ref float Timer => ref Projectile.localAI[1];

        public override int GetItemType()
            => ItemType<ReverseFlash>();

        public override Vector2 GetOffset()
            => new(4, 0);

        public override void DashAttackAI()
        {
            if (Timer < DashTime)
                Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * 32;
            else if (Timer == DashTime)
                Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * 2;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1
                , OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            return false;
        }
    }

    public class ReverseFlashProj : ModProjectile
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderProj";

        public ref float State => ref Projectile.ai[0];
        public Vector2 TargetPos
        {
            get
            {
                return new Vector2(Projectile.ai[1], Projectile.ai[2]);
            }
            set
            {
                Projectile.ai[1] = value.X;
                Projectile.ai[2] = value.Y;
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 3;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0:
                    if (Projectile.localAI[0] == 0)//直接飞速运动命中
                    {
                        Projectile.timeLeft = 100;
                        Projectile.extraUpdates = 100;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1://返回
                    {
                        if (Projectile.localAI[0] == 0)//直接飞速运动命中
                        {
                            Projectile.friendly = true;
                            TargetPos = Main.player[Projectile.owner].Center;
                            Projectile.localAI[0] = 1;
                        }

                        if (Vector2.Distance(Projectile.Center, TargetPos) < 32)
                        {
                            Projectile.Kill();
                        }

                        Projectile.localAI[1]++;
                        if (Projectile.localAI[1] > 10)
                        {
                            Projectile.velocity = (TargetPos - Projectile.Center).SafeNormalize(Vector2.Zero)
                                .RotateByRandom(-0.2f, 0.2f) * Projectile.velocity.Length();
                            Projectile.localAI[1] = 0;

                            for (int i = 0; i < 3; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Helper.NextVec2Dir(0.5f, 4),
                                     newColor: Coralite.ThunderveinYellow);
                                d.noGravity = true;
                            }
                        }

                        Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && State == 0)
            {
                Projectile.NewProjectileFromThis<ReverseFlashProj>(Projectile.Center,
                    (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16, Projectile.damage, Projectile.knockBack, 1);
            }

            if (State != 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.oldPos[i] + (new Vector2(Projectile.width, Projectile.height) / 2), DustID.PortalBoltTrail, Projectile.velocity.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(0.2f, 0.6f)
                        , newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 0)
                return false;

            Projectile.DrawShadowTrails(Color.White, 0.8f, 0.8f / 10, 0, 10, 1);
            Projectile.QuickDraw(Color.White, 0);

            return false;
        }
    }
}
