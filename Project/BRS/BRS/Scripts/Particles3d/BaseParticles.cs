// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Particles;
using BRS.Scripts.Elements;

namespace BRS.Scripts.Particles3D {
    /// <summary>
    /// Particle-effect for base of the player
    /// </summary>
    class BaseParticles : Component {
        private enum PlayerParticleType { CashDrop };

        readonly List<Type> _effects = new List<Type> { typeof(CashDrop)};
        private ParticleComponent[] _particleComponents;

        // base where the particle emitting systems are attached to
        private Base _base;
        // --------------------- BASE METHODS ------------------

        public override void Awake() {
            _particleComponents = new ParticleComponent[_effects.Count];

            foreach (Type type in _effects) {
                ParticleComponent pc = (ParticleComponent)Activator.CreateInstance(type);
                pc.Awake();
                pc.gameObject = gameObject;

                PlayerParticleType ppt = (PlayerParticleType) Enum.Parse(typeof(PlayerParticleType), type.Name);
                _particleComponents[(int)ppt] = pc;
            }
        }

        public override void Start() {
            _base = gameObject.GetComponent<Base>();
            
            foreach (ParticleComponent pc in _particleComponents) {
                pc.Start();
            }
        }

        public override void Update() {
            _particleComponents[(int)PlayerParticleType.CashDrop].IsEmitting = _base.FullDeloadDone;

            foreach (ParticleComponent pc in _particleComponents) {
                pc.Update();
            }
        }
    }
}
