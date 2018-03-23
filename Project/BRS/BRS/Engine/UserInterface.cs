﻿// (c) Simone Guggiari 2018
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
        SpriteFont winnerFont;
        Texture2D bar;
        Texture2D[] powerupsPng = new Texture2D[6];
        Texture2D policeCar, policeLight;
        Dictionary<string, int> mapNamePowerupIndexPng = new Dictionary<string, int>();
        string[] namePowerupsPng = { "bomb", "key", "capacity", "speed", "health", "shield" };

        const int BARWIDTH = 256; const int BARHEIGHT = 16;
        public Timer roundtime;
        bool showWinner;
        bool showPolice;
        string winnerString = "";

        PlayerUI[] playerUI;

        //circle UI
        Texture2D circleBg, circleFg, circleGr;


        //reference
        static ContentManager Content;



        // --------------------- BASE METHODS ------------------
        public void Start() {
            showWinner = showPolice = false;
            instance = this;
            myfont = Content.Load<SpriteFont>("font1");
            winnerFont = Content.Load<SpriteFont>("font2");
            bar = Content.Load<Texture2D>("progress_bar");
            policeCar = Content.Load<Texture2D>("images/policecar");
            policeLight = Content.Load<Texture2D>("images/policecar_lights");

            playerUI = new PlayerUI[GameManager.numPlayers];

            for (int i = 0; i < namePowerupsPng.Length; ++i) {
                powerupsPng[i] = Content.Load<Texture2D>("images/" + namePowerupsPng[i] + "_pic");
                if (!mapNamePowerupIndexPng.ContainsKey(namePowerupsPng[i]))
                    mapNamePowerupIndexPng.Add(namePowerupsPng[i], i);
            }

            circleBg = Content.Load<Texture2D>("images/circle_bg");
            circleFg = Content.Load<Texture2D>("images/circle_fg");
            circleGr = Content.Load<Texture2D>("images/circle_gradient");
            CircleBar.Initialize(circleFg, circleGr, Game1.instance.GraphicsDevice);



        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            Vector2 centerPos = new Vector2(Screen.WIDTH / 2 - 100, Screen.HEIGHT / 2 - 100);
            spriteBatch.DrawString(myfont, "round: " + roundtime.span.ToReadableString(), centerPos, Color.White);

            //police bar
            Rectangle fgrect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
            Rectangle bgrect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);
            fgrect.Width = (int)(BARWIDTH * (1 - roundtime.span.TotalSeconds / GameManager.roundTime));
            spriteBatch.Draw(bar, centerPos + new Vector2(0, 60), bgrect, Color.White);
            spriteBatch.Draw(bar, centerPos + new Vector2(0, 60), fgrect, Color.Gray);
            spriteBatch.Draw(policeCar, centerPos + new Vector2(fgrect.Width, 67), null, Color.White, 0, Vector2.One * 64, .6f, SpriteEffects.None, 1);

            if (showPolice) {
                spriteBatch.DrawString(myfont, "Get back to your base!", centerPos + new Vector2(-50, 100), Color.White);
                if ((Time.frame / 10) % 2 == 0)
                    spriteBatch.Draw(policeLight, centerPos + new Vector2(fgrect.Width, 67), null, Color.White, 0, Vector2.One * 64, .6f, SpriteEffects.None, 1);
            }

            if (Minimap.instance != null) {
                Minimap.instance.Draw(spriteBatch);
            }

            if (showWinner) {
                spriteBatch.DrawString(winnerFont, winnerString, new Vector2(Screen.WIDTH / 2 - 200, Screen.HEIGHT / 2), Color.White);
            }


            //TRY CIRCLE
            spriteBatch.Draw(circleBg, new Vector2(300, 200), Color.White);
            spriteBatch.Draw(CircleBar.Mix(playerUI[0].stamina), new Vector2(300, 200), Color.White);
        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) {
            int offset = index % 2 == 0 ? 0 : +650;
            spriteBatch.DrawString(myfont, "cash: " + playerUI[index].totalMoneyInBase, new Vector2(10 + offset, 80), Color.White);
            spriteBatch.DrawString(myfont, "carrying: " + playerUI[index].carryingMoneyValue, new Vector2(10 + offset, 120), Color.White);


            Rectangle fgrect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
            Rectangle bgrect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);

            //health
            fgrect.Width = (int)(BARWIDTH * playerUI[index].healthPercent);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 170), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 170), fgrect, Color.Green);
            spriteBatch.DrawString(myfont, playerUI[index].health + "/" + playerUI[index].maxHealth, new Vector2(75 + offset, 160), Color.White);
            //stamina
            fgrect.Width = (int)(BARWIDTH * playerUI[index].staminaPercent);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 220), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 220), fgrect, Color.Red);
            spriteBatch.DrawString(myfont, Math.Round(playerUI[index].stamina * 100) + "/" + playerUI[index].maxStamina * 100, new Vector2(75 + offset, 210), Color.White);
            //capacity
            fgrect.Width = (int)(BARWIDTH * playerUI[index].carryingPercent);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 270), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 270), fgrect, Color.Blue);
            spriteBatch.DrawString(myfont, playerUI[index].carryingWeight + "/" + playerUI[index].maxCarrying, new Vector2(100 + offset, 260), Color.White);
            //base health
            fgrect.Width = (int)(BARWIDTH * playerUI[index].baseHealthPercent);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 320), bgrect, Color.White);
            spriteBatch.Draw(bar, new Vector2(10 + offset, 320), fgrect, Color.Orange);
            spriteBatch.DrawString(myfont, playerUI[index].baseHealth + "/" + playerUI[index].baseMaxHealth, new Vector2(75 + offset, 310), Color.White);
            //power ups
            Rectangle powerupRectDestination = new Rectangle(10 + offset, 370, 50, 50);
            if (playerUI[index].currentPowerup != null) {
                foreach (string name in playerUI[index].currentPowerup) {
                    spriteBatch.Draw(powerupsPng[mapNamePowerupIndexPng[name]], powerupRectDestination, Color.AliceBlue);
                }
            } else {
                playerUI[index].currentPowerup = new List<string>();
            }

        }

        //public void Draw() { }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void GiveContent(ContentManager c) {
            Content = c;
        }

        //PLAYER
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


        //BASE
        public void UpdateBaseUI(int index, float baseHealth, float baseStartingHealth, int value) {
            if (playerUI.Length > index) {
                playerUI[index].baseHealthPercent =
                    baseHealth / baseStartingHealth; // TODO make this relative to base, not player!!
                playerUI[index].baseHealth = baseHealth;
                playerUI[index].baseMaxHealth = baseStartingHealth;
                playerUI[index].totalMoneyInBase = value;
            }
        }

        //POWERUP
        public void UpdatePlayerPowerupUI(int index, string name, bool add) {
            if (add)
                playerUI[index].currentPowerup.Add(name);
            else
                playerUI[index].currentPowerup.Remove(name);
        }


        //WINNER
        public void UpdateGameWinnerUI(int winner) {
            winnerString = "Player " + winner + " won!";
            showWinner = true;
        }

        public void UpdatePoliceComing() {
            showPolice = true;
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
        public float healthPercent;     // green
        public float staminaPercent;    // red
        public float carryingPercent;   // blue
        public float baseHealthPercent; // yellow

        // max
        public float maxHealth;
        public float maxStamina;
        public int maxCarrying;
        public float baseMaxHealth;

        // current
        public int totalMoneyInBase;
        public int carryingMoneyValue;
        public float health;
        public float stamina;
        public int carryingWeight;
        public float baseHealth;

        //power ups
        public List<string> currentPowerup;

    }

}