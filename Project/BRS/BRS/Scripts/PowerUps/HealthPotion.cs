namespace BRS.Scripts.PowerUps {
    class HealthPotion : Powerup {
        private const float ValuePotion = 20;

        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Health; // TODO distinguish
        }

        public override void UsePowerup() {
            base.UsePowerup();
            Owner.AddHealth(ValuePotion);
        }
    }
}