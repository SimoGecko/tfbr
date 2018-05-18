// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the vault
    /// </summary>
    class CashDrop : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        ParticleSystem3D _smokePlumeParticles;

        public override bool IsEmitting { get; set; } = true;


        // --------------------- BASE METHODS ------------------

        /// <summary>
        /// Initialization of the particle-system
        /// </summary>
        public override void Awake() {
            _smokePlumeParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "dollar2",
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

                    //MinColor = new Color(255, 215, 0, 255),
                    //MaxColor = new Color(255, 215, 0, 128),

                    MinRotateSpeed = 0,
                    MaxRotateSpeed = 0,

                    MinStartSize = 1,
                    MaxStartSize = 3,

                    MinEndSize = 2,
                    MaxEndSize = 5
                }
            };


            _smokePlumeParticles.Awake();
        }

        /// <summary>
        /// Initialize the particle system
        /// </summary>
        public override void Start() {
            _smokePlumeParticles.Start();
        }

        /// <summary>
        /// Emit new particles and update the existing
        /// </summary>
        public override void Update() {
            if (IsEmitting) {
                _smokePlumeParticles.AddParticles(transform.position, Vector3.Zero);
            }

            _smokePlumeParticles.Update();
        }
        
        /// <summary>
        /// Draw the living particles in the 3D space on the current camera
        /// </summary>
        /// <param name="camera">Camera to draw</param>
        public override void Draw3D(Camera camera) {
            _smokePlumeParticles.Draw3D(camera);
        }
    }
}