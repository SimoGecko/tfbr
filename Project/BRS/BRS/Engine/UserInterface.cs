// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;


namespace BRS.Scripts {
    class UserInterface {
        ////////// acts as HUB to draw everything related to the UI, either in splitscreen (each window) or global (just once) //////////


        // --------------------- VARIABLES ---------------------

        public enum VerticalAnchor { Top, Middle, Bottom}; // how to align the UI
        public enum HorizontAnchor { Left, Middle, Right};
        //public

        //private
        SpriteFont smallFont;
        SpriteFont bigFont;
        Texture2D bar;
        Texture2D barBig;
        public const int BARWIDTH = 128;
        public const int BARBIGWIDTH = 256;
        public const int BARHEIGHT = 16;

        Rectangle barRect, bigRect;

        //reference
        public static UserInterface instance;
        SpriteBatch sB;



        // --------------------- BASE METHODS ------------------
        public void Start() {
            instance = this;
            smallFont = File.Load<SpriteFont>("Other/font/font1");
            bigFont   = File.Load<SpriteFont>("Other/font/font2");
            bar       = File.Load<Texture2D>("Images/UI/progress_bar_small");
            barBig    = File.Load<Texture2D>("Images/UI/progress_bar");

            barRect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);
            bigRect = new Rectangle(0, 0, BARBIGWIDTH, BARHEIGHT);
            //fgRect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            sB = spriteBatch;

            //callbacks
            Minimap.instance.Draw(sB);
            GameUI.instance.Draw();
            
        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) {
            sB = spriteBatch;

            BaseUI.instance.Draw(index%2);
            PlayerUI.instance.Draw(index);
            PowerupUI.instance.Draw(index);
        }





        // --------------------- CUSTOM METHODS ----------------

        //DRAW CALLBACKS
        public void DrawBar(Vector2 position, float percent, Color color) {
            barRect.Width = BARWIDTH;
            sB.Draw(bar, position, barRect, Color.LightGray);
            barRect.Width = (int)(BARWIDTH * percent);
            sB.Draw(bar, position, barRect, color);
        }
        public void DrawBarVertical(Vector2 position, float percent, Color color) {
            barRect.Width = BARWIDTH;
            //sB.Draw(bar, position, barRect, Color.LightGray);
            sB.Draw(bar, position, barRect, Color.LightGray, MathHelper.ToRadians(-90), Vector2.Zero, 1f, SpriteEffects.None, 1);
            barRect.Width = (int)(BARWIDTH * percent);
            //sB.Draw(bar, position, barRect, color);
            sB.Draw(bar, position, barRect, color, MathHelper.ToRadians(-90), Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
        public void DrawBarBig(Vector2 position, float percent, Color color) {
            bigRect.Width = BARBIGWIDTH;
            sB.Draw(barBig, position, bigRect, Color.LightGray);
            bigRect.Width = (int)(BARBIGWIDTH * percent);
            sB.Draw(barBig, position, bigRect, color);
        }


        public void DrawString(Vector2 position, string text) {
            sB.DrawString(smallFont, text, position, Color.White);
        }
        public void DrawStringBig(Vector2 position, string text) {
            sB.DrawString(bigFont, text, position, Color.White);
        }

        public void DrawPicture(Rectangle destination, Texture2D pic) {
            sB.Draw(pic, destination, Color.White);
        }
        public void DrawPicture(Vector2 position, Texture2D pic, Vector2 origin, float scale) {
            sB.Draw(pic, position, null, Color.White, 0, origin, scale, SpriteEffects.None, 1);
        }

        public int GetOffset(int index) { return index % 2 == 0 ? 0 : +680; }



        [Flags]
        public enum Alignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

        public void DrawStringAlign(SpriteFont font, string text, Rectangle bounds, Alignment align, Color color) {
            Vector2 size = smallFont.MeasureString(text);
            Vector2 pos = bounds.GetCenter();
            Vector2 origin = size * 0.5f;

            if (align.HasFlag(Alignment.Left))   origin.X += bounds.Width / 2  - size.X / 2;
            if (align.HasFlag(Alignment.Right))  origin.X -= bounds.Width / 2  - size.X / 2;
            if (align.HasFlag(Alignment.Top))    origin.Y += bounds.Height / 2 - size.Y / 2;
            if (align.HasFlag(Alignment.Bottom)) origin.Y -= bounds.Height / 2 - size.Y / 2;

            sB.DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        }

    }


}