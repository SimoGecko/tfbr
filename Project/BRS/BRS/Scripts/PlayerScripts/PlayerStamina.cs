// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PlayerScripts {
    class PlayerStamina : Component {
        ////////// deals with player stamina, usage and replenishment //////////

        // --------------------- VARIABLES ---------------------

        //public
        public float MaxStamina = 1;
        public float Stamina = 1;
        const float TimeOfUnsuccessfulTry = .3f; // how to long to notify unsuccessful use of stamina

        //private
        private bool _canReloadStamina = true;
        bool triedToUseStaminaUnsuccessfully = false;
        float timerForUnsuccessfulStamina;
        // const
        private const float StaminaReloadPerSecond = .15f;
        private const float StaminaPerBoost = .3f;
        private const float StaminaPerAttack = .3f;
        private const float StaminaReloadDelay = .1f;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            MaxStamina = Stamina = 1f;
            _canReloadStamina = true;
        }

        public override void Update() {
            if (Time.CurrentTime > timerForUnsuccessfulStamina) {
                triedToUseStaminaUnsuccessfully = false;
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void UpdateStamina() {
            if (_canReloadStamina && Stamina < 0) {
                _canReloadStamina = false;
                Stamina = 0;
                Timer t = new Timer(1, () => _canReloadStamina = true);
            }
            if (_canReloadStamina) AddStamina(StaminaReloadPerSecond * Time.DeltaTime);
        }

        public void AddStamina(float amount) {
            Stamina = MathHelper.Min(Stamina + amount, MaxStamina);
        }

        public void UpdateMaxStamina(float amountToAdd) {
            MaxStamina += amountToAdd;
        }

        //USAGE
        public void UseStaminaForBoost() {
            Stamina -= StaminaPerBoost * Time.DeltaTime;
        }
        public void UseStaminaForAttack() {
            Stamina -= StaminaPerAttack;
        }


        // queries
        public bool HasStaminaForBoost() {
            bool result = Stamina > 0;
            if (!result) {
                triedToUseStaminaUnsuccessfully = true;
                timerForUnsuccessfulStamina = Time.CurrentTime + TimeOfUnsuccessfulTry;
            }
            return result; //staminaPerBoost * Time.deltatime)
        }

        public bool HasStaminaForAttack() {
            bool result = Stamina >= StaminaPerAttack;//staminaPerBoost * Time.deltatime)
            if (!result) {
                triedToUseStaminaUnsuccessfully = true;
                timerForUnsuccessfulStamina = Time.CurrentTime + TimeOfUnsuccessfulTry;
            }
            return result; 
        }



        // other
        public float StaminaPercent { get { return Stamina / MaxStamina; } }
        public bool TriedToUseUnsuccessfully { get { return triedToUseStaminaUnsuccessfully || Stamina<=0; } }

    }

}