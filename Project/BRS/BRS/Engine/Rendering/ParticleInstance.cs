using System;
using System.Collections.Generic;
using System.Linq;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Rendering {
    public class ParticleInstance {
        public readonly ParticleSystem3D ParticleSystem;
        public readonly Dictionary<ParticleComponent, float> Positions = new Dictionary<ParticleComponent, float>();
        public List<float> Times = new List<float>();

        public ParticleInstance(ParticleSystem3D particleSystem) {
            ParticleSystem = particleSystem;
        }

        public void AddInstance(ParticleComponent transform) {
            Positions.Add(transform, Time.CurrentTime);
        }

        public void RemoveInstance(ParticleComponent transform) {
            Positions.Remove(transform);
        }

        public void Update() {
            
            foreach (var keyValue in Positions.ToList()) {
                float currentTime = Time.CurrentTime;

                if (currentTime > keyValue.Value + ParticleSystem.Settings.TimeBetweenRounds) {
                    List<Tuple<Vector3, Vector3>> newParticles = keyValue.Key.GetNextPositions();
                    foreach (var particle in newParticles) {
                        ParticleSystem.AddSingleParticle(particle.Item1, particle.Item2);
                    }

                    Positions[keyValue.Key] = currentTime;
                }
            }
        }
    }
}
