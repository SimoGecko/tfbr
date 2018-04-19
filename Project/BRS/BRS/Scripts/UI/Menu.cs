// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Menu;
using BRS.Scripts.Managers;

namespace BRS.Scripts.UI {
    public class Menu : Component{

        public Dictionary<string, Texture2D> texturesButtons;
        public Dictionary<string, EventHandler> functionsMenu;

        public static Menu Instance;

        readonly Vector2 _middleScreen = new Vector2(Screen.Width / 2, Screen.Height / 2);
        readonly Vector2 _screenSizeVec = new Vector2(Screen.Width, Screen.Height);

        List<Button> linkedButtonLeftRight = new List<Button>();

        public void LoadContent() {
            Instance = this;

            Texture2D textureButton = File.Load<Texture2D>("Images/UI/panel");
            Texture2D textureButtonBackground = File.Load<Texture2D>("Images/UI/panel_Background");
            Texture2D textureButtonBigBackground = File.Load<Texture2D>("Images/UI/panel_BigBackground");
            Texture2D textureButtonTitle = File.Load<Texture2D>("Images/UI/panel_Title");
            Texture2D textureButtonCircle = File.Load<Texture2D>("Images/UI/CircleSmall");
            Texture2D textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            Texture2D textureTickBoxCliqued = File.Load<Texture2D>("Images/UI/tickbox_clicked");
            Texture2D textureTickBoxNotCliqued = File.Load<Texture2D>("Images/UI/tickbox_notclicked");
            Texture2D textureArrowLeft = File.Load<Texture2D>("Images/UI/ArrowLeft");
            Texture2D textureArrowRight = File.Load<Texture2D>("Images/UI/ArrowRight");
            Texture2D textureButtonAccept = File.Load<Texture2D>("Images/UI/Accept");
            Texture2D textureButtonForkLift = File.Load<Texture2D>("Images/UI/forklift_icon");

            texturesButtons = new Dictionary<string, Texture2D> {
                { "button", textureButton },
                { "background", textureButtonBackground },
                { "bigBackground", textureButtonBigBackground },
                { "buttonCircle", textureButtonCircle },
                { "title", textureButtonTitle },
                { "arrowLeft", textureArrowLeft },
                { "arrowRight", textureArrowRight },
                { "forklift", textureButtonForkLift },
                { "buttonAccept", textureButtonAccept }
            };

            functionsMenu = new Dictionary<string, EventHandler> {
                { "SwitchToMenu", MenuManager.Instance.SwitchToMenu },
                { "SetDefaultParametersGame", MenuManager.Instance.SetDefaultParametersGame },
                { "UpdateRoundDuration", MenuManager.Instance.UpdateRoundDuration },
                { "UpdateNoPlayers", MenuManager.Instance.UpdateNoPlayers },
                { "StartGameFunction", MenuManager.Instance.StartGameFunction },
                { "SwitchRankingDisplay", MenuManager.Instance.SwitchRankingDisplay },
                { "UpdateTemporaryNamePlayer", MenuManager.Instance.UpdateTemporaryNamePlayer },
                { "UpdatePlayersChangeTo", MenuManager.Instance.UpdatePlayersChangeTo },
                { "ChangeNamePlayer", MenuManager.Instance.ChangeNamePlayer },
                { "ChangeModelPlayer", MenuManager.Instance.ChangeModelPlayer },
                { "UpdatePlayersNameInfosToChange", MenuManager.Instance.UpdatePlayersNameInfosToChange },
                { "HighlightBorders", MenuManager.Instance.HighlightBorders },
                { "GoDown", MenuManager.Instance.GoDown },
                { "GoRight", MenuManager.Instance.GoRight }
            };
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
            CreatePanel("Load/MenuPanels/MainMenu.txt", "main", true);
            CreatePanel("Load/MenuPanels/Play1.txt", "play1");
            CreatePanel("Load/MenuPanels/Play2.txt", "play2");
            CreatePanel("Load/MenuPanels/Rankings.txt", "ranking");
            CreatePanel("Load/MenuPanels/Tutorial1.txt", "tutorial1");
            CreatePanel("Load/MenuPanels/Tutorial2.txt", "tutorial2");
            CreatePanel("Load/MenuPanels/Tutorial3.txt", "tutorial3");
            CreatePanel("Load/MenuPanels/Tutorial4.txt", "tutorial4");
            CreatePanel("Load/MenuPanels/Options.txt", "options");
            CreatePanel("Load/MenuPanels/Credits.txt", "credits");
        }

        public void CreateAlphabetButtons(string panelName) {
            string[] firstLine = { "q", "w", "e", "r", "t", "z", "u", "i", "o", "p" };
            string[] secondLine = { "a", "s", "d", "f", "g", "h", "j", "k", "l" };
            string[] thirdLine = { "y", "x", "c", "v", "b", "n", "m" };
            string[][] keyboard = { firstLine, secondLine, thirdLine };
            Vector2[] startoffset = { new Vector2(1152, 540), new Vector2(1210, 594), new Vector2(1248, 648) };

            List<Button> buttonsCurrentPanel2 = new List<Button>();
            for (int i = 0; i < keyboard.Length; i++) {
                int count = 0;
                foreach (var elem in keyboard[i]) {
                    var letterButton = new Button(texturesButtons["button"], startoffset[i] + count * new Vector2(.5f * texturesButtons["button"].Width, 0)) {
                        Text = elem,
                        ScaleWidth = .5f,
                        ScaleHeight = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    MenuManager.Instance.MenuRect["play2"].AddComponent(letterButton);

                    linkedButtonLeftRight.Add(letterButton);
                    buttonsCurrentPanel2.Add(letterButton);

                    if (i == 0 && count == 0)
                        letterButton.nameIdentifier = "Alphabet";

                    if (i == 0)
                        letterButton.NeighborUp = FindButtonPanelWithName("Player3", panelName);
                    if (i == keyboard.Length - 1)
                        letterButton.NeighborUp = FindButtonPanelWithName("SaveAlphabet", panelName);

                    ++count;

                }
                if (i == keyboard.Length - 1) {
                    var letterButton = new Button(texturesButtons["button"], _middleScreen + startoffset[i] * _screenSizeVec + count * new Vector2(.5f * texturesButtons["button"].Width, 0)) {
                        Text = "del",
                        ScaleWidth = .5f,
                        ScaleHeight = .5f
                    };
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    letterButton.NeighborDown = FindButtonPanelWithName("SaveAlphabet", panelName);
                    MenuManager.Instance.MenuRect["play2"].AddComponent(letterButton);
                    linkedButtonLeftRight.Add(letterButton);
                }
                SetNeighborsButtonLeftRight(false);
            }

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
                    buttonsCurrentPanel2[i].NeighborDown = FindButtonPanelWithName("SaveAlphabet", panelName);
                    buttonsCurrentPanel2[i].NeighborUp = buttonsCurrentPanel2[i - secondLine.Length];
                }
            }
        }

        public void LoadRankingsText() {
            ListComponents rankings = new ListComponents("rankings_game");
            string[] pathRankings = { "ranking2min.txt", "ranking3min.txt", "ranking5min.txt" };
            Vector2[] offsetStart = { new Vector2(864, 432), new Vector2(1056, 432) };

            for (int i = 0; i < pathRankings.Length; ++i) {
                List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/" + pathRankings[i]);

                ListComponents listPersons = new ListComponents("rankings_" + i.ToString());
                int count = 0;
                foreach (var aPerson in rankinglist) {
                    var namePerson = new TextBox() {
                        InitPos = offsetStart[0] + new Vector2(0, count * 65),
                        Text = aPerson.Item1
                    };

                    var score = new TextBox() {
                        InitPos = offsetStart[1] + new Vector2(0, count * 65),
                        Text = aPerson.Item2
                    };

                    listPersons.AddComponent(namePerson); listPersons.AddComponent(score);
                    count++;
                }

                if (i != 0) listPersons.Active = false;
                rankings.AddComponent(listPersons);
            }
            MenuManager.Instance.MenuRect["ranking"].AddComponent(rankings);
        }

        public void CreatePanel(string pathName, string panelName, bool setPanelActive = false) {
            MenuManager.Instance.MenuRect[panelName].active = setPanelActive;
            List<MenuStruct> panelObjects = File.ReadMenuPanel(pathName);
            foreach (MenuStruct MS in panelObjects) {
                if (MS.menuType == MenuType.Button) {
                    if (MS.Name == "Alphabet") CreateAlphabetButtons(panelName);
                    else {
                        Button button = new Button(texturesButtons[MS.TextureName], MS.Position);
                        if (MS.TextureInsideName != null) button.InsideImage = texturesButtons[MS.TextureInsideName];
                        if (MS.Text != null) button.Text = MS.Text;
                        if (MS.NameToSwitchTo != null) button.NameMenuToSwitchTo = MS.NameToSwitchTo;
                        if (MS.Name != null) button.nameIdentifier = MS.Name;
                        if (MS.ScaleHeight != default(float)) button.ScaleHeight = MS.ScaleHeight;
                        if (MS.ScaleWidth != default(float)) button.ScaleWidth = MS.ScaleWidth;
                        if (MS.ScaleHeightInside != default(float)) button.ScaleHeightInside = MS.ScaleHeightInside;
                        if (MS.ScaleWidthInside != default(float)) button.ScaleWidthInside = MS.ScaleWidthInside;
                        if (MS.Index != default(int)) button.Index = MS.Index;
                        if (MS.Color != default(Color)) button.ImageColor = MS.Color;
                        if (MS.ColorInside != default(Color)) button.InsideObjectColor = MS.ColorInside;
                        button.IsCurrentSelection = MS.CurrentSelection;
                        button.IsClicked = MS.IsClicked;
                        MenuManager.Instance.MenuRect[panelName].AddComponent(button);
                    }
                }
                else if (MS.menuType == MenuType.Text) {
                    TextBox textBox = new TextBox();
                    if (MS.Name != null) textBox.NameIdentifier = MS.Name;
                    if (MS.Position != null) textBox.InitPos = MS.Position;
                    if (MS.Text != null) textBox.Text = MS.Text;
                    MenuManager.Instance.MenuRect[panelName].AddComponent(textBox);
                }
                else if (MS.menuType == MenuType.Image) {
                    Image img = new Image(texturesButtons[MS.TextureName]);
                    if (MS.Position != null) img.Position = MS.Position;
                    if (MS.Name != null) img.NameIdentifier = MS.Name;
                    img.Active = MS.Active; 
                    MenuManager.Instance.MenuRect[panelName].AddComponent(img);
                }
                else if (MS.menuType == MenuType.Slider) {
                    Slider slider = new Slider();
                    if (MS.Position != null) slider.Position = MS.Position;
                    if (MS.Name != null) slider.NameIdentifier = MS.Name;
                    MenuManager.Instance.MenuRect[panelName].AddComponent(slider);

                    Button testbuttonSlider = new Button(texturesButtons["button"], MS.Position);
                    testbuttonSlider.ScaleHeight = 0.5f;
                    testbuttonSlider.ScaleWidth = 0.5f;
                    slider.ButtonSlider = testbuttonSlider;
                }
            }

            foreach (MenuStruct MS in panelObjects) {
                if (MS.Name != null && MS.NeighborsUpDownLeftRight != null) {
                    Button bu = FindButtonPanelWithName(MS.Name, panelName);
                    if (MS.NeighborsUpDownLeftRight[0] != "null") bu.NeighborUp = FindButtonPanelWithName(MS.NeighborsUpDownLeftRight[0], panelName);
                    if (MS.NeighborsUpDownLeftRight[1] != "null") bu.NeighborDown = FindButtonPanelWithName(MS.NeighborsUpDownLeftRight[1], panelName);
                    if (MS.NeighborsUpDownLeftRight[2] != "null") bu.NeighborLeft = FindButtonPanelWithName(MS.NeighborsUpDownLeftRight[2], panelName);
                    if (MS.NeighborsUpDownLeftRight[3] != "null") bu.NeighborRight = FindButtonPanelWithName(MS.NeighborsUpDownLeftRight[3], panelName);
                }

                if (MS.Name != null && MS.UniqueChoiceButtonWith != null) {
                    Button bu = FindButtonPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.UniqueChoiceButtonWith)
                        bu.neighbors.Add(FindButtonPanelWithName(elem, panelName));
                }
                
                if (MS.Name != null && MS.Functions != null) {
                    Button bu = FindButtonPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.Functions) {
                        bu.Click += functionsMenu[elem];
                    }
                }
            }

            if (panelName == "play2") {
                FindButtonPanelWithName("Model3", panelName).NeighborDown = FindButtonPanelWithName("Back", panelName);
                FindButtonPanelWithName("Back", panelName).NeighborLeft = FindButtonPanelWithName("Model3", panelName); ;
                FindButtonPanelWithName("Next", panelName).NeighborRight = FindButtonPanelWithName("SaveAlphabet", panelName);
            }

            if (panelName == "ranking")
                LoadRankingsText();
        }

        public Button FindButtonPanelWithName(string buttonName, string panelName) {
            foreach (Component comp in MenuManager.Instance.MenuRect[panelName].components) {
                if (comp is Button bu) {
                    if (bu.nameIdentifier == buttonName) {
                        return bu;
                    }
                }
            }
            return default(Button);
        }

        public enum MenuType {Button, Text, Slider, Image, TickBox, none};

        public struct MenuStruct {
            public MenuType menuType;
            public Vector2 Position;
            public string TextureName, TextureInsideName;
            public string[] NeighborsUpDownLeftRight; // NeighborUpName, NeighborDownName, NeighborLeftName, NeighborRightName;
            public string Text;
            public string NameToSwitchTo;
            public string Name;
            public float ScaleHeight, ScaleWidth, ScaleHeightInside, ScaleWidthInside;
            public int Index;
            public Color Color, ColorInside;
            public List<string> Functions;
            public bool Active, CurrentSelection, IsClicked;
            public List<string> UniqueChoiceButtonWith;
        }
    }
}
