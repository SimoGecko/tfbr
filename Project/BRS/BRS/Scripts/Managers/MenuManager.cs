﻿// (c) Nicolas Huart 2018
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

        public List<Model> modelCharacter;
        int idModel = 0;

        public string NamePlayerInfosToChange;
        public string panelPlay2NameOption = "play2Shared";

        Color[] colorModel = { new Color(215,173,35), Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Violet };
        int idColor = 0;
        Color defaultColorModel = new Color(215, 173, 35);

        bool moveCam = false;
        float time = 0;
        public float transitionTime = 0.5f;
        //Transform goalTransform;
        string _changeToMenu;

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

            string[] namePanels = { "main", "play1", "tutorial1", "tutorial2", "tutorial3", "ranking", "options", "credits", "play2Shared0", "play2Shared1", "play2Shared2", "play2Shared3" };
            Vector3[] posCam = { new Vector3(0,15,12), new Vector3(0,20,15), new Vector3(0,10,0), new Vector3(0,10,-10), new Vector3(0,10,-10), new Vector3(-15,10,0), new Vector3(15,10,0), new Vector3(0,10,10), new Vector3(0,25,20), new Vector3(0,25,20), new Vector3(0,25,20), new Vector3(0,25,20) };
            Vector3[] rotCam = { new Vector3(-37,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-35,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0), new Vector3(-40,0,0) };

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

            modelCharacter = new List<Model> {
                File.Load<Model>("Models/vehicles/forklift"),
                File.Load<Model>("Models/vehicles/sweeper"),
                File.Load<Model>("Models/vehicles/bulldozer")
            };

        }

        public override void Update() {
            uniqueMenuSwitchUsed = false;
            for (int i = 0; i < uniqueFrameInputUsed.Length; ++i)
                uniqueFrameInputUsed[i] = false;

            if (Input.GetKeyUp(Keys.B) || Input.GetButtonUp(Buttons.B)) {
                Button bu = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Back", _currentMenuName);
                if (bu != default(Button)) bu.Click?.Invoke(bu, new EventArgs());
            }


            if (moveCam) {
                string newMenuName = _changeToMenu;
                if (_changeToMenu == "play2Shared")
                    newMenuName = _changeToMenu + "0";
                Transform goalTransform = MenuCam[newMenuName];
                

                if (/*goalTransform.position != _currentMenuCam.position && goalTransform.eulerAngles != _currentMenuCam.eulerAngles &&*/  time < transitionTime) {
                    float percent = time / transitionTime; // TODO: SmoothDamp

                    foreach (Camera c in Screen.Cameras) {
                        c.transform.position = Vector3.Lerp(_currentMenuCam.position, goalTransform.position, percent);
                        //c.transform.rotation = Quaternion.Lerp(_currentMenuCam.rotation, goalTransform.rotation, percent);
                        c.transform.eulerAngles = Vector3.Lerp(_currentMenuCam.eulerAngles, goalTransform.eulerAngles, percent);
                    }
                    time += Time.DeltaTime;
                }
                else {
                    time = 0;
                    moveCam = false;

                    foreach (Camera c in Screen.Cameras) {
                        c.transform.position = goalTransform.position;
                        //c.transform.rotation = goalTransform.rotation;
                        c.transform.eulerAngles = goalTransform.eulerAngles;
                    }

                    _currentMenuCam = MenuCam[newMenuName];
                    if (_changeToMenu != "play2Shared")
                        _currentMenu.active = true;
                    else {
                        MenuRect[_changeToMenu + "0"].active = true;
                        if (GameManager.NumPlayers == 2 || GameManager.NumPlayers == 4)
                            MenuRect[_changeToMenu + "1"].active = true;
                    }

                }
            }

        }

        public override void Draw2D(int i) {
            
        }


        // --------------------- CUSTOM METHODS ----------------

        public void TransitionUI(object sender, EventArgs e) {
            Button button = (Button)sender;
            //goalTransform = MenuCam[button.NameMenuToSwitchTo]; 
                   /*Transform goalTransform = MenuCam[button.NameMenuToSwitchTo];         

            float time = 0;
            Task.Run(() => {
                while (time < transitionTime) {
                    float percent = time / transitionTime; // TODO: SmoothDamp

                    foreach (Camera c in Screen.Cameras) {
                        c.transform.position = Vector3.Lerp(_currentMenuCam.position, goalTransform.position, percent);
                        //c.transform.rotation = Quaternion.Lerp(_currentMenuCam.rotation, goalTransform.rotation, percent);
                        c.transform.eulerAngles = Vector3.Lerp(_currentMenuCam.eulerAngles, goalTransform.eulerAngles, percent);
                    }

                    time += Time.DeltaTime;
                    //await;
                }
                //Task.fr
            });*/
            //moveCam = true;
        }

        public void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;

            _changeToMenu = button.NameMenuToSwitchTo;
            moveCam = true;

            if (!uniqueMenuSwitchUsed) {
                if (_currentMenu != null)
                    _currentMenu.active = false;
                if (_currentMenuName == "play2Shared0") {
                    MenuRect["play2Shared0"].active = false;
                    MenuRect["play2Shared1"].active = false;
                    MenuRect["play2Shared2"].active = false;
                    MenuRect["play2Shared3"].active = false;
                }

                if (button.NameMenuToSwitchTo != "play2Shared") {
                    _currentMenu = MenuRect[button.NameMenuToSwitchTo];
                    _currentMenuName = button.NameMenuToSwitchTo;
                    //_currentMenu.active = true;

                    //_changeToMenu = button.NameMenuToSwitchTo;
                }
                else {
                    _currentMenuName = button.NameMenuToSwitchTo + "0";
                    _currentMenu = MenuRect[_currentMenuName];

                    //_changeToMenu = _currentMenuName;

                    /*MenuRect[button.NameMenuToSwitchTo + "0"].active = true;
                    if (GameManager.NumPlayers == 2 || GameManager.NumPlayers == 4)
                        MenuRect[button.NameMenuToSwitchTo + "1"].active = true;*/
                }
                uniqueMenuSwitchUsed = true;
            }
        }

        public void SetDefaultParametersGame(object sender, EventArgs e) {
            if (GameManager.NumPlayers != 1 && GameManager.NumPlayers != 2 && GameManager.NumPlayers != 4)
                GameManager.NumPlayers = 2;
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
            else if (GameManager.NumPlayers == 2 && Menu.Instance.FindMenuComponentinPanelWithName("Ready", panelPlay2NameOption + ((button.indexAssociatedPlayerScreen + 1) % 2).ToString()).IsCurrentSelection) {
                allPlayersReady = true;
            }
            else if (GameManager.NumPlayers == 4) {
                if (button.indexAssociatedPlayerScreen == 0 || button.indexAssociatedPlayerScreen == 1) {
                    if (Menu.Instance.FindMenuComponentinPanelWithName("Ready", panelPlay2NameOption + ((button.indexAssociatedPlayerScreen + 1) % 2).ToString()).IsCurrentSelection) {
                        MenuRect[panelPlay2NameOption + "0"].active = false;
                        MenuRect[panelPlay2NameOption + "1"].active = false;
                        MenuRect[panelPlay2NameOption + "2"].active = true;
                        MenuRect[panelPlay2NameOption + "3"].active = true;
                        uniqueMenuSwitchUsed = true;
                    }
                }
                else if (button.indexAssociatedPlayerScreen == 2 || button.indexAssociatedPlayerScreen == 3) {
                    if (Menu.Instance.FindMenuComponentinPanelWithName("Ready", panelPlay2NameOption + ((button.indexAssociatedPlayerScreen + 1) % 2 + 2).ToString()).IsCurrentSelection) {
                        allPlayersReady = true;
                    }
                }
            }

            if (allPlayersReady) {
                for (int i = 0; i < GameManager.NumPlayers; ++i) {
                    PostProcessingManager.Instance._effects[2].Activate(i, false);
                    PostProcessingManager.Instance._effects[3].Activate(i, false);
                }

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
        }

        public void SetMap(object sender, EventArgs e) {
            Button button = (Button)sender;
        }

        public void ResumeGame(object sender, EventArgs e) {
            MenuRect["pause"].active = false;
            GameManager.state = GameManager.State.Playing;
        }

        public void SwitchRankingDisplay(object sender, EventArgs e) {
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
        }

        public void UpdateTemporaryNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            foreach (var elem in MenuRect[panelPlay2NameOption + button.indexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Button bu) {
                    if (bu.nameIdentifier == "NamePlayer") {
                        if (button.Text == "del") {
                            if (bu.Text.Length > 0)
                                bu.Text = bu.Text.Substring(0, bu.Text.Length - 1);
                        }
                        else
                            bu.Text += button.Text;
                    }
                }
            }
        }

        public void ChangeNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (panelPlay2NameOption == "play2Shared")
                NamePlayerInfosToChange = "player_" + button.indexAssociatedPlayerScreen.ToString();

            foreach (var elem in MenuRect[panelPlay2NameOption + button.indexAssociatedPlayerScreen.ToString()].components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "NamePlayer") {
                        if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                            ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model, Color>(textBox.Text, ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item2, defaultColorModel);
                        else
                            ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model, Color>(textBox.Text, null, defaultColorModel));

                        textBox.Text = "";
                    }
                }
            }

        }

        public void ChangeModelPlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            // Change color used
            if (button.nameIdentifier == "ModelChangeRight") {
                ++idModel;
                if (idModel >= modelCharacter.Count) idModel = modelCharacter.Count - 1; //idModel = 0;
            }
            else if (button.nameIdentifier == "ModelChangeLeft") {
                --idModel;
                if (idModel < 0) idModel = 0; //idModel = modelCharacter.Count - 1;
            }
            else
                Debug.Log("Model was not Changed. NameIdentifier of current button not recognized!");

            if (panelPlay2NameOption == "play2Shared")
                NamePlayerInfosToChange = "player_" + button.indexAssociatedPlayerScreen.ToString();

            if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model, Color>(ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item1, modelCharacter[idModel], defaultColorModel);
            else
                ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model, Color>(NamePlayerInfosToChange, modelCharacter[idModel], defaultColorModel));

            foreach (var elem in MenuRect[panelPlay2NameOption + button.indexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Image img) {
                    if (img.NameIdentifier == "pictureModel" + (idModel + 1).ToString())
                        img.Active = true;
                    else if (img.NameIdentifier == "pictureModel" + (idModel + 1).ToString() + "Color")
                        img.Active = true;
                    else
                        img.Active = false;
                }
            }
        }

        public void UpdateChosenColor(object sender, EventArgs e) {
            Button button = (Button)sender;
                
            // Change color used
            if (button.nameIdentifier == "ColorChangeRight") {
                ++idColor;
                if (idColor >= colorModel.Length) idColor = 0;
            }
            else if (button.nameIdentifier == "ColorChangeLeft") {
                --idColor;
                if (idColor < 0) idColor = colorModel.Length - 1;
            }
            else
                Debug.Log("Color was not Changed. NameIdentifier of current button not recognized!");

            // update color for model pictures
            foreach (var elem in MenuRect[panelPlay2NameOption + button.indexAssociatedPlayerScreen.ToString()].components) {
                if (elem is Image img) {
                    for (int i = 0; i < modelCharacter.Count; ++i) {
                        if (img.NameIdentifier == "pictureModel" + (i + 1).ToString() + "Color")
                            img.colour = colorModel[idColor];
                    }
                }
                if (elem is Button bu && bu.nameIdentifier == "ColorChosen")
                    bu.ImageColor = colorModel[idColor];
            }

            // update color for 3d model
            Color test = colorModel[idColor];
            if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = 
                    new Tuple<string, Model, Color>(ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item1, 
                    ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item2, 
                    colorModel[idColor]);
            else
                ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model, Color>(NamePlayerInfosToChange, null, colorModel[idColor]));
        }
        

        public void UpdatePlayersNameInfosToChange(object sender, EventArgs e) {
            Button button = (Button)sender;
            NamePlayerInfosToChange = "player_" + button.Index.ToString();
        }

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
            //Audio.SetMusicVolume(.01f);
            Audio.SetSoundVolume(slider.percentPosButon);
        }

        public void ChangeModelNameColorPlayer(GameObject player, int i) {
            if (ScenesCommunicationManager.Instance != null) {
                if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey("player_" + i)) {
                    string userName = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item1;
                    Model userModel = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item2;
                    Color colorPlayer = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item3;

                    if (userName != null) player.GetComponent<Player>().PlayerName = userName;
                    if (userModel != null) player.Model = userModel;
                    if (colorPlayer != null) {
                        Rectangle areaChange = new Rectangle(4, 8, 4, 4);
                        SetRectanglePixelColor(areaChange, colorPlayer, ScenesCommunicationManager.Instance.textureColorPlayers["player_" + i]);
                    }
                    
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
