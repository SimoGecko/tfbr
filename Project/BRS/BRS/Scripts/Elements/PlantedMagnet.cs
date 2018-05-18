using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.PostProcessing;
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
        private const float soundDuration = 1.5f;

        private List<PlayerMovement> _pMAffected;
        private bool _active;
        private bool canPlaySound = true;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _active = false;
            _pMAffected = new List<PlayerMovement>();
            Invoke(StartDelay, () => _active = true);
            Audio.Play("active_magnet", transform.position);
            ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Energy, 1.5f);
            new Timer(Duration, () => RemoveMagnet(), boundToRound:true);

            PostProcessingManager.Instance.ActivateWave(transform.position, deactivate: true, deactivateAfter: Duration);
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

            bool playSound = false;


            foreach (Player p in ElementManager.Instance.Players()) {
                if (InActionRadius(p.gameObject)) {
                    playSound = true;
                    PlayerMovement pM = p.gameObject.GetComponent<PlayerMovement>();
                    pM.SetSlowdown(true);
                    _pMAffected.Add(pM);
                }
            }
            if (playSound && canPlaySound) {
                Audio.Play("active_magnet", transform.position);
                ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Energy, 1.5f);
                canPlaySound = false;
                new Timer(soundDuration, () => canPlaySound = true);
            }

        }

        void RemoveMagnet() {
            foreach (var pm in _pMAffected) pm.SetSlowdown(false);
            ElementManager.Instance.Remove(this.gameObject);
            GameObject.Destroy(gameObject);

        }


        // queries
        bool InActionRadius(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= ActionRadius * ActionRadius;
        }


        // other

    }
}