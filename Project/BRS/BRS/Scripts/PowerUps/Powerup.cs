// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.RigidBodies;
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
        protected bool _useInstantly = false;

        // const
        private const float RotSpeed = 1;

        //reference
        public Player Owner { get; protected set; }

        private DynamicRigidBody _rigidBody;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            _rotate = true;
            transform.rotation = MyRandom.YRotation();
            CreateUseCallbacks();

            if (gameObject.HasComponent<DynamicRigidBody>()) {
                _rigidBody = gameObject.GetComponent<DynamicRigidBody>();
            }
        }

        public override void Update() {
            base.Update();

            if (_rotate) {
                if (_rigidBody != null) {
                    _rigidBody.RigidBody.AngularVelocity = new JVector(0, 2, 0);
                } else {
                    transform.Rotate(Vector3.Up, RotSpeed * Time.DeltaTime);
                }
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void DoPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                Audio.Play(PowerupType.ToString().ToLower()+ "_pickup", transform.position);
                ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Star);
                Owner = p;
                if (_useInstantly) UsePowerup();
                else pp.Collect(this);

                ElementManager.Instance.Remove(this);

                //if(!destroyOnUse) gameObject.active = false;
                GameObject.Destroy(gameObject);
            }
        }

        public virtual void UsePowerup() {
            transform.position = Owner.transform.position;
            Audio.Play(PowerupType.ToString().ToLower() + "_use", transform.position);
        }

        // queries
        public virtual bool CanUse() {
            //fill 
            return true;
        }

        void CreateUseCallbacks() {

        }


        // other

    }

}