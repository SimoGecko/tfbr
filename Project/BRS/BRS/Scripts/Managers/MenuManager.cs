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

namespace BRS.Scripts.Managers {
    class MenuManager : Component{
        public static bool uniqueFrameInputUsed = false;
        // --------------------- VARIABLES ---------------------
        public Dictionary<string, GameObject> MenuRect = new Dictionary<string, GameObject>();
        GameObject _currentMenu;
        public float transitionTime = 3f;

        public static MenuManager Instance;

        readonly Menu _menuGame = new Menu();

        int _noCharacters = 3;
        public List<Model> modelCharacter;
        //const float RotSpeed = 90;
        //public readonly List<GameObject> CharacterToChoose = new List<GameObject>();

        public Dictionary<string, Tuple<string, Model>> PlayersInfo; // playerName -> userName, Model 
        public string NamePlayerInfosToChange;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            LoadContent();
        }

        public void LoadContent() {
            Instance = this;
            _menuGame.LoadContent();

            string[] namePanels = { "main", "play1", "play2", "tutorial1", "tutorial2", "tutorial3", "tutorial4", "ranking", "options", "credits"};
            foreach (string name in namePanels) {
                GameObject go = new GameObject(name);
                MenuRect.Add(go.name, go);
            }

            _currentMenu = MenuRect["main"];
            Menu.Instance.BuildMenuPanels();

            PlayersInfo = new Dictionary<string, Tuple<string, Model>>();
            NamePlayerInfosToChange = "player_0";

            /*float[] posX = { 0, -5, -5, -5 };
            for (int i = 0; i < _noCharacters; ++i) {
                GameObject playerCharacter = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift_tex"));
                playerCharacter.transform.position = new Vector3(posX[i], 0, 0);
                
                if (i == 1)
                    playerCharacter.active = true;
                else
                    playerCharacter.active = false;

                CharacterToChoose.Add(playerCharacter);
            }

            //Game1.Instance.ScreenAdditionalSetup(); // don't do this
            GameObject camObject = GameObject.FindGameObjectWithName("camera_0");
            camObject.Start();*/

            modelCharacter = new List<Model>();
            modelCharacter.Add(File.Load<Model>("Models/vehicles/forklift_tex"));
            modelCharacter.Add(File.Load<Model>("Models/vehicles/sweeper"));
            modelCharacter.Add(File.Load<Model>("Models/vehicles/forklift_tex"));

        }

        public override void Update() {
            uniqueFrameInputUsed = false;
            /*foreach (var go in GameObject.All)
                go.Update();*/

            /*foreach (GameObject go in CharacterToChoose)
                go.transform.Rotate(Vector3.Up, RotSpeed * Time.DeltaTime);*/

            //foreach(GameObject)
        }

        public void Draw() {
            /*foreach (var go in GameObject.All)
                if (go.active)
                    foreach (Component component in go.components)
                        if (component.Active)
                            component.Draw(0);*/

            /*if (_currentMenu == MenuRect["play2"]) 
                foreach (Camera cam in Screen.Cameras) 
                    foreach (GameObject go in CharacterToChoose)
                        go.Draw(cam);*/
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
            GameManager.state = GameManager.State.Playing;
            //SceneManager.LoadScene("Level1");

            /*Game1.Instance.MenuDisplay = false;
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
            }*/

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
                Menu.Instance.FindButtonPanelWithName("Player3", "play2").Active = false;
                Menu.Instance.FindButtonPanelWithName("Player4", "play2").Active = false;

                Button bu1 = Menu.Instance.FindButtonPanelWithName("Player1", "play2");
                Button bu2 = Menu.Instance.FindButtonPanelWithName("Player2", "play2");
                bu1.NeighborLeft = bu2;
                bu2.NeighborRight = bu1;
            }
            else if (GameManager.NumPlayers == 4) {
                Button bu3 = Menu.Instance.FindButtonPanelWithName("Player3", "play2");
                bu3.Active = true;
                Button bu4 = Menu.Instance.FindButtonPanelWithName("Player4", "play2");
                bu4.Active = true;

                Button bu1 = Menu.Instance.FindButtonPanelWithName("Player1", "play2");
                Button bu2 = Menu.Instance.FindButtonPanelWithName("Player2", "play2");
                bu1.NeighborLeft = bu4;
                bu2.NeighborRight = bu3;
            }





            /*foreach (var elem in MenuRect["play2"].components) {
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
                            if (count == 1 && GameManager.NumPlayers == 4) {
                                ((Button)lC).NeighborRight = (Button)listComp.Components[2];
                                ((Button)listComp.Components[2]).NeighborLeft = (Button)lC;
                            }
                            ++count;
                        }
                    }
                }
            }*/
        }

        public void ChangeNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;
            foreach (var elem in MenuRect["play2"].components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "name_player") {
                        if (PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                            PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model>(textBox.Text, PlayersInfo[NamePlayerInfosToChange].Item2);
                        else
                            PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model>(textBox.Text, null));
                        
                        //GameObject.FindGameObjectWithName(NamePlayerInfosToChange).GetComponent<Player>().PlayerName = textBox.Text;
                        //int playerIndex = Int32.Parse(NamePlayerInfosToChange[NamePlayerInfosToChange.Length - 1].ToString());
                        //ElementManager.Instance.Player(playerIndex).PlayerName = textBox.Text;
                        textBox.Text = "";
                    }
                }
            }

        }

        public void ChangeModelPlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            /*Model toChange = File.Load<Model>("Models/vehicles/forklift_tex");
            foreach (var elem in CharacterToChoose)
                elem.active = false;
            NamePlayerInfosToChange = "player_" + button.Index.ToString();

            toChange = CharacterToChoose[button.Index].Model;
            CharacterToChoose[button.Index+1].active = true;
            */

            //int playerIndex = Int32.Parse(NamePlayerInfosToChange[NamePlayerInfosToChange.Length - 1].ToString());
            if (PlayersInfo.ContainsKey(NamePlayerInfosToChange))
                PlayersInfo[NamePlayerInfosToChange] = new Tuple<string, Model>(PlayersInfo[NamePlayerInfosToChange].Item1, modelCharacter[button.Index]);
            else
                PlayersInfo.Add(NamePlayerInfosToChange, new Tuple<string, Model>(NamePlayerInfosToChange, modelCharacter[button.Index]));
            

            //GameObject.FindGameObjectWithName(NamePlayerInfosToChange).Model = modelCharacter[button.Index];
            
            /*foreach (KeyValuePair<string, Scene> entry in SceneManager.scenes) {
                entry.Value.
                ElementManager.Instance.Player(playerIndex).gameObject.Model = modelCharacter[button.Index];
            }*/

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

        public void ChangeModelNamePlayer(GameObject player, int i) {

            if (MenuManager.Instance.PlayersInfo.ContainsKey("player_" + i)) {
                string userName = MenuManager.Instance.PlayersInfo["player_" + i].Item1;
                Model userModel = MenuManager.Instance.PlayersInfo["player_" + i].Item2;

                if (userName != null) player.GetComponent<Player>().PlayerName = userName;
                if (userModel != null) player.Model = userModel;
            }
        }
    }
}
