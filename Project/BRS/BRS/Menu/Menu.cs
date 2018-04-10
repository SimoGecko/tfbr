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

        readonly Vector2 _middleScreen = new Vector2(Screen.Width / 2, Screen.Height / 2);
        readonly Vector2 _screenSizeVec = new Vector2(Screen.Width, Screen.Height);
        readonly Vector2 _positionNextButton = new Vector2(Screen.Width / 2 + 0.1f *Screen.Width, Screen.Height - 0.3f * Screen.Height) ;
        readonly Vector2 _positionBackButton = new Vector2(Screen.Width / 2 - 0.1f * Screen.Width, Screen.Height - 0.3f * Screen.Height);

        List<Button> linkedButtonDownUp = new List<Button>();
        List<Button> linkedButtonLeftRight = new List<Button>();

        public void LoadContent() {
            Instance = this;

            _textureButton = File.Load<Texture2D>("Images/UI/panel");
            _textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            _textureTickBoxCliqued = File.Load<Texture2D>("Images/UI/tickbox_clicked");
            _textureTickBoxNotCliqued = File.Load<Texture2D>("Images/UI/tickbox_notclicked");
        }

        private void SetNeighborsButtonUpDown(bool setSelectionFirstElem = true) {
            int indexUp, indexDown;
            if (setSelectionFirstElem) linkedButtonDownUp[0].IsCurrentSelection = true;

            for (int i=0; i< linkedButtonDownUp.Count; ++i) {
                if (i == 0) indexUp = linkedButtonDownUp.Count - 1;
                else indexUp = i - 1;

                if (i == linkedButtonDownUp.Count - 1) indexDown = 0;
                else indexDown = i + 1;

                linkedButtonDownUp[i].NeighborUp = linkedButtonDownUp[indexUp];
                linkedButtonDownUp[i].NeighborDown = linkedButtonDownUp[indexDown];
            }
            linkedButtonDownUp.Clear();
        }

        private void SetNeighborsButtonLeftRight(bool setSelectionFirstElem = true) {

            int indexLeft, indexRight;
            if (setSelectionFirstElem) linkedButtonLeftRight[0].IsCurrentSelection = true;

            for (int i = 0; i < linkedButtonLeftRight.Count; ++i) {
                if (i == 0) indexLeft = linkedButtonLeftRight.Count - 1;
                else indexLeft = i - 1;

                if (i == linkedButtonLeftRight.Count - 1) indexRight = 0;
                else indexRight = i + 1;

                linkedButtonLeftRight[i].NeighborLeft = linkedButtonLeftRight[indexLeft];
                linkedButtonLeftRight[i].NeighborRight = linkedButtonLeftRight[indexRight];
            }
            linkedButtonLeftRight.Clear();
        }

        public void BuildMenuPanels() {
            BuildMainMenu();
            BuildPlayMenu();
            BuildTutorialMenu();
            BuildRankingMenu();
            BuildOptionsMenu();
            BuildCreditsMenu();
            BuildPlayerInfoMenu();
        }

        public void BuildMainMenu() {
            var ButtonName = new Button(_textureButton, _middleScreen + new Vector2(0,-0.35f) * _screenSizeVec) {
                Text = "Menu",
                //ScaleHeight = 1.5f,
                ScaleWidth = 4f
            };
            ButtonName.TextColor = Color.White;
            ButtonName.ImageColor = new Color(148,148,148);
            MenuManager.Instance.MenuRect["main"].AddComponent(ButtonName);

            var ButtonBackground = new Button(_textureButton, _middleScreen + new Vector2(0, -0.1f) * _screenSizeVec) {
                //ScaleHeight = 1f,
                ScaleWidth = 6f
            };
            ButtonBackground.ImageColor = Color.White;
            MenuManager.Instance.MenuRect["main"].AddComponent(ButtonBackground);


            Vector2[] offset = { new Vector2(0, -0.2f), new Vector2(0, -0.1f), new Vector2(0, 0), new Vector2(0, 0.1f), new Vector2(0, 0.2f) };
            string[] textButtons = { "Play", "Tutorial", "Ranking", "Options", "Credits" };
            string[] switchTo = { "play1", "tutorial1", "ranking", "options", "credits" };

            for (int i=0; i<textButtons.Length; ++i) {
                var playButton = new Button(_textureButton, _middleScreen + offset[i]*_screenSizeVec) {
                    Text = textButtons[i],
                    NameMenuToSwitchTo = switchTo[i],
                    //ScaleHeight = 1.5f,
                    ScaleWidth = 2f
                };
                //if (i == 0) playButton.Click += MenuManager.Instance.TransitionUI;
                //else
                playButton.Click += MenuManager.Instance.SwitchToMenu;
                MenuManager.Instance.MenuRect["main"].AddComponent(playButton);

                linkedButtonDownUp.Add(playButton);
            }

            SetNeighborsButtonUpDown();
        }

        public void BuildPlayMenu() {
            var picturePage = new Image(File.Load<Texture2D>("Images/UI/Play1")) { Position = new Vector2(0, 0) };
            picturePage.Active = false;
            MenuManager.Instance.MenuRect["play1"].AddComponent(picturePage);

            string text = "Number of players";
            var chooseNumberPlayerText = new TextBox() {
                InitPos = _middleScreen + new Vector2(0,-0.2f)*_screenSizeVec,
                Text = text
            };

            Vector2[] offset = { new Vector2(-0.1f, -0.1f), new Vector2(0.1f, -0.1f), new Vector2(-0.12f, 0.1f), new Vector2(0, 0.1f), new Vector2(0.12f, 0.1f) };
            string[] textButtons = { "2", "4", "2 min", "3 min", "5 min" };

            List<Button> buttonsCurrentPanel = new List<Button>();
            for (int i = 0; i < textButtons.Length; ++i) {
                var playButton = new Button(_textureButton, _middleScreen + offset[i]*_screenSizeVec) {
                    Text = textButtons[i],
                };
                if (i < 2) {
                    playButton.Click += MenuManager.Instance.UpdateNoPlayers;
                    playButton.Click += MenuManager.Instance.GoDown;
                }
                else {
                    playButton.Click += MenuManager.Instance.UpdateRoundDuration;
                    playButton.Click += MenuManager.Instance.GoDown;
                    playButton.ScaleWidth = .5f;
                    //playButton.Texture.Width *= .5f;
                }

                linkedButtonLeftRight.Add(playButton);
                if (i == 1) SetNeighborsButtonLeftRight();
                if (i == 4) SetNeighborsButtonLeftRight(false);

                buttonsCurrentPanel.Add(playButton);

                MenuManager.Instance.MenuRect["play1"].AddComponent(playButton);
            }

            string text2 = "Game duration";
            var chooseDurationRoundText = new TextBox(){
                InitPos = _middleScreen,
                Text = text2
            };

            var backButton1 = new Button(_textureButton, _positionBackButton) {
                Text = "go back", NameMenuToSwitchTo = "main"
            };
            backButton1.Click += MenuManager.Instance.SwitchToMenu;
            buttonsCurrentPanel.Add(backButton1);
            linkedButtonLeftRight.Add(backButton1);

            var nextButton = new Button(_textureButton, _positionNextButton) {
                Text = "next", NameMenuToSwitchTo = "play2"
            };
            nextButton.Click += MenuManager.Instance.SetDefaultParametersGame;
            nextButton.Click += MenuManager.Instance.SwitchToMenu;
            nextButton.Click += MenuManager.Instance.UpdatePlayersChangeTo;
            buttonsCurrentPanel.Add(nextButton);
            linkedButtonLeftRight.Add(nextButton);
            SetNeighborsButtonLeftRight(false);

            buttonsCurrentPanel[0].NeighborDown = buttonsCurrentPanel[2];
            buttonsCurrentPanel[1].NeighborDown = buttonsCurrentPanel[2];
            buttonsCurrentPanel[2].NeighborDown = buttonsCurrentPanel[6];
            buttonsCurrentPanel[3].NeighborDown = buttonsCurrentPanel[6];
            buttonsCurrentPanel[4].NeighborDown = buttonsCurrentPanel[6];

            buttonsCurrentPanel[2].NeighborUp = buttonsCurrentPanel[0];
            buttonsCurrentPanel[3].NeighborUp = buttonsCurrentPanel[0];
            buttonsCurrentPanel[4].NeighborUp = buttonsCurrentPanel[0];
            buttonsCurrentPanel[5].NeighborUp = buttonsCurrentPanel[2];
            buttonsCurrentPanel[6].NeighborUp = buttonsCurrentPanel[2];

            MenuManager.Instance.MenuRect["play1"].AddComponent(chooseNumberPlayerText);
            MenuManager.Instance.MenuRect["play1"].AddComponent(chooseDurationRoundText);
            MenuManager.Instance.MenuRect["play1"].AddComponent(nextButton);
            MenuManager.Instance.MenuRect["play1"].AddComponent(backButton1);
            MenuManager.Instance.MenuRect["play1"].active = false;

            var startGameButton = new Button(_textureButton, _positionNextButton) { Text = "Start Game" };
            startGameButton.Click += MenuManager.Instance.StartGameFunction;

            var backButton2 = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = "play1" };
            backButton2.Click += MenuManager.Instance.SwitchToMenu;

            MenuManager.Instance.MenuRect["play2"].AddComponent(startGameButton);
            MenuManager.Instance.MenuRect["play2"].AddComponent(backButton2);

            linkedButtonLeftRight.Add(backButton2);
            linkedButtonLeftRight.Add(startGameButton);
            SetNeighborsButtonLeftRight(false);

            ListComponents buttonPlayersChanges = new ListComponents("playerInfoToChange");
            Vector2[] offsetPlayersChanges = { new Vector2(-0.15f, -0.3f), new Vector2(-0.05f, -0.3f), new Vector2(0.05f, -0.3f), new Vector2(0.15f, -0.3f) };
            string[] textButtonsPlayersChanges = { "player 1", "player 2", "player 3", "player 4" };

            for (int i = 0; i < textButtonsPlayersChanges.Length; ++i) {
                var playerChangeButton = new Button(_textureButton, _middleScreen + offsetPlayersChanges[i] * _screenSizeVec) {
                    Text = textButtonsPlayersChanges[i],
                    Index = i,
                    NameMenuToSwitchTo = "playerInfos",
                    NeighborDown = startGameButton
                };
                playerChangeButton.Click += MenuManager.Instance.UpdatePlayersNameInfosToChange;
                playerChangeButton.Click += MenuManager.Instance.SwitchToMenu;
                playerChangeButton.ScaleWidth = .5f;
                buttonPlayersChanges.AddComponent(playerChangeButton);

                if (i == 0) {
                    backButton2.NeighborUp = playerChangeButton;
                    startGameButton.NeighborUp = playerChangeButton;
                }
                linkedButtonLeftRight.Add(playerChangeButton);
            }
            SetNeighborsButtonLeftRight();
            MenuManager.Instance.MenuRect["play2"].AddComponent(buttonPlayersChanges);
            MenuManager.Instance.MenuRect["play2"].active = false;

        }

        internal void BuildPlayerInfoMenu() {

            var namePlayer = new TextBox() {
                InitPos = _middleScreen + new Vector2(0, -0.4f)*_screenSizeVec,
                Text = "",
                NameIdentifier = "name_player"
            };
            MenuManager.Instance.MenuRect["playerInfos"].AddComponent(namePlayer);

            string[] firstLine = { "q", "w", "e", "r", "t", "z", "u", "i", "o", "p" };
            string[] secondLine = { "a", "s", "d", "f", "g", "h", "j", "k", "l" };
            string[] thirdLine = { "y", "x", "c", "v", "b", "n", "m" };
            string[][] keyboard = { firstLine, secondLine, thirdLine };
            Vector2[] startoffset = { new Vector2(-0.3f, -0.3f), new Vector2(-0.25f, -0.2f), new Vector2(-0.2f, -0.1f) };

            List<Button> buttonsCurrentPanel = new List<Button>();
            for (int i = 0; i < keyboard.Length; i++) {
                int count = 0;
                foreach (var elem in keyboard[i]) {
                    var letterButton = new Button(_textureButton, _middleScreen + startoffset[i]*_screenSizeVec + count * new Vector2(.5f * _textureButton.Width, 0)) {
                        Text = elem,
                        ScaleWidth = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    ++count;
                    MenuManager.Instance.MenuRect["playerInfos"].AddComponent(letterButton);

                    linkedButtonLeftRight.Add(letterButton);
                    buttonsCurrentPanel.Add(letterButton);
                }
                if (i == keyboard.Length - 1) {
                    var letterButton = new Button(_textureButton, _middleScreen + startoffset[i]*_screenSizeVec + count * new Vector2(.5f * _textureButton.Width, 0)) {
                        Text = "remove",
                        ScaleWidth = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    MenuManager.Instance.MenuRect["playerInfos"].AddComponent(letterButton);
                    linkedButtonLeftRight.Add(letterButton);
                }
                SetNeighborsButtonLeftRight(false);
            }

            var saveButton = new Button(_textureButton, _positionNextButton) {
                Text = "Save changes",
                NameMenuToSwitchTo = "play2",
                NeighborUp = buttonsCurrentPanel[firstLine.Length + secondLine.Length]
            };
            saveButton.Click += MenuManager.Instance.ChangeNamePlayer;
            saveButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.MenuRect["playerInfos"].AddComponent(saveButton);
            

            for (int i=0; i<buttonsCurrentPanel.Count; ++i) {
                if (i < firstLine.Length) {
                    int offset = firstLine.Length;
                    if (i > secondLine.Length - 1) offset = secondLine.Length;

                    buttonsCurrentPanel[i].NeighborDown = buttonsCurrentPanel[i + offset];
                }
                else if (i < firstLine.Length + secondLine.Length) {
                    int offset = secondLine.Length;
                    if (i - firstLine.Length > thirdLine.Length - 1) offset = thirdLine.Length;

                    buttonsCurrentPanel[i].NeighborDown = buttonsCurrentPanel[i + offset];
                    buttonsCurrentPanel[i].NeighborUp = buttonsCurrentPanel[i - firstLine.Length];
                }
                else {
                    buttonsCurrentPanel[i].NeighborDown = saveButton;
                    buttonsCurrentPanel[i].NeighborUp = buttonsCurrentPanel[i - secondLine.Length];
                }
            }



            var backButton = new Button(_textureButton, _positionBackButton) {
                Text = "go back",
                NameMenuToSwitchTo = "play2",
                NeighborUp = buttonsCurrentPanel[firstLine.Length + secondLine.Length]
            };
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.MenuRect["playerInfos"].AddComponent(backButton);

            MenuManager.Instance.MenuRect["playerInfos"].active = false;

            linkedButtonLeftRight.Add(saveButton);
            linkedButtonLeftRight.Add(backButton);
            SetNeighborsButtonLeftRight();

        }

        public void BuildTutorialMenu() {
            string[] switchTo = { "main", "tutorial1", "tutorial2", "tutorial3", "tutorial4" };
            int noPages = 4;
            string[] namePics = { "xBox_ controller", "gameView", "minimapDisplay", "powerups" };

            for (int i = 0; i < noPages; ++i) {
                var picturePage = new Image(File.Load<Texture2D>("Images/UI/" + namePics[i])) { Position = new Vector2(0, 0) };
                MenuManager.Instance.MenuRect["tutorial" + (i+1).ToString()].AddComponent(picturePage);

                if (i != noPages - 1) { 
                    var nextButtonTuto1 = new Button(_textureButton, _positionNextButton) {
                        Text = "next",
                        NameMenuToSwitchTo = switchTo[i + 2],
                    };
                    nextButtonTuto1.Click += MenuManager.Instance.SwitchToMenu;
                    nextButtonTuto1.Click += MenuManager.Instance.GoRight;
                    MenuManager.Instance.MenuRect["tutorial" + (i + 1).ToString()].AddComponent(nextButtonTuto1);
                    linkedButtonLeftRight.Add(nextButtonTuto1);
                }

                var backButton1 = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = switchTo[i] };
                backButton1.Click += MenuManager.Instance.SwitchToMenu;
                backButton1.Click += MenuManager.Instance.GoRight;

                if (i != noPages - 1) {
                    linkedButtonLeftRight.Add(backButton1);
                    SetNeighborsButtonLeftRight();
                }
                else backButton1.IsCurrentSelection = true;


                MenuManager.Instance.MenuRect["tutorial" + (i + 1).ToString()].AddComponent(backButton1);
                MenuManager.Instance.MenuRect["tutorial" + (i + 1).ToString()].active = false;
            }
        }

        public void BuildRankingMenu() {

            ListComponents rankings = new ListComponents("rankings_game");
            string[] pathRankings = { "ranking2min.txt", "ranking3min.txt", "ranking5min.txt" };
            Vector2[] offsetStart = { new Vector2(-0.05f, -0.2f), new Vector2(0.05f, -0.2f) };

            for (int i=0; i<pathRankings.Length; ++i) {
                List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/" + pathRankings[i]);

                ListComponents listPersons = new ListComponents("rankings_" + i.ToString());
                int count = 0;
                foreach (var aPerson in rankinglist) {
                    var namePerson = new TextBox() {
                        InitPos = _middleScreen + offsetStart[0]*_screenSizeVec + new Vector2(0, count*100),
                        Text = aPerson.Item1
                    };

                    var score = new TextBox() {
                        InitPos = _middleScreen + offsetStart[1]*_screenSizeVec + new Vector2(0, count * 100),
                        Text = aPerson.Item2 
                    };

                    listPersons.AddComponent(namePerson); listPersons.AddComponent(score);
                    count++;
                }

                if (i != 0) listPersons.Active = false;
                rankings.AddComponent(listPersons);
            }

            MenuManager.Instance.MenuRect["ranking"].AddComponent(rankings);

            var backButton = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.MenuRect["ranking"].AddComponent(backButton);

            Vector2[] offset = { new Vector2(-0.1f, -0.3f), new Vector2(0, -0.3f), new Vector2(0.1f, -0.3f) };
            string[] textButtons = { "2 min", "3 min", "5 min" };

            for (int i = 0; i < textButtons.Length; ++i) {
                var playButton = new Button(_textureButton, _middleScreen + offset[i]*_screenSizeVec) {
                    Text = textButtons[i],
                    Index = i,
                    NeighborDown = backButton
                };
                playButton.Click += MenuManager.Instance.SwitchRankingDisplay;
                playButton.ScaleWidth = .5f;
                MenuManager.Instance.MenuRect["ranking"].AddComponent(playButton);
                linkedButtonLeftRight.Add(playButton);
                if (i == 0) backButton.NeighborUp = playButton;
            }
            SetNeighborsButtonLeftRight();


            MenuManager.Instance.MenuRect["ranking"].active = false;


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
            backButton.IsCurrentSelection = true;

            MenuManager.Instance.MenuRect["options"].AddComponent(testSlider);
            MenuManager.Instance.MenuRect["options"].AddComponent(testTickBox);
            MenuManager.Instance.MenuRect["options"].AddComponent(backButton);
            MenuManager.Instance.MenuRect["options"].active = false;
        }

        public void BuildCreditsMenu() {
            var backButton = new Button(_textureButton, _positionBackButton) { Text = "go back", NameMenuToSwitchTo = "main" };
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            backButton.IsCurrentSelection = true;
            MenuManager.Instance.MenuRect["credits"].AddComponent(backButton);
            MenuManager.Instance.MenuRect["credits"].active = false;
        }



        
    }
}
