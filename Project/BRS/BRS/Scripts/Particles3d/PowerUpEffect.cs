﻿// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for the vault
    /// </summary>
    class PowerUpEffect : ParticleComponent {

        // --------------------- VARIABLES ---------------------

        ParticleSystem3D _rayParticles;
        ParticleSystem3D _starParticles;
        Random _random = new Random();

        public override bool IsEmitting { get; set; } = true;


        // --------------------- BASE METHODS ------------------

        /// <summary>
        /// Initialization of the particle-system
        /// </summary>
        public override void Awake() {
            _rayParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "CFX3_T_RayStraight",
                    MaxParticles = 500,
                    ParticlesPerRound = 10,
                    Duration = TimeSpan.FromSeconds(3),
                    // Create a wind effect by tilting the gravity vector sideways.
                    Gravity = new Vector3(0, 0, 0),
                    EndVelocity = 0.75f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 0,

                    MinVerticalVelocity = 0.5f,
                    MaxVerticalVelocity = 1.5f,

                    MinColor = new Color(255, 255, 200, 0),
                    MaxColor = new Color(255, 255, 200, 128),

                    MinRotateSpeed = 0,
                    MaxRotateSpeed = 0,

                    MinStartSize = 0.1f,
                    MaxStartSize = 0.2f,

                    MinEndSize = 0.2f,
                    MaxEndSize = 0.5f
                }
            };

            _starParticles = new ParticleSystem3D {
                Settings = new Settings {
                    TextureName = "CFX3_T_GlowStar",
                    MaxParticles = 50,
                    ParticlesPerRound = 1,
                    Duration = TimeSpan.FromSeconds(7),
                    // Create a wind effect by tilting the gravity vector sideways.
                    Gravity = new Vector3(0, 0, 0),
                    EndVelocity = 0.75f,

                    MinHorizontalVelocity = 0,
                    MaxHorizontalVelocity = 0,

                    MinVerticalVelocity = 0.1f,
                    MaxVerticalVelocity = 0.5f,

                    MinColor = new Color(255, 255, 200, 255),
                    MaxColor = new Color(255, 255, 200, 255),

                    MinRotateSpeed = 0,
                    MaxRotateSpeed = 0,

                    MinStartSize = 0.1f,
                    MaxStartSize = 0.2f,

                    MinEndSize = 0.2f,
                    MaxEndSize = 0.5f
                }
            };

            _rayParticles.Awake();
            _starParticles.Awake();
        }

        /// <summary>
        /// Initialize the particle system
        /// </summary>
        public override void Start() {
            _rayParticles.Start();
            _starParticles.Start();

        }

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
                    transform.position + RandomPointOnCircle(1.25f, 0.0f), 
                    Vector3.Zero);
                float height =  (float)_random.NextDouble();
                _starParticles.AddParticles(
                    transform.position + RandomPointOnCircle(1.25f, height),
                    Vector3.Zero);
            }

            _rayParticles.Update();
            _starParticles.Update();
        }

        /// <summary>
        /// Draw the living particles in the 3D space on the current camera
        /// </summary>
        /// <param name="camera">Camera to draw</param>
        public override void Draw3D(Camera camera) {
            _rayParticles.Draw3D(camera);
            _starParticles.Draw3D(camera);
        }
    }
}