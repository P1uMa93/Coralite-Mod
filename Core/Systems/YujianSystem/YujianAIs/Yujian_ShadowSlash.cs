﻿using Coralite.Content.Particles;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class Yujian_ShadowSlash : Yujian_BaseSlash
    {
        private bool canSlash = false;

        private Trail trail;

        public Yujian_ShadowSlash(int startTime, int slashWidth, int slashTime, float startAngle, float totalAngle, float turnSpeed, float roughlyVelocity, float halfShortAxis, float halfLongAxis, ISmoother smoother) : base(startTime, slashWidth, slashTime, startAngle, totalAngle, turnSpeed, roughlyVelocity, halfShortAxis, halfLongAxis, smoother)
        {
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            //先尝试接近NPC，距离小于一定值后开始斩击
            if (canSlash)
            {
                //斩击AI
                int time = StartTime - (int)yujianProj.Timer;
                if (time < 3)
                    yujianProj.InitTrailCaches();

                if (time < SlashTime)
                {
                    Slash(Projectile, time);
                    return;
                }

                if (time == SlashTime)
                {
                    canDamage = false;
                    AfterSlash(Projectile);
                }

                return;
            }

            TryGetClosed2Target(yujianProj, out float distance, out _);

            if (distance < SlashWidth * 2 && distance > SlashWidth * 1.8f)      //瞬移并生成2次粒子
            {
                canSlash = true;
                canDamage = true;
                SpawnShadowDust(Projectile);

                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetDir = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.Center = targetCenter + targetDir * distance;
                Projectile.rotation = targetDir.ToRotation() + 1.57f;

                Vector2 slashCenter = Projectile.Center - (Projectile.rotation - 1.57f).ToRotationVector2() * SlashWidth;
                Projectile.localAI[0] = slashCenter.X;
                Projectile.localAI[1] = slashCenter.Y;
                targetDir = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
                targetRotation = targetDir.ToRotation();

                SpawnShadowDust(Projectile);

                yujianProj.InitTrailCaches();
                trail?.SetVertical(StartAngle < 0);      //开始角度为正时设为false
            }

        }

        private void SpawnShadowDust(Projectile Projectile)
        {
            float r = 0f;
            for (int i = 0; i < 14; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Granite, r.ToRotationVector2() * Main.rand.NextFloat(1.6f, 2.5f), default, default, Main.rand.NextFloat(1.4f, 2f));
                dust.noGravity = true;
                r += 0.45f;
            }

            Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<HorizontalStar>(), Color.Purple, 0.2f);
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            //StartAngle = -StartAngle;
            yujianProj.Projectile.velocity += (yujianProj.Projectile.rotation - 1.57f).ToRotationVector2() * 0.02f;
            canDamage = false;
            canSlash = false;
        }

        protected override bool UpdateTime(BaseYujianProj yujianProj)
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, yujianProj.Projectile.oldPos.Length, new NoTip(), factor => yujianProj.Projectile.height / 2,
            factor =>
            {
                return Color.Lerp(yujianProj.color1, yujianProj.color2, factor.X) * 0.8f;
            },
            flipVertical: StartAngle < 0
            );

            trail.Positions = yujianProj.Projectile.oldPos;
            return canSlash;
        }

        public override void DrawPrimitives(BaseYujianProj yujianProj)
        {
            int time = StartTime - (int)yujianProj.Timer;
            if (!canSlash || time > SlashTime || time < yujianProj.trailCacheLength || smoother.Smoother(time, SlashTime) > 0.99f)
                return;

            Effect effect = Filters.Scene["SimpleTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(yujianProj.SlashTexture).Value);

            trail?.Render(effect);
        }

    }
}
