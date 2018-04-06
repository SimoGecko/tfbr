// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB


using BRS.Engine;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.PowerUps {
    class SpeedBoost : Powerup {
        private const float BoostTime = 3f;

        public override void Start() {
            base.Start();
            PowerupType = PowerupType.Speed;
        }

        public override void UsePowerup() {
            base.UsePowerup();
            PlayerMovement pm = Owner.GameObject.GetComponent<PlayerMovement>();
            pm.PowerupBoosting = true;
            new Timer(BoostTime, () => pm.PowerupBoosting = false);
        }
    }

}