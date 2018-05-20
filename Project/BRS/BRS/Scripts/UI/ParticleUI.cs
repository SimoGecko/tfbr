// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BRS.Engine;
using System;

namespace BRS.Scripts {
    enum ParticleType {Debris, Diamond, Drill, Energy, Explosion, FireExplosion, Fireworks, PurpleExplosion, Sparks, Star, Stun, RotatingStars };

    class ParticleUI : Component {
        ////////// calls appropriate functions to display particle UI //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float defaultDistance = 14f; // at this distance the particle is drawn to original size


        //private
        List<ParticleOrder> particleOrders = new List<ParticleOrder>();
        //int[,] rowcols = new int[,] { { 8, 3 }, { 8, 3 }, { 4, 4 }, { 8, 4 }, { 7, 6 }, { 8, 8 }, { 4, 4 }, { 8, 8 }, { 4, 4 }, { 8, 3 }, { 4, 4 } };
        bool[] is128 = new bool[] { false, true, false, true, false, false, false, false, false, true, false, false }; // if not 128 then 256
        int numSpritesheets;

        //reference
        SpriteSheet[] spritesheets;
        public static ParticleUI Instance;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            numSpritesheets = Enum.GetNames(typeof(ParticleType)).Length;
            spritesheets = new SpriteSheet[numSpritesheets];

            for(int i=0; i<numSpritesheets; i++) {
                string fileName = ((ParticleType)i).ToString();
                fileName = Char.ToLowerInvariant(fileName[0]) + fileName.Substring(1); // first letter lowercase
                //spritesheets[i] = new SpriteSheet(File.Load<Texture2D>("Images/particles/" + fileName), rows, cols);
                spritesheets[i] = new SpriteSheet(File.Load<Texture2D>("Images/particles/" + fileName), is128[i]?128:256);
            }
        }

        public override void Update() {
            UpdateFrames();
        }

        public override void Reset() {
            if (particleOrders != null) particleOrders.Clear();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void UpdateFrames() {
            if (Time.Frame % 3 == 0) return; // updates once every 2 frames // now it's 30fps, not 15fps
            for (int i = 0; i < particleOrders.Count; i++) {
                ParticleOrder p = particleOrders[i];
                //if(Time.Frame%10==0) // slowmo
                p.frame++;
                if (SpriteSheetFromType(p.effect).FrameEnded(p.frame)) {
                    particleOrders.RemoveAt(i--);
                }
            }
        }

        public override void Draw2D(int index) {
            if (index == -1) return;

            foreach (ParticleOrder p in particleOrders) {
                Vector2 position = Camera.GetCamera(index).WorldToScreenPoint(p.position);

                float dist = (p.position - Camera.GetCamera(index).transform.position).Length();
                float scaling = defaultDistance / dist;
                SpriteSheetFromType(p.effect).Draw(position, p.frame, p.colorTint, p.scale*scaling);
            }
        }


        public void GiveOrder(Vector3 p, ParticleType t, float scale=1, Color? tint = null) {
            particleOrders.Add(new ParticleOrder(p, t, tint, scale));
        }

        // queries
        SpriteSheet SpriteSheetFromType(ParticleType t) {
            return spritesheets[(int)t];
        }


        // other
        class ParticleOrder {
            public Vector3 position;
            public int frame;
            public ParticleType effect;
            public Color colorTint;
            public float scale;
            public ParticleOrder(Vector3 p, ParticleType e, Color? tint = null, float sc=1) {
                frame = 0;
                position = p; effect = e;
                colorTint = tint ?? Color.White;
                scale = sc;
            }
        }

    }
}