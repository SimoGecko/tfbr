// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;


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

            if (Minimap.instance != null) {
                Minimap.instance.Draw(spriteBatch);
            }
        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) {
            spriteBatch.DrawString(myfont, "cash: " + playerUI[index].totalMoneyInBase, new Vector2(10, 80), Color.White);
            spriteBatch.DrawString(myfont, "carrying: " + playerUI[index].carryingMoneyValue, new Vector2(10, 120), Color.White);


            Rectangle fgrect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
            Rectangle bgrect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);

            //health
            fgrect.Width = (int)(BARWIDTH * playerUI[index].healthPercent);
            spriteBatch.Draw(bar, new Vector2(10, 170), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10, 170), fgrect, Color.Green);
            spriteBatch.DrawString(myfont, playerUI[index].health + "/" + playerUI[index].maxHealth, new Vector2(75, 160), Color.White);
            //stamina
            fgrect.Width = (int)(BARWIDTH * playerUI[index].staminaPercent);
            spriteBatch.Draw(bar, new Vector2(10, 220), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10, 220), fgrect, Color.Red);
            spriteBatch.DrawString(myfont, Math.Round(playerUI[index].stamina * 100) + "/" + playerUI[index].maxStamina * 100, new Vector2(75, 210), Color.White);
            //capacity
            fgrect.Width = (int)(BARWIDTH * playerUI[index].carryingPercent);
            spriteBatch.Draw(bar, new Vector2(10, 270), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10, 270), fgrect, Color.Blue);
            spriteBatch.DrawString(myfont, playerUI[index].carryingWeight + "/" + playerUI[index].maxCarrying, new Vector2(100, 260), Color.White);
        }

        //public void Draw() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void GiveContent(ContentManager c) {
            Content = c;
        }

        public void UpdatePlayerUI(int index, float health, float startingHealth, float stamina, float maxStamina, int capacity, int carryingValue, int carryingWeight) {
            // percent
            playerUI[index].healthPercent = health / startingHealth;
            playerUI[index].staminaPercent = stamina / maxStamina;
            playerUI[index].carryingPercent = (float)carryingWeight / capacity;

            // max
            playerUI[index].maxHealth = startingHealth;
            playerUI[index].maxStamina = maxStamina;
            playerUI[index].maxCarrying = capacity;

            // current
            playerUI[index].health = health;
            playerUI[index].stamina = stamina;
            playerUI[index].carryingMoneyValue = carryingValue;
            playerUI[index].carryingWeight = carryingWeight;

        }
        public void SetPlayerMoneyBase(int v, int index) {
            playerUI[index].totalMoneyInBase = v;
        }

        //GENERAL ACCESS
        public Texture2D LoadTexture2D(string name) {
            return Content.Load<Texture2D>(name);
        }


        // queries



        // other

    }

    public struct PlayerUI {
        // percent
        public float healthPercent; // green
        public float staminaPercent;//red
        public float carryingPercent;//blue

        // max
        public float maxHealth;
        public float maxStamina;
        public int maxCarrying;

        // current
        public int totalMoneyInBase;
        public int carryingMoneyValue;
        public float health;
        public float stamina;
        public int carryingWeight;

    }

}