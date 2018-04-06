// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Jitter.LinearMath;

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

        //private float _rotationAngle = 0.0f;

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
                //_rotationAngle += RotSpeed * Time.DeltaTime;

                RigidBodyComponent rbc = GameObject.GetComponent<DynamicRigidBody>();
                if (rbc != null) {
                    rbc.RigidBody.AngularVelocity = new JVector(0, 2, 0);
                }

                //transform.Rotate(Vector3.Up, rotSpeed * Time.deltaTime);
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