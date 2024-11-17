using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Tornado : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + "Tornado2";
        //public override bool ShouldUpdatePosition() => false;

        private float time;

        public override void SetProperty()
        {
            Frame = new Rectangle(0, Main.rand.Next(8) * 64, 128, 64);
        }

        public override void AI()
        {
            if (fadeIn % 4 == 0)
            {
                Frame.Y += 64;
                if (Frame.Y > 448)
                    Frame.Y = 0;
            }

            if (fadeIn > time)
                Scale *= 1.09f;
            else
                Scale *= 0.975f;

            if (fadeIn < 20)
                Color *= 0.92f;

            fadeIn--;
            if (fadeIn < 0)
                active = false;
        }


        public static void Spawn(Vector2 center, Vector2 velocity, Color color, float fadeIn, float rotation, float scale = 1f)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            Tornado particle = PRTLoader.NewParticle<Tornado>(center, velocity, color, scale);
            if (particle != null)
            {
                particle.fadeIn = fadeIn;
                particle.Rotation = rotation + 1.57f;
                particle.time = fadeIn - 10f;
            }
        }
    }
}