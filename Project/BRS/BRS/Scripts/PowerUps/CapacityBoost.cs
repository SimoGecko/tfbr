namespace BRS.Scripts {
    class CapacityBoost : Powerup {
        private const int ValueBoost = 2;

        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Capacity;
        }

        public override void UsePowerup() {
            base.UsePowerup();
            Owner.GameObject.GetComponent<PlayerInventory>().UpdateCapacity(ValueBoost);
        }
    }
}