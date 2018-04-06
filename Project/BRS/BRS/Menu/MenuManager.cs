using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BRS.Menu {
    class MenuManager {
        // each game object is a Panel (main menu, play1, play2, tuto1, tuto2, tuto3, options, credits)
        readonly Dictionary<string, GameObject> _menuRect = new Dictionary<string, GameObject>();
        GameObject _currentMenu;

        public readonly GameObject MainMenu = new GameObject("main");
        public readonly GameObject PlayMenu1 = new GameObject("play1");
        public readonly GameObject PlayMenu2 = new GameObject("play2");
        public readonly GameObject[] TutorialMenu = { new GameObject("tutorial1"), new GameObject("tutorial2"), new GameObject("tutorial3"), new GameObject("tutorial4") };
        public readonly GameObject RankingMenu = new GameObject("ranking");
        public readonly GameObject OptionsMenu = new GameObject("options");
        public readonly GameObject CreditsMenu = new GameObject("credits");
        public readonly GameObject PlayerInfoMenu = new GameObject("playerInfos");


        public static MenuManager Instance;

        readonly Menu _menuGame = new Menu();

        int _noCharacters = 3;
        const float RotSpeed = 90;
        public readonly List<GameObject> CharacterToChoose = new List<GameObject>();

        public Dictionary<string, Tuple<string, Model>> PlayersInfo; // playerName -> userName, Model 
        public string NamePlayerInfosToChange;

        public void LoadContent() {
            Instance = this;

            PlayersInfo = new Dictionary<string, Tuple<string, Model>>();
            NamePlayerInfosToChange = "player_0";

            _menuGame.LoadContent();

            float[] posX = { 0, -5, 5 };
            for (int i = 0; i < _noCharacters; ++i) {
                GameObject playerCharacter = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift_tex"));
                playerCharacter.transform.position = new Vector3(posX[i], 0, 0);
                CharacterToChoose.Add(playerCharacter);
            }

            Game1.Instance.ScreenAdditionalSetup();
            GameObject camObject = GameObject.FindGameObjectWithName("camera_0");
            camObject.Start();


            //// MAIN MENU ////
            Menu.Instance.BuildMainMenu();
            _currentMenu = MainMenu;

            Menu.Instance.BuildPlayMenu();
            Menu.Instance.BuildTutorialMenu();
            Menu.Instance.BuildRankingMenu();
            Menu.Instance.BuildOptionsMenu();
            Menu.Instance.BuildCreditsMenu();
            Menu.Instance.BuildPlayerInfoMenu();


            //// Create Menu's dictionary
            _menuRect.Add(MainMenu.name, MainMenu);
            _menuRect.Add(PlayMenu1.name, PlayMenu1);
            _menuRect.Add(PlayMenu2.name, PlayMenu2);

            foreach (GameObject go in TutorialMenu)
                _menuRect.Add(go.name, go);

            _menuRect.Add(OptionsMenu.name, OptionsMenu);
            _menuRect.Add(CreditsMenu.name, CreditsMenu);
            _menuRect.Add(RankingMenu.name, RankingMenu);
            _menuRect.Add(PlayerInfoMenu.name, PlayerInfoMenu);
        }

        public void HighlightBorders(object sender, EventArgs e) {
            Button button = (Button)sender;
            foreach (Button bu in button.Neighbors) {
                bu.IsClicked = false;
            }
            button.IsClicked = true;
        }

        public void UpdateRoundDuration(object sender, EventArgs e) {
            Button button = (Button)sender;
            RoundManager.RoundTime = Int32.Parse(button.Text[0].ToString()) * 60;
        }

        public void UpdateNoPlayers(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameManager.NumPlayers = Int32.Parse(button.Text);
        }

        //public void CreatePlayers(object sender, EventArgs e) {
        //    Game1.instance.scene.CreatePlayers();
        //}

        public void UpdateTemporaryNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            foreach (var elem in PlayerInfoMenu.components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "name_player") {
                        if (button.Text == "remove") {
                            if (textBox.Text.Length > 0)
                                textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                        } else
                            textBox.Text += button.Text;
                    }
                }
            }
        }

        public void UpdatePlayersChangeTo(object sender, EventArgs e) {
            //Button button = (Button)sender;

            foreach (var elem in PlayMenu2.components) {
                if (elem is ListComponents listComp) {
                    if (listComp.NameIdentifier == "playerInfoToChange") {
                        int count = 0;
                        foreach (var lC in listComp.Components) {
                            if (count < GameManager.NumPlayers)
                                lC.Active = true;
                            else
                                lC.Active = false;
                            ++count;
                        }
                    }
                }
            }
        }

        public void SetDefaultParametersGame(object sender, EventArgs e) {
            if (GameManager.NumPlayers == 1)
                GameManager.NumPlayers = 2;
        }

        public void ChangeNamePlayer(object sender, EventArgs e) {
            foreach (var elem in PlayerInfoMenu.components) {
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
        public void StartGameFunction(object sender, EventArgs e) {
            Game1.Instance.MenuDisplay = false;
            _currentMenu.active = false;

            for (int i = 0; i < 4; ++i) {
                GameObject camObjectMenu = GameObject.FindGameObjectWithName("camera_" + i);
                GameObject.Destroy(camObjectMenu);
            }

            foreach (GameObject go in CharacterToChoose) {
                GameObject.Destroy(go);
            }

            Game1.Instance.ScreenAdditionalSetup();
            Game1.Instance.Scene.Start();


            for (int i = 0; i < GameManager.NumPlayers; i++) {
                GameObject camObject = GameObject.FindGameObjectWithName("camera_" + i);
                camObject.Start();

                List<GameObject> cams = GameObject.FindGameObjectsWithName("camera_" + i);
                Debug.Log("Not unique?!: " + "'camera_" + i + "' = " + cams.Count);
            }

        }

        public void SwitchRankingDisplay(object sender, EventArgs e) {
            Button button = (Button)sender;

            foreach (var elem in RankingMenu.components) {
                if (elem is ListComponents listComp) {
                    if (listComp.NameIdentifier == "rankings_game") {
                        foreach (var lC in listComp.Components)
                            lC.Active = false;
                        listComp.Components[button.Index].Active = true;
                    }
                }
            }
        }

        public void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;

            //Transform goalTransform = camTransf[menu];
            if (_currentMenu != null)
                _currentMenu.active = false;

            _currentMenu = _menuRect[button.NameMenuToSwitchTo];
            _currentMenu.active = true;
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

            if (_currentMenu == PlayMenu2) {
                foreach (Camera cam in Screen.Cameras) {
                    foreach (GameObject go in CharacterToChoose)
                        go.Draw(cam);
                }
            }

        }

    }
}
