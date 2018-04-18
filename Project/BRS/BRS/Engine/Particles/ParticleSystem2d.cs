// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Particles {
    public class ParticleSystem2d : Component {
        ////////// class that represents a 2D particle engine for various effects //////////
        //TODO test
        //TODO make 3d

        // --------------------- VARIABLES ---------------------

        //public
        public Vector2 EmitterLocation { get; set; }


        //private
        private readonly Random _random;
        private readonly List<Particle2d> _particles;
        private readonly List<Texture2D> _textures;

        //extra
        float emitVelocity = 2;
        public float EmitRate = 100;
        //emitShape / angle
        Color range; // or color over lifetime

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {
            int numNewParticles = (int)(EmitRate * Time.DeltaTime);

            for (int i = 0; i < numNewParticles; i++) {
                _particles.Add(GenerateNewParticle());
            }

            for (int i = 0; i < _particles.Count; i++) {
                _particles[i].Update();
                if (_particles[i].Lifetime <= 0) {
                    _particles.RemoveAt(i);
                    i--; // not to skip particles to update
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) { // TODO make sure it's called
            spriteBatch.Begin(); // add effects here
            foreach (Particle2d particle in _particles) {
                particle.Draw(spriteBatch);
            }
            spriteBatch.End();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public ParticleSystem2d(List<Texture2D> textures, Vector2 location) {
            EmitterLocation = location;
            _textures = textures;
            _particles = new List<Particle2d>();
            _random = new Random();
        }

        private Particle2d GenerateNewParticle() {
            Texture2D texture = _textures[_random.Next(_textures.Count)];
            return new Particle2d(texture, EmitLocation(), EmitVelocity(), StartAngle(), StartAngularVelocity(), StartColor(), StartSize(), Lifetime());
        }

        // queries
        Vector2 EmitLocation() {
            return EmitterLocation;
        }

        Vector2 EmitVelocity() {
            return new Vector2(
                    emitVelocity * (float)(_random.NextDouble() * 2 - 1),
                    emitVelocity * (float)(_random.NextDouble() * 2 - 1));
        }

        float StartSize() {
            return (float)_random.NextDouble();
        }

        float Lifetime() {
            return 20 + _random.Next(40);
        }

        Color StartColor() {
            return new Color(
                    (float)_random.NextDouble(),
                    (float)_random.NextDouble(),
                    (float)_random.NextDouble());
        }

        float StartAngle() {
            return 0f;
        }

        float StartAngularVelocity() {
            return 0.1f * (float)(_random.NextDouble() * 2 - 1);
        }


        // other

    }
}
