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
        public int PlayerIndex { get; private set; } // player index - to select input and camera
        public int TeamIndex { get; private set; } // to differentiate teams
        string playerName;
        public string nameUser;

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
        PlayerLift      pL;

        CameraController camController;


        // --------------------- BASE METHODS ------------------
        public Player(int playerIndex, int teamIndex, string _name = "Player")
        {
            PlayerIndex = playerIndex;
            TeamIndex = teamIndex;
            playerName = _name + (playerIndex + 1).ToString();
            nameUser = playerName;
        }
        public override void Start() {
            base.Start();

            camController = GameObject.FindGameObjectWithName("camera_" + PlayerIndex).GetComponent<CameraController>();

            //subcomponents (shorten)
            pA = gameObject.GetComponent<PlayerAttack>();
            pM = gameObject.GetComponent<PlayerMovement>();
            pI = gameObject.GetComponent<PlayerInventory>();
            pP = gameObject.GetComponent<PlayerPowerup>();
            pS = gameObject.GetComponent<PlayerStamina>();
            pL = gameObject.GetComponent<PlayerLift>();
        }

        public override void Update() {
            if (!GameManager.GameActive) {
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

                if (PowerupInput())  pP.UsePowerup(this);
                if (DropCashInput()) pI.DropMoney();

                if (AttackInput() && pS.HasStaminaForAttack()) {
                    state = State.attack;
                    pS.UseStaminaForAttack();
                    pA.BeginAttack();
                    camController.Shake(.5f);
                }

                if (LiftInput()) {
                    pL.Lift();
                }

            }
            else if (state == State.attack) {
                pA.AttackCoroutine();
                if (pA.AttackEnded) state = State.normal;
            }

            pS.UpdateStamina();
            UpdateUI();
        }

        public override void OnCollisionEnter(Collider c) {
            //camController.Shake(.3f);
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
            transform.position = new Vector3(-5 + 10 * PlayerIndex, 0, 0); // store base position
        }

        void UpdateUI() {
            //Base ba = GameObject.FindGameObjectWithName("Base_" + playerIndex).GetComponent<Base>();
            // WHY SHOULD THE PLAYER KNOW ABOUT THE BASE??
            PlayerUI.instance.UpdatePlayerUI(PlayerIndex,
                health, startingHealth,
                pS.stamina, pS.maxStamina,
                pI.Capacity, pI.CarryingValue, pI.CarryingWeight, playerName);//, ba.Health, ba.startingHealth);
        }

        //-------------------------------------------------------------------------------------------
        // INPUT queries
        //note: keyboard assumes max 2 players, gamepad works for 4

        Vector2 MoveInput() {
            if (PlayerIndex == 0)
                return new Vector2(Input.GetAxisRaw0("Horizontal"), Input.GetAxisRaw0("Vertical"));
            else
                return new Vector2(Input.GetAxisRaw1("Horizontal"), Input.GetAxisRaw1("Vertical"));
        }

        bool AttackInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.Space) : Input.GetKeyDown(Keys.Enter))
                || Input.GetButtonDown(Buttons.A, PlayerIndex);
        }

        bool DropCashInput() {
            return (PlayerIndex == 0 ? Input.GetKey(Keys.C) : Input.GetKey(Keys.M))
               || Input.GetButton(Buttons.B, PlayerIndex);
        }

        bool PowerupInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.R) : Input.GetKeyDown(Keys.P))
               || Input.GetButtonDown(Buttons.X, PlayerIndex);
        }

        bool LiftInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.F) : Input.GetKeyDown(Keys.L))
               || Input.GetButtonDown(Buttons.Y, PlayerIndex);
        }

        bool BoostInput() {
            return (PlayerIndex == 0 ? Input.GetKey(Keys.LeftShift) : Input.GetKey(Keys.RightShift))
               || Input.GetButton(Buttons.RightShoulder, PlayerIndex) || Input.GetButton(Buttons.RightTrigger, PlayerIndex);
        }

        // other


    }

}