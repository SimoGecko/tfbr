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

        //const
        private const float ActionRadius = 3f;
        private const float StartDelay = 2f;
        private const float Duration = 60f;
        private const float EffectDuration = 1.5f;

        //private
        private bool _active;
        private bool _canPlayEffect;

        //reference
        private List<PlayerMovement> _pMAffected;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _active = false;
            Invoke(StartDelay, () => _active = true);
            _pMAffected = new List<PlayerMovement>();

            PlayEffect();

            new Timer(Duration, () => RemoveMagnet(), boundToRound:true);
            PostProcessingManager.Instance.ActivateWave(transform.position, deactivate: true, deactivateAfter: Duration);
        }

        public override void Update() {
            if (_active) {
                ClearSlowdown();
                FindPlayersInRange();
                SlowDown();
                if (SomeoneSlowedDown() && _canPlayEffect)
                    PlayEffect();
            }
        }




        // --------------------- CUSTOM METHODS ----------------


        // commands
        void ClearSlowdown() {
            foreach (var pm in _pMAffected) pm.SetSlowdown(false);
            _pMAffected.Clear();
        }

        void FindPlayersInRange() {
            foreach (Player p in ElementManager.Instance.Players()) {
                if (InActionRadius(p.gameObject)) {
                    PlayerMovement pM = p.gameObject.GetComponent<PlayerMovement>();
                    _pMAffected.Add(pM);
                }
            }
        }

        void SlowDown() {
            foreach (PlayerMovement pm in _pMAffected)
                pm.SetSlowdown(true);
        }

        void PlayEffect() {
            Audio.Play("active_magnet", transform.position);
            ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Energy, 1.5f);
            _canPlayEffect = false;
            new Timer(EffectDuration, () => _canPlayEffect = true);
        }

        void RemoveMagnet() {
            ClearSlowdown();
            ElementManager.Instance.Remove(this.gameObject);
            GameObject.Destroy(gameObject);
        }


        // queries
        bool InActionRadius(GameObject o) {
            return (o.transform.position - transform.position).LengthSquared() <= ActionRadius * ActionRadius;
        }
        bool SomeoneSlowedDown() {
            return _pMAffected.Count > 0;
        }


        // other

    }
}