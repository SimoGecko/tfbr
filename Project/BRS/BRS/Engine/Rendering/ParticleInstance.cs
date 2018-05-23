// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.Linq;
using BRS.Engine.Particles;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Rendering {
    /// <summary>
    /// Stores the information for a particle system with mulitple simple emitters
    /// </summary>
    public class ParticleInstance {

        #region Properties and attributes

        /// <summary>
        /// Particle-System which is used to render and update the particles
        /// </summary>
        public readonly ParticleSystem3D ParticleSystem;

        /// <summary>
        /// Stores all emitters which are emitting for this system
        /// </summary>
        private readonly Dictionary<ParticleComponent, float> _emitters = new Dictionary<ParticleComponent, float>();

        /// <summary>
        /// Setting of the system
        /// </summary>
        private Settings Settings => ParticleSystem.Settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Instanciate a new particle-system
        /// </summary>
        /// <param name="particleSystem">System which is used for rendering</param>
        public ParticleInstance(ParticleSystem3D particleSystem) {
            ParticleSystem = particleSystem;
        }

        #endregion

        #region Collection-handling

        /// <summary>
        /// Add a new emitter to the particle-system.
        /// </summary>
        /// <param name="emitter">New emitter-component which adds particles to the system.</param>
        public void Add(ParticleComponent emitter) {
            _emitters.Add(emitter, Time.CurrentTime);
        }

        /// <summary>
        /// Remove an emitter from the particle-system.
        /// </summary>
        /// <param name="emitter">Emitter-component to remove.</param>
        public void RemoveInstance(ParticleComponent emitter) {
            _emitters.Remove(emitter);
        }

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Updates the particle-system by letting all emitters create new particles.
        /// </summary>
        public void Update() {
            // For each emitter in the list update the particles
            foreach (var keyValue in _emitters.ToList()) {
                // If the emitter is currently not emitting, ignore it
                if (!keyValue.Key.IsEmitting) {
                    continue;
                }

                // Only generate new particles if the emitter is allowed to
                float currentTime = Time.CurrentTime;

                if (currentTime > keyValue.Value + Settings.TimeBetweenRounds) {
                    Tuple<Vector3, Vector3> posVel = keyValue.Key.GetNextPosition();

                    for (int i = 0; i < Settings.ParticlesPerRound; ++i) {
                        ParticleSystem.AddSingleParticle(posVel.Item1, posVel.Item2);
                    }

                    _emitters[keyValue.Key] = currentTime;
                }
            }
        }

        #endregion
    }
}
