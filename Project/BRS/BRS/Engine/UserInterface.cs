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
        Texture2D bar;
        const int BARWIDTH = 256; const int BARHEIGHT = 16;
        public Timer roundtime;

        PlayerUI[] playerUI;

        //reference
        static ContentManager Content;



        // --------------------- BASE METHODS ------------------
        public void Start() {
            instance = this;
            myfont = Content.Load<SpriteFont>("font1");
            bar = Content.Load<Texture2D>("progress_bar");

            playerUI = new PlayerUI[GameManager.numPlayers];

        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(myfont, "round: " + roundtime.span.ToReadableString(), new Vector2(10, 40), Color.White);

        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) {
            spriteBatch.DrawString(myfont, "cash: " + playerUI[index].totalMoneyInBase, new Vector2(10, 80), Color.White);

            Rectangle fgrect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
            Rectangle bgrect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);

            //health
            fgrect.Width = (int)(BARWIDTH * playerUI[index].healthPercent);
            spriteBatch.Draw(bar, new Vector2(10, 120), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10, 120), fgrect, Color.Green);
            //stamina
            fgrect.Width = (int)(BARWIDTH * playerUI[index].staminaPercent);
            spriteBatch.Draw(bar, new Vector2(10, 160), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10, 160), fgrect, Color.Red);
            //capacity
            fgrect.Width = (int)(BARWIDTH * playerUI[index].carryingPercent);
            spriteBatch.Draw(bar, new Vector2(10, 200), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10, 200), fgrect, Color.Blue);
        }

        //public void Draw() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void GiveContent(ContentManager c) {
            Content = c;
        }

        public void UpdatePlayerUI(int index, float health, float stamina, float carrying) {
            playerUI[index].healthPercent = health;
            playerUI[index].staminaPercent = stamina;
            playerUI[index].carryingPercent = carrying;
        }
        public void SetPlayerMoneyBase(int v, int index) {
            playerUI[index].totalMoneyInBase = v;
        }


        // queries



        // other

    }

    public struct PlayerUI {
        public int totalMoneyInBase;
        public float healthPercent; // green
        public float staminaPercent;//red
        public float carryingPercent;//blue
    }

}