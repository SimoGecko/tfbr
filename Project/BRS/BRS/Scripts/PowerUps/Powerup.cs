// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Rendering;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.PowerUps {

    // Important: Create for each powerup the corresponding particle-effect in Engine/Rendering/ParticleRendering.cs
    public enum PowerupType { Health, Capacity, Speed, Stamina, Bomb, Key, Shield, Trap, Explodingbox, Weight, Magnet };

    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        public PowerupType powerupType;
        public ParticleType3D ParticleRay;
        public ParticleType3D ParticleStar;
        private const float RotSpeed = 1;

        //private
        protected bool _useInstantly = false;

        static Color[] powerupColors;

        // const

        //reference
        public Player Owner { get; protected set; }

        private DynamicRigidBody _rigidBody;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            transform.rotation = MyRandom.YRotation();

            if (gameObject.HasComponent<DynamicRigidBody>()) {
                _rigidBody = gameObject.GetComponent<DynamicRigidBody>();
            }
        }

        public override void Update() {
            base.Update();
            Rotate();
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
        void Rotate() {
            if (_rigidBody != null) {
                _rigidBody.RigidBody.AngularVelocity = new JVector(0, 2, 0);
            } else {
                transform.Rotate(Vector3.Up, RotSpeed * Time.DeltaTime);
            }
        }

        public override void DoPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                Audio.Play("pickup", transform.position); //+PowerupType.ToString().ToLower()
                ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Star, 1f, powerupColor);
                Owner = p;
                if (_useInstantly) UsePowerup();
                else pp.Collect(this);

                ElementManager.Instance.Remove(this);
                GameObject.Destroy(gameObject);
            } else {
                pp.AddCollided(this);
            }
        }

        public virtual void UsePowerup() {
            transform.position = Owner.transform.position + Vector3.Up;
            string audioName = "use_" + powerupType.ToString().ToLower();
            if (Audio.Contains(audioName)) Audio.Play(audioName, transform.position);
            else Audio.Play("use_various", transform.position);
        }

        // queries
        public virtual bool CanUse() {
            //fill 
            return true;
        }

        public Color powerupColor {
            get {
                return PowerupColor((int)powerupType);
            }
        }


        public static Color PowerupColor(int index) {
            if (powerupColors == null) { // TODO see if it can be moved to start
                Texture2D pcol = File.Load<Texture2D>("Images/colors/powerup");
                powerupColors = Graphics.TextureTo1DArray(pcol);
            }
            return powerupColors[index];
        }

        public static int NumPowerups{
            get {
                return System.Enum.GetNames(typeof(PowerupType)).Length;
            }
        }

        public static int StringToInt(string p) {
            switch (p) {
                case "health":      return 0;
                case "capacity":    return 1;
                case "speed":       return 2;
                case "stamina":     return 3;
                case "bomb":        return 4;
                case "key":         return 5;
                case "shield":      return 6;
                case "trap":        return 7;
                case "explodingbox":return 8;
                case "weight":      return 9;
                case "magnet":      return 10;
                default: return 0;
            }
        }

        public static Powerup PowerupFromIndex(int i) {
            switch (i) {
                case 0: return new HealthPotion();
                case 1: return new CapacityBoost();
                case 2: return new SpeedBoost();
                case 3: return new StaminaPotion();
                case 4: return new Bomb();
                case 5: return new Key();
                case 6: return new ShieldPotion();
                case 7: return new Trap();
                case 8: return new ExplodingBox();
                case 9: return new Weight();
                case 10:return new Magnet();
            }
            return new Bomb();
        }

        // other

    }

}