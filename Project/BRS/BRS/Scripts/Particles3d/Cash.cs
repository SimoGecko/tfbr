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
        public override void Awake() {
            _projectileTrailParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "cash",
                    MaxParticles = 100,
                    Duration = TimeSpan.FromSeconds(1),
                    DurationRandomness = 3.5f,
                    EmitterVelocitySensitivity = 0.1f,

                    MinHorizontalVelocity = 2,
                    MaxHorizontalVelocity = 3,

                    MinVerticalVelocity = -1,
                    MaxVerticalVelocity = 1,

                    MinRotateSpeed = -4,
                    MaxRotateSpeed = 4,

                    MinStartSize = 0.1f,
                    MaxStartSize = 0.3f,

                    MinEndSize = 0.4f,
                    MaxEndSize = 0.11f
                }
            };

            //smokePlumeParticles.DrawOrder = 100;
            _projectileTrailParticles.Awake();
        }

        public override void Start() {
            _projectileTrailParticles.Start();

            // init the projectile
            _projectile = new Projectile(_projectileTrailParticles, transform.position, 5);
        }

        public override void Update() {
            _projectile.Update(transform.position);
        }

        public override void Draw3D(Camera camera) {
            _projectileTrailParticles.Draw3D(camera);
        }
    }
}