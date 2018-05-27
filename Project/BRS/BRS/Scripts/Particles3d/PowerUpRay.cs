// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Particles;
using BRS.Engine.Rendering;
using Microsoft.Xna.Framework;
using System;
using BRS.Engine;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the vault
    /// </summary>
    class PowerUpRay : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        private readonly ParticleType3D _particleType;

        public PowerUpRay(ParticleType3D particleType) {
            _particleType = particleType;
        }

        public override bool IsEmitting { get; set; } = true;


        // --------------------- BASE METHODS ------------------

        /// <summary>
        /// Initialization of the particle-system
        /// </summary>
        public override void Awake() {
            ParticleRendering.AddInstance(_particleType, this);
        }

        /// <summary>
        /// Create a random sample on a circle witha given radius on agiven height
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="height">Height of the circle</param>
        Vector3 RandomPointOnCircle(float radius, float height) {
            double angle = MyRandom.Value * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float z = (float)Math.Sin(angle);

            return new Vector3(x * radius, height, z * radius);
        }

        /// <summary>
        /// Get the next position and velocity for the particle.
        /// </summary>
        /// <returns>Tuple with the first item representing the position of the new particle and the second is the velocity.</returns>
        public override Tuple<Vector3, Vector3> GetNextPosition() {
            Vector3 pos = transform.position + RandomPointOnCircle(0.5f, -0.5f);
            Vector3 vel = Vector3.Zero;

            return new Tuple<Vector3, Vector3>(pos, vel);
        }

        /// <summary>
        /// Component is destroyed => Remove particles from drawings
        /// </summary>
        public override void Destroy() {
            ParticleRendering.RemoveInstance(_particleType, this);
        }
    }
}