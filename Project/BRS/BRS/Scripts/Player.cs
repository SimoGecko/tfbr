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
        ////////// it manages the state, team, calls relative functions with input //////////

        // --------------------- VARIABLES ---------------------
        enum State { normal, attack, stun, dead};
        //public
        public int playerIndex = 0; // player index - to select input and camera

        //STAMINA
        const float staminaReloadPerSecond = .2f;
        const float staminaPerBoost = .4f;
        const float staminaPerAttack = .6f;
        const float staminaReloadDelay = .3f;
        float stamina = 1;

        //private
        State state = State.normal;
        bool canReloadStamina = true;

        //reference
        Player otherPlayer;
        bool hasOtherPlayer = false;

        PlayerAttack playerAttack;
        PlayerMovement playerMovement;
        PlayerInventory playerInventory;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            
            hasOtherPlayer = GameObject.FindGameObjectWithName("player_" + (1-playerIndex) ) != null;
            if (hasOtherPlayer) {
                hasOtherPlayer = true;
                otherPlayer = GameObject.FindGameObjectWithName("player_" + (1-playerIndex)).GetComponent<Player>();
            }

            //subcomponents
            playerAttack = gameObject.GetComponent<PlayerAttack>();
            playerMovement = gameObject.GetComponent<PlayerMovement>();
            playerInventory = gameObject.GetComponent<PlayerInventory>();
        }

        public override void Update() {
            if (state == State.normal) {
                playerMovement.boosting = BoostInput();
                Vector3 moveInput =  MoveInput();
                playerMovement.Move(moveInput);

                if (AttackInput()) playerAttack.BeginAttack();
            }
            else if (state == State.attack) {
                playerAttack.AttackCoroutine();
                if(hasOtherPlayer)
                    playerAttack.CheckCollision(otherPlayer);
                if (playerAttack.AttackEnded) state = State.normal;
            }

            UpdateStamina();
            UpdateUI();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void GetHit() {
            state = State.stun;
            Timer t = new Timer(.2f, () => state = State.normal);
            playerInventory.LoseMoney();
            TakeDamage(40);
        }


        protected override void Die() {
            base.Die();
            Timer timer = new Timer(0, 5, Respawn);
        }

        protected override void Respawn() {
            base.Respawn();
            transform.position = new Vector3(-5 + 10 * playerIndex, 0, 0);
        }

        void UpdateStamina() {
            if(canReloadStamina && stamina < 0) {
                canReloadStamina = false;
                stamina = 0;
                Timer t = new Timer(1, () => canReloadStamina = true);
            }
            if (canReloadStamina) stamina += staminaReloadPerSecond * Time.deltatime;
            stamina = Utility.Clamp01(stamina);
        }

        void UpdateUI() {
            UserInterface.instance.UpdatePlayerUI(playerIndex, HealthPercent, stamina, playerInventory.MoneyPercent);
        }


        // INPUT queries
        bool BoostInput() {
            if (Input.GetKey(Keys.LeftShift)) {
                if (stamina > 0){//staminaPerBoost * Time.deltatime) {
                    stamina -= staminaPerBoost * Time.deltatime;
                    return true;
                }
            }
            return false;
        }

        Vector3 MoveInput() {
            if (playerIndex == 0)
                return new Vector3(Input.GetAxisRaw0("Horizontal"), 0, Input.GetAxisRaw0("Vertical"));
            else
                return new Vector3(Input.GetAxisRaw1("Horizontal"), 0, Input.GetAxisRaw1("Vertical"));
        }

        bool AttackInput() {
            bool inputfire = playerIndex == 0 ? Input.Fire1() : Input.Fire2();
            if (inputfire && state==State.normal && stamina >= staminaPerAttack) {
                state = State.attack;
                stamina -= staminaPerAttack;
                return true;
            }
            return false;
        }

        // other



    }

}