// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace BRS.Scripts {
    class UserInterface {
        ////////// allows to draw interface in 2D //////////

            //TODO: move particular stuff into own class ("load")

        // --------------------- VARIABLES ---------------------

        //public
        public static UserInterface instance;

        //private
        SpriteFont myfont;
        Texture2D moneybar;
        public Timer roundtime;

        PlayerUI[] playerUI;

        //reference
        static ContentManager Content;



        // --------------------- BASE METHODS ------------------
        public void Start() {
            instance = this;
            myfont = Content.Load<SpriteFont>("font1");
            moneybar = Content.Load<Texture2D>("money_bar");

            playerUI = new PlayerUI[GameManager.numPlayers];

        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(myfont, "round: " + roundtime.span.ToReadableString(), new Vector2(10, 40), Color.White);

        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) {
            spriteBatch.DrawString(myfont, "cash: " + playerUI[index].totalMoneyInBase, new Vector2(10, 80), Color.White);

            Rectangle fgrect = new Rectangle(0, 0, (int)(291 * playerUI[index].playerMoneyPercent), 32);
            Rectangle bgrect = new Rectangle(0, 32, 291, 32);
            spriteBatch.Draw(moneybar, new Vector2(10, 120), bgrect, Color.White);
            spriteBatch.Draw(moneybar, new Vector2(10, 120), fgrect, Color.White);
        }

        //public void Draw() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void GiveContent(ContentManager c) {
            Content = c;
        }

        public void SetPlayerMoneyPercent(float v, int index) {
            playerUI[index].playerMoneyPercent = v;
        }
        public void SetPlayerMoneyBase(int v, int index) {
            playerUI[index].totalMoneyInBase = v;
        }


        // queries



        // other

    }

    public struct PlayerUI {
        public float playerMoneyPercent;
        public int totalMoneyInBase;
    }

}