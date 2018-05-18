// (c) Alexander Lelidis and Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Particles;
using BRS.Scripts.Elements;
using BRS.Scripts.Particles3D;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Particle-effect for base of the player
    /// </summary>
    class BaseParticles : Component { 
        // why is this under player scripts?
        // => because it defines the particles of the player base, we could move it to Scripts/Elements as well
        // used particle types for the base
        private enum PlayerParticleType { CashDrop };
        List<Type> _effects = new List<Type> { typeof(CashDrop)};
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
