﻿// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Particles;
using BRS.Engine.Rendering;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the vault
    /// </summary>
    class CashDrop : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        ParticleSystem3D _cashDropParticles;

        public override bool IsEmitting { get; set; } = true;


        // --------------------- BASE METHODS ------------------

        /// <summary>
        /// Initialization of the particle-system
        /// </summary>
        public override void Awake() {
            _cashDropParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "base_cash_drop",
                    MaxParticles = 2,
                    ParticlesPerRound = 1,
                    Duration = 5.0f,
                    // Create a wind effect by tilting the gravity vector sideways.
                    Gravity = new Vector3(-0.5f, -0.2f, 0),
                    EndVelocity = 0.75f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 1.0f,

                    MinVerticalVelocity = 2.5f,
                    MaxVerticalVelocity = 3.0f,

                    MinRotateSpeed = 0,
                    MaxRotateSpeed = 0,

                    MinStartSize = 1,
                    MaxStartSize = 3,

                    MinEndSize = 2,
                    MaxEndSize = 5
                }
            };


            _cashDropParticles.Awake();
            ParticleRendering.AddInstance(_cashDropParticles);
        }

        /// <summary>
        /// Initialize the particle system
        /// </summary>
        public override void Start() {
            _cashDropParticles.Start();
        }

        /// <summary>
        /// Emit new particles and update the existing
        /// </summary>
        public override void Update() {
            if (IsEmitting) {
                _cashDropParticles.AddParticles(transform.position, Vector3.Zero);
            }

            _cashDropParticles.Update();
        }

        /// <summary>
        /// Component is destroyed => Remove particles from drawings
        /// </summary>
        public override void Destroy() {
            ParticleRendering.RemoveInstance(_cashDropParticles);
        }
    }
}