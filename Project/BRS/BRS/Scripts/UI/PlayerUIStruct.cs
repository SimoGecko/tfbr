namespace BRS.Scripts {
    public struct PlayerUIStruct {
        // current
        public string Name;
        public int CarryingValue;

        public int CarryingWeight;
        public float Health;
        public float Stamina;

        // max
        public float MaxHealth;
        public float MaxStamina;
        public int MaxCapacity;

        // helper
        public bool CanAttack;
    }
}