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
    class Cash : Component
    {

        // --------------------- VARIABLES ---------------------
        ParticleSystem3d projectileTrailParticles;
        Projectile projectile;
        PlayerInventory playerInventory;


        // --------------------- BASE METHODS ------------------
        public override void Start()
        {
            projectileTrailParticles = new ParticleSystem3d();
            playerInventory = gameObject.GetComponent<PlayerInventory>();
            projectileTrailParticles.Settings.TextureName = "cash";
            projectileTrailParticles.Settings.MaxParticles = 100;
            projectileTrailParticles.Settings.Duration = TimeSpan.FromSeconds(1);

            projectileTrailParticles.Settings.DurationRandomness = 3.5f;

            projectileTrailParticles.Settings.EmitterVelocitySensitivity = 0.1f;

            projectileTrailParticles.Settings.MinHorizontalVelocity = 2;
            projectileTrailParticles.Settings.MaxHorizontalVelocity = 3;

            projectileTrailParticles.Settings.MinVerticalVelocity = -1;
            projectileTrailParticles.Settings.MaxVerticalVelocity = 1;


            projectileTrailParticles.Settings.MinRotateSpeed = -4;
            projectileTrailParticles.Settings.MaxRotateSpeed = 4;

            projectileTrailParticles.Settings.MinStartSize = 0.1f;
            projectileTrailParticles.Settings.MaxStartSize = 0.3f;

            projectileTrailParticles.Settings.MinEndSize = 0.4f;
            projectileTrailParticles.Settings.MaxEndSize = 0.11f;

            //smokePlumeParticles.DrawOrder = 100;
            projectileTrailParticles.Awake();
            projectileTrailParticles.Start();

            // init the projectile
            projectile = new Projectile(projectileTrailParticles, transform.position, 5);
            
            
        }

        public override void Update()
        {
            // TODO: this is a hack
            if(playerInventory.IsFull())
            {
                projectile.Update(Time.Gt, transform.position);
            }
            
            
        }

        public void Draw(Camera camera)
        {
            projectileTrailParticles.SetCamera(camera.View, camera.Proj);
            projectileTrailParticles.Draw(Time.Gt);
        }
    }
}