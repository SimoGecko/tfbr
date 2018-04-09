using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using System.Threading.Tasks;

namespace BRS.Menu {
    class MenuManager {

        // --------------------- VARIABLES ---------------------
        public Dictionary<string, GameObject> MenuRect = new Dictionary<string, GameObject>();
        GameObject _currentMenu;
        public float transitionTime = 3f;

        public static MenuManager Instance;

        readonly Menu _menuGame = new Menu();

        int _noCharacters = 3;
        const float RotSpeed = 90;
        public readonly List<GameObject> CharacterToChoose = new List<GameObject>();

        public Dictionary<string, Tuple<string, Model>> PlayersInfo; // playerName -> userName, Model 
        public string NamePlayerInfosToChange;

        // --------------------- BASE METHODS ------------------
        public void LoadContent() {
            Instance = this;
            _menuGame.LoadContent();

            string[] namePanels = { "main", "play1", "play2", "tutorial1", "tutorial2", "tutorial3", "tutorial4", "ranking", "options", "credits", "playerInfos" };
            foreach (string name in namePanels) {
                GameObject go = new GameObject(name);
                MenuRect.Add(go.name, go);
            }

            _currentMenu = MenuRect["main"];
            Menu.Instance.BuildMenuPanels();

            PlayersInfo = new Dictionary<string, Tuple<string, Model>>();
            NamePlayerInfosToChange = "player_0";

            float[] posX = { 0, -5, 5 };
            for (int i = 0; i < _noCharacters; ++i) {
                GameObject playerCharacter = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift_tex"));
                playerCharacter.transform.position = new Vector3(posX[i], 0, 0);
                CharacterToChoose.Add(playerCharacter);
            }

            Game1.Instance.ScreenAdditionalSetup();
            GameObject camObject = GameObject.FindGameObjectWithName("camera_0");
            camObject.Start();

        }

        public void Update() {
            foreach (var go in GameObject.All)
                go.Update();

            foreach (GameObject go in CharacterToChoose)
                go.transform.Rotate(Vector3.Up, RotSpeed * Time.DeltaTime);

        }

        public void Draw() {
            foreach (var go in GameObject.All)
                if (go.active)
                    foreach (var component in go.components)
                        component.Draw();

            if (_currentMenu == MenuRect["play2"]) 
                foreach (Camera cam in Screen.Cameras) 
                    foreach (GameObject go in CharacterToChoose)
                        go.Draw(cam);
        }

        // --------------------- CUSTOM METHODS ----------------

        public void TransitionUI(object sender, EventArgs e) {
            Button button = (Button)sender;

            float time = 0;
            Image test = MenuRect["play1"].GetComponent<Image>();
            test.Position = test.Position + new Vector2(Screen.Width, 0);
            test.StartPos = test.Position;
            test.Active = true;

            Task.Run(() => {
                while (time < transitionTime) {
                    float percent = time / transitionTime;
                    test.Position = test.StartPos - percent * new Vector2(Screen.Width, 0);
                    time += Time.DeltaTime;
                }
                if (_currentMenu != null)
                    _currentMenu.active = false;
                _currentMenu = MenuRect[button.NameMenuToSwitchTo];
                _currentMenu.active = true;
            });


        }

        public void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;

            if (_currentMenu != null)
                _currentMenu.active = false;

            _currentMenu = MenuRect[button.NameMenuToSwitchTo];
            _currentMenu.active = true;
        }

        public void SetDefaultParametersGame(object sender, EventArgs e) {
            if (GameManager.NumPlayers != 2 || GameManager.NumPlayers != 4)
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
            Game1.Instance.MenuDisplay = false;
            _currentMenu.active = false;

            for (int i = 0; i < 4; ++i) {
                GameObject camObjectMenu = GameObject.FindGameObjectWithName("camera_" + i);
                GameObject.Destroy(camObjectMenu);
            }

            foreach (GameObject go in CharacterToChoose) 
                GameObject.Destroy(go);

            Game1.Instance.ScreenAdditionalSetup();
            Game1.Instance.Scene.Start();

            for (int i = 0; i < GameManager.NumPlayers; i++) {
                GameObject camObject = GameObject.FindGameObjectWithName("camera_" + i);
                camObject.Start();
            }

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

            foreach (var elem in MenuRect["playerInfos"].components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "name_player") {
                        if (button.Text == "remove") {
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
            foreach (var elem in MenuRect["play2"].components) {
                if (elem is ListComponents listComp) {
                    if (listComp.NameIdentifier == "playerInfoToChange") {
                        int count = 0;
                        foreach (var lC in listComp.Components) {
                            if (count < GameManager.NumPlayers)
                                lC.Active = true;
                            else
                                lC.Active = false;

                            if (count == GameManager.NumPlayers - 1) {
                                ((Button)lC).NeighborRight = (Button)listComp.Components[0];
                                ((Button)listComp.Components[0]).NeighborLeft = (Button)lC;
                            }
                            ++count;
                        }
                    }
                }
            }
        }

        public void ChangeNamePlayer(object sender, EventArgs e) {
            foreach (var elem in MenuRect["playerInfos"].components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "name_player") {
                        if (PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                            PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model>(textBox.Text, PlayersInfo[NamePlayerInfosToChange].Item2);
                        else
                            PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model>(textBox.Text, null));

                        textBox.Text = "";
                    }
                }
            }

        }

        public void ChangeModelPlayer(object sender, EventArgs e) {
            Model test = File.Load<Model>("Models/vehicles/forklift_tex");
            if (PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model>(PlayersInfo[NamePlayerInfosToChange].Item1, test);
            else
                PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model>(NamePlayerInfosToChange, test));
        }

        public void UpdatePlayersNameInfosToChange(object sender, EventArgs e) {
            Button button = (Button)sender;
            NamePlayerInfosToChange = "player_" + button.Index.ToString();
        }

        /*public void HighlightBordersAndGoDown(object sender, EventArgs e) {
            Button button = (Button)sender;
            button.IsCurrentSelection = false;
            if (button.NeighborDown != null) button.NeighborDown.IsCurrentSelection = true;
            button.IsClicked = true;
        }*/

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
    }
}
