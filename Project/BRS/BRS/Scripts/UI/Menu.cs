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
            Texture2D textureSlider = File.Load<Texture2D>("Images/UI/progress_bar");
            Texture2D textureButtonTitle = File.Load<Texture2D>("Images/UI/panel_Title");
            Texture2D textureButtonCircle = File.Load<Texture2D>("Images/UI/CircleSmall");
            Texture2D textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            Texture2D textureTickBoxCliqued = File.Load<Texture2D>("Images/UI/tickbox_clicked");
            Texture2D textureTickBoxNotCliqued = File.Load<Texture2D>("Images/UI/tickbox_notclicked");
            Texture2D textureArrowLeft = File.Load<Texture2D>("Images/UI/ArrowLeft");
            Texture2D textureArrowRight = File.Load<Texture2D>("Images/UI/ArrowRight");
            Texture2D textureButtonAccept = File.Load<Texture2D>("Images/UI/Accept");
            Texture2D textureForkLift = File.Load<Texture2D>("Images/UI/forklift_icon");
            Texture2D textureModel3 = File.Load<Texture2D>("Images/UI/model3_image");
            Texture2D textureMenuIcon = File.Load<Texture2D>("Images/UI/Menu");
            Texture2D textureRestartIcon = File.Load<Texture2D>("Images/UI/Restart");

            texturesButtons = new Dictionary<string, Texture2D> {
                { "button", textureButton },
                { "background", textureButtonBackground },
                { "bigBackground", textureButtonBigBackground },
                { "buttonCircle", textureButtonCircle },
                { "title", textureButtonTitle },
                { "arrowLeft", textureArrowLeft },
                { "arrowRight", textureArrowRight },
                { "forklift", textureForkLift },
                { "bulldozer", textureModel3 },
                { "buttonAccept", textureButtonAccept },
                { "slider", textureSlider },
                { "menu", textureMenuIcon },
                { "restart", textureRestartIcon }
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
                { "GoRight", MenuManager.Instance.GoRight },
                { "UpdateVolume", MenuManager.Instance.UpdateVolume },
                { "LoadMenu", MenuManager.Instance.LoadMenuFunction },
                { "ResumeGame", MenuManager.Instance.ResumeGame }
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

        public void BuildPausePanel() {
            CreatePanel("Load/MenuPanels/PauseMenu.txt", "pause");
        }

        public void BuildMenuPanels(string panelPlay2Name) {
            CreatePanel("Load/MenuPanels/MainMenu.txt", "main", true);
            CreatePanel("Load/MenuPanels/Play1.txt", "play1");
            
            CreatePanel("Load/MenuPanels/Rankings.txt", "ranking");
            CreatePanel("Load/MenuPanels/Tutorial1.txt", "tutorial1");
            CreatePanel("Load/MenuPanels/Tutorial2.txt", "tutorial2");
            CreatePanel("Load/MenuPanels/Tutorial3.txt", "tutorial3");
            CreatePanel("Load/MenuPanels/Tutorial4.txt", "tutorial4");
            CreatePanel("Load/MenuPanels/Options.txt", "options");
            CreatePanel("Load/MenuPanels/Credits.txt", "credits");

            //if (panelPlay2Name == "play2_")
                CreatePanel("Load/MenuPanels/Play2.txt", "play2_0");
            //else if (panelPlay2Name == "play2Shared") {
                CreatePanel("Load/MenuPanels/Play2Right.txt", "play2Shared1", offsetWidth: 480, idAssociatePlayerScreen: 0);
                CreatePanel("Load/MenuPanels/Play2Right.txt", "play2Shared0", offsetWidth: -480, idAssociatePlayerScreen: 1);
            //}
            
        }

        public void CreateAlphabetButtons(string panelName, int offsetWidth = 0, int idAssociatePlayerScreen = 0) {
            string[] firstLine = { "q", "w", "e", "r", "t", "z", "u", "i", "o", "p" };
            string[] secondLine = { "a", "s", "d", "f", "g", "h", "j", "k", "l" };
            string[] thirdLine = { "y", "x", "c", "v", "b", "n", "m" };
            string[][] keyboard = { firstLine, secondLine, thirdLine };
            Vector2[] startoffset = { new Vector2(935, 560), new Vector2(960, 560), new Vector2(985, 560) };
            float scaleAlphabet = 0.37f;

            List<Button> buttonsCurrentPanel2 = new List<Button>();
            for (int i = 0; i < keyboard.Length; i++) {
                int count = 0;
                foreach (var elem in keyboard[i]) {
                    var letterButton = new Button(texturesButtons["button"], startoffset[i] + new Vector2(offsetWidth, i* scaleAlphabet * texturesButtons["button"].Height) + count * new Vector2(scaleAlphabet * texturesButtons["button"].Width, 0)) {
                        Text = elem,
                        ScaleWidth = scaleAlphabet,
                        ScaleHeight = scaleAlphabet
                    };
                    letterButton.indexAssociatedPlayerScreen = idAssociatePlayerScreen;
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    MenuManager.Instance.MenuRect[panelName].AddComponent(letterButton);

                    linkedButtonLeftRight.Add(letterButton);
                    buttonsCurrentPanel2.Add(letterButton);

                    if (i == 0 && count == 0)
                        letterButton.nameIdentifier = "Alphabet1";
                    if (elem == secondLine[0])
                        letterButton.nameIdentifier = "Alphabet2";
                    if (elem == thirdLine[0])
                        letterButton.nameIdentifier = "Alphabet3";

                    if (i == 0)
                        letterButton.NeighborUp = FindMenuComponentinPanelWithName("Player2", panelName);
                    if (i == keyboard.Length - 1)
                        letterButton.NeighborUp = FindMenuComponentinPanelWithName("SaveAlphabet", panelName);

                    ++count;

                }
                if (i == keyboard.Length - 1) {
                    var letterButton = new Button(texturesButtons["button"], startoffset[i] + new Vector2(offsetWidth, i * scaleAlphabet * texturesButtons["button"].Height) + count * new Vector2(scaleAlphabet * texturesButtons["button"].Width, 0)) {
                        Text = "del",
                        ScaleWidth = scaleAlphabet,
                        ScaleHeight = scaleAlphabet
                    };
                    letterButton.indexAssociatedPlayerScreen = idAssociatePlayerScreen;
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    letterButton.NeighborDown = FindMenuComponentinPanelWithName("SaveAlphabet", panelName);
                    MenuManager.Instance.MenuRect[panelName].AddComponent(letterButton);
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
                    buttonsCurrentPanel2[i].NeighborDown = FindMenuComponentinPanelWithName("SaveAlphabet", panelName);
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

        public void CreatePanel(string pathName, string panelName, bool setPanelActive = false, int offsetWidth = 0, int idAssociatePlayerScreen = 0) {
            MenuManager.Instance.MenuRect[panelName].active = setPanelActive;
            List<MenuStruct> panelObjects = File.ReadMenuPanel(pathName);
            foreach (MenuStruct MS in panelObjects) {
                if (MS.menuType == MenuType.Button) {
                    if (MS.Name == "Alphabet") CreateAlphabetButtons(panelName, offsetWidth, idAssociatePlayerScreen);
                    else {
                        Button button = new Button(texturesButtons[MS.TextureName], MS.Position + new Vector2(offsetWidth, 0));
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
                        button.indexAssociatedPlayerScreen = idAssociatePlayerScreen;
                        MenuManager.Instance.MenuRect[panelName].AddComponent(button);
                    }
                }
                else if (MS.menuType == MenuType.Text) {
                    TextBox textBox = new TextBox();
                    if (MS.Name != null) textBox.NameIdentifier = MS.Name;
                    if (MS.Position != null) textBox.InitPos = MS.Position + new Vector2(offsetWidth, 0);
                    if (MS.Text != null) textBox.Text = MS.Text;
                    else textBox.Text = "";
                    MenuManager.Instance.MenuRect[panelName].AddComponent(textBox);
                }
                else if (MS.menuType == MenuType.Image) {
                    Image img = new Image(texturesButtons[MS.TextureName]);
                    if (MS.Position != null) img.Position = MS.Position + new Vector2(offsetWidth, 0);
                    if (MS.Name != null) img.NameIdentifier = MS.Name;
                    if (MS.ScaleHeight != default(float)) img.ScaleHeight = MS.ScaleHeight;
                    if (MS.ScaleWidth != default(float)) img.ScaleWidth = MS.ScaleWidth;
                    img.Active = MS.Active; 
                    MenuManager.Instance.MenuRect[panelName].AddComponent(img);
                }
                else if (MS.menuType == MenuType.Slider) {
                    Slider slider = new Slider(MS.Position + new Vector2(offsetWidth, 0), texturesButtons["button"]);
                    //if (MS.Position != null) slider.Position = ;
                    if (MS.Name != null) slider.nameIdentifier = MS.Name;
                    if (MS.TextureName != null) slider.Texture = texturesButtons[MS.TextureName];
                    slider.indexAssociatedPlayerScreen = idAssociatePlayerScreen;
                    MenuManager.Instance.MenuRect[panelName].AddComponent(slider);
                }
            }

            foreach (MenuStruct MS in panelObjects) {
                //if (panelName == "options")
                  //  panelName = panelName;

                if (MS.Name != null && MS.NeighborsUpDownLeftRight != null) {
                    MenuComponent menuComp = FindMenuComponentinPanelWithName(MS.Name, panelName);

                    if (MS.NeighborsUpDownLeftRight[0] != "null") menuComp.NeighborUp = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[0], panelName);
                    if (MS.NeighborsUpDownLeftRight[1] != "null") menuComp.NeighborDown = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[1], panelName);
                    if (MS.NeighborsUpDownLeftRight[2] != "null") menuComp.NeighborLeft = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[2], panelName);
                    if (MS.NeighborsUpDownLeftRight[3] != "null") menuComp.NeighborRight = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[3], panelName);

                }
                if (MS.Name != null && MS.UniqueChoiceButtonWith != null) {
                    Button bu = (Button)FindMenuComponentinPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.UniqueChoiceButtonWith)
                        bu.neighbors.Add((Button)FindMenuComponentinPanelWithName(elem, panelName));
                }
                
                if (MS.menuType == MenuType.Button && MS.Name != null && MS.Functions != null) {
                    Button bu = (Button)FindMenuComponentinPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.Functions) {
                        bu.Click += functionsMenu[elem];
                    }
                }
                else if (MS.menuType == MenuType.Button && MS.Name != null && MS.Functions != null) {
                    Slider bu = (Slider)FindMenuComponentinPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.Functions) {
                        bu.OnReleaseSlider += functionsMenu[elem];
                    }
                }
            }

            if (panelName == "play2" || panelName == "play2Shared0" || panelName == "play2Shared1") {
                FindMenuComponentinPanelWithName("Model3", panelName).NeighborDown = FindMenuComponentinPanelWithName("Back", panelName);
                FindMenuComponentinPanelWithName("Back", panelName).NeighborLeft = FindMenuComponentinPanelWithName("Model3", panelName); ;
                FindMenuComponentinPanelWithName("Next", panelName).NeighborRight = FindMenuComponentinPanelWithName("SaveAlphabet", panelName);

                FindMenuComponentinPanelWithName("Alphabet1", panelName).NeighborLeft = FindMenuComponentinPanelWithName("Model1", panelName);
                FindMenuComponentinPanelWithName("Alphabet2", panelName).NeighborLeft = FindMenuComponentinPanelWithName("Model2", panelName);
                FindMenuComponentinPanelWithName("Alphabet3", panelName).NeighborLeft = FindMenuComponentinPanelWithName("Model3", panelName);
            }

            if (panelName == "ranking")
                LoadRankingsText();

        }

        public MenuComponent FindMenuComponentinPanelWithName(string buttonName, string panelName) {
            foreach (Component comp in MenuManager.Instance.MenuRect[panelName].components) {
                if (comp is MenuComponent bu) {
                    if (bu.nameIdentifier == buttonName) {
                        return bu;
                    }
                }
            }
            return default(MenuComponent);
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
