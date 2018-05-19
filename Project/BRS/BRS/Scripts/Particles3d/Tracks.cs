// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Particles;
using BRS.Engine.Rendering;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the normal-state of the player
    /// </summary>
    class Tracks : ParticleComponent {

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
            float size = 1.0f;
            _projectileTrailParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "tracks",
                    MaxParticles = 400,
                    ParticlesPerRound = 0.1f,
                    Duration = 4.0f,
                    DurationRandomness = 1.5f,
                    EmitterVelocitySensitivity = 0.00f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 0.0f,

                    MinVerticalVelocity = 0.00f,
                    MaxVerticalVelocity = 0.0000001f,

                    MinColor = new Color(255, 255, 255, 255),
                    MaxColor = new Color(255, 255, 255, 255),

                    MinRotateSpeed = 0,
                    MaxRotateSpeed = 0,

                    MinStartSize = size,
                    MaxStartSize = size,

                    MinEndSize = size,
                    MaxEndSize = size
                }
            };

            _projectileTrailParticles.Awake();
            ParticleRendering.AddInstance(_projectileTrailParticles);
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
        /// Component is destroyed => Remove particles from drawings
        /// </summary>
        public override void Destroy() {
            ParticleRendering.RemoveInstance(_projectileTrailParticles);
        }
    }
}