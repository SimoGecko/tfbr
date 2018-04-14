using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BRS.Engine;

namespace BRS.Menu {
    public class Menu {

        private Texture2D _textureButton;
        private Texture2D _textureButtonSlider;
        private Texture2D _textureTickBoxCliqued;
        private Texture2D _textureTickBoxNotCliqued;

        public static Menu Instance;

        readonly Vector2 _positionNextButton = new Vector2(Screen.Width / 2 + 200, Screen.Height - 400);
        readonly Vector2 _positionBackButton = new Vector2(Screen.Width / 2 - 200, Screen.Height - 400);
        readonly Vector2 _middleScreen = new Vector2(Screen.Width / 2, Screen.Height / 2);

        public void LoadContent() {
            Instance = this;

            _textureButton = File.Load<Texture2D>("Images/UI/button");
            _textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            _textureTickBoxCliqued = File.Load<Texture2D>("Images/UI/tickbox_clicked");
            _textureTickBoxNotCliqued = File.Load<Texture2D>("Images/UI/tickbox_notclicked");
        }

        public void BuildMainMenu() {
            Vector2[] offset = { new Vector2(0, -200), new Vector2(0, -100), new Vector2(0, 0), new Vector2(0, 100), new Vector2(0, 200) };
            string[] textButtons = { "Play", "Tutorial", "Ranking", "Options", "Credits" };
            string[] switchTo = { "play1", "tutorial1", "ranking", "options", "credits" };
            
            for (int i=0; i<textButtons.Length; ++i) {
                var playButton = new Button(_textureButton, _middleScreen +  offset[i]) {
                    Text = textButtons[i],
                    NameMenuToSwitchTo = switchTo[i],
                };
                playButton.Click += MenuManager.Instance.SwitchToMenu;
                MenuManager.Instance.MainMenu.AddComponent(playButton);
            }
        }

        public void BuildPlayMenu() {

            string text = "Number of players";
            var chooseNumberPlayerText = new TextBox() {
                InitPos = _middleScreen + new Vector2(0,-200),
                Text = text
            };

            Vector2[] offset = { new Vector2(-200, -100), new Vector2(200, -100), new Vector2(-175, 100), new Vector2(88, 100), new Vector2(350, 100) };
            string[] textButtons = { "2", "4", "2 min", "3 min", "5 min" };

            List<Button> linkedButton = new List<Button>();
            for (int i = 0; i < textButtons.Length; ++i) {
                var playButton = new Button(_textureButton, _middleScreen + offset[i]) {
                    Text = textButtons[i],
                };
                if (i < 2) {
                    playButton.Click += MenuManager.Instance.UpdateNoPlayers;
                    playButton.Click += MenuManager.Instance.HighlightBorders;
                }
                else {
                    playButton.Click += MenuManager.Instance.UpdateRoundDuration;
                    playButton.Click += MenuManager.Instance.HighlightBorders;
                    playButton.ScaleWidth = .5f;
                }

                if (i == 2) linkedButton.Clear();

                linkedButton.Add(playButton);
                foreach (var bu in linkedButton) {
                    if (bu != playButton) {
                        playButton.Neighbors.Add(bu);
                        bu.Neighbors.Add(playButton);
                    }
                }

                MenuManager.Instance.PlayMenu1.AddComponent(playButton);
            }

            string text2 = "Game duration";
            var chooseDurationRoundText = new TextBox(){
                InitPos = _middleScreen,
                Text = text2
            };

            var nextButton = new Button(_textureButton, _positionNextButton) {
                Text = "next", NameMenuToSwitchTo = "play2"
            };
            nextButton.Click += MenuManager.Instance.SetDefaultParametersGame;
            nextButton.Click += MenuManager.Instance.SwitchToMenu;
            nextButton.Click += MenuManager.Instance.UpdatePlayersChangeTo;


            var backButton1 = new Button(_textureButton, _positionBackButton) {
                Text = "go back", NameMenuToSwitchTo = "main"
            };
            backButton1.Click += MenuManager.Instance.SwitchToMenu;

            MenuManager.Instance.PlayMenu1.AddComponent(chooseNumberPlayerText);
            MenuManager.Instance.PlayMenu1.AddComponent(chooseDurationRoundText);
            MenuManager.Instance.PlayMenu1.AddComponent(nextButton);
            MenuManager.Instance.PlayMenu1.AddComponent(backButton1);
            MenuManager.Instance.PlayMenu1.active = false;

            var startGameButton = new Button(_textureButton, _positionNextButton) { Text = "Start Game" };
            startGameButton.Click += MenuManager.Instance.StartGameFunction;

            var backButton2 = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = "play1" };
            backButton2.Click += MenuManager.Instance.SwitchToMenu;

            MenuManager.Instance.PlayMenu2.AddComponent(startGameButton);
            MenuManager.Instance.PlayMenu2.AddComponent(backButton2);

            ListComponents buttonPlayersChanges = new ListComponents("playerInfoToChange");
            Vector2[] offsetPlayersChanges = { new Vector2(-275, -300), new Vector2(-12, -300), new Vector2(250, -300), new Vector2(512, -300) };
            string[] textButtonsPlayersChanges = { "player 1", "player 2", "player 3", "player 4" };

            for (int i = 0; i < textButtonsPlayersChanges.Length; ++i) {
                var playerChangeButton = new Button(_textureButton, _middleScreen + offsetPlayersChanges[i]) {
                    Text = textButtonsPlayersChanges[i],
                    Index = i,
                    NameMenuToSwitchTo = "playerInfos",
                };
                playerChangeButton.Click += MenuManager.Instance.UpdatePlayersNameInfosToChange;
                playerChangeButton.Click += MenuManager.Instance.SwitchToMenu;
                playerChangeButton.ScaleWidth = .5f;
                buttonPlayersChanges.AddComponent(playerChangeButton);
            }

            MenuManager.Instance.PlayMenu2.AddComponent(buttonPlayersChanges);
            MenuManager.Instance.PlayMenu2.active = false;

        }

        internal void BuildPlayerInfoMenu() {

            var namePlayer = new TextBox() {
                InitPos = _middleScreen + new Vector2(0, -400),
                Text = "",
                NameIdentifier = "name_player"
            };
            MenuManager.Instance.PlayerInfoMenu.AddComponent(namePlayer);

            string[] firstLine = { "q", "w", "e", "r", "t", "z", "u", "i", "o", "p" };
            string[] secondLine = { "a", "s", "d", "f", "g", "h", "j", "k", "l" };
            string[] thirdLine = { "y", "x", "c", "v", "b", "n", "m" };
            string[][] keyboard = { firstLine, secondLine, thirdLine};
            Vector2[] startoffset = { new Vector2(-500, -300), new Vector2(-400,-200), new Vector2(-200,-100) };

            for (int i = 0; i < keyboard.Length; i++) {
                int count = 0;
                foreach (var elem in keyboard[i]) {
                    var letterButton = new Button(_textureButton, _middleScreen + startoffset[i] + count * new Vector2(.5f * _textureButton.Width, 0)) {
                        Text = elem,
                        ScaleWidth = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    ++count;
                    MenuManager.Instance.PlayerInfoMenu.AddComponent(letterButton);
                }
                if (i == keyboard.Length - 1) {
                    var letterButton = new Button(_textureButton, _middleScreen + startoffset[i] + count * new Vector2(.5f * _textureButton.Width, 0)) {
                        Text = "remove",
                        //ScaleWidth = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    MenuManager.Instance.PlayerInfoMenu.AddComponent(letterButton);
                }
            }

            var saveButton = new Button(_textureButton, _positionNextButton) {
                Text = "Save changes",
                NameMenuToSwitchTo = "play2"
            };
            saveButton.Click += MenuManager.Instance.ChangeNamePlayer;
            saveButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.PlayerInfoMenu.AddComponent(saveButton);

            var backButton = new Button(_textureButton, _positionBackButton) {
                Text = "go back",
                NameMenuToSwitchTo = "play2"
            };
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.PlayerInfoMenu.AddComponent(backButton);

            MenuManager.Instance.PlayerInfoMenu.active = false;

        }

        public void BuildTutorialMenu() {
            string[] switchTo = { "main", "tutorial1", "tutorial2", "tutorial3", "tutorial4" };
            int noPages = 4;
            string[] namePics = { "xBox_ controller", "gameView", "minimapDisplay", "powerups" };

            for (int i = 0; i < noPages; ++i) {
                var picturePage = new Image(File.Load<Texture2D>("Images/UI/" + namePics[i])) { Position = new Vector2(0, 0) };
                MenuManager.Instance.TutorialMenu[i].AddComponent(picturePage);

                if (i != noPages - 1) { 
                    var nextButtonTuto1 = new Button(_textureButton, _positionNextButton) {
                        Text = "next",
                        NameMenuToSwitchTo = switchTo[i + 2],
                    };
                    nextButtonTuto1.Click += MenuManager.Instance.SwitchToMenu;
                    MenuManager.Instance.TutorialMenu[i].AddComponent(nextButtonTuto1);
                }

                var backButton1 = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = switchTo[i] };
                backButton1.Click += MenuManager.Instance.SwitchToMenu;


                MenuManager.Instance.TutorialMenu[i].AddComponent(backButton1);
                MenuManager.Instance.TutorialMenu[i].active = false;
            }
        }

        public void BuildRankingMenu() {

            ListComponents rankings = new ListComponents("rankings_game");
            string[] pathRankings = { "ranking2min.txt", "ranking3min.txt", "ranking5min.txt" };
            Vector2[] offsetStart = { new Vector2(-200, -200), new Vector2(100, -200) };

            for (int i=0; i<pathRankings.Length; ++i) {
                List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/" + pathRankings[i]);

                ListComponents listPersons = new ListComponents("rankings_" + i.ToString());
                int count = 0;
                foreach (var aPerson in rankinglist) {
                    var namePerson = new TextBox() {
                        InitPos = _middleScreen + offsetStart[0] + new Vector2(0, count*100),
                        Text = aPerson.Item1
                    };

                    var score = new TextBox() {
                        InitPos = _middleScreen + offsetStart[1] + new Vector2(0, count * 100),
                        Text = aPerson.Item2 
                    };

                    listPersons.AddComponent(namePerson); listPersons.AddComponent(score);
                    count++;
                }

                if (i != 0) listPersons.Active = false;
                rankings.AddComponent(listPersons);
            }

            MenuManager.Instance.RankingMenu.AddComponent(rankings);


            Vector2[] offset = { new Vector2(-175, -300), new Vector2(88, -300), new Vector2(350, -300) };
            string[] textButtons = { "2 min", "3 min", "5 min" };

            for (int i = 0; i < textButtons.Length; ++i) {
                var playButton = new Button(_textureButton, _middleScreen + offset[i]) {
                    Text = textButtons[i],
                    Index = i,
                };
                playButton.Click += MenuManager.Instance.SwitchRankingDisplay;
                playButton.ScaleWidth = .5f;
                MenuManager.Instance.RankingMenu.AddComponent(playButton);
            }

            var backButton = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.RankingMenu.AddComponent(backButton);
            MenuManager.Instance.RankingMenu.active = false;
        }

        public void BuildOptionsMenu() {
            var testSlider = new Slider {
                Position = new Vector2(200, 500),

                ButtonSlider = new Button(_textureButtonSlider, new Vector2(200 - _textureButtonSlider.Width / 2, 500 - (_textureButtonSlider.Height - UserInterface.BarHeight) / 2))
            };

            var testTickBox = new TickBox(_textureTickBoxNotCliqued, _textureTickBoxCliqued) {
                Position = new Vector2(200, 400),
            };

            var backButton = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.Instance.SwitchToMenu;

            MenuManager.Instance.OptionsMenu.AddComponent(testSlider);
            MenuManager.Instance.OptionsMenu.AddComponent(testTickBox);
            MenuManager.Instance.OptionsMenu.AddComponent(backButton);
            MenuManager.Instance.OptionsMenu.active = false;
        }

        public void BuildCreditsMenu() {
            var backButton = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.CreditsMenu.AddComponent(backButton);
            MenuManager.Instance.CreditsMenu.active = false;
        }



        
    }
}
