// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Menu;
using BRS.Scripts.Managers;

namespace BRS.Scripts.UI {

    /// <summary>
    /// Create the menu Panels from files
    /// </summary>
    public class Menu : Component{

        #region Properties and attributes

        /// <summary>
        /// Store the different textures used for the menu
        /// </summary>
        public Dictionary<string, Texture2D> TexturesButtons;

        /// <summary>
        /// Store the different action to be be performed for specific events
        /// </summary>
        public Dictionary<string, EventHandler> FunctionsMenu;

        /// <summary>
        /// Instance of the class
        /// </summary>
        public static Menu Instance;

        /// <summary>
        /// Specification of the screen
        /// </summary>
        readonly Vector2 _middleScreen = new Vector2(Screen.Width / 2, Screen.Height / 2);
        readonly Vector2 _screenSizeVec = new Vector2(Screen.Width, Screen.Height);

        /// <summary>
        /// Circular list of button where each element is connected to his two neighbors
        /// </summary>
        public List<Button> linkedButtonLeftRight = new List<Button>();

        /// <summary>
        /// Defines the possible components types of the menu
        /// </summary>
        public enum MenuType { Button, Text, Slider, Image, TickBox, none};

        /// <summary>
        /// Describes the attributes of a component of the menu
        /// </summary>
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
            public bool Active, CurrentSelection, IsClicked, deSelectOnMove, UseBigFont;
            public List<string> UniqueChoiceButtonWith;
            public int transparency;
        }

        #endregion

        #region Monogame-methods

        public void LoadContent() {
            Instance = this;

            // Load needed textures
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
            Texture2D textureMenuIcon = File.Load<Texture2D>("Images/UI/Menu");
            Texture2D textureRestartIcon = File.Load<Texture2D>("Images/UI/Restart");
            Texture2D textureTuto1 = File.Load<Texture2D>("Images/tutorial/tutorial_1");
            Texture2D textureTuto2 = File.Load<Texture2D>("Images/tutorial/tutorial_2");
            Texture2D textureTuto3 = File.Load<Texture2D>("Images/tutorial/tutorial_3");
            Texture2D textureModel1Back = File.Load<Texture2D>("Images/vehicles_menu_pics/fl_back");
            Texture2D textureModel2Back = File.Load<Texture2D>("Images/vehicles_menu_pics/sw_back");
            Texture2D textureModel3Back = File.Load<Texture2D>("Images/vehicles_menu_pics/bz_back");
            Texture2D textureModel1Color = File.Load<Texture2D>("Images/vehicles_menu_pics/fl_color");
            Texture2D textureModel2Color = File.Load<Texture2D>("Images/vehicles_menu_pics/sw_color");
            Texture2D textureModel3Color = File.Load<Texture2D>("Images/vehicles_menu_pics/bz_color");
            Texture2D textureCredits = File.Load<Texture2D>("Images/tutorial/Credits");
            Texture2D textureButtonDelete = File.Load<Texture2D>("Images/Ui/ArrowDelete");

            // Set mapping name - textures
            TexturesButtons = new Dictionary<string, Texture2D> {
                { "model1Back", textureModel1Back },
                { "model2Back", textureModel2Back },
                { "model3Back", textureModel3Back },
                { "model1Color", textureModel1Color },
                { "model2Color", textureModel2Color },
                { "model3Color", textureModel3Color },
                { "button", textureButton },
                { "background", textureButtonBackground },
                { "bigBackground", textureButtonBigBackground },
                { "buttonCircle", textureButtonCircle },
                { "title", textureButtonTitle },
                { "arrowLeft", textureArrowLeft },
                { "arrowRight", textureArrowRight },
                { "buttonAccept", textureButtonAccept },
                { "slider", textureSlider },
                { "menu", textureMenuIcon },
                { "restart", textureRestartIcon },
                { "imageTuto1", textureTuto1 },
                { "imageTuto2", textureTuto2 },
                { "imageTuto3", textureTuto3 },
                { "imageCredits", textureCredits },
                { "deleteLetter", textureButtonDelete },
                { "tickBoxCliqued", textureTickBoxCliqued },
                { "tickBoxNotCliqued", textureTickBoxNotCliqued }
            };

            // Set mapping name - functions
            FunctionsMenu = new Dictionary<string, EventHandler> {
                { "SwitchToMenu", MenuManager.Instance.SwitchToMenu },
                { "SetDefaultParametersGame", MenuManager.Instance.SetDefaultParametersGame },
                { "UpdateRoundDuration", MenuManager.Instance.UpdateRoundDuration },
                { "UpdateNoPlayers", MenuManager.Instance.UpdateNoPlayers },
                //{ "SwitchRankingDisplay", MenuManager.Instance.SwitchRankingDisplay },
                { "UpdateTemporaryNamePlayer", MenuManager.Instance.UpdateTemporaryNamePlayer },
                { "ChangeNamePlayer", MenuManager.Instance.ChangeNamePlayer },
                { "ChangeModelPlayer", MenuManager.Instance.ChangeModelPlayer },
                //{ "UpdatePlayersNameInfosToChange", MenuManager.Instance.UpdatePlayersNameInfosToChange },
                { "HighlightBorders", MenuManager.Instance.HighlightBorders },
                { "GoDown", MenuManager.Instance.GoDown },
                { "GoRight", MenuManager.Instance.GoRight },
                { "UpdateVolume", MenuManager.Instance.UpdateVolume },
                { "LoadMenu", MenuManager.Instance.LoadMenuFunction },
                { "ResumeGame", MenuManager.Instance.ResumeGame },
                { "UpdateChosenColor", MenuManager.Instance.UpdateChosenColor },
                { "StartGamePlayersReady", MenuManager.Instance.StartGamePlayersReady },
                { "StartGameFunction", MenuManager.Instance.StartGameFunction },
                { "UpdateRanking", MenuManager.Instance.UpdateRanking },
                { "SwitchModelStat", MenuManager.Instance.SwitchModelStat },
                { "SetMusic", MenuManager.Instance.SetMusic },
                { "SetLevelDiffculty", MenuManager.Instance.SetLevelDiffculty },
                { "SetCamera", MenuManager.Instance.SetCamera},
                { "SetMode", MenuManager.Instance.SetMode}
            };
        }

        #endregion

        #region Set up menu
        // Pause Menu for the game level
        public void BuildPausePanel() {
            CreatePanel("Load/MenuPanels/PauseMenu.txt", "pause");
        }

        // Normal menu
        public void BuildMenuPanels(string panelPlay2Name) {
            CreatePanel("Load/MenuPanels/MainMenu.txt", "main", true);
            CreatePanel("Load/MenuPanels/Play1.txt", "play1");
            CreatePanel("Load/MenuPanels/Rankings.txt", "ranking");
            CreatePanel("Load/MenuPanels/Tutorial1.txt", "tutorial1");
            CreatePanel("Load/MenuPanels/Tutorial2.txt", "tutorial2");
            CreatePanel("Load/MenuPanels/Tutorial3.txt", "tutorial3");
            CreatePanel("Load/MenuPanels/Options.txt", "options");
            CreatePanel("Load/MenuPanels/Credits.txt", "credits");
            CreatePanel("Load/MenuPanels/Play2SharedTeamA.txt", "play2Shared0", offsetWidth: -480, idAssociatePlayerScreen: 0);
            CreatePanel("Load/MenuPanels/Play2SharedTeamB.txt", "play2Shared1", offsetWidth: 480, idAssociatePlayerScreen: 1);
            CreatePanel("Load/MenuPanels/Play2SharedTeamA.txt", "play2Shared2", offsetWidth: -480, idAssociatePlayerScreen: 2);
            CreatePanel("Load/MenuPanels/Play2SharedTeamB.txt", "play2Shared3", offsetWidth: 480, idAssociatePlayerScreen: 3);
        }
        #endregion

        #region Parse files

        /// <summary>
        /// Create the menu Panels from files
        /// </summary>
        /// <param name="pathName">Path to the file which define the panel</param> 
        /// <param name="panelName">Name of the current panel to contstruct</param>
        /// <param name="setPanelActive">If true, set the panel to active</param>
        /// <param name="offsetWidth">Offset by which all components will be shifted (position)</param>
        /// <param name="idAssociatePlayerScreen">Index of associate player screen, Usefull for split screen menu</param>
        public void CreatePanel(string pathName, string panelName, bool setPanelActive = false, int offsetWidth = 0, int idAssociatePlayerScreen = 0) {
            MenuManager.Instance.MenuRect[panelName].active = setPanelActive;

            // General parse of the file
            List<MenuStruct> panelObjects = File.ReadMenuPanel(pathName);

            // Define menu components
            foreach (MenuStruct MS in panelObjects) {
                if (MS.menuType == MenuType.Button) {
                    if (MS.Name == "Alphabet")
                        CreateAlphabetButtons(panelName, offsetWidth, idAssociatePlayerScreen);
                    else {
                        Button button = new Button(TexturesButtons[MS.TextureName], MS.Position + new Vector2(offsetWidth, 0));

                        if (MS.TextureInsideName != null) button.InsideImage = TexturesButtons[MS.TextureInsideName];
                        if (MS.Text != null) button.Text = MS.Text;
                        if (MS.NameToSwitchTo != null) button.NameMenuToSwitchTo = MS.NameToSwitchTo;
                        if (MS.Name != null) button.nameIdentifier = MS.Name;
                        if (MS.ScaleHeight != default(float)) button.ScaleHeight = MS.ScaleHeight;
                        if (MS.ScaleWidth != default(float)) button.ScaleWidth = MS.ScaleWidth;
                        if (MS.ScaleHeightInside != default(float)) button.ScaleHeightInside = MS.ScaleHeightInside;
                        if (MS.ScaleWidthInside != default(float)) button.ScaleWidthInside = MS.ScaleWidthInside;
                        //if (MS.Index != default(int)) button.Index = MS.Index;
                        if (MS.Color != default(Color)) button.ImageColor = MS.Color;
                        if (MS.ColorInside != default(Color)) button.InsideObjectColor = MS.ColorInside;
                        if (MS.transparency != default(int))
                            button.ImageColor.A = (byte)MS.transparency;

                        button.IsCurrentSelection = MS.CurrentSelection;
                        button.IsClicked = MS.IsClicked;
                        button.IndexAssociatedPlayerScreen = idAssociatePlayerScreen;
                        button.Index = MS.Index;
                        button.DeSelectOnMove = MS.deSelectOnMove;

                        if (MS.UseBigFont)
                            button.Font = UserInterface.menuBigFont;

                        MenuManager.Instance.MenuRect[panelName].AddComponent(button);
                    }
                }
                else if (MS.menuType == MenuType.Text) {
                    if (MS.Name == "ModelsStats")
                        CreateTextModelsStats(panelName, offsetWidth, idAssociatePlayerScreen);
                    else {
                        TextBox textBox = new TextBox();

                        if (MS.Name != null) textBox.NameIdentifier = MS.Name;
                        if (MS.Position != null) textBox.InitPos = MS.Position + new Vector2(offsetWidth, 0);
                        if (MS.Text != null) textBox.Text = MS.Text;
                        else textBox.Text = "";

                        MenuManager.Instance.MenuRect[panelName].AddComponent(textBox);
                    }
                }
                else if (MS.menuType == MenuType.Image) {
                    Image img = new Image(TexturesButtons[MS.TextureName]);

                    if (MS.Position != null) img.Position = MS.Position + new Vector2(offsetWidth, 0);
                    if (MS.Name != null) img.NameIdentifier = MS.Name;
                    if (MS.ScaleHeight != default(float)) img.ScaleHeight = MS.ScaleHeight;
                    if (MS.ScaleWidth != default(float)) img.ScaleWidth = MS.ScaleWidth;
                    if (MS.Color != default(Color)) img.colour = MS.Color;

                    img.Active = MS.Active;

                    MenuManager.Instance.MenuRect[panelName].AddComponent(img);
                }
                else if (MS.menuType == MenuType.Slider) {
                    Slider slider = new Slider(MS.Position + new Vector2(offsetWidth, 0), TexturesButtons["slider"], TexturesButtons["button"]);

                    if (MS.Name != null) slider.nameIdentifier = MS.Name;
                    if (MS.TextureName != null) slider.Texture = TexturesButtons[MS.TextureName];

                    slider.IndexAssociatedPlayerScreen = idAssociatePlayerScreen;

                    MenuManager.Instance.MenuRect[panelName].AddComponent(slider);
                }
                else if (MS.menuType == MenuType.TickBox) {
                    TickBox tickBox = new TickBox(MS.Position + new Vector2(offsetWidth, 0), TexturesButtons["buttonCircle"], TexturesButtons["buttonCircle"]);

                    if (MS.Name != null) tickBox.nameIdentifier = MS.Name;
                    if (MS.ScaleHeight != default(float)) tickBox.ScaleHeight = MS.ScaleHeight;
                    if (MS.ScaleWidth != default(float)) tickBox.ScaleWidth = MS.ScaleWidth;
                    if (MS.ScaleHeightInside != default(float)) tickBox.ScaleHeightClicked = MS.ScaleHeightInside;
                    if (MS.ScaleWidthInside != default(float)) tickBox.ScaleWidthClicked = MS.ScaleWidthInside;

                    tickBox.IsClicked = MS.IsClicked;
                    tickBox.IndexAssociatedPlayerScreen = idAssociatePlayerScreen;

                    MenuManager.Instance.MenuRect[panelName].AddComponent(tickBox);
                }
            }

            // dirty hack :(
            if (panelName == "play2Shared0" || panelName == "play2Shared1" || panelName == "play2Shared2" || panelName == "play2Shared3") {
                Button bu = ((Button)FindMenuComponentinPanelWithName("NamePlayer", panelName));
                bu.Text = "Player " + (idAssociatePlayerScreen + 1).ToString();
                bu.IndexAssociatedPlayerScreen = idAssociatePlayerScreen;
            }

            // Set constraints
            foreach (MenuStruct MS in panelObjects) {
                // Up, Down, Left, Right selection change
                if (MS.Name != null && MS.NeighborsUpDownLeftRight != null) {
                    MenuComponent menuComp = FindMenuComponentinPanelWithName(MS.Name, panelName);

                    if (MS.NeighborsUpDownLeftRight[0] != "null") menuComp.NeighborUp = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[0], panelName);
                    if (MS.NeighborsUpDownLeftRight[1] != "null") menuComp.NeighborDown = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[1], panelName);
                    if (MS.NeighborsUpDownLeftRight[2] != "null") menuComp.NeighborLeft = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[2], panelName);
                    if (MS.NeighborsUpDownLeftRight[3] != "null") menuComp.NeighborRight = FindMenuComponentinPanelWithName(MS.NeighborsUpDownLeftRight[3], panelName);

                }

                // Set list of component with unique clicked possibility
                if (MS.Name != null && MS.UniqueChoiceButtonWith != null) {
                    Button bu = (Button)FindMenuComponentinPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.UniqueChoiceButtonWith)
                        bu.neighbors.Add((Button)FindMenuComponentinPanelWithName(elem, panelName));
                }

                // Set actions on pressed
                if (MS.menuType == MenuType.Button && MS.Name != null && MS.Functions != null) {
                    Button bu = (Button)FindMenuComponentinPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.Functions) {
                        bu.Click += FunctionsMenu[elem];
                    }
                }
                else if (MS.menuType == MenuType.Slider && MS.Name != null && MS.Functions != null) {
                    Slider sl = (Slider)FindMenuComponentinPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.Functions) {
                        sl.OnReleaseSlider += FunctionsMenu[elem];
                    }
                }
                else if (MS.menuType == MenuType.TickBox && MS.Name != null && MS.Functions != null) {
                    TickBox tc = (TickBox)FindMenuComponentinPanelWithName(MS.Name, panelName);
                    foreach (string elem in MS.Functions) {
                        tc.Click += FunctionsMenu[elem];
                    }
                }
            }

            // Dirty hack
            // Create rankings
            if (panelName == "ranking")
                LoadRankingsText();

        }

        #endregion

        #region Custom methods

        /// <summary>
        /// Set the left/right selection from the circular list
        /// </summary>
        /// <param name="setSelectionFirstElem">If true, set the selection of the first element to true</param>  
        private void SetNeighborsButtonLeftRight(bool setSelectionFirstElem = true) {
            int indexLeft, indexRight;
            if (setSelectionFirstElem) linkedButtonLeftRight[0].IsCurrentSelection = true;

            for (int i = 0; i < linkedButtonLeftRight.Count; ++i) {
                // circular property
                if (i == 0) indexLeft = linkedButtonLeftRight.Count - 1;
                else indexLeft = i - 1;

                if (i == linkedButtonLeftRight.Count - 1) indexRight = 0;
                else indexRight = i + 1;

                // Set constraints
                linkedButtonLeftRight[i].NeighborLeft = linkedButtonLeftRight[indexLeft];
                linkedButtonLeftRight[i].NeighborRight = linkedButtonLeftRight[indexRight];
            }
            linkedButtonLeftRight.Clear();
        }

        /// <summary>
        /// Find the menuComponent with the given name in the given panel
        /// </summary>
        /// <param name="buttonName">Name of the component to find</param>
        /// <param name="panelName">Name of the panel in which to search the menuComponent</param>
        public MenuComponent FindMenuComponentinPanelWithName(string buttonName, string panelName) {
            foreach (Component comp in MenuManager.Instance.MenuRect[panelName].components) {
                if (comp is MenuComponent MC) {
                    if (MC.nameIdentifier == buttonName) {
                        return MC;
                    }
                }
            }
            return default(MenuComponent);
        }

        /// <summary>
        /// Create components do display the statistics of the model used
        /// </summary>
        /// <param name="panelName">Name of the current panel to contstruct</param>
        /// <param name="offsetWidth">Offset by which all components will be shifted (position)</param>
        /// <param name="idAssociatePlayerScreen">Index of associate player screen, Usefull for split screen menu</param>
        public void CreateTextModelsStats(string panelName, int offsetWidth = 0, int idAssociatePlayerScreen = 0) {
            Vector2[] offsetStart = { new Vector2(950, 720), new Vector2(1150, 720) };

            // For each model create a list of stats
            for (int i = 0; i < ScenesCommunicationManager.ValuesStats.Length; ++i) {
                ListComponents listStats = new ListComponents("model" + i.ToString() + "Stats");
                int count = 0;
                float offsetHeight = 40;

                // Name of the stats
                foreach (string name in ScenesCommunicationManager.NameStats) {
                    var nameCapacity = new TextBox() {
                        InitPos = offsetStart[0] + new Vector2(offsetWidth, count * offsetHeight),
                        Text = name,
                    };
                    nameCapacity.Font = UserInterface.menuSmallFont;
                    nameCapacity.Colour = new Color(148, 148, 148);
                    listStats.AddComponent(nameCapacity);
                    ++count;
                }
                
                // Capacity
                var statCapacity = new Slider(offsetStart[1] + new Vector2(offsetWidth, 0 * offsetHeight), TexturesButtons["slider"]) { };
                statCapacity.lengthSlider = 175;
                statCapacity.percentPosButon = ((float)ScenesCommunicationManager.ValuesStats[i].Capacity) / ((float)ScenesCommunicationManager.maxModelStats.Capacity);

                // Distance of attack
                var statAttack = new Slider(offsetStart[1] + new Vector2(offsetWidth, 1 * offsetHeight), TexturesButtons["slider"]) { };
                statAttack.lengthSlider = 175;
                statAttack.percentPosButon = ScenesCommunicationManager.ValuesStats[i].AttackDistance / ScenesCommunicationManager.maxModelStats.AttackDistance;

                // Speed
                var statSpeed = new Slider(offsetStart[1] + new Vector2(offsetWidth, 2 * offsetHeight), TexturesButtons["slider"]) { };
                statSpeed.lengthSlider = 175;
                statSpeed.percentPosButon = ScenesCommunicationManager.ValuesStats[i].MaxSpeed / ScenesCommunicationManager.maxModelStats.MaxSpeed;

                // Add to list of stat for the current model
                listStats.AddComponent(statCapacity);
                listStats.AddComponent(statAttack);
                listStats.AddComponent(statSpeed);

                if (i != 0) listStats.Active = false;

                

                // Add list to current panel
                MenuManager.Instance.MenuRect[panelName].AddComponent(listStats);
            }

        }

        /// <summary>
        /// Create buttons for the alpahbet to change the name of a player
        /// </summary>
        /// <param name="panelName">Name of the current panel to contstruct</param>
        /// <param name="offsetWidth">Offset by which all components will be shifted (position)</param>
        /// <param name="idAssociatePlayerScreen">Index of associate player screen, Usefull for split screen menu</param>
        public void CreateAlphabetButtons(string panelName, int offsetWidth = 0, int idAssociatePlayerScreen = 0) {
            // Describe keyboard
            string[] firstLine = { "Q", "W", "E", "R", "T", "Z", "U", "I", "O", "P" };
            string[] secondLine = { "A", "S", "D", "F", "G", "H", "J", "K", "L", "-" };
            string[] thirdLine = { "Y", "X", "C", "V", "B", "N", "M", "?", "!" };
            string[][] keyboard = { firstLine, secondLine, thirdLine };
            float scaleAlphabet;

            Vector2[] startoffset;
            //startoffset = new Vector2[] { new Vector2(860, 230), new Vector2(885, 230), new Vector2(910, 230) };
            startoffset = new Vector2[] { new Vector2(860, 230), new Vector2(860, 230), new Vector2(860, 230) };
            scaleAlphabet = 0.37f;

            List<Button> buttonsCurrentPanel2 = new List<Button>();

            // Create components for the keyboard
            for (int i = 0; i < keyboard.Length; i++) {
                int count = 0;
                
                // Create button for each letter of the current line
                foreach (var elem in keyboard[i]) {
                    var letterButton = new Button(TexturesButtons["button"], startoffset[i] + new Vector2(offsetWidth, i * scaleAlphabet * TexturesButtons["button"].Height) + count * new Vector2(scaleAlphabet * TexturesButtons["button"].Width, 0)) {
                        Text = elem,
                        ScaleWidth = scaleAlphabet,
                        ScaleHeight = scaleAlphabet
                    };
                    letterButton.Font = UserInterface.menuSmallFont;
                    letterButton.IndexAssociatedPlayerScreen = idAssociatePlayerScreen;
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    MenuManager.Instance.MenuRect[panelName].AddComponent(letterButton);

                    linkedButtonLeftRight.Add(letterButton);
                    buttonsCurrentPanel2.Add(letterButton);

                    if (i == 0 && count == 0) {
                        letterButton.nameIdentifier = "Alphabet1";
                        letterButton.IsCurrentSelection = true;
                    }
                    if (elem == secondLine[0])
                        letterButton.nameIdentifier = "Alphabet2";
                    if (elem == thirdLine[0])
                        letterButton.nameIdentifier = "Alphabet3";

                    ++count;

                }
                // create button to delete a character of the player's name
                if (i == keyboard.Length - 1) {
                    var letterButton = new Button(TexturesButtons["button"], startoffset[i] + new Vector2(offsetWidth, i * scaleAlphabet * TexturesButtons["button"].Height) + count * new Vector2(scaleAlphabet * TexturesButtons["button"].Width, 0)) {
                        //Text = "del",
                        ScaleWidth = scaleAlphabet,
                        ScaleHeight = scaleAlphabet,
                        nameIdentifier = "delete"
                    };
                    letterButton.InsideImage = TexturesButtons["deleteLetter"];
                    letterButton.Font = UserInterface.menuSmallFont;
                    letterButton.IndexAssociatedPlayerScreen = idAssociatePlayerScreen;
                    letterButton.Click += MenuManager.Instance.UpdateTemporaryNamePlayer;
                    letterButton.NeighborUp = buttonsCurrentPanel2[firstLine.Length + secondLine.Length - 2];
                    MenuManager.Instance.MenuRect[panelName].AddComponent(letterButton);

                    linkedButtonLeftRight.Add(letterButton);
                    buttonsCurrentPanel2.Add(letterButton);
                }

                // Set left/right selection for the elements of the current line
                SetNeighborsButtonLeftRight(false);
            }

            // set down/up selection for the elements of the keyboard
            for (int i = 0; i < buttonsCurrentPanel2.Count; ++i) {
                if (i < firstLine.Length) {
                    int offsetTmp = firstLine.Length;
                    if (i > secondLine.Length - 1) offsetTmp = secondLine.Length;

                    buttonsCurrentPanel2[i].NeighborDown = buttonsCurrentPanel2[i + offsetTmp];
                }
                else if (i < firstLine.Length + secondLine.Length) {
                    int offsetTmp = secondLine.Length;
                    if (i - firstLine.Length > thirdLine.Length) offsetTmp = thirdLine.Length;

                    buttonsCurrentPanel2[i].NeighborDown = buttonsCurrentPanel2[i + offsetTmp];
                    buttonsCurrentPanel2[i].NeighborUp = buttonsCurrentPanel2[i - firstLine.Length];
                }
                else {
                    buttonsCurrentPanel2[i].NeighborDown = FindMenuComponentinPanelWithName("ModelChangeLeft", panelName);
                    buttonsCurrentPanel2[i].NeighborUp = buttonsCurrentPanel2[i - secondLine.Length];
                }
            }
        }

        /// <summary>
        /// Create components to display the rankings
        /// </summary>
        public void LoadRankingsText() {
            ListComponents rankings = new ListComponents("rankings_game");
            Vector2[] offsetStart = { new Vector2(864, 320), new Vector2(1056, 320) };

            int i = 0;
            // Create list of components for each combinaison of (#players <-> duration of a round)
            foreach (var noPlayers in MenuManager.Instance.RankingPlayersText) {
                foreach (var durationRound in MenuManager.Instance.RankingDurationText) {
                    List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/ranking" + durationRound + noPlayers + ".txt");

                    ListComponents listPersons = new ListComponents("ranking" + durationRound + noPlayers);
                    int count = 0;
                    // Text component with player's name and score
                    foreach (var aPerson in rankinglist) {
                        var namePerson = new TextBox() {
                            InitPos = offsetStart[0] + new Vector2(0, count * 40),
                            Text = aPerson.Item1
                        };
                        namePerson.Font = UserInterface.menuSmallFont;

                        var score = new TextBox() {
                            InitPos = offsetStart[1] + new Vector2(0, count * 40),
                            Text = aPerson.Item2
                        };
                        score.Font = UserInterface.menuSmallFont;

                        // Add to list
                        listPersons.AddComponent(namePerson); listPersons.AddComponent(score);
                        count++;
                    }

                    if (i != 0) listPersons.Active = false;
                    ++i;

                    rankings.AddComponent(listPersons);
                }
            }
            MenuManager.Instance.MenuRect["ranking"].AddComponent(rankings);
        }

        #endregion

    }
}
