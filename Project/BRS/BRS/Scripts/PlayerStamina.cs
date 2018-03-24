// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerStamina : Component {
        ////////// deals with player stamina, usage and replenishment //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float staminaReloadPerSecond = .2f;
        const float staminaPerBoost = .4f;
        const float staminaPerAttack = .6f;
        const float staminaReloadDelay = .3f;

        //private
        public float maxStamina = 1;
        public float stamina = 1;
        bool canReloadStamina = true;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() { }
        public override void Update() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void UpdateStamina() {
            if (canReloadStamina && stamina < 0) {
                canReloadStamina = false;
                stamina = 0;
                Timer t = new Timer(1, () => canReloadStamina = true);
            }
            if (canReloadStamina) AddStamina(staminaReloadPerSecond * Time.deltaTime);
        }

        public void AddStamina(float amount) {
            stamina = MathHelper.Min(stamina + amount, maxStamina);
        }

        public void UpdateMaxStamina(float amountToAdd) {
            maxStamina += amountToAdd;
        }

        //USAGE
        public void UseStaminaForBoost() {
            stamina -= staminaPerBoost * Time.deltaTime;
        }
        public void UseStaminaForAttack() {
            stamina -= staminaPerAttack;
        }


        // queries
        public bool HasStaminaForBoost() {
            return stamina > 0; //staminaPerBoost * Time.deltatime)
        }

        public bool HasStaminaForAttack() {
            return stamina >= staminaPerAttack;
        }



        // other
        public float StaminaPercent { get { return stamina / maxStamina; } }

    }

}