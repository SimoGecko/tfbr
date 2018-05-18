// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Particles;
using BRS.Engine.Rendering;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the boost-state of the player
    /// </summary>
    class Boost : ParticleComponent {

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
                    TextureName = "CFX_T_Flame1_ABP",
                    MaxParticles = 200,
                    ParticlesPerRound = 5,
                    Duration = 0.85f,
                    DurationRandomness = 1.5f,
                    EmitterVelocitySensitivity = 0.0f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 0.5f,

                    MinVerticalVelocity = 1.3f,
                    MaxVerticalVelocity = 1.5f,

                    MinColor = new Color(255, 255, 255, 0),
                    MaxColor = new Color(255, 255, 255, 128),

                    MinRotateSpeed = -4,
                    MaxRotateSpeed = 4,

                    MinStartSize = 0.1f,
                    MaxStartSize = 0.15f,

                    MinEndSize = 0.5f,
                    MaxEndSize = 0.75f
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