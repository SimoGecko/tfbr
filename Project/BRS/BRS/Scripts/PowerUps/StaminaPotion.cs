using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.PowerUps {
    class StaminaPotion : Powerup {
        private const float ValuePotion = .2f;

        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Stamina;
        }

        public override void UsePowerup() {
            base.UsePowerup();
            Owner.GameObject.GetComponent<PlayerStamina>().AddStamina(ValuePotion);
        }
    }
}