// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts {
    class Player : LivingEntity {
        ////////// player class that allows the user to control the in game avatar //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        public int playerIndex = 0; // player index - to select input and camera

        //MOVEMENT
        const float minSpeed = 4f;
        const float maxSpeed = 7f;
        const float maxTurningRate = 360; // deg/sec
        float rotation;
        float smoothMagnitude, refMagnitude;
        float refangle, refangle2;
        float inputAngle;
        float targetRotation;

        //BOOST
        const float boostMultiplier = 1.5f;
        const float staminaPerBoost = .4f;
        bool boosting;

        //STAMINA
        float stamina = 1;
        const float staminaPerSecond = .2f;

        //MONEY
        const int capacity = 10;
        public int carryingmoney = 0;

        //ATTACK
        const float staminaPerAttack = .6f;
        const float attackTime = .2f;
        const float attackDistance = 3;
        bool attacking = false;
        Vector3 attackStartPos, attackEndPos;
        float attackRefTime;
        bool hasAppliedDamage = false;

        //reference
        Player otherPlayer;
        bool hasOtherPlayer = false;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            transform.Rotate(Vector3.Up, -90);
            rotation = targetRotation = -90;
            hasOtherPlayer = GameObject.FindGameObjectWithName("player_" + (playerIndex + 1) % 2) != null;
            if (hasOtherPlayer) {
                otherPlayer = GameObject.FindGameObjectWithName("player_" + (playerIndex + 1) % 2).GetComponent<Player>();
            }
        }

        public override void Update() {
            if (!attacking) {
                BoostInput();
                MoveInput();
            }
            AttackInput();
            if (attacking) {
                AttackCoroutine(ref attackRefTime);
                if(hasOtherPlayer)
                    CheckCollision();
            }

            UpdateStamina();

            UpdateUI();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void BoostInput() {
            boosting = false;
            if (Input.GetKey(Keys.LeftShift)) {
                if(stamina >= staminaPerBoost * Time.deltatime) {
                    stamina -= staminaPerBoost * Time.deltatime;
                    boosting = true;
                }
            }
        }

        void MoveInput() {

            Vector3 input;
            if(playerIndex==0)
                input = new Vector3(Input.GetAxisRaw0("Horizontal"), 0, Input.GetAxisRaw0("Vertical"));
            else
                input = new Vector3(Input.GetAxisRaw1("Horizontal"), 0, Input.GetAxisRaw1("Vertical"));

            float magnitude = Utility.Clamp01(input.Length());
            smoothMagnitude = Utility.SmoothDamp(smoothMagnitude, magnitude, ref refMagnitude, .1f);

            //rotate towards desired angle
            if (smoothMagnitude > .05f) { // avoid changing if 0
                inputAngle = MathHelper.ToDegrees((float)Math.Atan2(input.Z, input.X)); 
                inputAngle = Utility.WrapAngle(inputAngle, targetRotation);
                targetRotation = Utility.SmoothDampAngle(targetRotation, inputAngle-90, ref refangle, .3f, maxTurningRate*smoothMagnitude);
            } else {
                targetRotation = Utility.SmoothDampAngle(targetRotation, rotation, ref refangle2, .3f, maxTurningRate*smoothMagnitude);
            }

            rotation = MathHelper.Lerp(rotation, targetRotation, smoothMagnitude);
            transform.eulerAngles = new Vector3(0, rotation, 0);

            //move forward
            float speedboost = boosting ? boostMultiplier : 1f;
            transform.Translate(Vector3.Forward * currentSpeed * speedboost * smoothMagnitude * Time.deltatime);
        }

        void AttackInput() {
            bool inputfire = playerIndex==0 ? Input.Fire1() : Input.Fire2();
            if (inputfire && !attacking && stamina >= staminaPerAttack) {
                stamina -= staminaPerAttack;
                attacking = true;
                attackRefTime = 0;
                attackStartPos = transform.position;
                attackEndPos = transform.position + transform.Forward * attackDistance;
                hasAppliedDamage = false;
            }
        }

        void CheckCollision() {
            const float distanceThreshold = 1f;
            if(Vector3.DistanceSquared(transform.position, otherPlayer.transform.position) < distanceThreshold && !hasAppliedDamage) {
                otherPlayer.GetHit();
                hasAppliedDamage = true;
            }
        }

        public void GetHit() {
            //TODO STUN

            //LOSE MONEY
            TakeDamage(50);
        }

        protected override void Die() {
            base.Die();
            Timer timer = new Timer(0, 5, Respawn);
        }

        protected override void Respawn() {
            base.Respawn();
            transform.position = Vector3.Zero;
        }



        public void CollectMoney(int amount) {
            carryingmoney += Math.Min(amount, capacity-carryingmoney);
        }

        public void Deload() {
            carryingmoney = 0;
        }

        void UpdateStamina() {
            if (!boosting) stamina += staminaPerSecond * Time.deltatime;
            stamina = Utility.Clamp01(stamina);
        }

        void UpdateUI() {
            UserInterface.instance.UpdatePlayerUI(playerIndex, HealthPercent, stamina, MoneyPercent);
        }


        // queries
        public float MoneyPercent { get { return (float)carryingmoney / capacity; } }
        public bool CanPickUp { get { return carryingmoney < capacity; } }

        float currentSpeed { get { return MathHelper.Lerp(maxSpeed, minSpeed, MoneyPercent); } }
        // other

        void AttackCoroutine(ref float percent) {
            if (percent <= 1) {
                percent += Time.deltatime / attackTime;
                float t = Curve.EvaluateSqrt(percent);
                transform.position = Vector3.LerpPrecise(this.attackStartPos, attackEndPos, t);
            } else {
                attacking = false;
            }
        }

    }

}