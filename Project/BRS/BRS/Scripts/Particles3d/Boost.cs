// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;
using System;

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
        public override void Awake() {
            _projectileTrailParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "CFX_T_Flame1_ABP",
                    MaxParticles = 10000,
                    ParticlesPerRound = 10,
                    Duration = TimeSpan.FromSeconds(.25f),
                    DurationRandomness = 1.5f,
                    EmitterVelocitySensitivity = 0.0f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 0.1f,

                    MinVerticalVelocity = 0.01f,
                    MaxVerticalVelocity = 0.01f,

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
        }

        public override void Start() {
            _projectileTrailParticles.Start();

            // init the projectile
            _projectile = new Projectile(_projectileTrailParticles, transform.position);
        }

        public override void Update() {
            _projectile.Update(transform.position);
        }

        public override void Draw3D(Camera camera) {
            _projectileTrailParticles.Draw3D(camera);
        }
    }
}