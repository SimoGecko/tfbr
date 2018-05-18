// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using BRS.Engine.Menu;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts.Managers {

    /// <summary>
    /// Controls the menu: set,update and draw menu
    ///                    handles transition, 
    ///                    defines functions for the components of the menu
    /// </summary>
    class MenuManager : Component{

        #region Properties and attributes

        /// <summary>
        /// Instance of MenuManager
        /// </summary>
        public static MenuManager Instance;

        /// <summary>
        /// List of menu panels and their associated camera positions
        /// </summary>
        public Dictionary<string, GameObject> MenuRect = new Dictionary<string, GameObject>();
        public Dictionary<string, Transform> MenuCam = new Dictionary<string, Transform>();

        /// <summary>
        /// Store current settings
        /// </summary>
        GameObject _currentMenu;
        string _currentMenuName;
        Transform _currentMenuCam;
        readonly Menu _menuGame = new Menu();

        /// <summary>
        /// Name of the new panel for transition
        /// </summary>
        string _changeToMenu;

        /// <summary>
        /// Name of the panel for splitScreen
        /// </summary>
        public string panelPlay2NameOption = "play2Shared";

        /// <summary>
        /// Rankings possibilities
        /// </summary>
        public string[] RankingPlayersText = { "1P", "2P", "4P" };
        public string[] RankingDurationText = { "2 min", "3 min", "5 min", "10 min" };
        int _idRankingPlayerText = 0;
        int _idRankingDurationText = 0;

        /// <summary>
        /// Camera transition settings
        /// </summary>
        public float TransitionTime = 0.5f;
        public float DistTransition = 13;
        public float CurrentDistTransition;
        Vector3 _velocityPos = Vector3.Zero;
        Vector3 _velocityRot = Vector3.Zero;
        float _time = 0;

        /// <summary>
        /// Name of the player for which paramaters are going to change
        /// </summary>
        public string NamePlayerInfosToChange;

        /// <summary>
        /// Ensure that the default players name get overwritten for the first letter button press
        /// </summary>
        bool[] NamePlayersChanged = { false, false, false, false };

        /// <summary>
        /// Ensure only one change per frame
        /// </summary>
        public static bool[] uniqueFrameInputUsed = { false, false, false, false };
        private bool uniqueMenuSwitchUsed = false;

        /// <summary>
        /// Index for chosable parameter from ScenesCommunicationManager
        /// </summary>
        int[] _idModel = { 0, 0 , 0, 0};
        int[] _idColor = { 0, 1 };

        #endregion

        #region Monogame-methods

        /// <summary>
        /// Monogame Start function
        /// </summary>
        public override void Start() {
            base.Start();

            if (ScenesCommunicationManager.loadOnlyPauseMenu)
                LoadPauseMenu();
            else
                LoadContent();
        }

        /// <summary>
        /// Load Pause Menu
        /// </summary>
        public void LoadPauseMenu() {
            Instance = this;

            // Load menu content (texture,functions)
            _menuGame.LoadContent();

            // Create panels to be load from files
            string[] namePanels = { "pause" };
            foreach (string name in namePanels) {
                GameObject go = new GameObject(name);
                MenuRect.Add(go.name, go);
            }

            // Set current settings
            _currentMenu = MenuRect["pause"];
            _currentMenuName = "pause";

            // Load panels components from files
            Menu.Instance.BuildPausePanel();
        }

        /// <summary>
        /// Load Menu
        /// </summary>
        public void LoadContent() {
            Instance = this;

            // Load menu content (texture,functions)
            SetDefaultValues();
            _menuGame.LoadContent();
            GameManager.state = GameManager.State.Menu;

            // Create panels to be load from files
            string[] namePanels = { "main", "credits", "tutorial1", "tutorial2", "tutorial3", "tutorial4", "ranking", "options", "play1", "play2Shared0", "play2Shared1", "play2Shared2", "play2Shared3" };
            Vector3[] posCam = { new Vector3(0,15,12), new Vector3(20,12,10), new Vector3(0,10,0), new Vector3(9,2,-32), new Vector3(0,10,-25), new Vector3(0, 10, 0), new Vector3(-22,7,-15), new Vector3(22,7,-15), new Vector3(-7,3,0), new Vector3(2,3,0), new Vector3(2, 3, 0), new Vector3(2, 3, 0), new Vector3(2, 3, 0) };
            Vector3[] rotCam = { new Vector3(-37,0,0), new Vector3(-35,45,0), new Vector3(-30,0,0), new Vector3(-15,70,0), new Vector3(-40,0,0), new Vector3(-30, 0, 0), new Vector3(-20,-40,0), new Vector3(-20,40,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0) };

            for (int i = 0; i < namePanels.Length; ++i) {
                GameObject go = new GameObject(namePanels[i]);
                MenuRect.Add(namePanels[i], go);

                Transform tr = new Transform(posCam[i], rotCam[i]);
                MenuCam.Add(namePanels[i], tr);
            }

            // Set current settings
            _currentMenu = MenuRect["main"];
            _currentMenuName = "main";
            _currentMenuCam = MenuCam["main"];

            NamePlayerInfosToChange = "player_0";

            foreach (Camera c in Screen.Cameras) {
                c.transform.position = _currentMenuCam.position;
                c.transform.eulerAngles = _currentMenuCam.eulerAngles;
            }

            // Load panels components from files
            Menu.Instance.BuildMenuPanels(panelPlay2NameOption);
        }

        /// <summary>
        /// Monogame Update function
        /// </summary>
        public override void Update() {
            
            // Ensure one change per frame
            uniqueMenuSwitchUsed = false;
            for (int i = 0; i < uniqueFrameInputUsed.Length; ++i)
                uniqueFrameInputUsed[i] = false;

            // Check for button B pressed to go to previous panel
            if (Input.GetKeyUp(Keys.B) || Input.GetButtonUp(Buttons.B)) {
                if (_currentMenuName == "play2Shared2") {
                    MenuRect["play2Shared0"].active = true;
                    MenuRect["play2Shared1"].active = true;
                    MenuRect["play2Shared2"].active = false;
                    MenuRect["play2Shared3"].active = false;
                    _currentMenuName = "play2Shared0";
                    _changeToMenu = "play2Shared0";
                }
                else {
                    Button bu = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Back", _currentMenuName);
                    if (bu != default(Button)) {
                        Audio.Play("button_press_B", transform.position);
                        bu.Click?.Invoke(bu, new EventArgs());
                    }
                } 
            }

            if (Input.GetKeyUp(Keys.A) || Input.GetButtonUp(Buttons.A)) {
                if (_currentMenuName.Substring(0, _currentMenuName.Length - 1) == "tutorial") {
                    Button bu = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Next", _currentMenuName);
                    if (bu != default(Button)) bu.Click?.Invoke(bu, new EventArgs());
                }

            }

            // Update camera transition
            if (_changeToMenu != null)
                TransitionCam();               
        }

        /// <summary>
        /// Monogame Draw function
        /// </summary>
        public override void Draw2D(int i) {
            if (_currentMenuName.Substring(0, _currentMenuName.Length - 1) == "tutorial") {
                Image blackBackground = new Image(Menu.Instance.TexturesButtons["textureBlack"]);
                blackBackground.Position = new Vector2(960, 540);
                blackBackground.colour.A = 25;
                blackBackground.Draw2D(i);

                UserInterface.DrawString("Next:", new Rectangle(115, 100, 40, 40), Align.TopLeft, Align.Center, Align.Center, font: UserInterface.menuSmallFont);
                Suggestions.Instance.GiveCommand(0, new Rectangle(185, 95, 40, 40), XboxButtons.A, Align.TopLeft);
            }
        }

        #endregion

        #region Custom methods

        /// <summary>
        /// Set default the parameters of the game
        /// </summary>
        public void SetDefaultValues() {
            GameManager.NumPlayers = 2;
            RoundManager.RoundTime = 2 * 60;
            GameMode.currentGameMode = ScenesCommunicationManager.ModesName[0];
            GameManager.LvlScene = 1;
            GameManager.lvlDifficulty = 1;
        }

        /// <summary>
        /// Updates Camera position and rotation for a smooth camera transition towards a goal
        /// </summary>
        public void TransitionCam() {
            // Transition destination (=goal)
            string newMenuName = _changeToMenu;
            if (_changeToMenu == "play2Shared")
                newMenuName = _changeToMenu + "0";
            Transform goalTransform = MenuCam[newMenuName];

            // Compute time of transition based on length
            CurrentDistTransition = (Screen.Cameras[0].transform.position - goalTransform.position).Length();
            float newTransitionTime = TransitionTime * CurrentDistTransition / DistTransition;

            // Update camera position and rotation
            foreach (Camera c in Screen.Cameras) {
                c.transform.position = Utility.SmoothDamp(c.transform.position, goalTransform.position, ref _velocityPos, newTransitionTime);
                c.transform.eulerAngles = Utility.SmoothDampAngle(c.transform.eulerAngles, goalTransform.eulerAngles, ref _velocityRot, newTransitionTime);
            }
            
            // Show new panel after a certain time
            if (_time < newTransitionTime*2) {
                _time += Time.DeltaTime;
            }
            else {
                // Reset time for next transition
                _time = 0;

                // Set new current menu settings
                if (_changeToMenu != "play2Shared") {
                    _currentMenu = MenuRect[_changeToMenu];
                    _currentMenuName = _changeToMenu;
                }
                else {
                    _currentMenuName = _changeToMenu + "0";
                    _currentMenu = MenuRect[_currentMenuName];
                }

                // activate new current menu panel
                if (_changeToMenu != "play2Shared")
                    _currentMenu.active = true;
                else if (_currentMenuName == "play2Shared0") {
                    MenuRect[_changeToMenu + "0"].active = true;
                    if (GameManager.NumPlayers == 2 || GameManager.NumPlayers == 4)
                        MenuRect[_changeToMenu + "1"].active = true;
                }
            }
        }

        /// <summary>
        /// Change player's name, model and color according to the saved infos
        /// </summary>
        /// <param name="player">Player</param> 
        /// <param name="i">Player's Index</param> 
        public void ChangeModelNameColorPlayer(GameObject player, int i) {
            if (ScenesCommunicationManager.Instance != null) {
                if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey("player_" + i)) {
                    // Get infos
                    string userName = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item1;
                    int currIdModel = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item2;
                    Model userModel = ScenesCommunicationManager.Instance.ModelCharacter[currIdModel];
                    Color colorPlayer = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item3;

                    // Set infos
                    if (userName != null) player.GetComponent<Player>().PlayerName = userName;
                    if (userModel != null) player.Model = userModel;
                    if (colorPlayer != null) {
                        Rectangle areaChange = new Rectangle(4, 8, 4, 4);
                        SetRectanglePixelColor(areaChange, colorPlayer, ScenesCommunicationManager.Instance.textureColorPlayers["player_" + i]);
                    }

                    // Set corresponding statistic for the model
                    player.GetComponent<PlayerInventory>().SetCapacity(ScenesCommunicationManager.ValuesStats[currIdModel].Capacity);
                    player.GetComponent<PlayerAttack>().AttackDistance = ScenesCommunicationManager.ValuesStats[currIdModel].AttackDistance;
                    player.GetComponent<PlayerMovement>().SetMaxSpeed(ScenesCommunicationManager.ValuesStats[currIdModel].MaxSpeed);
                    player.GetComponent<PlayerMovement>().SetMinSpeed(ScenesCommunicationManager.ValuesStats[currIdModel].MinSpeed);
                }
            }
        }

        /// <summary>
        /// Set the given pixel of a texture to a color
        /// </summary>
        /// <param name="x">X coordinate of pixel</param> 
        /// <param name="y">Y coordinate of pixel</param>
        /// <param name="color">Color to set to</param>
        /// <param name="texture">Texture whose pixel color to change</param>
        public void SetPixelColor(int x, int y, Color color, Texture2D texture) {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colorData);
            colorData[x + y * texture.Width] = color;
            texture.SetData<Color>(colorData);
        }

        /// <summary>
        /// Set the given area of pixels of a texture to a color
        /// </summary>
        /// <param name="rec">Area of pixels</param> 
        /// <param name="color">Color to set to</param>
        /// <param name="texture">Texture whose pixels color to change</param>
        public void SetRectanglePixelColor(Rectangle rec, Color color, Texture2D texture) {
            for (int x = rec.X; x < rec.X + rec.Width; ++x)
                for (int y = rec.Y; y < rec.Y + rec.Height; ++y)
                    SetPixelColor(x, y, color, texture);
        }

        #endregion

        #region Callbacks method when button are pressed

        #region Changes between menu panels

        /// <summary>
        /// Set information of the new menu panel to switch to
        /// </summary>
        public void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;

            // Audio transition
            //Audio.Play("menu_change", transform.position);

            // Set Menu to change to
            _changeToMenu = button.NameMenuToSwitchTo;

            // Desactivate current menu panel
            if (!uniqueMenuSwitchUsed) {
                if (_currentMenu != null)
                    _currentMenu.active = false;
                if (_currentMenuName == "play2Shared") {
                    MenuRect["play2Shared0"].active = false;
                    MenuRect["play2Shared1"].active = false;
                    MenuRect["play2Shared2"].active = false;
                    MenuRect["play2Shared3"].active = false;
                }

                // only one change per frame
                uniqueMenuSwitchUsed = true;
            }
        }

        /// <summary>
        /// Start (directly) the game
        /// </summary>
        public void StartGameFunction(object sender, EventArgs e) {
            ScenesCommunicationManager.loadOnlyPauseMenu = true;
            GameManager.state = GameManager.State.Playing;
            SceneManager.LoadGame = true;
        }

        /// <summary>
        /// Start the game if all other players are ready 
        /// </summary>
        public void StartGamePlayersReady(object sender, EventArgs e) {
            Button button = (Button)sender;

            // Per default other players are not ready
            bool allPlayersReady = false;

            // Check if others players are ready
            if (GameManager.NumPlayers == 1)
                allPlayersReady = true;
            else if (GameManager.NumPlayers == 2 && ((Button)Menu.Instance.FindMenuComponentinPanelWithName("Ready", panelPlay2NameOption + ((button.IndexAssociatedPlayerScreen + 1) % 2).ToString())).IsClicked) {
                allPlayersReady = true;
            }
            else if (GameManager.NumPlayers == 4) {
                if (button.IndexAssociatedPlayerScreen == 0 || button.IndexAssociatedPlayerScreen == 1) {
                    if (((Button)Menu.Instance.FindMenuComponentinPanelWithName("Ready", panelPlay2NameOption + ((button.IndexAssociatedPlayerScreen + 1) % 2).ToString())).IsClicked) {
                        MenuRect[panelPlay2NameOption + "0"].active = false;
                        MenuRect[panelPlay2NameOption + "1"].active = false;
                        MenuRect[panelPlay2NameOption + "2"].active = true;
                        MenuRect[panelPlay2NameOption + "3"].active = true;

                        _currentMenuName = panelPlay2NameOption + "2";
                        _changeToMenu = panelPlay2NameOption + "2";
                        _currentMenu = MenuRect[_currentMenuName];
                     
                        // only one change per frame
                        uniqueMenuSwitchUsed = true;
                    }
                }
                else if (button.IndexAssociatedPlayerScreen == 2 || button.IndexAssociatedPlayerScreen == 3) {
                    if (((Button)Menu.Instance.FindMenuComponentinPanelWithName("Ready", panelPlay2NameOption + ((button.IndexAssociatedPlayerScreen + 1) % 2 + 2).ToString())).IsClicked) {
                        allPlayersReady = true;
                    }
                }
            }

            // Start Game if all players are ready
            if (allPlayersReady) {
                ScenesCommunicationManager.loadOnlyPauseMenu = true;
                GameManager.state = GameManager.State.Playing;
                SceneManager.LoadGame = true;
            }
        }

        /// <summary>
        /// Load menu
        /// </summary>
        public void LoadMenuFunction(object sender, EventArgs e) {
            ScenesCommunicationManager.loadOnlyPauseMenu = false;
            SceneManager.LoadMenu = true;
        }

        /// <summary>
        /// Resume game
        /// </summary>
        public void ResumeGame(object sender, EventArgs e) {
            MenuRect["pause"].active = false;
            GameManager.state = GameManager.State.Playing;
        }

        #endregion

        #region Change game settings

        /// <summary>
        /// Update the duration of a round 
        /// </summary>
        public void UpdateRoundDuration(object sender, EventArgs e) {
            Button button = (Button)sender;
            RoundManager.RoundTime = Int32.Parse(button.Text) * 60;
        }

        /// <summary>
        /// Update the number of players
        /// </summary>
        public void UpdateNoPlayers(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameManager.NumPlayers = Int32.Parse(button.Text);
        }

        /// <summary>
        /// Update the game mode
        /// </summary>
        public void SetMode(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameMode.currentGameMode = ScenesCommunicationManager.ModesName[button.Index];

            foreach (var elem in MenuRect[_currentMenuName].components) {
                if (elem is TextBox tb) {
                    if (tb.NameIdentifier == "ModeName") {
                        tb.Text = ScenesCommunicationManager.ModesDescription[button.Index];
                    }
                }
            }
        }

        /// <summary>
        /// Update the game map
        /// </summary>
        public void SetMap(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameManager.LvlScene = button.Index;
        }

        /// <summary>
        /// Update volume of in game effects
        /// </summary>
        public void UpdateVolume(object sender, EventArgs e) {
            Slider slider = (Slider)sender;
            Audio.SetSoundVolume(slider.percentPosButon);
        }

        /// <summary>
        /// Mute or not the background music
        /// </summary>
        public void SetMusic(object sender, EventArgs e) {
            TickBox tickbox = (TickBox)sender;
            if (tickbox.IsClicked)
                Audio.SetMusicVolume(.1f);
            else
                Audio.SetMusicVolume(0);
        }

        /// <summary>
        /// Set camera option (auto or manual)
        /// </summary>
        public void SetCamera(object sender, EventArgs e) {
            TickBox tickbox = (TickBox)sender;
            if (tickbox.IsClicked)
                CameraController.autoFollow = true;
            else
                CameraController.autoFollow = false;
        }

        /// <summary>
        /// Set difficulty of the level 
        /// </summary>
        public void SetLevelDiffculty(object sender, EventArgs e) {
            Button bu = (Button)sender;
            if (bu.Text == "Easy") GameManager.lvlDifficulty = 0;
            else if (bu.Text == "Normal") GameManager.lvlDifficulty = 1;
            else if (bu.Text == "Hard") GameManager.lvlDifficulty = 2;
            else Debug.Log("Wrong level difficulty");
        }
        
        #endregion

        #region Change player's information

        /// <summary>
        /// Update temporary player's name. 
        /// ! Not saved yet
        /// </summary>
        public void UpdateTemporaryNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            // find object which store his name (in the right panel)
            if (!uniqueMenuSwitchUsed) {
                foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                    if (elem is Button bu) {
                        if (bu.nameIdentifier == "NamePlayer") {
                            // if first change => overwrite
                            if (!NamePlayersChanged[button.IndexAssociatedPlayerScreen]) {
                                bu.Text = "";
                                NamePlayersChanged[button.IndexAssociatedPlayerScreen] = true;
                            }

                            // Add or remove letter
                            if (button.nameIdentifier == "delete") {
                                if (bu.Text.Length > 0)
                                    bu.Text = bu.Text.Substring(0, bu.Text.Length - 1);
                            }
                            else
                                bu.Text += button.Text;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save temporary player's name 
        /// </summary>
        public void ChangeNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            // Get key name of the player
            if (panelPlay2NameOption == "play2Shared")
                NamePlayerInfosToChange = "player_" + button.IndexAssociatedPlayerScreen.ToString();

            // Find temporary name in the correct panel
            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Button bu) {
                    if (bu.nameIdentifier == "NamePlayer") {

                        // Save it 
                        if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                            ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, int, Color>(bu.Text, ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item2, button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor);
                        else
                            ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, int, Color>(bu.Text, 0, button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor));
                    }
                }
            }
        }

        /// <summary>
        /// Activate the correct statistics for the current model
        /// </summary>
        public void SwitchModelStat(object sender, EventArgs e) {
            Button button = (Button)sender;
            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is ListComponents lC) {
                    if (lC.NameIdentifier == "model" + _idModel[button.IndexAssociatedPlayerScreen].ToString() + "Stats")
                        lC.Active = true;
                    else
                        lC.Active = false;
                }
            }
        }

        /// <summary>
        /// Save model choice
        /// </summary>
        public void ChangeModelPlayer(object sender, EventArgs e) {
            Button button = (Button)sender;
            int idButton = button.IndexAssociatedPlayerScreen;

            // Audio feedback
            Audio.Play("characters_popup", transform.position);

            // Change model used
            if (button.nameIdentifier == "ModelChangeRight") {
                ++_idModel[idButton];
                if (_idModel[idButton] >= ScenesCommunicationManager.Instance.ModelCharacter.Count) _idModel[idButton] = 0; //_idModel = 0;
            }
            else if (button.nameIdentifier == "ModelChangeLeft") {
                --_idModel[idButton];
                if (_idModel[idButton] < 0) _idModel[idButton] = ScenesCommunicationManager.Instance.ModelCharacter.Count - 1; //_idModel = modelCharacter.Count - 1;
            }
            else
                Debug.Log("Model was not Changed. NameIdentifier of current button not recognized!");

            // Get key name of the player
            if (panelPlay2NameOption == "play2Shared")
                NamePlayerInfosToChange = "player_" + button.IndexAssociatedPlayerScreen.ToString();

            // Save new model Choice
            if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, int, Color>(ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item1, _idModel[idButton], button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor);
            else
                ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, int, Color>(NamePlayerInfosToChange, _idModel[idButton], button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor));

            // Activate correct image in the menu
            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Image img) {
                    if (img.NameIdentifier == "pictureModel" + (_idModel[idButton] + 1).ToString())
                        img.Active = true;
                    else if (img.NameIdentifier == "pictureModel" + (_idModel[idButton] + 1).ToString() + "Color")
                        img.Active = true;
                    else
                        img.Active = false;
                }
            }
        }

        /// <summary>
        /// Save team and model color choice
        /// </summary>
        public void UpdateChosenColor(object sender, EventArgs e) {
            Button button = (Button)sender;
            int idButton = button.IndexAssociatedPlayerScreen % 2;

            // Change color used
            if (button.nameIdentifier == "ColorChangeRight") {
                ++_idColor[idButton];

                if (_idColor[idButton] >= ScenesCommunicationManager.ColorModel.Length)
                    _idColor[idButton] = 0;
                if (button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.ColorModel[_idColor[idButton]] == ScenesCommunicationManager.TeamBColor : ScenesCommunicationManager.ColorModel[_idColor[idButton]] == ScenesCommunicationManager.TeamAColor)
                    ++_idColor[idButton];
                if (_idColor[idButton] >= ScenesCommunicationManager.ColorModel.Length)
                    _idColor[idButton] = 0;
            }
            else if (button.nameIdentifier == "ColorChangeLeft") {
                --_idColor[idButton];
                if (_idColor[idButton] < 0)
                    _idColor[idButton] = ScenesCommunicationManager.ColorModel.Length - 1;
                if (button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.ColorModel[_idColor[idButton]] == ScenesCommunicationManager.TeamBColor : ScenesCommunicationManager.ColorModel[_idColor[idButton]] == ScenesCommunicationManager.TeamAColor)
                    --_idColor[idButton];
                if (_idColor[idButton] < 0) _idColor[idButton] = ScenesCommunicationManager.ColorModel.Length - 1;
            }
            else
                Debug.Log("Color was not Changed. NameIdentifier of current button not recognized!");

            // Save color
            if (button.IndexAssociatedPlayerScreen % 2 == 0)
                ScenesCommunicationManager.TeamAColor = ScenesCommunicationManager.ColorModel[_idColor[idButton]];
            else
                ScenesCommunicationManager.TeamBColor = ScenesCommunicationManager.ColorModel[_idColor[idButton]];

            // Update color for model pictures
            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Image img) {
                    for (int i = 0; i < ScenesCommunicationManager.Instance.ModelCharacter.Count; ++i) {
                        if (img.NameIdentifier == "pictureModel" + (i + 1).ToString() + "Color")
                            img.colour = ScenesCommunicationManager.ColorModel[_idColor[idButton]];
                    }
                }
                if (elem is Button bu && bu.nameIdentifier == "ColorChosen")
                    bu.ImageColor = ScenesCommunicationManager.ColorModel[_idColor[idButton]];
            }

            if (GameManager.NumPlayers == 4) {
                foreach (var elem in MenuRect[panelPlay2NameOption + ((button.IndexAssociatedPlayerScreen + 2) % 4).ToString()].components) {
                    if (elem is Image img) {
                        for (int i = 0; i < ScenesCommunicationManager.Instance.ModelCharacter.Count; ++i) {
                            if (img.NameIdentifier == "pictureModel" + (i + 1).ToString() + "Color")
                                img.colour = ScenesCommunicationManager.ColorModel[_idColor[idButton]];
                        }
                    }
                    if (elem is Button bu && bu.nameIdentifier == "ColorChosen")
                        bu.ImageColor = ScenesCommunicationManager.ColorModel[_idColor[idButton]];
                }
            }


            // Save color for the 3d model
            if (panelPlay2NameOption == "play2Shared")
                NamePlayerInfosToChange = "player_" + button.IndexAssociatedPlayerScreen.ToString();

            string[] playersToChange;
            if (GameManager.NumPlayers == 4)
                playersToChange = new string[2] { NamePlayerInfosToChange, NamePlayerInfosToChange.Substring(0, NamePlayerInfosToChange.Length - 1) + ((button.IndexAssociatedPlayerScreen + 2) % 4).ToString() };
            else
                playersToChange = new string[1] { NamePlayerInfosToChange };

            foreach (string playerName in playersToChange) {
                if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(playerName))
                    ScenesCommunicationManager.Instance.PlayersInfo[playerName] =
                        new Tuple<string, int, Color>(ScenesCommunicationManager.Instance.PlayersInfo[playerName].Item1,
                        ScenesCommunicationManager.Instance.PlayersInfo[playerName].Item2,
                        ScenesCommunicationManager.ColorModel[_idColor[idButton]]);
                else
                    ScenesCommunicationManager.Instance.PlayersInfo.Add(playerName, new Tuple<string, int, Color>(playerName, 0, ScenesCommunicationManager.ColorModel[_idColor[idButton]]));
            }
        }

        #endregion

        #region Others

        /// <summary>
        /// Activate the chosen ranking display
        /// </summary>
        public void UpdateRanking(object sender, EventArgs e) {
            Button button = (Button)sender;

            // Change chosen ranking
            if (button.nameIdentifier == "PlayersChangeRight") {
                ++_idRankingPlayerText;
                if (_idRankingPlayerText >= RankingPlayersText.Length) _idRankingPlayerText = 0;
            }
            else if (button.nameIdentifier == "PlayersChangeLeft") {
                --_idRankingPlayerText;
                if (_idRankingPlayerText < 0) _idRankingPlayerText = RankingPlayersText.Length - 1;
            }
            else if (button.nameIdentifier == "TimeChangeRight") {
                ++_idRankingDurationText;
                if (_idRankingDurationText >= RankingDurationText.Length) _idRankingDurationText = 0;
            }
            else if (button.nameIdentifier == "TimeChangeLeft") {
                --_idRankingDurationText;
                if (_idRankingDurationText < 0) _idRankingDurationText = RankingDurationText.Length - 1;
            }
            else
                Debug.Log("Color was not Changed. NameIdentifier of current button not recognized!");

            // Update text display in menu for the current ranking
            ((Button)Menu.Instance.FindMenuComponentinPanelWithName("RankingTime", _currentMenuName)).Text = RankingDurationText[_idRankingDurationText];
            ((Button)Menu.Instance.FindMenuComponentinPanelWithName("RankingPlayers", _currentMenuName)).Text = RankingPlayersText[_idRankingPlayerText];

            // Activate correct rankings list
            foreach (var elem in MenuRect[_currentMenuName].components) {
                if (elem is ListComponents listComp) {
                    if (listComp.NameIdentifier == "rankings_game") {
                        foreach (var lC in listComp.Components) {
                            if (((ListComponents)lC).NameIdentifier == "ranking" + RankingDurationText[_idRankingDurationText] + RankingPlayersText[_idRankingPlayerText])
                                lC.Active = true;
                            else
                                lC.Active = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Change highlighted (=clicked) button among the list where unique button can be clicked
        /// </summary>
        public void HighlightBorders(object sender, EventArgs e) {
            Button button = (Button)sender;
            foreach (Button bu in button.neighbors) {
                bu.IsClicked = false;
            }
            button.IsClicked = true;
        }

        /// <summary>
        /// Update current selection to neighborDown
        /// </summary>
        public void GoDown(object sender, EventArgs e) {
            Button button = (Button)sender;
            
            if (button.NeighborDown != null) {
                button.NeighborDown.IsCurrentSelection = true;
                button.IsCurrentSelection = false;
            }
        }

        /// <summary>
        /// Update current selection to neighborRight
        /// </summary>
        public void GoRight(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (button.NeighborRight != null) {
                button.NeighborRight.IsCurrentSelection = true;
                button.IsCurrentSelection = false;
            }
        }

        #endregion

        #endregion
    }
}
