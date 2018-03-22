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
        public int teamIndex = 0;

        //STAMINA
        const float staminaReloadPerSecond = .2f; // maybe move to its own class?
        const float staminaPerBoost = .4f;
        const float staminaPerAttack = .6f;
        const float staminaReloadDelay = .3f;
        float maxStamina = 1;
        float stamina = 1;

        //HIT and STUN
        const float damage = 40; // put into attack
        const float stunTime = 2f;

        const float respawnTime = 5f;

        //private
        State state = State.normal;
        bool canReloadStamina = true;

        //reference
        //Player otherPlayer;
        //bool hasOtherPlayer = false;

        //subcomponents
        PlayerAttack playerAttack;
        PlayerMovement playerMovement;
        PlayerInventory playerInventory;
        PlayerPowerup playerPowerup;

        CameraController camController;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();

            camController = GameObject.FindGameObjectWithName("camera_" + playerIndex).GetComponent<CameraController>();

            //subcomponents
            playerAttack    = gameObject.GetComponent<PlayerAttack>();
            playerMovement  = gameObject.GetComponent<PlayerMovement>();
            playerInventory = gameObject.GetComponent<PlayerInventory>();
            playerPowerup   = gameObject.GetComponent<PlayerPowerup>();
        }

        public override void Update() {
            if (state == State.normal) {
                playerMovement.boosting = BoostInput();
                Vector2 moveInput =  MoveInput().Rotate(camController.YRotation);
                playerMovement.Move(moveInput.To3());

                if (AttackInput()) playerAttack.BeginAttack();

                if (PowerUpInput()) playerInventory.UsePowerUp(this);
                if (DropCashInput()) playerInventory.DropMoney();
            }
            else if (state == State.attack) {
                playerAttack.AttackCoroutine();
                if (playerAttack.AttackEnded) state = State.normal;
            }

            UpdateStamina();
            UpdateUI();
        }

        public override void OnCollisionEnter(Collider c) {
            //if (c.gameObject.tag == "player") Debug.Log("collision enter player");
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void GetHit() {
            state = State.stun;
            Timer t = new Timer(stunTime, () => { if (state == State.stun) state = State.normal; });
            playerInventory.LoseMoney();
            TakeDamage(damage);
        }


        protected override void Die() {
            base.Die();
            state = State.dead;
            Timer timer = new Timer(respawnTime, Respawn);
        }

        protected override void Respawn() {
            base.Respawn();
            state = State.normal;

            // Todo: Position to be defined => can be that it is droped from the air and while not reaching the floor it can't drive.
            // For now there is 2 because the bounding-box is not yet correctly translated into the local-space. Will be fixed in the next commit.
            transform.position = new Vector3(-5 + 10 * playerIndex, 0, 0);
        }

        void UpdateStamina() {
            if(canReloadStamina && stamina < 0) {
                canReloadStamina = false;
                stamina = 0;
                Timer t = new Timer(1, () => canReloadStamina = true);
            }
            //if (canReloadStamina) stamina += staminaReloadPerSecond * Time.deltatime;
            //stamina = Utility.Clamp01(stamina);

            if (canReloadStamina) AddStamina(staminaReloadPerSecond * Time.deltatime);
        }

        public void AddStamina(float amount) {
            stamina = MathHelper.Min(stamina + amount, maxStamina);
        }

        public void UpdateMaxStamina(float amountToAdd) {
            maxStamina += amountToAdd;
        }

        void UpdateUI() {
            UserInterface.instance.UpdatePlayerUI(playerIndex, health, startingHealth, stamina, maxStamina, playerInventory.Capacity, playerInventory.CarryingValue, playerInventory.CarryingWeight);
        }

        // INPUT queries
        bool BoostInput() {
            if (Input.GetKey(Keys.LeftShift) || Input.GetButton(Buttons.RightShoulder, playerIndex) || Input.GetButton(Buttons.RightTrigger, playerIndex)) {
                if (stamina > 0){//staminaPerBoost * Time.deltatime) {
                    stamina -= staminaPerBoost * Time.deltatime;
                    return true;
                }
            }
            return false;
        }

        Vector2 MoveInput() {
            if (playerIndex == 0)
                return new Vector2(Input.GetAxisRaw0("Horizontal"), Input.GetAxisRaw0("Vertical"));
            else
                return new Vector2(Input.GetAxisRaw1("Horizontal"), Input.GetAxisRaw1("Vertical"));
        }

        bool PowerUpInput() {
            if (Input.GetKey(Keys.P) || Input.GetButton(Buttons.X, playerIndex)) {
                return true;
            }
            return false;
        }

        bool DropCashInput() {
            if (Input.GetKey(Keys.L) || Input.GetButton(Buttons.B, playerIndex)) {
                return true;
            }
            return false;
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
        public float StaminaPercent { get { return stamina / maxStamina; } }


    }

}