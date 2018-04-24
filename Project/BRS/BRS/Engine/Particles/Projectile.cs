//-----------------------------------------------------------------------------
// Projectile.cs by alex
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
// Managed for this project by Alexander Lelidis and Andreas Emch

using Microsoft.Xna.Framework;

namespace BRS.Engine.Particles {
    /// <summary>
    /// This class demonstrates how to combine several different particle systems
    /// to build up a more sophisticated composite effect. It implements a rocket
    /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
    /// steady stream of trail particles behind it. After a while it explodes,
    /// creating a sudden burst of explosion and smoke particles.
    /// </summary>
    class Projectile {

        #region Properties and attributes

        private readonly Emitter _trailEmitter;

        public bool IsEmitting {
            get => _trailEmitter.IsEmitting;
            set => _trailEmitter.IsEmitting = value;
        }

        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Projectile(ParticleSystem3D projectileTrailParticles, Vector3 position, float trailParticlesPerSecond = 200) {
            // Use the particle emitter helper to output our trail particles.
            _trailEmitter = new Emitter(projectileTrailParticles, trailParticlesPerSecond, position);
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public void Update(Vector3 position) {
            // Update the particle emitter, which will create our particle trail.
            _trailEmitter.Update(position);
        }
    }
}
