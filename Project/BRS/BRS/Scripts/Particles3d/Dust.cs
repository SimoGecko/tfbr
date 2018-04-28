// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the normal-state of the player
    /// </summary>
    class Dust : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        private ParticleSystem3D _projectileTrailParticles;
        private Projectile _projectile;

        public override bool IsEmitting {
            get => _projectile.IsEmitting;
            set => _projectile.IsEmitting = value;
        }


        // --------------------- BASE METHODS ------------------

        /// <summary>
        /// Initialization of the particle-system
        /// </summary>
        public override void Awake() {
            _projectileTrailParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "smoke",
                    MaxParticles = 1000,
                    ParticlesPerRound = 3,
                    Duration = TimeSpan.FromSeconds(2),
                    DurationRandomness = 1.5f,
                    EmitterVelocitySensitivity = 0.01f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 0.1f,

                    MinVerticalVelocity = 0.01f,
                    MaxVerticalVelocity = 0.01f,

                    MinColor = new Color(255, 255, 255, 255),
                    MaxColor = new Color(255, 255, 255, 255),

                    MinRotateSpeed = 0,
                    MaxRotateSpeed = 0,

                    MinStartSize = 0.1f,
                    MaxStartSize = 0.2f,

                    MinEndSize = 1.5f,
                    MaxEndSize = 2.0f
                }
            };

            _projectileTrailParticles.Awake();
        }

        /// <summary>
        /// Initialize the projectile with the correct position
        /// </summary>
        public override void Start() {
            _projectileTrailParticles.Start();
            _projectile = new Projectile(_projectileTrailParticles, transform.position);
        }

        /// <summary>
        /// Update the projectile with emitting new particles and update the living
        /// </summary>
        public override void Update() {
            _projectile.Update(transform.position);
        }

        /// <summary>
        /// Draw the living particles in the 3D space on the current camera
        /// </summary>
        /// <param name="camera">Camera to draw</param>
        public override void Draw3D(Camera camera) {
            _projectileTrailParticles.Draw3D(camera);
        }
    }
}