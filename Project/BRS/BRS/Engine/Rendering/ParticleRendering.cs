// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Particles;

namespace BRS.Engine.Rendering {
    public static class ParticleRendering {

        #region Properties and attributes

        //reference
        private static readonly List<ParticleSystem3D> ParticleSystems = new List<ParticleSystem3D>();

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Load all needed effects
        /// </summary>
        public static void Start() {
            //_instanceEffect = File.Load<Effect>("Other/shaders/instancedModel");
        }

        /// <summary>
        /// Reset all instances so that no game-object belongs to any hardware-instance
        /// </summary>
        public static void Reset() {
            ParticleSystems.Clear();
        }

        /// <summary>
        /// Update the informration which are needed for the instance-drawing
        /// </summary>
        public static void Update() {
            //foreach (var keyValue in ModelTransformations) {
            //    keyValue.Value.Update();
            //}
        }

        /// <summary>
        /// Draw all the models with hardware-instancing
        /// </summary>
        public static void Draw() {
            foreach (ParticleSystem3D particleSystem in ParticleSystems) {
                particleSystem.Draw3D();
            }
        }

        #endregion

        #region Instanciation-handling

        /// <summary>
        /// Add a particle-system to render
        /// </summary>
        /// <param name="particleSystem">New particle-system</param>
        public static void AddInstance(ParticleSystem3D particleSystem) {
            ParticleSystems.Add(particleSystem);
        }

        /// <summary>
        /// Remove a particle-system to not be drawn anymore
        /// </summary>
        /// <param name="particleSystem">Particle-system to remove</param>
        public static void RemoveInstance(ParticleSystem3D particleSystem) {
            ParticleSystems.Remove(particleSystem);
        }

        #endregion

    }
}
