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

        //public

        //private
        SpriteFont smallFont;
        SpriteFont bigFont;
        Texture2D bar;
        public const int BARWIDTH = 256;
        public const int BARHEIGHT = 16;

        Rectangle bgRect, fgRect;

        //reference
        public static UserInterface instance;
        SpriteBatch sB;



        // --------------------- BASE METHODS ------------------
        public void Start() {
            instance = this;
            smallFont = File.Load<SpriteFont>("Other/font/font1");
            bigFont   = File.Load<SpriteFont>("Other/font/font2");
            bar       = File.Load<Texture2D>("Images/UI/progress_bar");

            bgRect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);
            fgRect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            sB = spriteBatch;

            //callbacks
            Minimap.instance.Draw(sB);
            GameUI.instance.Draw();
            
        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) {
            sB = spriteBatch;

            BaseUI.instance.Draw(index);
            PlayerUI.instance.Draw(index);
            PowerupUI.instance.Draw(index);
        }





        // --------------------- CUSTOM METHODS ----------------

        //DRAW CALLBACKS
        public void DrawBar(Vector2 position, float percent, Color color) {
            fgRect.Width = (int)(BARWIDTH * percent);
            sB.Draw(bar, position, bgRect, Color.White);
            sB.Draw(bar, position, fgRect, color);
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

        public int GetOffset(int index) { return index % 2 == 0 ? 0 : +650; }
    }


}