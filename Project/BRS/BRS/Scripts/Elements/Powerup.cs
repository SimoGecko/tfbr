// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics.RigidBodies;
using Jitter.LinearMath;

namespace BRS.Scripts {

    public enum PowerupType { health, capacity, speed, stamina, bomb, key, shield, trap, explodingbox, weight, magnet };

    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float rotSpeed = 1;

        //private
        public PowerupType powerupType;
        //protected bool destroyOnUse = true;
        protected bool rotate = true;
        private float _rotationAngle = 0.0f;

        //reference
        public Player owner { get; protected set; }

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            rotate = true;
            transform.rotation = MyRandom.YRotation();
        }

        public override void Update() {
            base.Update();

            if (rotate) {
                _rotationAngle += rotSpeed * Time.deltaTime;

                RigidBodyComponent rbc = gameObject.GetComponent<DynamicRigidBody>();
                if (rbc != null) {
                    rbc.RigidBody.AngularVelocity = new JVector(0, 2, 0);
                }

                //transform.Rotate(Vector3.Up, rotSpeed * Time.deltaTime);
            }

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void DoPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                owner = p;
                pp.Collect(this);

                Elements.instance.Remove(this);

                //if(!destroyOnUse) gameObject.active = false;
                GameObject.Destroy(gameObject);
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