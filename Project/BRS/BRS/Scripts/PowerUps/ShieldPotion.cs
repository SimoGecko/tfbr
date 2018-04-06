using BRS.Engine;
using BRS.Scripts.Elements;

namespace BRS.Scripts.PowerUps {
    class ShieldPotion : Powerup {
        private const float ValuePotion = 20f;

        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Shield;
        }

        public override void UsePowerup() {
            base.UsePowerup();
            GameObject b = GameObject.FindGameObjectWithName("base_" + Owner.TeamIndex);

            if (b != null) {
                b.GetComponent<Base>().AddHealth(ValuePotion);
            }
        }
    }
}