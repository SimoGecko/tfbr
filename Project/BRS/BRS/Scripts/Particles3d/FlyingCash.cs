// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Particles;
using BRS.Engine.Rendering;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the vault
    /// </summary>
    class FlyingCash : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        private ParticleSystem3D _cashParticles;

        private static int _maxParticles = 250;
        private int _particlesCreated = 0;
        
        // position
        private static float _minHeight = 10.0f;
        private static float _maxHeight = 15.0f;
        private static float _minX = -40.0f;
        private static float _maxX = 40.0f;
        private static float _minZ = -40.0f;
        private static float _maxZ = 40.0f;

        // velocity
        private static float _minYV = 1.0f;
        private static float _maxYV = 10.0f;
        private static float _minXV = 1.0f;
        private static float _maxXV = -1.0f;
        private static float _minZV = 1.0f;
        private static float _maxZV = -1.0f;

        public override bool IsEmitting { get; set; } = true;


        // --------------------- BASE METHODS ------------------

        /// <summary>
        /// Initialization of the particle-system
        /// </summary>
        public override void Awake() {
            _cashParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "cash",
                    MaxParticles = 1800,
                    Duration = 8.0f,
                    // Create a wind effect by tilting the gravity vector sideways.
                    Gravity = new Vector3(-1.0f, -5.5f, 0),
                    EndVelocity = 0.75f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 5,

                    MinVerticalVelocity = 2.5f,
                    MaxVerticalVelocity = 10.0f,
                
                    MinRotateSpeed = -1,
                    MaxRotateSpeed = 1,

                    MinStartSize = 1.0f,
                    MaxStartSize = 1.01f,

                    MinEndSize = 1.02f,
                    MaxEndSize = 1.03f
                }
            };

            _cashParticles.Awake();
            ParticleRendering.AddInstance(_cashParticles);
        }

        /// <summary>
        /// Initialize the particle system
        /// </summary>
        public override void Start() {
            _cashParticles.Start();
        }

        /// <summary>
        /// Emit new particles and update the existing
        /// </summary>
        public override void Update() {
            if (IsEmitting) {
                // compute a random position of the level
                Vector3 position = transform.position;
                position.X += MathHelper.Lerp(_minX, _maxX, MyRandom.Value);
                position.Y += MathHelper.Lerp(_minHeight, _maxHeight, MyRandom.Value);
                position.Z += MathHelper.Lerp(_minZ, _maxZ, MyRandom.Value);

                // compute a random velocity
                Vector3 velocity = Vector3.Zero;
                velocity.X += MathHelper.Lerp(_minXV, _maxXV, MyRandom.Value);
                velocity.Z += MathHelper.Lerp(_minZV, _maxZV, MyRandom.Value);
                // falling down
                velocity.Y -= MathHelper.Lerp(_minYV, _maxYV, MyRandom.Value);

                _cashParticles.AddParticles(position, velocity);
                _particlesCreated++;
            }

            // only emitted in the beginning till the max number is created
            if (_particlesCreated > _maxParticles) {
                IsEmitting = false;
            }

            _cashParticles.Update();
        }

        /// <summary>
        /// Component is destroyed => Remove particles from drawings
        /// </summary>
        public override void Destroy() {
            ParticleRendering.RemoveInstance(_cashParticles);
        }
    }
}