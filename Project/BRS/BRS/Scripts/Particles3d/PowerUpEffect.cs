// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Particles;
using BRS.Engine.Rendering;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the vault
    /// </summary>
    class PowerUpEffect : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        private ParticleSystem3D _rayParticles;
        private ParticleSystem3D _starParticles;
        private readonly Random _random = new Random();
        private readonly Color _mainColor;
        private float _particlesPerRound = 0;

        public PowerUpEffect(Color color) {
            _mainColor = new Color(color, 255);
        }

        public override bool IsEmitting { get; set; } = true; //GameManager.NumPlayers <= 4;


        // --------------------- BASE METHODS ------------------

        /// <summary>
        /// Initialization of the particle-system
        /// </summary>
        public override void Awake() {
            //Color minColor = _mainColor;
            //minColor.A = 0;
            //Color maxColor = _mainColor;
            //maxColor.A = 64;

            //_rayParticles = new ParticleSystem3D {
            //    Settings = new Settings {
            //        TextureName = "powerup_ray",
            //        MaxParticles = 500,
            //        ParticlesPerRound = 10,
            //        Duration = 1.0f,
            //        Gravity = new Vector3(0, 0, 0),
            //        EndVelocity = 0.75f,

            //        MinHorizontalVelocity = 0,
            //        MaxHorizontalVelocity = 0,

            //        MinVerticalVelocity = 0.5f,
            //        MaxVerticalVelocity = 1.0f,

            //        MinColor = minColor,
            //        MaxColor = maxColor,

            //        MinRotateSpeed = 0,
            //        MaxRotateSpeed = 0,

            //        MinStartSize = 0.1f,
            //        MaxStartSize = 0.2f,

            //        MinEndSize = 0.2f,
            //        MaxEndSize = 0.5f
            //    }
            //};
            //maxColor.A = 128;
            //_starParticles = new ParticleSystem3D {
            //    Settings = new Settings {
            //        TextureName = "powerup_star",
            //        MaxParticles = 50,
            //        ParticlesPerRound = 1,
            //        Duration = 1.0f,

            //        Gravity = new Vector3(0, 0, 0),
            //        EndVelocity = 0.75f,

            //        MinHorizontalVelocity = 0,
            //        MaxHorizontalVelocity = 0,

            //        MinVerticalVelocity = 0.1f,
            //        MaxVerticalVelocity = 0.5f,

            //        MinColor = minColor,
            //        MaxColor = maxColor,

            //        MinRotateSpeed = 0,
            //        MaxRotateSpeed = 0,

            //        MinStartSize = 0.1f,
            //        MaxStartSize = 0.2f,

            //        MinEndSize = 0.2f,
            //        MaxEndSize = 0.5f
            //    }
            //};

            //_rayParticles.Awake();
            //_starParticles.Awake();

            //ParticleRendering.AddInstance(_rayParticles);
            //ParticleRendering.AddInstance(_starParticles);
            _rayParticles = ParticleRendering.AddInstance(ParticleType3D.PowerUpRay, this, false);
            _particlesPerRound = _rayParticles.Settings.ParticlesPerRound;
        }

        /// <summary>
        /// Initialize the particle system
        /// </summary>
        public override void Start() {
            //_rayParticles.Start();
            //_starParticles.Start();

        }

        /// <summary>
        /// Create a random sample on a circle witha given radius on agiven height
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="height">Height of the circle</param>
        Vector3 RandomPointOnCircle(float radius, float height) {
            double angle = _random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float z = (float)Math.Sin(angle);

            return new Vector3(x * radius, height, z * radius);
        }

        /// <summary>
        /// Emit new particles and update the existing
        /// </summary>
        public override void Update() {
            if (IsEmitting) {
                _rayParticles.AddParticles(
                    transform.position + RandomPointOnCircle(0.5f, -0.5f),
                    Vector3.Zero);
                //float height = (float)_random.NextDouble() - 0.5f;
                //_starParticles.AddParticles(
                //    transform.position + RandomPointOnCircle(0.5f, height),
                //    Vector3.Zero);
            }

            //_rayParticles.Update();
            //_starParticles.Update();
        }

        public override List<Tuple<Vector3, Vector3>> GetNextPositions() {
            List<Tuple<Vector3, Vector3>> positions = new List<Tuple<Vector3, Vector3>>();
            Vector3 pos = transform.position + RandomPointOnCircle(0.5f, -0.5f);
            Vector3 vel = Vector3.Zero;

            for (int i = 0; i < _particlesPerRound; ++i) {
                positions.Add(new Tuple<Vector3, Vector3>(pos, vel));
            }

            return positions;
        }

        /// <summary>
        /// Component is destroyed => Remove particles from drawings
        /// </summary>
        public override void Destroy() {
            ParticleRendering.RemoveInstance(_rayParticles);
            ParticleRendering.RemoveInstance(_starParticles);
        }
    }
}