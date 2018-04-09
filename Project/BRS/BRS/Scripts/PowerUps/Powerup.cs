// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PowerUps {

    public enum PowerupType { Health, Capacity, Speed, Stamina, Bomb, Key, Shield, Trap, Explodingbox, Weight, Magnet };

    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        public PowerupType PowerupType;

        //private
        //protected bool destroyOnUse = true;
        private bool _rotate = true;

        // const
        private const float RotSpeed = 1;

        //reference
        public Player Owner { get; protected set; }

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            _rotate = true;
            transform.rotation = MyRandom.YRotation();
        }

        public override void Update() {
            base.Update();

            if (_rotate) {
                //DynamicCollider rbc = GameObject.GetComponent<DynamicCollider>();
                //if (rbc != null) {
                //    rbc.RigidBody.AngularVelocity = new JVector(0, 2, 0);
                //}

                transform.Rotate(Vector3.Up, RotSpeed * Time.DeltaTime);
            }

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void DoPickup(Player p) {
            PlayerPowerup pp = p.GameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                Owner = p;
                pp.Collect(this);

                ElementManager.Instance.Remove(this);

                //if(!destroyOnUse) gameObject.active = false;
                GameObject.Destroy(GameObject);
            }
        }

        public virtual void UsePowerup() { }

        // queries
        public virtual bool CanUse() {
            //fill 
            return true;
        }


        // other

    }

}