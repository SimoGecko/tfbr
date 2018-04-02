using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRS;

namespace BRS.Menu {
    class Menu {

        Texture2D textureButton;
        Texture2D textureButtonSlider;
        Texture2D textureTickBoxCliqued;
        Texture2D textureTickBoxNotCliqued;

        public static Menu instance;

        Vector2 positionNextButton = new Vector2(Screen.WIDTH / 2 + 200, Screen.HEIGHT - 400);
        Vector2 positionBackButton = new Vector2(Screen.WIDTH / 2 - 200, Screen.HEIGHT - 400);
        Vector2 middleScreen = new Vector2(Screen.WIDTH / 2, Screen.HEIGHT / 2);

        public void LoadContent() {
            instance = this;

            textureButton = File.Load<Texture2D>("Images/UI/button");
            textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            textureTickBoxCliqued = File.Load<Texture2D>("Images/UI/tickbox_clicked"); ;
            textureTickBoxNotCliqued = File.Load<Texture2D>("Images/UI/tickbox_notclicked");
        }

        public void BuildMainMenu() {
            Vector2[] offset = { new Vector2(0, -200), new Vector2(0, -100), new Vector2(0, 0), new Vector2(0, 100), new Vector2(0, 200) };
            string[] textButtons = { "Play", "Tutorial", "Ranking", "Options", "Credits" };
            string[] switchTo = { "play1", "tutorial1", "ranking", "options", "credits" };
            
            for (int i=0; i<textButtons.Length; ++i) {
                var playButton = new Button(textureButton, middleScreen +  offset[i]) {
                    Text = textButtons[i],
                    NameMenuToSwitchTo = switchTo[i],
                };
                playButton.Click += MenuManager.instance.SwitchToMenu;
                MenuManager.instance.mainMenu.AddComponent(playButton);
            }
        }

        public void BuildPlayMenu() {

            string text = "Number of players";
            var chooseNumberPlayerText = new TextBox() {
                InitPos = middleScreen - new Vector2(0,-200),
                Text = text
            };

            Vector2[] offset = { new Vector2(-200, -100), new Vector2(200, -100), new Vector2(-175, 100), new Vector2(88, 100), new Vector2(350, 100) };
            string[] textButtons = { "2", "4", "2 min", "3 min", "5 min" };

            for (int i = 0; i < textButtons.Length; ++i) {
                var playButton = new Button(textureButton, middleScreen + offset[i]) {
                    Text = textButtons[i],
                };
                if (i < 2) playButton.Click += MenuManager.instance.UpdateNoPlayers;
                else {
                    playButton.Click += MenuManager.instance.UpdateRoundDuration;
                    playButton.ScaleWidth = .5f;
                }
                MenuManager.instance.playMenu1.AddComponent(playButton);
            }

            string text2 = "Game duration";
            var chooseDurationRoundText = new TextBox(){
                InitPos = middleScreen,
                Text = text2
            };

            var nextButton = new Button(textureButton, positionNextButton) {
                Text = "next", NameMenuToSwitchTo = "play2"
            };
            nextButton.Click += MenuManager.instance.SwitchToMenu;

            var backButton1 = new Button(textureButton, positionBackButton) {
                Text = "go back", NameMenuToSwitchTo = "main"
            };
            backButton1.Click += MenuManager.instance.SwitchToMenu;

            MenuManager.instance.playMenu1.AddComponent(chooseNumberPlayerText);
            MenuManager.instance.playMenu1.AddComponent(chooseDurationRoundText);
            MenuManager.instance.playMenu1.AddComponent(nextButton);
            MenuManager.instance.playMenu1.AddComponent(backButton1);
            MenuManager.instance.playMenu1.active = false;

            var startGameButton = new Button(textureButton, positionNextButton) { Text = "Start Game" };
            startGameButton.Click += MenuManager.instance.StartGameFunction;

            var backButton2 = new Button(textureButton, positionBackButton) { Text = "go back", NameMenuToSwitchTo = "play1" };
            backButton2.Click += MenuManager.instance.SwitchToMenu;

            MenuManager.instance.playMenu2.AddComponent(startGameButton);
            MenuManager.instance.playMenu2.AddComponent(backButton2);
            MenuManager.instance.playMenu2.active = false;
        }

        public void BuildTutorialMenu() {
            string[] switchTo = { "main", "tutorial1", "tutorial2", "tutorial3", "tutorial4" };
            int noPages = 4;
            string[] namePics = { "xBox_ controller", "gameView", "minimapDisplay", "powerups" };

            for (int i = 0; i < noPages; ++i) {
                var picturePage = new Image(File.Load<Texture2D>("Images/UI/" + namePics[i])) { Position = new Vector2(0, 0) };
                MenuManager.instance.tutoMenu[i].AddComponent(picturePage);

                if (i != noPages - 1) { 
                    var nextButtonTuto1 = new Button(textureButton, positionNextButton) {
                        Text = "next",
                        NameMenuToSwitchTo = switchTo[i + 2],
                    };
                    nextButtonTuto1.Click += MenuManager.instance.SwitchToMenu;
                    MenuManager.instance.tutoMenu[i].AddComponent(nextButtonTuto1);
                }

                var backButton1 = new Button(textureButton, positionBackButton) { Text = "go back", NameMenuToSwitchTo = switchTo[i] };
                backButton1.Click += MenuManager.instance.SwitchToMenu;


                MenuManager.instance.tutoMenu[i].AddComponent(backButton1);
                MenuManager.instance.tutoMenu[i].active = false;
            }
        }

        public void BuildRankingMenu() {

            ListComponents rankings = new ListComponents("rankings_game");
            string[] pathRankings = { "ranking2min.txt", "ranking3min.txt", "ranking5min.txt" };
            Vector2[] offsetStart = { new Vector2(-200, -200), new Vector2(100, -200) };

            for (int i=0; i<pathRankings.Length; ++i) {
                List<Tuple<string, string>> rankinglist;
                rankinglist = File.ReadRanking("Load/Rankings/" + pathRankings[i]);

                ListComponents listPersons = new ListComponents("rankings_" + i.ToString());
                int count = 0;
                foreach (var aPerson in rankinglist) {
                    var NamePerson = new TextBox() {
                        InitPos = middleScreen + offsetStart[0] + new Vector2(0, count*100),
                        Text = aPerson.Item1
                    };

                    var Score = new TextBox() {
                        InitPos = middleScreen + offsetStart[1] + new Vector2(0, count * 100),
                        Text = aPerson.Item2 
                    };

                    listPersons.AddComponent(NamePerson); listPersons.AddComponent(Score);
                    count++;
                }

                if (i != 0) listPersons.active = false;
                rankings.AddComponent(listPersons);
            }

            MenuManager.instance.rankingMenu.AddComponent(rankings);


            Vector2[] offset = { new Vector2(-175, -300), new Vector2(88, -300), new Vector2(350, -300) };
            string[] textButtons = { "2 min", "3 min", "5 min" };

            for (int i = 0; i < textButtons.Length; ++i) {
                var playButton = new Button(textureButton, middleScreen + offset[i]) {
                    Text = textButtons[i],
                    index = i,
                };
                playButton.Click += MenuManager.instance.SwitchRankingDisplay;
                playButton.ScaleWidth = .5f;
                MenuManager.instance.rankingMenu.AddComponent(playButton);
            }

            var backButton = new Button(textureButton, positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.instance.SwitchToMenu;
            MenuManager.instance.rankingMenu.AddComponent(backButton);
            MenuManager.instance.rankingMenu.active = false;
        }

        public void BuildOptionsMenu() {
            var testSlider = new Slider() {
                Position = new Vector2(200, 500),

                buttonSlider = new Button(textureButtonSlider, new Vector2(200 - textureButtonSlider.Width / 2, 500 - (textureButtonSlider.Height - UserInterface.BARHEIGHT) / 2)) {}
            };

            var testTickBox = new TickBox(textureTickBoxNotCliqued, textureTickBoxCliqued) {
                Position = new Vector2(200, 400),
            };

            var backButton = new Button(textureButton, positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.instance.SwitchToMenu;

            MenuManager.instance.optionsMenu.AddComponent(testSlider);
            MenuManager.instance.optionsMenu.AddComponent(testTickBox);
            MenuManager.instance.optionsMenu.AddComponent(backButton);
            MenuManager.instance.optionsMenu.active = false;
        }

        public void BuildCreditsMenu() {
            var backButton = new Button(textureButton, positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.instance.SwitchToMenu;
            MenuManager.instance.creditsMenu.AddComponent(backButton);
            MenuManager.instance.creditsMenu.active = false;
        }



        
    }
}
