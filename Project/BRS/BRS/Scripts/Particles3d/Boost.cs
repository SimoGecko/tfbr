// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using Microsoft.Xna.Framework;
using BRS.Engine.Particles;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Deals with the movement of the player around the map
    /// </summary>
    class Boost : Component
    {

        // --------------------- VARIABLES ---------------------
        ParticleSystem3d projectileTrailParticles;
        Projectile projectile;

        private PlayerAttack _playerAttack;
        private PlayerMovement _playerMovement;

        // --------------------- BASE METHODS ------------------
        public override void Start()
        {
            _playerAttack = gameObject.GetComponent<PlayerAttack>();
            _playerMovement = gameObject.GetComponent<PlayerMovement>();

            projectileTrailParticles = new ParticleSystem3d();
            projectileTrailParticles.Settings.TextureName = "CFX_T_Flame1_ABP";
            projectileTrailParticles.Settings.MaxParticles = 1000;
            projectileTrailParticles.Settings.Duration = TimeSpan.FromSeconds(3);

            projectileTrailParticles.Settings.DurationRandomness = 1.5f;

            projectileTrailParticles.Settings.EmitterVelocitySensitivity = 0.1f;

            projectileTrailParticles.Settings.MinHorizontalVelocity = 0;
            projectileTrailParticles.Settings.MaxHorizontalVelocity = 0.1f;

            projectileTrailParticles.Settings.MinVerticalVelocity = 0.01f;
            projectileTrailParticles.Settings.MaxVerticalVelocity = 0.01f;

            projectileTrailParticles.Settings.MinColor = Color.White;// new Color(255, 255, 255, 255);
            projectileTrailParticles.Settings.MaxColor = Color.White;// new Color(255, 255, 255, 128);

            projectileTrailParticles.Settings.MinRotateSpeed = -4;
            projectileTrailParticles.Settings.MaxRotateSpeed = 4;

            projectileTrailParticles.Settings.MinStartSize = 0.1f;
            projectileTrailParticles.Settings.MaxStartSize = 0.2f;

            projectileTrailParticles.Settings.MinEndSize = 1.5f;
            projectileTrailParticles.Settings.MaxEndSize = 2.0f;

            //smokePlumeParticles.DrawOrder = 100;
            projectileTrailParticles.Awake();
            projectileTrailParticles.Start();

            // init the projectile
            projectile = new Projectile(projectileTrailParticles, transform.position);
            
        }

        public override void Update()
        {
            Color dustColor = _playerAttack.IsAttacking ? Color.Red : Color.Red;
                projectileTrailParticles.Settings.MinColor = dustColor;
                projectileTrailParticles.Settings.MaxColor = dustColor;

            //if (_playerMovement.Boosting)
            //{
            //    projectile.
            //}

            projectile.Update(Time.Gt, transform.position);
            
        }

        public void Draw(Camera camera)
        {
            projectileTrailParticles.SetCamera(camera.View, camera.Proj);
            projectileTrailParticles.Draw(Time.Gt);
        }
    }
}