// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts {
    class CircleBar : Component {
        ////////// represents a circle bar //////////

        // --------------------- VARIABLES ---------------------

        //public
        const int width = 128;
        const int height = 128;

        //private
        static float[] gradient;
        static Color[] wheel;
        static Color clear;

        public static Texture2D result;
        public Texture2D circleBg;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            circleBg = File.Load<Texture2D>("Images/UI/circle_bg");
            Texture2D circleFg = File.Load<Texture2D>("Images/UI/circle_fg");
            Texture2D circleGr = File.Load<Texture2D>("Images/UI/circle_gradient");
            Initialize(circleFg, circleGr, Game1.instance.GraphicsDevice);
        }

        public override void Update() {
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(SpriteBatch spriteBatch, float percent) {
            spriteBatch.Draw(circleBg, new Vector2(300, 200), Color.White);
            spriteBatch.Draw(Mix(percent), new Vector2(300, 200), Color.White);
        }

        public static void Initialize(Texture2D fg, Texture2D gr, GraphicsDevice gd) {
            gradient = new float[width * height];
            wheel = new Color[width * height];
            Color[] gtemp = new Color[width * height];
            fg.GetData(wheel);
            gr.GetData(gtemp);
            for (int i = 0; i < width * height; i++)
                gradient[i] = gtemp[i].R;

            clear = new Color(0, 0, 0, 0);
            result = new Texture2D(gd, width, height);
        }


        public static Texture2D Mix(float amount) {
            Color[] res = new Color[width*height];
            for(int i=0; i<width*height; i++) {
                //TODO make smooth & deal with bordercases (0, 1)
                    res[i] = (gradient[i]<=amount*255) ? wheel[i] : clear;
            }
            result.SetData(res);
            return result;
        }


        // queries



        // other

    }

}