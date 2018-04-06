using BRS.Engine.Physics;

namespace BRS.Scripts
{
    class OilTrap : Component {
        ////////// oiltrap that slows down enemy when collided with //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private const float SlowdownTime = 3f;
        private const float TimeToEffect = 1f;

        private bool _inUse = false;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _inUse = false;
            new Timer(TimeToEffect, () => _inUse = true);
        }

        public override void Update() {
        }

        public override void OnCollisionEnter(Collider c) {
            bool isplayer = c.GameObject.tag == ObjectTag.Player;

            if (isplayer && _inUse) {
                PlayerMovement pM = c.GameObject.GetComponent<PlayerMovement>();
                pM.SetSlowdown(true);
                new Timer(SlowdownTime, () => pM.SetSlowdown(false));
                GameObject.Destroy(GameObject);
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands


        // queries

        // other
    }
}