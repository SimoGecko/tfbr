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
        ////////// player class that is a center hub for all things related to the player //////////
        ////////// it manages the state, team, calls relative functions with input BUT NOTHING ELSE //////////

        // --------------------- VARIABLES ---------------------
        enum State { normal, attack, stun, dead};

        //public
        public int playerIndex = 0; // player index - to select input and camera
        public int teamIndex = 0; // to differentiate teams


        //HIT and STUN
        const float stunTime = 2f;
        const float respawnTime = 5f;

        //private
        State state = State.normal;

        //reference

        //subcomponents
        PlayerAttack    pA;
        PlayerMovement  pM;
        PlayerInventory pI;
        PlayerPowerup   pP;
        PlayerStamina   pS;

        CameraController camController;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();

            camController = GameObject.FindGameObjectWithName("camera_" + playerIndex).GetComponent<CameraController>();

            //subcomponents (shorten)
            pA = gameObject.GetComponent<PlayerAttack>();
            pM = gameObject.GetComponent<PlayerMovement>();
            pI = gameObject.GetComponent<PlayerInventory>();
            pP = gameObject.GetComponent<PlayerPowerup>();
            pS = gameObject.GetComponent<PlayerStamina>();
        }

        public override void Update() {
            if (!GameManager.gameActive) {
                pM.Move(Vector3.Zero); // smooth stop
                return;
            }

            //only if game is running
            if (state == State.normal) {
                bool boosting = BoostInput() && pS.HasStaminaForBoost();
                pM.boosting = boosting;
                if (boosting) pS.UseStaminaForBoost();

                Vector2 moveInput =  MoveInput().Rotate(camController.YRotation);
                pM.Move(moveInput.To3());

                if (PowerUpInput())  pP.UsePowerup(this);
                if (DropCashInput()) pI.DropMoney();

                if (AttackInput() && pS.HasStaminaForAttack()) {
                    state = State.attack;
                    pS.UseStaminaForAttack();
                    pA.BeginAttack();
                }
            }
            else if (state == State.attack) {
                pA.AttackCoroutine();
                if (pA.AttackEnded) state = State.normal;
            }

            pS.UpdateStamina();
            UpdateUI();
        }


        // --------------------- CUSTOM METHODS ----------------


        // LIVING STUFF
        public override void TakeDamage(float damage) { // for bombs aswell
            base.TakeDamage(damage); // don't override state
            if (!dead) {
                state = State.stun;
                pI.LoseMoney();
                Timer t = new Timer(stunTime, () => { if (state == State.stun) state = State.normal; });
            }
        }

        protected override void Die() {
            base.Die();
            state = State.dead;
            pI.LoseAllMoney();
            Timer timer = new Timer(respawnTime, Respawn);
        }

        protected override void Respawn() {
            base.Respawn();
            state = State.normal;
            transform.position = new Vector3(-5 + 10 * playerIndex, 0, 0); // store base position
        }

        void UpdateUI() {
            //Base ba = GameObject.FindGameObjectWithName("Base_" + playerIndex).GetComponent<Base>();
            // WHY SHOULD THE PLAYER KNOW ABOUT THE BASE??
            UserInterface.instance.UpdatePlayerUI(playerIndex,
                health, startingHealth,
                pS.stamina, pS.maxStamina,
                pI.Capacity, pI.CarryingValue, pI.CarryingWeight);//, ba.Health, ba.startingHealth);
        }

        //-------------------------------------------------------------------------------------------
        // INPUT queries

        Vector2 MoveInput() {
            if (playerIndex == 0)
                return new Vector2(Input.GetAxisRaw0("Horizontal"), Input.GetAxisRaw0("Vertical"));
            else
                return new Vector2(Input.GetAxisRaw1("Horizontal"), Input.GetAxisRaw1("Vertical"));
        }

        bool AttackInput() {
            return (playerIndex == 0 ? Input.GetKey(Keys.Space) : Input.GetKey(Keys.Enter))
                || Input.GetButton(Buttons.A, playerIndex);
        }

        bool DropCashInput() {
            return (playerIndex == 0 ? Input.GetKey(Keys.C) : Input.GetKey(Keys.M))
               || Input.GetButton(Buttons.B, playerIndex);
        }

        bool PowerUpInput() {
            return (playerIndex == 0 ? Input.GetKey(Keys.R) : Input.GetKey(Keys.P))
               || Input.GetButton(Buttons.X, playerIndex);
        }

        bool LiftInput() {
            return (playerIndex == 0 ? Input.GetKey(Keys.F) : Input.GetKey(Keys.L))
               || Input.GetButton(Buttons.Y, playerIndex);
        }

        bool BoostInput() {
            return (playerIndex == 0 ? Input.GetKey(Keys.LeftShift) : Input.GetKey(Keys.RightShift))
               || Input.GetButton(Buttons.RightShoulder, playerIndex) || Input.GetButton(Buttons.RightTrigger, playerIndex);
        }

        // other


    }

}