using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BRS.Engine;

namespace BRS.Menu {
    public class Menu {

        private Texture2D _textureButton;
        private Texture2D _textureButtonBackground;
        private Texture2D _textureButtonBigBackground;
        private Texture2D _textureButtonTitle;
        private Texture2D _textureButtonCircle;
        private Texture2D _textureButtonSlider;
        private Texture2D _textureTickBoxCliqued;
        private Texture2D _textureTickBoxNotCliqued;
        private Texture2D _textureArrowLeft;
        private Texture2D _textureArrowRight;
        private Texture2D _textureButtonAccept;
        private Texture2D _textureButtonForkLift;

        public static Menu Instance;

        readonly Vector2 _middleScreen = new Vector2(Screen.Width / 2, Screen.Height / 2);
        readonly Vector2 _screenSizeVec = new Vector2(Screen.Width, Screen.Height);
        readonly Vector2 _positionNextButton = new Vector2(Screen.Width / 2 + 0.05f *Screen.Width, Screen.Height - 0.265f * Screen.Height) ;
        readonly Vector2 _positionBackButton = new Vector2(Screen.Width / 2 - 0.05f * Screen.Width, Screen.Height - 0.265f * Screen.Height);

        List<Button> linkedButtonDownUp = new List<Button>();
        List<Button> linkedButtonLeftRight = new List<Button>();

        public void LoadContent() {
            Instance = this;

            _textureButton = File.Load<Texture2D>("Images/UI/panel");
            _textureButtonBackground = File.Load<Texture2D>("Images/UI/panel_Background");
            _textureButtonBigBackground = File.Load<Texture2D>("Images/UI/panel_BigBackground");
            _textureButtonTitle = File.Load<Texture2D>("Images/UI/panel_Title");
            _textureButtonCircle = File.Load<Texture2D>("Images/UI/CircleSmall");
            _textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            _textureTickBoxCliqued = File.Load<Texture2D>("Images/UI/tickbox_clicked");
            _textureTickBoxNotCliqued = File.Load<Texture2D>("Images/UI/tickbox_notclicked");
            _textureArrowLeft = File.Load<Texture2D>("Images/UI/ArrowLeft");
            _textureArrowRight = File.Load<Texture2D>("Images/UI/ArrowRight");
            _textureButtonAccept = File.Load<Texture2D>("Images/UI/Accept");
            _textureButtonForkLift = File.Load<Texture2D>("Images/UI/forklift_icon");
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
        }

        public void BuildMainMenu() {

            var ButtonBackground = new Button(_textureButtonBackground, _middleScreen + new Vector2(0, 0) * _screenSizeVec) {
                ScaleHeight = 0.87f,
                ScaleWidth = 0.87f
            };
            ButtonBackground.ImageColor = Color.White;
            MenuManager.Instance.MenuRect["main"].AddComponent(ButtonBackground);

            var ButtonName = new Button(_textureButtonTitle, _middleScreen + new Vector2(0,-0.35f) * _screenSizeVec) {
                Text = "Menu",
            };
            ButtonName.InsideObjectColor = Color.White;
            ButtonName.ImageColor = new Color(148,148,148);
            MenuManager.Instance.MenuRect["main"].AddComponent(ButtonName);

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
            var ButtonBackground = new Button(_textureButtonBackground, _middleScreen + new Vector2(0, 0) * _screenSizeVec) {
                ScaleHeight = 0.87f,
                ScaleWidth = 0.87f
            };
            ButtonBackground.ImageColor = Color.White;
            MenuManager.Instance.MenuRect["play1"].AddComponent(ButtonBackground);

            var ButtonName = new Button(_textureButtonTitle, _middleScreen + new Vector2(0, -0.35f) * _screenSizeVec) {
                Text = "Game Settings",
            };
            ButtonName.InsideObjectColor = Color.White;
            ButtonName.ImageColor = new Color(148, 148, 148);
            MenuManager.Instance.MenuRect["play1"].AddComponent(ButtonName);

            string text = "Number of players";
            var chooseNumberPlayerText = new TextBox() {
                InitPos = _middleScreen + new Vector2(0,-0.235f)*_screenSizeVec,
                Text = text
            };

            Vector2[] offset = { new Vector2(-0.05f, -0.135f), new Vector2(0.05f, -0.135f), new Vector2(-0.08f, 0.065f), new Vector2(0, 0.065f), new Vector2(0.08f, 0.065f) };
            string[] textButtons = { "2", "4", "2 min", "3 min", "5 min" };

            List<Button> buttonsCurrentPanel = new List<Button>();
            List<Button> linkedButton = new List<Button>();
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
                    //playButton.ScaleWidth = .5f;
                    //playButton.Texture.Width *= .5f;
                }
                playButton.Click += MenuManager.Instance.HighlightBorders;

                linkedButtonLeftRight.Add(playButton);
                if (i == 1) SetNeighborsButtonLeftRight();
                if (i == 4) SetNeighborsButtonLeftRight(false);

                buttonsCurrentPanel.Add(playButton);

                if (i == 2) linkedButton.Clear();
                linkedButton.Add(playButton);
                foreach (var bu in linkedButton) {
                    if (bu != playButton) {
                        playButton.neighbors.Add(bu);
                        bu.neighbors.Add(playButton);
                    }
                }


                MenuManager.Instance.MenuRect["play1"].AddComponent(playButton);
            }

            string text2 = "Game duration";
            var chooseDurationRoundText = new TextBox(){
                InitPos = _middleScreen + new Vector2(0,-0.035f) * _screenSizeVec,
                Text = text2
            };

            var backButton1 = new Button(_textureButtonCircle, _positionBackButton) {
                NameMenuToSwitchTo = "main",
                ScaleHeight = 1.5f, ScaleWidth = 1.5f,
                ScaleHeightInside = 1.5f, ScaleWidthInside = 1.5f,
                InsideImage = _textureArrowLeft
            };
            backButton1.ImageColor = new Color(250,139,139);
            backButton1.InsideObjectColor = Color.White;
            backButton1.Click += MenuManager.Instance.SwitchToMenu;
            buttonsCurrentPanel.Add(backButton1);
            linkedButtonLeftRight.Add(backButton1);

            var nextButton = new Button(_textureButtonCircle, _positionNextButton) {
                NameMenuToSwitchTo = "play2",
                ScaleHeight = 1.5f, ScaleWidth = 1.5f,
                ScaleHeightInside = 1.5f, ScaleWidthInside = 1.5f,
                InsideImage = _textureArrowRight
            };
            nextButton.ImageColor = new Color(110, 235, 150);
            nextButton.InsideObjectColor = Color.White;
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

            var ButtonBackgroundPlay2 = new Button(_textureButtonBigBackground, _middleScreen + new Vector2(0, 0) * _screenSizeVec) {
                ScaleHeight = 0.87f,
                ScaleWidth = 0.87f
            };
            ButtonBackgroundPlay2.ImageColor = Color.White;
            MenuManager.Instance.MenuRect["play2"].AddComponent(ButtonBackgroundPlay2);

            var ButtonNamePlay2 = new Button(_textureButtonTitle, _middleScreen + new Vector2(0, -0.35f) * _screenSizeVec) {
                Text = "Game Settings",
            };
            ButtonNamePlay2.InsideObjectColor = Color.White;
            ButtonNamePlay2.ImageColor = new Color(148, 148, 148);
            MenuManager.Instance.MenuRect["play2"].AddComponent(ButtonNamePlay2);

            var startGameButton = new Button(_textureButtonCircle, _positionNextButton) {
                ScaleHeight = 1.5f,
                ScaleWidth = 1.5f,
                ScaleHeightInside = 1.5f,
                ScaleWidthInside = 1.5f,
                InsideImage = _textureArrowRight
            };
            startGameButton.ImageColor = new Color(110, 235, 150);
            startGameButton.InsideObjectColor = Color.White;
            startGameButton.Click += MenuManager.Instance.StartGameFunction;

            var backButton2 = new Button(_textureButtonCircle, _positionBackButton) {
                NameMenuToSwitchTo = "play1",
                ScaleHeight = 1.5f,
                ScaleWidth = 1.5f,
                ScaleHeightInside = 1.5f,
                ScaleWidthInside = 1.5f,
                InsideImage = _textureArrowLeft
            };
            backButton2.ImageColor = new Color(250, 139, 139);
            backButton2.InsideObjectColor = Color.White;
            backButton2.Click += MenuManager.Instance.SwitchToMenu;

            MenuManager.Instance.MenuRect["play2"].AddComponent(startGameButton);
            MenuManager.Instance.MenuRect["play2"].AddComponent(backButton2);

            linkedButtonLeftRight.Add(backButton2);
            linkedButtonLeftRight.Add(startGameButton);
            SetNeighborsButtonLeftRight(false);

            ListComponents buttonPlayersChanges = new ListComponents("playerInfoToChange");
            Vector2[] offsetPlayersChanges = { new Vector2(-0.3f, -0.2f), new Vector2(-0.1f, -0.2f), new Vector2(0.1f, -0.2f), new Vector2(0.3f, -0.2f) };
            string[] textButtonsPlayersChanges = { "player 1", "player 2", "player 3", "player 4" };

            linkedButton.Clear();
            for (int i = 0; i < textButtonsPlayersChanges.Length; ++i) {
                var playerChangeButton = new Button(_textureButton, _middleScreen + offsetPlayersChanges[i] * _screenSizeVec) {
                    Text = textButtonsPlayersChanges[i],
                    Index = i,
                    NeighborDown = startGameButton,
                    ScaleWidth = 2f
                };
                playerChangeButton.Click += MenuManager.Instance.UpdatePlayersNameInfosToChange;
                playerChangeButton.Click += MenuManager.Instance.HighlightBorders;
                //playerChangeButton.Click += MenuManager.Instance.SwitchToMenu;
                //playerChangeButton.ScaleWidth = .5f;
                buttonPlayersChanges.AddComponent(playerChangeButton);

                if (i == 0) {
                    playerChangeButton.IsClicked = true;
                    backButton2.NeighborUp = playerChangeButton;
                    startGameButton.NeighborUp = playerChangeButton;
                }
                linkedButtonLeftRight.Add(playerChangeButton);

                linkedButton.Add(playerChangeButton);
                foreach (var bu in linkedButton) {
                    if (bu != playerChangeButton) {
                        playerChangeButton.neighbors.Add(bu);
                        bu.neighbors.Add(playerChangeButton);
                    }
                }
            }
            SetNeighborsButtonLeftRight();
            MenuManager.Instance.MenuRect["play2"].AddComponent(buttonPlayersChanges);
            MenuManager.Instance.MenuRect["play2"].active = false;

            List<Button> listModelButton = new List<Button>();
            Vector2[] offsetPlayersChanges2 = { new Vector2(-0.4f, -0.05f), new Vector2(-0.4f, 0.05f), new Vector2(-0.4f, 0.15f) };
            string[] textButtonsPlayersChanges2 = { "model 1", "model 2", "model 3" };

            linkedButton.Clear();
            for (int i = 0; i < textButtonsPlayersChanges2.Length; ++i) {
                var playerChangeButton = new Button(_textureButton, _middleScreen + offsetPlayersChanges2[i] * _screenSizeVec) {
                    Text = textButtonsPlayersChanges2[i],
                    Index = i,
                    NameMenuToSwitchTo = "playerInfos",
                    ScaleWidth = 1.5f
                };
                playerChangeButton.Click += MenuManager.Instance.ChangeModelPlayer;
                playerChangeButton.Click += MenuManager.Instance.HighlightBorders;
                MenuManager.Instance.MenuRect["play2"].AddComponent(playerChangeButton);
                //playerChangeButton.ScaleWidth = .5f;

                linkedButtonDownUp.Add(playerChangeButton);
                listModelButton.Add(playerChangeButton);

                if (i == 0) {
                    foreach (var el in buttonPlayersChanges.Components)
                        ((Button)el).NeighborUp = playerChangeButton;
                    playerChangeButton.IsClicked = true;
                }

                linkedButton.Add(playerChangeButton);
                foreach (var bu in linkedButton) {
                    if (bu != playerChangeButton) {
                        playerChangeButton.neighbors.Add(bu);
                        bu.neighbors.Add(playerChangeButton);
                    }
                }
            }
            SetNeighborsButtonUpDown(false);
            listModelButton[0].NeighborUp = (Button)buttonPlayersChanges.Components[0];

            MenuManager.Instance.MenuRect["play2"].active = false;

            var namePlayer = new TextBox() {
                InitPos = _middleScreen + new Vector2(0.25f, -0.075f) * _screenSizeVec,
                Text = "",
                NameIdentifier = "name_player"
            };
            namePlayer.Active = true;
            MenuManager.Instance.MenuRect["play2"].AddComponent(namePlayer);

            string[] firstLine = { "q", "w", "e", "r", "t", "z", "u", "i", "o", "p" };
            string[] secondLine = { "a", "s", "d", "f", "g", "h", "j", "k", "l" };
            string[] thirdLine = { "y", "x", "c", "v", "b", "n", "m" };
            string[][] keyboard = { firstLine, secondLine, thirdLine };
            Vector2[] startoffset = { new Vector2(0.1f, 0f), new Vector2(0.13f, 0.05f), new Vector2(0.15f, 0.1f) };

            List<Button> buttonsCurrentPanel2 = new List<Button>();
            for (int i = 0; i < keyboard.Length; i++) {
                int count = 0;
                foreach (var elem in keyboard[i]) {
                    var letterButton = new Button(_textureButton, _middleScreen + startoffset[i] * _screenSizeVec + count * new Vector2(.5f * _textureButton.Width, 0)) {
                        Text = elem,
                        ScaleWidth = .5f,
                        ScaleHeight = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    MenuManager.Instance.MenuRect["play2"].AddComponent(letterButton);

                    linkedButtonLeftRight.Add(letterButton);
                    buttonsCurrentPanel2.Add(letterButton);

                    if (i == 0 && count == 0) {
                        foreach (var el in buttonPlayersChanges.Components) 
                            ((Button)el).NeighborDown = letterButton;
                        foreach (var el in listModelButton)
                            ((Button)el).NeighborRight = letterButton;
                    }
                    if (i == 0)
                        letterButton.NeighborUp = (Button)buttonPlayersChanges.Components[0];

                    ++count;

                }
                if (i == keyboard.Length - 1) {
                    var letterButton = new Button(_textureButton, _middleScreen + startoffset[i] * _screenSizeVec + count * new Vector2(.5f * _textureButton.Width, 0)) {
                        Text = "del",
                        ScaleWidth = .5f,
                        ScaleHeight = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    MenuManager.Instance.MenuRect["play2"].AddComponent(letterButton);
                    linkedButtonLeftRight.Add(letterButton);
                }
                SetNeighborsButtonLeftRight(false);
            }

            var saveButton = new Button(_textureButtonCircle, _positionNextButton + new Vector2(0.2f, 0) * _screenSizeVec) {
                NameMenuToSwitchTo = "play2",
                NeighborUp = buttonsCurrentPanel2[firstLine.Length + secondLine.Length],
                NeighborLeft = startGameButton,
            };
            saveButton.InsideImage = _textureButtonAccept;
            saveButton.InsideObjectColor = Color.White;
            saveButton.ImageColor = new Color(110, 235, 150);
            saveButton.Click += MenuManager.Instance.ChangeNamePlayer;
            saveButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.MenuRect["play2"].AddComponent(saveButton);


            for (int i = 0; i < buttonsCurrentPanel2.Count; ++i) {
                if (i < firstLine.Length) {
                    int offsetTmp = firstLine.Length;
                    if (i > secondLine.Length - 1) offsetTmp = secondLine.Length;

                    buttonsCurrentPanel2[i].NeighborDown = buttonsCurrentPanel2[i + offsetTmp];
                }
                else if (i < firstLine.Length + secondLine.Length) {
                    int offsetTmp = secondLine.Length;
                    if (i - firstLine.Length > thirdLine.Length - 1) offsetTmp = thirdLine.Length;

                    buttonsCurrentPanel2[i].NeighborDown = buttonsCurrentPanel2[i + offsetTmp];
                    buttonsCurrentPanel2[i].NeighborUp = buttonsCurrentPanel2[i - firstLine.Length];
                }
                else {
                    buttonsCurrentPanel2[i].NeighborDown = saveButton;
                    buttonsCurrentPanel2[i].NeighborUp = buttonsCurrentPanel2[i - secondLine.Length];
                }
            }

            listModelButton[2].NeighborDown = backButton2;
            backButton2.NeighborLeft = listModelButton[2];
            startGameButton.NeighborRight = saveButton;

            Image pictureModel1 = new Image(_textureButtonForkLift) { Position = new Vector2(0.3f, 0.55f) * _screenSizeVec, NameIdentifier = "pictureModel1"}; pictureModel1.Active = true;
            Image pictureModel2 = new Image(_textureButtonCircle) { Position = new Vector2(0.25f, 0.55f) * _screenSizeVec, NameIdentifier = "pictureModel2" }; pictureModel2.Active = false;
            Image pictureModel3 = new Image(_textureButton) { Position = new Vector2(0.25f, 0.55f) * _screenSizeVec, NameIdentifier = "pictureModel3" }; pictureModel3.Active = false;

            MenuManager.Instance.MenuRect["play2"].AddComponent(pictureModel1);
            MenuManager.Instance.MenuRect["play2"].AddComponent(pictureModel2);
            MenuManager.Instance.MenuRect["play2"].AddComponent(pictureModel3);

        }


        public void BuildTutorialMenu() {
            string[] switchTo = { "main", "tutorial1", "tutorial2", "tutorial3", "tutorial4" };
            // Todo: Sorry, had to change this because it couldn't load the "powerups"
            int noPages = 3; // 4;
            string[] namePics = { "xBox_ controller", "gameView", "minimapDisplay" /* , "powerups" */ };

            for (int i = 0; i < noPages; ++i) {
                var picturePage = new Image(File.Load<Texture2D>("Images/UI/" + namePics[i])) { Position = new Vector2(0, 0) };
                MenuManager.Instance.MenuRect["tutorial" + (i+1).ToString()].AddComponent(picturePage);

                if (i != noPages - 1) { 
                    var nextButtonTuto1 = new Button(_textureButtonCircle, _positionNextButton) {
                        ScaleHeight = 1.5f,
                        ScaleWidth = 1.5f,
                        ScaleHeightInside = 1.5f,
                        ScaleWidthInside = 1.5f,
                        InsideImage = _textureArrowRight,
                        NameMenuToSwitchTo = switchTo[i + 2],
                    };
                    nextButtonTuto1.ImageColor = new Color(110, 235, 150);
                    nextButtonTuto1.InsideObjectColor = Color.White;
                    nextButtonTuto1.Click += MenuManager.Instance.SwitchToMenu;
                    nextButtonTuto1.Click += MenuManager.Instance.GoRight;
                    MenuManager.Instance.MenuRect["tutorial" + (i + 1).ToString()].AddComponent(nextButtonTuto1);
                    linkedButtonLeftRight.Add(nextButtonTuto1);
                }

                var backButton1 = new Button(_textureButtonCircle, _positionBackButton) {
                    NameMenuToSwitchTo = switchTo[i],
                    ScaleHeight = 1.5f,
                    ScaleWidth = 1.5f,
                    ScaleHeightInside = 1.5f,
                    ScaleWidthInside = 1.5f,
                    InsideImage = _textureArrowLeft
                };
                backButton1.ImageColor = new Color(250, 139, 139);
                backButton1.InsideObjectColor = Color.White;
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
            var ButtonBackground = new Button(_textureButtonBackground, _middleScreen + new Vector2(0, 0) * _screenSizeVec) {
                ScaleHeight = 0.87f,
                ScaleWidth = 0.87f
            };
            ButtonBackground.ImageColor = Color.White;
            MenuManager.Instance.MenuRect["ranking"].AddComponent(ButtonBackground);

            var ButtonName = new Button(_textureButtonTitle, _middleScreen + new Vector2(0, -0.35f) * _screenSizeVec) {
                Text = "Rankings",
            };
            ButtonName.InsideObjectColor = Color.White;
            ButtonName.ImageColor = new Color(148, 148, 148);
            MenuManager.Instance.MenuRect["ranking"].AddComponent(ButtonName);

            ListComponents rankings = new ListComponents("rankings_game");
            string[] pathRankings = { "ranking2min.txt", "ranking3min.txt", "ranking5min.txt" };
            Vector2[] offsetStart = { new Vector2(-0.05f, -0.1f), new Vector2(0.05f, -0.1f) };

            for (int i=0; i<pathRankings.Length; ++i) {
                List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/" + pathRankings[i]);

                ListComponents listPersons = new ListComponents("rankings_" + i.ToString());
                int count = 0;
                foreach (var aPerson in rankinglist) {
                    var namePerson = new TextBox() {
                        InitPos = _middleScreen + offsetStart[0]*_screenSizeVec + new Vector2(0, count*65),
                        Text = aPerson.Item1
                    };

                    var score = new TextBox() {
                        InitPos = _middleScreen + offsetStart[1]*_screenSizeVec + new Vector2(0, count * 65),
                        Text = aPerson.Item2 
                    };

                    listPersons.AddComponent(namePerson); listPersons.AddComponent(score);
                    count++;
                }

                if (i != 0) listPersons.Active = false;
                rankings.AddComponent(listPersons);
            }

            MenuManager.Instance.MenuRect["ranking"].AddComponent(rankings);

            var backButton = new Button(_textureButtonCircle, _positionBackButton) {
                NameMenuToSwitchTo = "main",
                ScaleHeight = 1.5f,
                ScaleWidth = 1.5f,
                ScaleHeightInside = 1.5f,
                ScaleWidthInside = 1.5f,
                InsideImage = _textureArrowLeft
            };
            backButton.ImageColor = new Color(250, 139, 139);
            backButton.InsideObjectColor = Color.White;
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.MenuRect["ranking"].AddComponent(backButton);



            Vector2[] offset = { new Vector2(-0.1f, -0.2f), new Vector2(0, -0.2f), new Vector2(0.1f, -0.2f) };
            string[] textButtons = { "2 min", "3 min", "5 min" };

            for (int i = 0; i < textButtons.Length; ++i) {
                var playButton = new Button(_textureButton, _middleScreen + offset[i]*_screenSizeVec) {
                    Text = textButtons[i],
                    Index = i,
                    NeighborDown = backButton
                };
                playButton.Click += MenuManager.Instance.SwitchRankingDisplay;
                //playButton.ScaleWidth = .5f;
                MenuManager.Instance.MenuRect["ranking"].AddComponent(playButton);
                linkedButtonLeftRight.Add(playButton);
                if (i == 0) backButton.NeighborUp = playButton;
            }
            SetNeighborsButtonLeftRight();


            MenuManager.Instance.MenuRect["ranking"].active = false;


        }

        public void BuildOptionsMenu() {
            var ButtonBackground = new Button(_textureButtonBackground, _middleScreen + new Vector2(0, 0) * _screenSizeVec) {
                ScaleHeight = 0.87f,
                ScaleWidth = 0.87f
            };
            ButtonBackground.ImageColor = Color.White;
            MenuManager.Instance.MenuRect["options"].AddComponent(ButtonBackground);

            var ButtonName = new Button(_textureButtonTitle, _middleScreen + new Vector2(0, -0.35f) * _screenSizeVec) {
                Text = "Options",
            };
            ButtonName.InsideObjectColor = Color.White;
            ButtonName.ImageColor = new Color(148, 148, 148);
            MenuManager.Instance.MenuRect["options"].AddComponent(ButtonName);

            var backButton = new Button(_textureButtonCircle, _positionBackButton) {
                NameMenuToSwitchTo = "main",
                ScaleHeight = 1.5f,
                ScaleWidth = 1.5f,
                ScaleHeightInside = 1.5f,
                ScaleWidthInside = 1.5f,
                InsideImage = _textureArrowLeft
            };
            backButton.ImageColor = new Color(250, 139, 139);
            backButton.InsideObjectColor = Color.White;
            backButton.IsCurrentSelection = true;
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.MenuRect["options"].AddComponent(backButton);

            MenuManager.Instance.MenuRect["options"].AddComponent(backButton);
            MenuManager.Instance.MenuRect["options"].active = false;
        }

        public void BuildCreditsMenu() {

            var backButton = new Button(_textureButtonCircle, _positionBackButton) {
                NameMenuToSwitchTo = "main",
                ScaleHeight = 1.5f,
                ScaleWidth = 1.5f,
                ScaleHeightInside = 1.5f,
                ScaleWidthInside = 1.5f,
                InsideImage = _textureArrowLeft
            };
            backButton.ImageColor = new Color(250, 139, 139);
            backButton.InsideObjectColor = Color.White;
            backButton.IsCurrentSelection = true;
            backButton.Click += MenuManager.Instance.SwitchToMenu;
            MenuManager.Instance.MenuRect["credits"].AddComponent(backButton);

            MenuManager.Instance.MenuRect["credits"].active = false;
        }



        
    }
}
