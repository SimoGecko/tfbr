using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Particles;
using BRS.Engine.Rendering;
using BRS.Engine.Utilities;
using BRS.Scripts.Particles3D;
using Microsoft.Xna.Framework;

namespace BRS.Scripts.PlayerScripts {
    class PlayerParticles : Component {

        private enum PlayerParticleType { Dust, Boost, Cash }
        private readonly List<Type> _effects = new List<Type> { typeof(Dust), typeof(Boost), typeof(Cash) };
        private ParticleComponent[] _particleComponents;

        private string _oilTrack = ModelType.TracksOil.GetDescription();
        private string _speedTrack = ModelType.TracksSpeed.GetDescription();

        private PlayerAttack _playerAttack;
        private PlayerMovement _playerMovement;
        private PlayerInventory _playerInventory;
        private float _lastTrackEmitted;
        private Vector3 _lastTrackPosition;


        // --------------------- BASE METHODS ------------------

        public override void Awake() {
            _particleComponents = new ParticleComponent[_effects.Count];

            foreach (Type type in _effects) {
                ParticleComponent pc = (ParticleComponent)Activator.CreateInstance(type);
                pc.Awake();
                pc.gameObject = gameObject;

                PlayerParticleType ppt = (PlayerParticleType)Enum.Parse(typeof(PlayerParticleType), type.Name);
                _particleComponents[(int)ppt] = pc;
            }
        }

        public override void Start() {
            _playerAttack = gameObject.GetComponent<PlayerAttack>();
            _playerMovement = gameObject.GetComponent<PlayerMovement>();
            _playerInventory = gameObject.GetComponent<PlayerInventory>();

            foreach (ParticleComponent pc in _particleComponents) {
                pc.Start();
            }
        }

        public override void Update() {
            _particleComponents[(int)PlayerParticleType.Dust].IsEmitting = _playerMovement.Speed > 3.0f;
            _particleComponents[(int)PlayerParticleType.Boost].IsEmitting = _playerMovement.Boosting || _playerMovement.PowerupBoosting || _playerMovement.SpeedPad;
            _particleComponents[(int)PlayerParticleType.Cash].IsEmitting = _playerInventory.IsAlmostFull();

            foreach (ParticleComponent pc in _particleComponents) {
                pc.Update();
            }

            Vector3 distance = _lastTrackPosition - transform.position;

            if (_playerMovement.OilTracks && distance.LengthSquared() > 0.05f) {
                _lastTrackPosition = transform.position;
                GameObject.Instantiate(_oilTrack, transform);
            }

            if (_playerMovement.Boosting || _playerMovement.SpeedPad || _playerMovement.PowerupBoosting || _playerAttack.IsAttacking) {
                _lastTrackPosition = transform.position;
                GameObject.Instantiate(_speedTrack, transform);
            }
        }
    }
}
