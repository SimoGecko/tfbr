using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.Elements {
    class PlantedMagnet : Component {
        ////////// magnet that slows down everything within its radius //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private const float ActionRadius = 3f;
        private const float StartDelay = 2f;
        private const float Duration = 60f;

        private List<PlayerMovement> _pMAffected;
        private bool _active;


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _active = false;
            _pMAffected = new List<PlayerMovement>();
            Invoke(StartDelay, () => _active = true);
            GameObject.Destroy(gameObject, Duration);
        }

        public override void Update() {
            if (_active)
                CheckSlowdownRadius();
        }




        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CheckSlowdownRadius() {
            foreach (var pm in _pMAffected) pm.SetSlowdown(false);
            _pMAffected.Clear();

            foreach (Player p in ElementManager.Instance.Players()) {
                if (InActionRadius(p.gameObject)) {
                    PlayerMovement pM = p.gameObject.GetComponent<PlayerMovement>();
                    pM.SetSlowdown(true);
                    _pMAffected.Add(pM);
                }
            }
        }


        // queries
        bool InActionRadius(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= ActionRadius * ActionRadius;
        }


        // other

    }
}