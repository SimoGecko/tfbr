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

namespace BRS.Scripts.Managers {
    class MenuManager : Component{
        // --------------------- VARIABLES ---------------------
        public static bool uniqueFrameInputUsed = false;

        public Dictionary<string, GameObject> MenuRect = new Dictionary<string, GameObject>();
        GameObject _currentMenu;
        string _currentMenuName;
        public float transitionTime = 3f;

        public static MenuManager Instance;

        readonly Menu _menuGame = new Menu();

        public List<Model> modelCharacter;

        public string NamePlayerInfosToChange;

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
            //MenuRect["pause"].active = false;
        }

        public void LoadContent() {
            Instance = this;
            _menuGame.LoadContent();
            GameManager.state = GameManager.State.Menu;

            string[] namePanels = { "main", "play1", "play2", "tutorial1", "tutorial2", "tutorial3", "tutorial4", "ranking", "options", "credits"};
            foreach (string name in namePanels) {
                GameObject go = new GameObject(name);
                MenuRect.Add(go.name, go);
            }

            _currentMenu = MenuRect["main"];
            _currentMenuName = "main";
            Menu.Instance.BuildMenuPanels();

            NamePlayerInfosToChange = "player_0";

            modelCharacter = new List<Model>();
            modelCharacter.Add(File.Load<Model>("Models/vehicles/forklift"));
            modelCharacter.Add(File.Load<Model>("Models/vehicles/sweeper"));
            modelCharacter.Add(File.Load<Model>("Models/vehicles/bulldozer"));

        }

        public override void Update() {
            uniqueFrameInputUsed = false;

            if (Input.GetKeyUp(Keys.B) || Input.GetButtonUp(Buttons.B)) {
                Button bu = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Back", _currentMenuName);
                if (bu != default(Button)) bu.Click?.Invoke(bu, new EventArgs());
            }
        }

        public void Draw() {
            
        }

        // --------------------- CUSTOM METHODS ----------------

        /*public async void TransitionUI(object sender, EventArgs e) {
            Button button = (Button)sender;

            Image test = MenuRect["play1"].GetComponent<Image>();
            test.StartPos = test.Position;
            test.Position = test.Position + new Vector2(Screen.Width, 0);
            test.Active = true;

            float time = 0;
            //Task.Run(() => {
                while (time < transitionTime) {
                    float percent = time / transitionTime;
                    //test.Position = test.StartPos - percent * new Vector2(Screen.Width, 0);
                    test.Position = Vector2.Lerp(test.Position, test.StartPos, percent);
                    time += Time.DeltaTime - 0.001f;
                    await Time.WaitForSeconds(0.001f);
                }
                if (_currentMenu != null)
                    _currentMenu.active = false;
                _currentMenu = MenuRect[button.NameMenuToSwitchTo];
                _currentMenu.active = true;
            //});
        }*/

        public void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (_currentMenu != null) 
                _currentMenu.active = false;
            

            _currentMenu = MenuRect[button.NameMenuToSwitchTo];
            _currentMenuName = button.NameMenuToSwitchTo;
            _currentMenu.active = true;
        }

        public void SetDefaultParametersGame(object sender, EventArgs e) {
            if (GameManager.NumPlayers != 2 && GameManager.NumPlayers != 4)
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

        public void LoadMenu(object sender, EventArgs e) {
            ScenesCommunicationManager.loadOnlyPauseMenu = false;
            SceneManager.LoadMenu = true;
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

            foreach (var elem in MenuRect["play2"].components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "NamePlayer") {
                        if (button.Text == "del") {
                            if (textBox.Text.Length > 0)
                                textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                        }
                        else
                            textBox.Text += button.Text;
                    }
                }
            }
        }

        public void UpdatePlayersChangeTo(object sender, EventArgs e) {
            if (GameManager.NumPlayers == 2) {
                Menu.Instance.FindMenuComponentinPanelWithName("Player3", "play2").Active = false;
                Menu.Instance.FindMenuComponentinPanelWithName("Player4", "play2").Active = false;

                Button bu1 = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Player1", "play2");
                Button bu2 = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Player2", "play2");
                bu1.NeighborLeft = bu2;
                bu2.NeighborRight = bu1;
            }
            else if (GameManager.NumPlayers == 4) {
                Button bu3 = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Player3", "play2");
                bu3.Active = true;
                Button bu4 = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Player4", "play2");
                bu4.Active = true;

                Button bu1 = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Player1", "play2");
                Button bu2 = (Button)Menu.Instance.FindMenuComponentinPanelWithName("Player2", "play2");
                bu1.NeighborLeft = bu4;
                bu2.NeighborRight = bu3;
            }
        }

        public void ChangeNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            foreach (var elem in MenuRect["play2"].components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "NamePlayer") {
                        if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                            ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model>(textBox.Text, ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item2);
                        else
                            ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model>(textBox.Text, null));

                        textBox.Text = "";
                    }
                }
            }

        }

        public void ChangeModelPlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model>(ScenesCommunicationManager.Instance.PlayersInfo[NamePlayerInfosToChange].Item1, modelCharacter[button.Index]);
            else
                ScenesCommunicationManager.Instance.PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model>(NamePlayerInfosToChange, modelCharacter[button.Index]));

            foreach (var elem in MenuRect["play2"].components) {
                if (elem is Image img) {
                    if (img.NameIdentifier == "pictureModel" + (button.Index+1).ToString()) 
                        img.Active = true;
                    else
                        img.Active = false;
                }
            }
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

        public void ChangeModelNamePlayer(GameObject player, int i) {

            if (ScenesCommunicationManager.Instance != null) {
                if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey("player_" + i)) {
                    string userName = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item1;
                    Model userModel = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item2;

                    if (userName != null) player.GetComponent<Player>().PlayerName = userName;
                    if (userModel != null) player.Model = userModel;
                }
            }
        }
    }
}
