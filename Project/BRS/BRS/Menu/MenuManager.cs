using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BRS.Menu {
    class MenuManager {
        // each game object is a Panel (main menu, play1, play2, tuto1, tuto2, tuto3, options, credits)
        Dictionary<string, GameObject> menuRect = new Dictionary<string, GameObject>();
        GameObject currentMenu;

        public GameObject mainMenu = new GameObject("main");
        public GameObject playMenu1 = new GameObject("play1");
        public GameObject playMenu2 = new GameObject("play2");
        public GameObject[] tutoMenu = { new GameObject("tutorial1"), new GameObject("tutorial2"), new GameObject("tutorial3"), new GameObject("tutorial4") };
        public GameObject rankingMenu = new GameObject("ranking");
        public GameObject optionsMenu = new GameObject("options");
        public GameObject creditsMenu = new GameObject("credits");
        public GameObject playerInfoMenu = new GameObject("playerInfos");


        public static MenuManager instance;

        Menu menuGame = new Menu();

        int noCharacters = 3;
        const float rotSpeed = 90;
        public List<GameObject> characterToChoose = new List<GameObject>();

        public Dictionary<string, Tuple<string, Model>> playersInfo; // playerName -> userName, Model 
        public string namePlayerInfosToChange;

        public void LoadContent() {
            instance = this;

            playersInfo = new Dictionary<string, Tuple<string, Model>>();
            namePlayerInfosToChange = "player_0";

            menuGame.LoadContent();

            float[] posX = {0, -5, 5}; 
            for (int i=0; i<noCharacters; ++i) {
                GameObject playerCharacter = new GameObject("player_" + i.ToString(), File.Load<Model>("Models/vehicles/forklift_tex"));
                playerCharacter.transform.position = new Vector3(posX[i], 0, 0);
                characterToChoose.Add(playerCharacter);
            }

            Game1.instance.ScreenAdditionalSetup();
            GameObject camObject = GameObject.FindGameObjectWithName("camera_0");
            camObject.Start();
            

            //// MAIN MENU ////
            Menu.instance.BuildMainMenu();
            currentMenu = mainMenu;

            Menu.instance.BuildPlayMenu();
            Menu.instance.BuildTutorialMenu();
            Menu.instance.BuildRankingMenu();
            Menu.instance.BuildOptionsMenu();
            Menu.instance.BuildCreditsMenu();
            Menu.instance.BuildPlayerInfoMenu();
            

            //// Create Menu's dictionary
            menuRect.Add(mainMenu.name, mainMenu);
            menuRect.Add(playMenu1.name, playMenu1);
            menuRect.Add(playMenu2.name, playMenu2);

            foreach (GameObject go in tutoMenu)
                menuRect.Add(go.name, go);

            menuRect.Add(optionsMenu.name, optionsMenu);
            menuRect.Add(creditsMenu.name, creditsMenu);
            menuRect.Add(rankingMenu.name, rankingMenu);
            menuRect.Add(playerInfoMenu.name, playerInfoMenu);
        }

        public void HighlightBorders(object sender, EventArgs e) {
            Button button = (Button)sender;
            foreach (Button bu in button.neighbors) {
                bu.isClicked = false;
            }
            button.isClicked = true;
        }

        public void UpdateRoundDuration(object sender, EventArgs e) {
            Button button = (Button)sender;
            RoundManager.roundTime = Int32.Parse(button.Text[0].ToString()) * 60;
        }

        public void UpdateNoPlayers(object sender, EventArgs e) {
            Button button = (Button)sender;
            GameManager.numPlayers = Int32.Parse(button.Text);
        }

        //public void CreatePlayers(object sender, EventArgs e) {
        //    Game1.instance.scene.CreatePlayers();
        //}

        public void UpdateTemporaryNamePlayer(object sender, EventArgs e) {
            Button button = (Button)sender;

            foreach (var elem in playerInfoMenu.components) {
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
            Button button = (Button)sender;

            foreach (var elem in playMenu2.components) {
                if (elem is ListComponents listComp) {
                    if (listComp.nameIdentifier == "playerInfoToChange") {
                        int count = 0;
                        foreach (var lC in listComp.listComponents) {
                            if (count < GameManager.numPlayers)
                                lC.active = true;
                            else
                                lC.active = false;
                            ++count;
                        }
                    }
                }
            }
        }

        public void SetDefaultParametersGame(object sender, EventArgs e) {
            if (GameManager.numPlayers == 1)
                GameManager.numPlayers = 2;
        }

        public void ChangeNamePlayer(object sender, EventArgs e) {
            foreach (var elem in playerInfoMenu.components) {
                if (elem is TextBox textBox) {
                    if (textBox.NameIdentifier == "name_player") {
                        if (playersInfo.ContainsKey(namePlayerInfosToChange))
                            playersInfo[namePlayerInfosToChange] = new Tuple<string, Model>(textBox.Text, playersInfo[namePlayerInfosToChange].Item2);
                        else
                            playersInfo.Add(namePlayerInfosToChange, new Tuple<string, Model>(textBox.Text, null));

                        textBox.Text = "";
                    }
                }
            }

        }

        public void ChangeModelPlayer(object sender, EventArgs e) {
            Model test = File.Load<Model>("Models/vehicles/forklift_tex");
            if (playersInfo.ContainsKey(namePlayerInfosToChange))
                playersInfo[namePlayerInfosToChange] = new Tuple<string, Model>(playersInfo[namePlayerInfosToChange].Item1, test);
            else
                playersInfo.Add(namePlayerInfosToChange, new Tuple<string, Model>(namePlayerInfosToChange, test));
        }

        public void UpdatePlayersNameInfosToChange(object sender, EventArgs e) {
            Button button = (Button)sender;
            namePlayerInfosToChange = "player_" + button.index.ToString();
        }
        public void StartGameFunction(object sender, EventArgs e) {
            Game1.instance.menuDisplay = false;
            currentMenu.active = false;

            GameObject camObjectMenu = GameObject.FindGameObjectWithName("camera_0");
            GameObject.Destroy(camObjectMenu);
            foreach (GameObject go in characterToChoose)
                GameObject.Destroy(go); 


            Game1.instance.ScreenAdditionalSetup();
            Game1.instance.scene.Start();


            for (int i = 0; i < GameManager.numPlayers; i++) {
                GameObject camObject = GameObject.FindGameObjectWithName("camera_" + i);
                camObject.Start();
            }

        }

        public void SwitchRankingDisplay(object sender, EventArgs e) {
            Button button = (Button)sender;

            foreach (var elem in rankingMenu.components) {
                if (elem is ListComponents listComp) {
                    if (listComp.nameIdentifier == "rankings_game") {
                        foreach (var lC in listComp.listComponents)
                            lC.active = false;
                        listComp.listComponents[button.index].active = true;
                    }
                }
            }
        }

        public void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;
 
            //Transform goalTransform = camTransf[menu];
            if (currentMenu != null)
                currentMenu.active = false;

            currentMenu = menuRect[button.NameMenuToSwitchTo];
            currentMenu.active = true;
        }

        public void Update() {
            foreach (var go in GameObject.All)
                go.Update();

            foreach (GameObject go in characterToChoose) 
                go.transform.Rotate(Vector3.Up, rotSpeed * Time.deltaTime);
            
        }

        public void Draw() {
            foreach (var go in GameObject.All)
                if (go.active)
                    foreach (var component in go.components)
                        component.Draw();

            if (currentMenu == playMenu2) {
                foreach (Camera cam in Screen.cameras) {
                    foreach (GameObject go in characterToChoose)
                        go.Draw(cam);
                }
            }

        }

    }
}
