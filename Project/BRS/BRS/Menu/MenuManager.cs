using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BRS.Load;
using BRS.Engine.Physics;
using BRS.Scripts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Menu {
    class MenuManager {
        // each game object is a Panel (main menu, play1, play2, tuto1, tuto2, tuto3, options, credits)
        Dictionary<string, GameObject> menuRect = new Dictionary<string, GameObject>();
        GameObject currentMenu;

        GameObject mainMenu = new GameObject("main");
        GameObject playMenu1 = new GameObject("play1");
        GameObject playMenu2 = new GameObject("play2");
        GameObject tutoMenu1 = new GameObject("tutorial1");
        GameObject tutoMenu2 = new GameObject("tutorial2");
        GameObject tutoMenu3 = new GameObject("tutorial3");
        GameObject tutoMenu4 = new GameObject("tutorial4");
        GameObject optionsMenu = new GameObject("options");
        GameObject creditsMenu = new GameObject("credits");

        Texture2D textureButton;
        Texture2D textureButtonSlider;
        Texture2D textureTickBoxCliqued;
        Texture2D textureTickBoxNotCliqued;

        public static MenuManager instance;

        public void LoadContent() {
            instance = this;
            textureButton = File.Load<Texture2D>("Images/UI/button");
            textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            textureTickBoxCliqued = File.Load<Texture2D>("Images/UI/tickbox_clicked"); ;
            textureTickBoxNotCliqued = File.Load<Texture2D>("Images/UI/tickbox_notclicked"); ;

            //// BACK BUTTON ////
            var backButton = new Button(textureButton) {
                Position = new Vector2(200, 700),
                Text = "go back",
                NameMenuToSwitchTo = "main",
            };
            backButton.Click += SwitchToMenu;

            //// MAIN MENU ////
            var playButton = new Button(textureButton) {
                Position = new Vector2(200, 200),
                Text = "Play",
                NameMenuToSwitchTo = "play1",
            };
            playButton.Click += SwitchToMenu;

            var TutorialButton = new Button(textureButton) {
                Position = new Vector2(200, 300),
                Text = "Tutorial",
                NameMenuToSwitchTo = "tutorial1",
            };
            TutorialButton.Click += SwitchToMenu;

            var optionsButton = new Button(textureButton) {
                Position = new Vector2(200, 400),
                Text = "Options",
                NameMenuToSwitchTo = "options",
            };
            optionsButton.Click += SwitchToMenu;

            var creditsButton = new Button(textureButton) {
                Position = new Vector2(200, 500),
                Text = "Credits",
                NameMenuToSwitchTo = "credits",
            };
            creditsButton.Click += SwitchToMenu;

            mainMenu.AddComponent(playButton);
            mainMenu.AddComponent(TutorialButton);
            mainMenu.AddComponent(optionsButton);
            mainMenu.AddComponent(creditsButton);

            currentMenu = mainMenu;

            //// Play MENU 1////
            var chooseNumberPlayerText = new TextBox() {
                Position = new Vector2(200, 100),
                Text = "Number of players",
            };

            var player2Button = new Button(textureButton) {
                Position = new Vector2(200, 200),
                Text = "2",
            };
            player2Button.Click += UpdateNoPlayers;

            var player4Button = new Button(textureButton) {
                Position = new Vector2(500, 200),
                Text = "4",
            };
            player4Button.Click += UpdateNoPlayers;

            var chooseDurationRoundText = new TextBox() {
                Position = new Vector2(200, 300),
                Text = "Game duration",
            };

            var round2MinButton = new Button(textureButton) {
                Position = new Vector2(200, 400),
                Text = "2 min",
            };
            round2MinButton.Click += UpdateRoundDuration;

            var round3MinButton = new Button(textureButton) {
                Position = new Vector2(500, 400),
                Text = "3 min",
            };
            round3MinButton.Click += UpdateRoundDuration;

            var round5MinButton = new Button(textureButton) {
                Position = new Vector2(800, 400),
                Text = "5 min",
            };
            round5MinButton.Click += UpdateRoundDuration;

            var nextButton = new Button(textureButton) {
                Position = new Vector2(200, 500),
                Text = "next",
                NameMenuToSwitchTo = "play2",
            };
            nextButton.Click += SwitchToMenu;

            playMenu1.AddComponent(chooseNumberPlayerText); playMenu1.AddComponent(player2Button); playMenu1.AddComponent(player4Button);
            playMenu1.AddComponent(chooseDurationRoundText); playMenu1.AddComponent(round2MinButton); playMenu1.AddComponent(round3MinButton); playMenu1.AddComponent(round5MinButton);
            playMenu1.AddComponent(nextButton);
            playMenu1.AddComponent(backButton);
            playMenu1.active = false;

            //// Play MENU 2////
            //TODO: image characters with stats, button to choose character, button to start game

            var startGameButton = new Button(textureButton) {
                Position = new Vector2(200, 200),
                Text = "Start Game",
            };
            startGameButton.Click += StartGameFunction;

            playMenu2.AddComponent(startGameButton);
            playMenu2.AddComponent(backButton);
            playMenu2.active = false;

            //// Tutorial MENU 1////
            var pictureControl = new Image(File.Load<Texture2D>("Images/UI/xBox_ controller")) {
                Position = new Vector2(0, 0),
            };

            var nextButtonTuto1 = new Button(textureButton) {
                Position = new Vector2(200, 500),
                Text = "next",
                NameMenuToSwitchTo = "tutorial2",
            };
            nextButtonTuto1.Click += SwitchToMenu;

            tutoMenu1.AddComponent(pictureControl);
            tutoMenu1.AddComponent(nextButtonTuto1);
            tutoMenu1.AddComponent(backButton);
            tutoMenu1.active = false;

            //// Tutorial MENU 2////
            var picturGameView = new Image(File.Load<Texture2D>("Images/UI/gameView")) {
                Position = new Vector2(0, 0),
            };

            var nextButtonTuto2 = new Button(textureButton) {
                Position = new Vector2(200, 500),
                Text = "next",
                NameMenuToSwitchTo = "tutorial3",
            };
            nextButtonTuto2.Click += SwitchToMenu;

            tutoMenu2.AddComponent(picturGameView);
            tutoMenu2.AddComponent(nextButtonTuto2);
            tutoMenu2.AddComponent(backButton);
            tutoMenu2.active = false;

            //// Tutorial MENU 3////
            var pictureMinimap = new Image(File.Load<Texture2D>("Images/minimap/minimapDisplay")) {
                Position = new Vector2(0, 0),
            };

            var nextButtonTuto3 = new Button(textureButton) {
                Position = new Vector2(200, 500),
                Text = "next",
                NameMenuToSwitchTo = "tutorial4",
            };
            nextButtonTuto3.Click += SwitchToMenu;

            tutoMenu3.AddComponent(pictureMinimap);
            tutoMenu3.AddComponent(nextButtonTuto3);
            tutoMenu3.AddComponent(backButton);
            tutoMenu3.active = false;

            //// Tutorial MENU 4////
            var picturePowerUpsList = new Image(File.Load<Texture2D>("Images/powerup/powerups")) {
                Position = new Vector2(0, 0),
            };

            tutoMenu4.AddComponent(picturePowerUpsList);
            tutoMenu4.AddComponent(backButton);
            tutoMenu4.active = false;

            //// Options MENU ////
            var testSlider = new Slider() {
                Position = new Vector2(200, 500),

                buttonSlider = new Button(textureButtonSlider) {
                    Position = new Vector2(200 - textureButtonSlider.Width / 2, 500 - (textureButtonSlider.Height - UserInterface.BARHEIGHT) / 2),
                }
            };

            var testTickBox = new TickBox(textureTickBoxNotCliqued, textureTickBoxCliqued) {
                Position = new Vector2(200, 400),
            };

            optionsMenu.AddComponent(testSlider);
            optionsMenu.AddComponent(testTickBox);
            optionsMenu.AddComponent(backButton);
            optionsMenu.active = false;

            //// Credits MENU ////
            //TODO: list name + corresponding role
            creditsMenu.AddComponent(backButton);
            creditsMenu.active = false;

            //// Create Menu's dictionary
            menuRect.Add(mainMenu.name, mainMenu);
            menuRect.Add(playMenu1.name, playMenu1);
            menuRect.Add(playMenu2.name, playMenu2);
            menuRect.Add(tutoMenu1.name, tutoMenu1);
            menuRect.Add(tutoMenu2.name, tutoMenu2);
            menuRect.Add(tutoMenu3.name, tutoMenu3);
            menuRect.Add(tutoMenu4.name, tutoMenu4);
            menuRect.Add(optionsMenu.name, optionsMenu);
            menuRect.Add(creditsMenu.name, creditsMenu);
        }

        private void UpdateRoundDuration(object sender, EventArgs e) {
            Button button = (Button)sender;
            RoundManager.roundTime = Int32.Parse(button.Text[0].ToString()) * 60;
        }

        private void UpdateNoPlayers(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameManager.numPlayers = Int32.Parse(button.Text);
        }

        private void StartGameFunction(object sender, EventArgs e) {
            Game1.instance.menuDisplay = false;
            currentMenu.active = false;
            Game1.instance.Start();
        }

        private void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;
 
            //Transform goalTransform = camTransf[menu];
            if (currentMenu != null)
                currentMenu.active = false;

            currentMenu = menuRect[button.NameMenuToSwitchTo];
            currentMenu.active = true;
        }

        public void Update() {
            foreach (var go in GameObject.All)
                go.Update();
        }

        public void Draw() {
            foreach (var go in GameObject.All) 
                if (go.active)
                    foreach (var component in go.components)
                        component.Draw();
        }
    }
}
