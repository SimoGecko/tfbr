// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    class CircleBar : Component {
        ////////// represents a circle bar //////////

        // --------------------- VARIABLES ---------------------

        //public
        public static Texture2D Result;
        public Texture2D CircleBg;

        //private
        private static float[] _gradient;
        private static Color[] _wheel;
        private static Color _clear;

        // const
        private const int Width = 128;
        private const int Height = 128;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            CircleBg = File.Load<Texture2D>("Images/UI/circle_bg");
            Texture2D circleFg = File.Load<Texture2D>("Images/UI/circle_fg");
            Texture2D circleGr = File.Load<Texture2D>("Images/UI/circle_gradient");
            Initialize(circleFg, circleGr, Game1.Instance.GraphicsDevice);
        }

        public override void Update() {
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void Draw(SpriteBatch spriteBatch, float percent) {
            spriteBatch.Draw(CircleBg, new Vector2(300, 200), Color.White);
            spriteBatch.Draw(Mix(percent), new Vector2(300, 200), Color.White);
        }

        private static void Initialize(Texture2D fg, Texture2D gr, GraphicsDevice gd) {
            _gradient = new float[Width * Height];
            _wheel = new Color[Width * Height];
            Color[] gtemp = new Color[Width * Height];
            fg.GetData(_wheel);
            gr.GetData(gtemp);

            for (int i = 0; i < Width * Height; i++) {
                _gradient[i] = gtemp[i].R;
            }

            _clear = new Color(0, 0, 0, 0);
            Result = new Texture2D(gd, Width, Height);
        }


        private static Texture2D Mix(float amount) {
            Color[] res = new Color[Width * Height];

            for (int i = 0; i < Width * Height; i++) {
                //TODO make smooth & deal with bordercases (0, 1)
                res[i] = (_gradient[i] <= amount * 255) ? _wheel[i] : _clear;
            }

            Result.SetData(res);
            return Result;
        }


        // queries



        // other

    }

}