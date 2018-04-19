// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using BRS.Engine.Particles;

namespace BRS.Scripts.PlayerScripts
{
    /// <summary>
    /// Deals with the movement of the player around the map
    /// </summary>
    class Smoke : Component
    {

        // --------------------- VARIABLES ---------------------
        ParticleSystem3d smokePlumeParticles;
        private static int _particlesPerLoop = 10;

        // --------------------- BASE METHODS ------------------
        public override void Start()
        {
            smokePlumeParticles = new ParticleSystem3d();
            smokePlumeParticles.Settings.TextureName = "CFX4Smoke";

            smokePlumeParticles.Settings.MaxParticles = 1800;
            smokePlumeParticles.Settings.Duration = TimeSpan.FromSeconds(3);

            smokePlumeParticles.Settings.MinHorizontalVelocity = 0;
            smokePlumeParticles.Settings.MaxHorizontalVelocity = 5;

            smokePlumeParticles.Settings.MinVerticalVelocity = 2.5f;
            smokePlumeParticles.Settings.MaxVerticalVelocity = 25;

            // Create a wind effect by tilting the gravity vector sideways.
            smokePlumeParticles.Settings.Gravity = new Vector3(-5, -2.5f, 0);

            smokePlumeParticles.Settings.EndVelocity = 0.75f;

            smokePlumeParticles.Settings.MinColor = new Color(0, 0, 0, 255);
            smokePlumeParticles.Settings.MaxColor = new Color(128, 128, 128, 128);

            smokePlumeParticles.Settings.MinRotateSpeed = -1;
            smokePlumeParticles.Settings.MaxRotateSpeed = 1;

            smokePlumeParticles.Settings.MinStartSize = 1;
            smokePlumeParticles.Settings.MaxStartSize = 2;

            smokePlumeParticles.Settings.MinEndSize = 2;
            smokePlumeParticles.Settings.MaxEndSize = 5;

            //smokePlumeParticles.DrawOrder = 100;
            smokePlumeParticles.Awake();
            smokePlumeParticles.Start();

        }

        public override void Update()
        {
            // particles per update
            for(var i = 0; i < _particlesPerLoop; i++)
            {
                smokePlumeParticles.AddParticle(transform.position, Vector3.Zero);
            }
            smokePlumeParticles.Update(Time.Gt);
        }

        public void Draw(Camera camera)
        {
            smokePlumeParticles.SetCamera(camera.View, camera.Proj);
            smokePlumeParticles.Draw(Time.Gt);
        }
    }
}