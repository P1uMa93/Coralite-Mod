﻿using InnoVault.Trails;

namespace Coralite.Core.Systems.ParticleSystem
{
    public abstract class TrailParticle : Particle, IDrawParticlePrimitive
    {
        public Trail trail;

        public virtual void DrawPrimitive() { }
    }
}
