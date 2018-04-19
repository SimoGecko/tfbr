#region File Description
//-----------------------------------------------------------------------------
// Projectile.cs by alex
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace BRS.Engine.Particles
{
    /// <summary>
    /// This class demonstrates how to combine several different particle systems
    /// to build up a more sophisticated composite effect. It implements a rocket
    /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
    /// steady stream of trail particles behind it. After a while it explodes,
    /// creating a sudden burst of explosion and smoke particles.
    /// </summary>
    class Projectile
    {
        #region Constants

        public float TrailParticlesPerSecond = 200;
   
        #endregion

        #region Fields

        ParticleEmitter trailEmitter;

        #endregion


        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Projectile(ParticleSystem3d projectileTrailParticles, Vector3 position, float TrailParticlesPerSecond)
        {
            // Use the particle emitter helper to output our trail particles.
            this.TrailParticlesPerSecond = TrailParticlesPerSecond;
            trailEmitter = new ParticleEmitter(projectileTrailParticles, TrailParticlesPerSecond, position);
        }
        public Projectile(ParticleSystem3d projectileTrailParticles, Vector3 position )
        {
            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(projectileTrailParticles, TrailParticlesPerSecond, position);
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public void Update(GameTime gameTime, Vector3 position)
        {
  
            // Update the particle emitter, which will create our particle trail.
            trailEmitter.Update(gameTime, position);
            
        }
    }
}
