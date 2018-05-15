using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Particles;
using BRS.Scripts.Elements;
using BRS.Scripts.Particles3D;

namespace BRS.Scripts.PlayerScripts {
    class BaseParticles : Component { // why is this under player scripts?

        private enum PlayerParticleType { CashDrop };
        List<Type> _effects = new List<Type>() { typeof(CashDrop)};
        private ParticleComponent[] _particleComponents;

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

                //if (pc is Dust) _dust = pc as Dust;
                //if (pc is Boost) _boost = pc as Boost;
                //if (pc is Cash) _cash = pc as Cash;
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
                //Debug.Log(pc.GetType().Name + ": " + pc.IsEmitting);
                pc.Update();
            }
        }


        public override void Draw3D(Camera camera) {
            foreach (ParticleComponent pc in _particleComponents) {
                pc.Draw3D(camera);
            }
        }
    }
}
