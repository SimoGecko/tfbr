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
    class Dust : Component
    {

        // --------------------- VARIABLES ---------------------
        ParticleSystem3d projectileTrailParticles;
        Projectile projectile;

        // --------------------- BASE METHODS ------------------
        public override void Start()
        {
            projectileTrailParticles = new ParticleSystem3d();
            projectileTrailParticles.Settings.TextureName = "smoke";
            projectileTrailParticles.Settings.MaxParticles = 1000;
            projectileTrailParticles.Settings.Duration = TimeSpan.FromSeconds(3);

            projectileTrailParticles.Settings.DurationRandomness = 1.5f;

            projectileTrailParticles.Settings.EmitterVelocitySensitivity = 0.1f;

            projectileTrailParticles.Settings.MinHorizontalVelocity = 0;
            projectileTrailParticles.Settings.MaxHorizontalVelocity = 1;

            projectileTrailParticles.Settings.MinVerticalVelocity = -1;
            projectileTrailParticles.Settings.MaxVerticalVelocity = 1;

            projectileTrailParticles.Settings.MinColor = new Color(64, 96, 128, 255);
            projectileTrailParticles.Settings.MaxColor = new Color(255, 255, 255, 128);

            projectileTrailParticles.Settings.MinRotateSpeed = -4;
            projectileTrailParticles.Settings.MaxRotateSpeed = 4;

            projectileTrailParticles.Settings.MinStartSize = 1;
            projectileTrailParticles.Settings.MaxStartSize = 3;

            projectileTrailParticles.Settings.MinEndSize = 4;
            projectileTrailParticles.Settings.MaxEndSize = 11;

            //smokePlumeParticles.DrawOrder = 100;
            projectileTrailParticles.Awake();
            projectileTrailParticles.Start();

            // init the projectile
            projectile = new Projectile(projectileTrailParticles, transform.position);
            
        }

        public override void Update()
        {
            projectile.Update(Time.Gt, transform.position);
            
        }

        public void Draw(Camera camera)
        {
            projectileTrailParticles.SetCamera(camera.View, camera.Proj);
            projectileTrailParticles.Draw(Time.Gt);
        }
    }
}