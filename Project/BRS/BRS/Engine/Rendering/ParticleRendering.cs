﻿// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine.Particles;

namespace BRS.Engine.Rendering {
    public static class ParticleRendering {

        #region Properties and attributes

        //reference
        private static readonly List<ParticleSystem3D> ParticleSystems = new List<ParticleSystem3D>();
        private static readonly List<ParticleSystem3D> DepthParticleSystems = new List<ParticleSystem3D>();
        private static readonly Dictionary<ParticleType3D, ParticleInstance> ParticleSystems3D = new Dictionary<ParticleType3D, ParticleInstance>();

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
            foreach (var keyValue in ParticleSystems3D) {
                keyValue.Value.Update();
                keyValue.Value.ParticleSystem.Update();
            }
        }

        /// <summary>
        /// Draw all the models with hardware-instancing
        /// </summary>
        public static void Draw() {
            foreach (ParticleSystem3D particleSystem in ParticleSystems) {
                particleSystem.Draw3D();
            }
            foreach (var keyValue in ParticleSystems3D) {
                keyValue.Value.ParticleSystem.Draw3D();
            }
        }

        /// <summary>
        /// Draw all the models with hardware-instancing
        /// </summary>
        public static void DrawDepth() {
            foreach (ParticleSystem3D particleSystem in DepthParticleSystems) {
                particleSystem.Draw3D();
            }
        }

        #endregion

        #region Instanciation-handling

        /// <summary>
        /// Initialize a new model for hardware-instanciation
        /// </summary>
        /// <param name="particleType">Type of the particles to store uniquely</param>
        /// <param name="particleSystem">Particle-system</param>
        public static void Initialize(ParticleType3D particleType, ParticleSystem3D particleSystem) {
            particleSystem.Awake();
            particleSystem.Start();

            ParticleSystems3D[particleType] = new ParticleInstance(particleSystem);
        }

        /// <summary>
        /// Add a particle-system to render
        /// </summary>
        /// <param name="particleType">Particle-type on which the emitter is added</param>
        /// <param name="transform">Transform-object of the emitter</param>
        /// <param name="useForDepth">Used for depth rendering</param>
        public static ParticleSystem3D AddInstance(ParticleType3D particleType, ParticleComponent transform, bool useForDepth = false) {
            if (ParticleSystems3D.ContainsKey(particleType)) {
                ParticleSystems3D[particleType].AddInstance(transform);
                return ParticleSystems3D[particleType].ParticleSystem;
            } else {
                throw new Exception("Should not be here");
            }
            if (useForDepth) {
                //DepthParticleSystems.Add(particleSystem);
            }
        }

        /// <summary>
        /// Add a particle-system to render
        /// </summary>
        /// <param name="particleSystem">New particle-system</param>
        /// <param name="useForDepth">Used for depth rendering</param>
        public static void AddInstance(ParticleSystem3D particleSystem, bool useForDepth = false) {
            ParticleSystems.Add(particleSystem);

            if (useForDepth) {
                DepthParticleSystems.Add(particleSystem);
            }
        }

        /// <summary>
        /// Remove a particle-system to not be drawn anymore
        /// </summary>
        /// <param name="particleSystem">Particle-system to remove</param>
        public static void RemoveInstance(ParticleSystem3D particleSystem) {
            ParticleSystems.Remove(particleSystem);
            DepthParticleSystems.Remove(particleSystem);
        }

        #endregion

    }
}
