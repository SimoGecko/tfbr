// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BRS {
    public class ParticleSystem : Component {
        ////////// class that represents a 2D particle engine for various effects //////////
        //TODO test
        //TODO make 3d

        // --------------------- VARIABLES ---------------------

        //public
        public Vector2 EmitterLocation { get; set; }


        //private
        private Random random;
        private List<Particle> particles;
        private List<Texture2D> textures;

        //extra
        float emitVelocity;
        public float EmitRate = 100;
        //emitShape / angle
        Color range; // or color over lifetime

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {
            int numNewParticles = (int)(EmitRate*Time.deltatime);

            for (int i = 0; i < numNewParticles; i++) {
                particles.Add(GenerateNewParticle());
            }

            for (int i = 0; i < particles.Count; i++) {
                particles[i].Update();
                if (particles[i].Lifetime <= 0) {
                    particles.RemoveAt(i);
                    i--; // not to skip particles to update
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) { // TODO make sure it's called
            spriteBatch.Begin(); // add effects here
            for (int i = 0; i < particles.Count; i++) {
                particles[i].Draw(spriteBatch);
            }
            spriteBatch.End();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public ParticleSystem(List<Texture2D> textures, Vector2 location) {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        private Particle GenerateNewParticle() {
            Texture2D texture = textures[random.Next(textures.Count)];
            return new Particle(texture, EmitLocation(), EmitVelocity(), StartAngle(), StartAngularVelocity(), StartColor(), StartSize(), Lifetime());
        }

        // queries
        Vector2 EmitLocation() {
            return EmitterLocation;
        }

        Vector2 EmitVelocity() {
            return new Vector2(
                    emitVelocity * (float)(random.NextDouble() * 2 - 1),
                    emitVelocity * (float)(random.NextDouble() * 2 - 1));
        }

        float StartSize() {
            return (float)random.NextDouble();
        }

        float Lifetime() {
            return 20 + random.Next(40);
        }

        Color StartColor() {
            return new Color(
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    (float)random.NextDouble());
        }

        float StartAngle() {
            return 0f;
        }

        float StartAngularVelocity() {
            return 0.1f * (float)(random.NextDouble() * 2 - 1);
        }


        // other
        public class Particle {
            public Texture2D Texture { get; set; }       
            public Vector2 Position { get; set; }        
            public Vector2 Velocity { get; set; }        
            public float Angle { get; set; }   
            public float AngularVelocity { get; set; }   
            public Color Color { get; set; }      
            public float Size { get; set; }                                                   
            public float Lifetime { get; set; }

            public Particle(Texture2D texture, Vector2 position, Vector2 velocity, float angle, float angularVelocity, Color color, float size, float lifetime) {
                Texture = texture;
                Position = position;
                Velocity = velocity;
                Angle = angle;
                AngularVelocity = angularVelocity;
                Color = color;
                Size = size;
                Lifetime = lifetime;
            }

            public void Update() {
                Lifetime -= Time.deltatime;
                Position += Velocity;
                Angle += AngularVelocity;
            }

            public void Draw(SpriteBatch spriteBatch) {
                Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
                Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
                spriteBatch.Draw(Texture, Position, sourceRectangle, Color, Angle, origin, Size, SpriteEffects.None, 0f);
            }

        }

    }
}
