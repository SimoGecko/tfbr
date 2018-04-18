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

        // --------------------- BASE METHODS ------------------
        public override void Start()
        {
            smokePlumeParticles = new ParticleSystem3d();
            smokePlumeParticles.Settings.TextureName = "CFX4Smoke";

            smokePlumeParticles.Settings.MaxParticles = 600;
            smokePlumeParticles.Settings.Duration = TimeSpan.FromSeconds(10);

            smokePlumeParticles.Settings.MinHorizontalVelocity = 0;
            smokePlumeParticles.Settings.MaxHorizontalVelocity = 15;

            smokePlumeParticles.Settings.MinVerticalVelocity = 10;
            smokePlumeParticles.Settings.MaxVerticalVelocity = 20;

            // Create a wind effect by tilting the gravity vector sideways.
            smokePlumeParticles.Settings.Gravity = new Vector3(-20, -5, 0);

            smokePlumeParticles.Settings.EndVelocity = 0.75f;

            smokePlumeParticles.Settings.MinColor = new Color(255, 140, 0, 255);
            smokePlumeParticles.Settings.MaxColor = new Color(255, 140, 0, 255);

            smokePlumeParticles.Settings.MinRotateSpeed = -1;
            smokePlumeParticles.Settings.MaxRotateSpeed = 1;

            smokePlumeParticles.Settings.MinStartSize = 1;
            smokePlumeParticles.Settings.MaxStartSize = 3;

            smokePlumeParticles.Settings.MinEndSize = 10;
            smokePlumeParticles.Settings.MaxEndSize = 30;

            //smokePlumeParticles.DrawOrder = 100;
            smokePlumeParticles.Awake();
            smokePlumeParticles.Start();

        }

        public override void Update()
        {
            smokePlumeParticles.AddParticle(transform.position, Vector3.Zero);
            smokePlumeParticles.Update(Time.Gt);
        }

        public void Draw(Camera camera)
        {
            smokePlumeParticles.SetCamera(camera.View, camera.Proj);
            smokePlumeParticles.Draw(Time.Gt);
        }
    }
}