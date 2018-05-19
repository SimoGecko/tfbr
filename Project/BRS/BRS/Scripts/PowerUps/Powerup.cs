// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.PowerUps {

    public enum PowerupType { Health, Capacity, Speed, Stamina, Bomb, Key, Shield, Trap, Explodingbox, Weight, Magnet };

    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        public PowerupType PowerupType;
        private const float RotSpeed = 1;

        //private
        //protected bool destroyOnUse = true;
        private bool _rotate = true;
        protected bool _useInstantly = false;

        public Color powerupColor = Color.White;
        static Color[] powerupColors;

        //private float _rotationAngle = 0.0f;

        // const

        //reference
        public Player Owner { get; protected set; }

        private DynamicRigidBody _rigidBody;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            _rotate = true;
            transform.rotation = MyRandom.YRotation();
            Texture2D pcol = File.Load<Texture2D>("Images/colors/powerup");
            powerupColors = Graphics.TextureTo1DArray(pcol);

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

        public override void OnCollisionEnd(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;

            if (isPlayer) {
                PlayerPowerup pp = c.GameObject.GetComponent<PlayerPowerup>();
                pp.RemoveCollided(this);
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void DoPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                Audio.Play("pickup", transform.position); //+PowerupType.ToString().ToLower()
                ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Star, 1f, powerupColor);
                Owner = p;
                if (_useInstantly) UsePowerup();
                else pp.Collect(this);

                ElementManager.Instance.Remove(this);

                //if(!destroyOnUse) gameObject.active = false;
                GameObject.Destroy(gameObject);
            } else {
                pp.AddCollided(this);
            }
        }

        public virtual void UsePowerup() {
            transform.position = Owner.transform.position;
            string audioName = "use_" + PowerupType.ToString().ToLower();
            if (Audio.Contains(audioName)) Audio.Play(audioName, transform.position);
            else Audio.Play("use_various", transform.position);
        }

        // queries
        public virtual bool CanUse() {
            //fill 
            return true;
        }

        public static Color PowerupColor(int index) {
            return powerupColors[11 - index % powerupColors.Length];
        }



        // other

    }

}