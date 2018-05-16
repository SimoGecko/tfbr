// (c) Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using System.Threading.Tasks;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using BRS.Engine.Menu;
using Microsoft.Xna.Framework.Input;
using BRS.Engine.PostProcessing;

namespace BRS.Scripts.Managers {
    class MenuManager : Component{
        // --------------------- VARIABLES ---------------------
        public static bool[] uniqueFrameInputUsed = { false, false, false, false };
        private bool uniqueMenuSwitchUsed = false;

        public Dictionary<string, GameObject> MenuRect = new Dictionary<string, GameObject>();
        GameObject _currentMenu;
        string _currentMenuName;
        readonly Menu _menuGame = new Menu();

        public Dictionary<string, Transform> MenuCam = new Dictionary<string, Transform>();
        Transform _currentMenuCam;

        public static MenuManager Instance;

        int _idModel = 0;

        public string NamePlayerInfosToChange;
        public string panelPlay2NameOption = "play2Shared";

        int _idColor = 0;

        public string[] RankingPlayersText = { "1P", "2P", "4P"};
        int _idRankingPlayerText = 0;
        public string[] RankingDurationText = { "2 min", "3 min", "5 min", "10 min" };
        int _idRankingDurationText = 0;

        bool _moveCam = false;
        float _time = 0;
        public float TransitionTime = 0.5f;
        public float DistTransition = 13;
        public float CurrentDistTransition;
        Vector3 _velocityPos = Vector3.Zero;
        Vector3 _velocityRot = Vector3.Zero;
        string _changeToMenu;

        bool[] NamePlayersChanged = { false, false, false, false };

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();

            if (ScenesCommunicationManager.loadOnlyPauseMenu)
                LoadPauseMenu();
            else
                LoadContent();
        }

        public void LoadPauseMenu() {
            Instance = this;
            _menuGame.LoadContent();

            string[] namePanels = { "pause" };
            foreach (string name in namePanels) {
                GameObject go = new GameObject(name);
                MenuRect.Add(go.name, go);
            }

            _currentMenu = MenuRect["pause"];
            _currentMenuName = "pause";
            Menu.Instance.BuildPausePanel();
        }

        public void LoadContent() {
            Instance = this;
            _menuGame.LoadContent();
            GameManager.state = GameManager.State.Menu;

            string[] namePanels = { "main", "credits", "tutorial1", "tutorial2", "tutorial3", "ranking", "options", "play1", "play2Shared0", "play2Shared1", "play2Shared2", "play2Shared3" };
            Vector3[] posCam = { new Vector3(0,15,12), new Vector3(20,12,10), new Vector3(0,10,0), new Vector3(9,2,-32), new Vector3(0,10,-25), new Vector3(-22,7,-15), new Vector3(22,7,-15), new Vector3(-7,3,0), new Vector3(2,3,0), new Vector3(0,25,20), new Vector3(0,25,20), new Vector3(0,25,20) };
            Vector3[] rotCam = { new Vector3(-37,0,0), new Vector3(-35,45,0), new Vector3(-30,0,0), new Vector3(-15,70,0), new Vector3(-40,0,0), new Vector3(-20,-40,0), new Vector3(-20,40,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0) };

            for (int i = 0; i < namePanels.Length; ++i) {
                GameObject go = new GameObject(namePanels[i]);
                MenuRect.Add(namePanels[i], go);

                Transform tr = new Transform(posCam[i], rotCam[i]);
                MenuCam.Add(namePanels[i], tr);
            }

            _currentMenu = MenuRect["main"];
            _currentMenuName = "main";

            _currentMenuCam = MenuCam["main"];
            foreach (Camera c in Screen.Cameras) {
                c.transform.position = _currentMenuCam.position;
                c.transform.eulerAngles = _currentMenuCam.eulerAngles;
            }

            Menu.Instance.BuildMenuPanels(panelPlay2NameOption);

            NamePlayerInfosToChange = "player_0";

        }

        public override void Update() {
            uniqueMenuSwitchUsed = false;
            for (int i = 0; i < uniqueFrameInputUsed.Length; ++i)
                uniqueFrameInputUsed[i] = false;

            if (Input.GetKeyUp(Keys.B) || Input.GetButtonUp(Buttons.B)) {
                if (_currentMenuName == "play2Shared2") {
                    MenuRect["play2Shared0"].active = true;
                    MenuRect["play2Shared1"].active = true;
                    MenuRect["play2Shared2"].active = false;
                    MenuRect["play2Shared3"].active = false;
                    _currentMenuName = "play2Shared0";
                }
                else {
                    Button bu = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Back", _currentMenuName);
                    if (bu != default(Button)) bu.Click?.Invoke(bu, new EventArgs());
                }
                
            }


            //if (_moveCam) 
            if (_changeToMenu != null)
                TransitionCam();               
            

        }

        public override void Draw2D(int i) {
            
        }


        // --------------------- CUSTOM METHODS ----------------

        public void TransitionCam() {
            string newMenuName = _changeToMenu;
            if (_changeToMenu == "play2Shared")
                newMenuName = _changeToMenu + "0";
            Transform goalTransform = MenuCam[newMenuName];

            CurrentDistTransition = (Screen.Cameras[0].transform.position - goalTransform.position).Length();
            float newTransitionTime = TransitionTime * CurrentDistTransition / DistTransition;

            foreach (Camera c in Screen.Cameras) {
                c.transform.position = Utility.SmoothDamp(c.transform.position, goalTransform.position, ref _velocityPos, newTransitionTime);
                c.transform.eulerAngles = Utility.SmoothDampAngle(c.transform.eulerAngles, goalTransform.eulerAngles, ref _velocityRot, newTransitionTime);
            }
            

            if (_time < newTransitionTime*2) {
                _time += Time.DeltaTime;
            }
            else {
                _time = 0;
                _moveCam = false;

                if (_changeToMenu != "play2Shared") {
                    _currentMenu = MenuRect[_changeToMenu];
                    _currentMenuName = _changeToMenu;
                }
                else {
                    _currentMenuName = _changeToMenu + "0";
                    _currentMenu = MenuRect[_currentMenuName];
                }

                //_currentMenuCam = MenuCam[newMenuName];
                if (_changeToMenu != "play2Shared")
                    _currentMenu.active = true;
                else {
                    MenuRect[_changeToMenu + "0"].active = true;
                    if (GameManager.NumPlayers == 2 || GameManager.NumPlayers == 4)
                        MenuRect[_changeToMenu + "1"].active = true;
                }


            }
        }

        public void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;

            _changeToMenu = button.NameMenuToSwitchTo;
            _moveCam = true;

            if (!uniqueMenuSwitchUsed) {
                if (_currentMenu != null)
                    _currentMenu.active = false;
                if (_currentMenuName == "play2Shared0") {
                    MenuRect["play2Shared0"].active = false;
                    MenuRect["play2Shared1"].active = false;
                    MenuRect["play2Shared2"].active = false;
                    MenuRect["play2Shared3"].active = false;
                }

                
                uniqueMenuSwitchUsed = true;
            }
        }

        public void SetDefaultParametersGame(object sender, EventArgs e) {
            if (GameManager.NumPlayers != 1 && GameManager.NumPlayers != 2 && GameManager.NumPlayers != 4)
                GameManager.NumPlayers = 2;
            if (RoundManager.RoundTime != 2 * 60 && RoundManager.RoundTime != 3 * 60 && RoundManager.RoundTime != 5 * 60 && RoundManager.RoundTime != 10 * 60)
                RoundManager.RoundTime = 2*60;
        }

        public void UpdateRoundDuration(object sender, EventArgs e) {
            Button button = (Button)sender;
            RoundManager.RoundTime = Int32.Parse(button.Text[0].ToString()) * 60;
        }

        public void UpdateNoPlayers(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameManager.NumPlayers = Int32.Parse(button.Text);
        }

        public void StartGameFunction(object sender, EventArgs e) {
            ScenesCommunicationManager.loadOnlyPauseMenu = true;
            GameManager.state = GameManager.State.Playing;
            SceneManager.LoadGame = true;
        }

        public void StartGamePlayersReady(object sender, EventArgs e) {
            Button button = (Button)sender;
            bool allPlayersReady = false;

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
                        _currentMenu = MenuRect[_currentMenuName];

                        uniqueMenuSwitchUsed = true;
                    }
                }
                else if (button.IndexAssociatedPlayerScreen == 2 || button.IndexAssociatedPlayerScreen == 3) {
                    if (((Button)Menu.Instance.FindMenuComponentinPanelWithName("Ready", panelPlay2NameOption + ((button.IndexAssociatedPlayerScreen + 1) % 2 + 2).ToString())).IsClicked) {
                        allPlayersReady = true;
                    }
                }
            }

            if (allPlayersReady) {
                ScenesCommunicationManager.loadOnlyPauseMenu = true;
                GameManager.state = GameManager.State.Playing;
                SceneManager.LoadGame = true;
            }
        }

        public void LoadMenuFunction(object sender, EventArgs e) {
            ScenesCommunicationManager.loadOnlyPauseMenu = false;
            SceneManager.LoadMenu = true;
        }

        public void SetMode(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameMode.currentGameMode = ScenesCommunicationManager.ModesName[button.Index];
        }

        public void SetMap(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameManager.LvlScene = button.Index;
        }

        public void ResumeGame(object sender, EventArgs e) {
            MenuRect["pause"].active = false;
            GameManager.state = GameManager.State.Playing;
        }

        /*public void SwitchRankingDisplay(object sender, EventArgs e) {
            Button button = (Button)sender;

            foreach (var elem in MenuRect["ranking"].components) {
                if (elem is ListComponents listComp) {
                    if (listComp.NameIdentifier == "rankings_game") {
                        foreach (var lC in listComp.Components)
                            lC.Active = false;
                        listComp.Components[button.Index].Active = true;
                    }
                }
            }
        }*/

        public void UpdateTemporaryNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (!uniqueMenuSwitchUsed) {
                foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                    if (elem is Button bu) {
                        if (bu.nameIdentifier == "NamePlayer") {
                            if (!NamePlayersChanged[button.IndexAssociatedPlayerScreen]) {
                                bu.Text = "";
                                NamePlayersChanged[button.IndexAssociatedPlayerScreen] = true;
                            }

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

        public void ChangeNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (panelPlay2NameOption == "play2Shared")
                NamePlayerInfosToChange = "player_" + button.IndexAssociatedPlayerScreen.ToString();

            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Button bu) {
                    if (bu.nameIdentifier == "NamePlayer") {
                        if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                            ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, int, Color>(bu.Text, ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item2, button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor);
                        else
                            ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, int, Color>(bu.Text, 0, button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor));

                        //bu.Text = "";
                    }
                }
            }

        }

        public void SwitchModelStat(object sender, EventArgs e) {
            Button button = (Button)sender;
            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is ListComponents lC) {
                    if (lC.NameIdentifier == "model" + _idModel.ToString() + "Stats")
                        lC.Active = true;
                    else
                        lC.Active = false;
                }
            }
        }

        public void ChangeModelPlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            // Change color used
            if (button.nameIdentifier == "ModelChangeRight") {
                ++_idModel;
                if (_idModel >= ScenesCommunicationManager.Instance.ModelCharacter.Count) _idModel = 0; //_idModel = 0;
            }
            else if (button.nameIdentifier == "ModelChangeLeft") {
                --_idModel;
                if (_idModel < 0) _idModel = ScenesCommunicationManager.Instance.ModelCharacter.Count - 1; //_idModel = modelCharacter.Count - 1;
            }
            else
                Debug.Log("Model was not Changed. NameIdentifier of current button not recognized!");

            if (panelPlay2NameOption == "play2Shared")
                NamePlayerInfosToChange = "player_" + button.IndexAssociatedPlayerScreen.ToString();

            if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, int, Color>(ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item1, _idModel, button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor);
            else
                ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, int, Color>(NamePlayerInfosToChange, _idModel, button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.TeamAColor : ScenesCommunicationManager.TeamBColor));

            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Image img) {
                    if (img.NameIdentifier == "pictureModel" + (_idModel + 1).ToString())
                        img.Active = true;
                    else if (img.NameIdentifier == "pictureModel" + (_idModel + 1).ToString() + "Color")
                        img.Active = true;
                    else
                        img.Active = false;
                }
            }
        }

        public void UpdateRanking(object sender, EventArgs e) {
            Button button = (Button)sender;

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

            ((Button)Menu.Instance.FindMenuComponentinPanelWithName("RankingTime", _currentMenuName)).Text = RankingDurationText[_idRankingDurationText];
            ((Button)Menu.Instance.FindMenuComponentinPanelWithName("RankingPlayers", _currentMenuName)).Text = RankingPlayersText[_idRankingPlayerText];

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

        public void UpdateChosenColor(object sender, EventArgs e) {
            Button button = (Button)sender;
                
            // Change color used
            if (button.nameIdentifier == "ColorChangeRight") {
                ++_idColor;
                if (_idColor >= ScenesCommunicationManager.ColorModel.Length)
                    _idColor = 0;
                if (button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.ColorModel[_idColor] == ScenesCommunicationManager.TeamBColor : ScenesCommunicationManager.ColorModel[_idColor] == ScenesCommunicationManager.TeamAColor)
                    ++_idColor;

                if (_idColor >= ScenesCommunicationManager.ColorModel.Length)
                    _idColor = 0;
                    
            }
            else if (button.nameIdentifier == "ColorChangeLeft") {
                --_idColor;
                if (_idColor < 0) _idColor = ScenesCommunicationManager.ColorModel.Length - 1;
                if (button.IndexAssociatedPlayerScreen % 2 == 0 ? ScenesCommunicationManager.ColorModel[_idColor] == ScenesCommunicationManager.TeamBColor : ScenesCommunicationManager.ColorModel[_idColor] == ScenesCommunicationManager.TeamAColor)
                    --_idColor;

                if (_idColor < 0) _idColor = ScenesCommunicationManager.ColorModel.Length - 1;
            }
            else
                Debug.Log("Color was not Changed. NameIdentifier of current button not recognized!");

            if (button.IndexAssociatedPlayerScreen % 2 == 0)
                ScenesCommunicationManager.TeamAColor = ScenesCommunicationManager.ColorModel[_idColor];
            else
                ScenesCommunicationManager.TeamBColor = ScenesCommunicationManager.ColorModel[_idColor];

            // update color for model pictures
            foreach (var elem in MenuRect[panelPlay2NameOption + button.IndexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Image img) {
                    for (int i = 0; i < ScenesCommunicationManager.Instance.ModelCharacter.Count; ++i) {
                        if (img.NameIdentifier == "pictureModel" + (i + 1).ToString() + "Color")
                            img.colour = ScenesCommunicationManager.ColorModel[_idColor];
                    }
                }
                if (elem is Button bu && bu.nameIdentifier == "ColorChosen")
                    bu.ImageColor = ScenesCommunicationManager.ColorModel[_idColor];
            }
            if (GameManager.NumPlayers == 4) {
                foreach (var elem in MenuRect[panelPlay2NameOption + ((button.IndexAssociatedPlayerScreen+2) % 4).ToString()].components) {
                    if (elem is Image img) {
                        for (int i = 0; i < ScenesCommunicationManager.Instance.ModelCharacter.Count; ++i) {
                            if (img.NameIdentifier == "pictureModel" + (i + 1).ToString() + "Color")
                                img.colour = ScenesCommunicationManager.ColorModel[_idColor];
                        }
                    }
                    if (elem is Button bu && bu.nameIdentifier == "ColorChosen")
                        bu.ImageColor = ScenesCommunicationManager.ColorModel[_idColor];
                }
            }


            // update color for 3d model
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
                        ScenesCommunicationManager.ColorModel[_idColor]);
                else
                    ScenesCommunicationManager.Instance.PlayersInfo.Add(playerName, new Tuple<string, int, Color>(playerName, 0, ScenesCommunicationManager.ColorModel[_idColor]));
            }
        }
        
        /*public void UpdatePlayersNameInfosToChange(object sender, EventArgs e) {
            Button button = (Button)sender;
            NamePlayerInfosToChange = "player_" + button.Index.ToString();
        }*/

        public void HighlightBorders(object sender, EventArgs e) {
            Button button = (Button)sender;
            foreach (Button bu in button.neighbors) {
                bu.IsClicked = false;
            }
            button.IsClicked = true;
        }

        public void GoDown(object sender, EventArgs e) {
            Button button = (Button)sender;
            
            if (button.NeighborDown != null) {
                button.NeighborDown.IsCurrentSelection = true;
                button.IsCurrentSelection = false;
            }
        }

        public void GoRight(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (button.NeighborRight != null) {
                button.NeighborRight.IsCurrentSelection = true;
                button.IsCurrentSelection = false;
            }
        }

        public void UpdateVolume(object sender, EventArgs e) {
            Slider slider = (Slider)sender;
            Audio.SetSoundVolume(slider.percentPosButon);
        }

        public void SetMusic(object sender, EventArgs e) {
            TickBox tickbox = (TickBox)sender;
            if (tickbox.IsClicked)
                Audio.SetMusicVolume(.005f);
            else
                Audio.SetMusicVolume(0);
        }

        public void SetCamera(object sender, EventArgs e) {
            TickBox tickbox = (TickBox)sender;
            if (tickbox.IsClicked)
                CameraController.autoFollow = true;
            else
                CameraController.autoFollow = false;
        }

        public void SetLevelDiffculty(object sender, EventArgs e) {
            Button bu = (Button)sender;
            if (bu.Text == "Easy") GameManager.lvlDifficulty = 0;
            else if (bu.Text == "Normal") GameManager.lvlDifficulty = 1;
            else if (bu.Text == "Hard") GameManager.lvlDifficulty = 2;
            else Debug.Log("Wrong level difficulty");
        }

        public void ChangeModelNameColorPlayer(GameObject player, int i) {
            if (ScenesCommunicationManager.Instance != null) {
                if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey("player_" + i)) {
                    string userName = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item1;
                    int currIdModel = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item2;
                    Model userModel = ScenesCommunicationManager.Instance.ModelCharacter[currIdModel];
                    Color colorPlayer = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item3;

                    if (userName != null) player.GetComponent<Player>().PlayerName = userName;
                    if (userModel != null) player.Model = userModel;
                    if (colorPlayer != null) {
                        Rectangle areaChange = new Rectangle(4, 8, 4, 4);
                        SetRectanglePixelColor(areaChange, colorPlayer, ScenesCommunicationManager.Instance.textureColorPlayers["player_" + i]);
                    }

                    player.GetComponent<PlayerInventory>().SetCapacity(ScenesCommunicationManager.ValuesStats[currIdModel].Capacity);
                    player.GetComponent<PlayerAttack>().AttackDistance = ScenesCommunicationManager.ValuesStats[currIdModel].AttackDistance;
                    player.GetComponent<PlayerMovement>().SetMaxSpeed(ScenesCommunicationManager.ValuesStats[currIdModel].MaxSpeed);
                    player.GetComponent<PlayerMovement>().SetMinSpeed(ScenesCommunicationManager.ValuesStats[currIdModel].MinSpeed);
                }
            }
        }

        public void SetPixelColor(int x, int y, Color color, Texture2D texture) {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colorData);
            colorData[x + y * texture.Width] = color;
            texture.SetData<Color>(colorData);
        }

        public void SetRectanglePixelColor(Rectangle rec, Color color, Texture2D texture) {
            for (int x = rec.X; x < rec.X + rec.Width; ++x)
                for (int y = rec.Y; y < rec.Y + rec.Height; ++y)
                    SetPixelColor(x, y, color, texture);
        }
    }
}
