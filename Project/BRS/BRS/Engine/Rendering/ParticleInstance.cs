using System;
using System.Collections.Generic;
using System.Linq;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Rendering {
    public class ParticleInstance {
        public readonly ParticleSystem3D ParticleSystem;
        public readonly Dictionary<ParticleComponent, float> Positions = new Dictionary<ParticleComponent, float>();
        private readonly Settings _settings;

        public ParticleInstance(ParticleSystem3D particleSystem) {
            ParticleSystem = particleSystem;
            _settings = ParticleSystem.Settings;
        }

        public void AddInstance(ParticleComponent transform) {
            Positions.Add(transform, Time.CurrentTime);
        }

        public void RemoveInstance(ParticleComponent transform) {
            Positions.Remove(transform);
        }

        public void Update() {

            foreach (var keyValue in Positions.ToList()) {
                if (!keyValue.Key.IsEmitting) {
                    continue;
                }

                float currentTime = Time.CurrentTime;

                if (currentTime > keyValue.Value + _settings.TimeBetweenRounds) {
                    Tuple<Vector3, Vector3> posVel = keyValue.Key.GetNextPosition();
                    for (int i = 0; i < _settings.ParticlesPerRound; ++i) {
                        ParticleSystem.AddSingleParticle(posVel.Item1, posVel.Item2);
                    }

                    Positions[keyValue.Key] = currentTime;
                }
            }
        }
    }
}
