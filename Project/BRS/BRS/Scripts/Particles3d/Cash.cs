// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Particles;
using System;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the full-state of the player
    /// </summary>
    class Cash : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        ParticleSystem3D _projectileTrailParticles;
        Projectile _projectile;

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
                    TextureName = "cash",
                    MaxParticles = 64,
                    Duration = TimeSpan.FromSeconds(1),
                    DurationRandomness = 3.5f,
                    EmitterVelocitySensitivity = 0.1f,

                    MinHorizontalVelocity = 0.5f,
                    MaxHorizontalVelocity = 0.75f,

                    MinVerticalVelocity = -1.0f,
                    MaxVerticalVelocity = 1.0f,

                    MinRotateSpeed = -4.0f,
                    MaxRotateSpeed = 4.0f,

                    MinStartSize = 0.1f,
                    MaxStartSize = 0.3f,

                    MinEndSize = 0.4f,
                    MaxEndSize = 0.11f
                }
            };

            _projectileTrailParticles.Awake();
        }

        /// <summary>
        /// Initialize the projectile with the correct position
        /// </summary>
        public override void Start() {
            _projectileTrailParticles.Start();
            _projectile = new Projectile(_projectileTrailParticles, transform.position, 32);
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