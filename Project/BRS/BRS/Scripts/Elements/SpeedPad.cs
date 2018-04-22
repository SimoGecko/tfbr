// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.Elements {
    class SpeedPad : Component {
        ////////// represents a speed pad that once passed over gives a quick speed boost //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private const float EffectTime = 2f;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            bool isplayer = c.GameObject.tag == ObjectTag.Player;
            if (isplayer) {
                PlayerMovement pM = c.GameObject.GetComponent<PlayerMovement>();
                pM.SetSpeedPad(true);
                Audio.Play("speedpad", transform.position);

                new Timer(EffectTime, () => pM.SetSpeedPad(false));
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands



        // queries



        // other

    }
}