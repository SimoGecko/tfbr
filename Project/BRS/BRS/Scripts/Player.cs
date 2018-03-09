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
    class Player : Component {
        ////////// player class that allows the user to control the in game avatar //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        public int playerIndex = 0; // player index - to select input and camera

        //MOVEMENT
        const float maxSpeed = 7f;
        const float minSpeed = 4f;
        const float maxTurningRate = 360; // degrees/sec
        float smoothMagnitude, refMagnitude;
        float rotation;
        float refangle, refangle2;
        float inputAngle; // store it
        float targetRotation;

        //BOOST
        const float boostMultiplier = 1.5f;
        const float staminaPerBoost = .4f;
        bool boosting;

        //STAMINA
        float stamina = 1;
        float staminaPerSecond = .2f;

        //MONEY
        const int capacity = 10;
        public int carryingmoney = 0;

        //attack
        const float staminaPerAttack = .6f;
        bool attacking = false;
        Vector3 startPos, endPos;
        float attackT;
        const float attackTime = .2f;
        const float attackDistance = 3;
        Vector3 oldPos;
        float beforeAttackSpeed;
        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            transform.Rotate(Vector3.Up, -90);
            rotation = targetRotation = -90;
        }

        public override void Update() {
            if (!attacking) {
                BoostInput();
                MoveInput();
            }
            AttackInput();
            if (attacking) {
                AttackCoroutine(ref attackT);
            }

            stamina += staminaPerSecond * Time.deltatime;
            stamina = Utility.Clamp01(stamina);
            Debug.Log(stamina.ToString());
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
            //based on index
            oldPos = transform.position;

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
            if (Input.Fire1() && !attacking && stamina >= staminaPerAttack) {
                stamina -= staminaPerAttack;
                attacking = true;
                attackT = 0;
                startPos = transform.position;
                endPos = transform.position + transform.Forward * attackDistance;
                beforeAttackSpeed = Vector3.Distance(transform.position, oldPos) / Time.deltatime;
            }
        }

        public void CollectMoney(int amount) {
            carryingmoney += Math.Min(amount, capacity-carryingmoney);
            UserInterface.instance.SetPlayerMoneyPercent(MoneyPercent, playerIndex);
        }

        public void Deload() {
            carryingmoney = 0;
            UserInterface.instance.SetPlayerMoneyPercent(MoneyPercent, playerIndex);
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
                transform.position = Vector3.LerpPrecise(startPos, endPos, t);
            } else {
                attacking = false;
            }
        }
        /*
        async void Attack() {
            attacking = true;
            float attackDistance = 6;
            float attackTime = 1f;

            Vector3 startPosition = transform.position;
            Vector3 finalPosition = transform.position + transform.Forward * attackDistance;

            float percent = 0;
            while (percent < 1) {
                percent += Time.deltatime / attackTime;
                float t = percent;//TODO evaluate curve
                transform.position = Vector3.Lerp(startPosition, finalPosition, t);

                await Time.WaitForFrame();
            }

            transform.position = finalPosition;
            attacking = false;
        }*/

    }

}