using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BRS.Load;
using BRS.Engine.Physics;
using BRS.Scripts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Menu {
    class MenuManager {
        // each game object is a Panel (main menu, play1, play2, tuto1, tuto2, tuto3, options, credits)
        Dictionary<string, GameObject> menuRect = new Dictionary<string, GameObject>();
        GameObject currentMenu;

        GameObject mainMenu = new GameObject("main");
        GameObject playMenu = new GameObject("play");
        GameObject optionsMenu = new GameObject("options");
        GameObject creditsMenu = new GameObject("credits");


        private List<Component> test;

        public static MenuManager instance;

        public void LoadContent() {
            instance = this;

            var testButton = new Button(File.Load<Texture2D>("Images/UI/button")) {
                Position = new Vector2(200, 200),
                Text = "testButton",
            };
            testButton.Click += TestButtonFunction;

            var testQuitButton = new Button(File.Load<Texture2D>("Images/UI/button")) {
                Position = new Vector2(200, 300),
                Text = "testQuitButton",
            };
            testQuitButton.Click += TestQuitFunction;

            var testTickBox = new TickBox(File.Load<Texture2D>("Images/UI/tickbox_notclicked"), File.Load<Texture2D>("Images/UI/tickbox_clicked")) {
                Position = new Vector2(200, 400),
            };

            Texture2D textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            var testSlider = new Slider() {
                Position = new Vector2(200, 500),
                
                buttonSlider = new Button(textureButtonSlider) {
                    Position = new Vector2(200 - textureButtonSlider.Width/2, 500 - (textureButtonSlider.Height - UserInterface.BARHEIGHT) /2),
                }
            };

            test = new List<Component>() {
                testButton,
                testQuitButton,
                testTickBox,
                testSlider,
            };

            var playButton = new Button(File.Load<Texture2D>("Images/UI/button")) {
                Position = new Vector2(200, 200),
                Text = "go to play menu",
                NameMenuToSwitchTo = "play",
            };
            playButton.Click += SwitchToMenu;

            var optionsButton = new Button(File.Load<Texture2D>("Images/UI/button")) {
                Position = new Vector2(200, 300),
                Text = "go to options menu",
                NameMenuToSwitchTo = "options",
            };
            optionsButton.Click += SwitchToMenu;

            var creditsButton = new Button(File.Load<Texture2D>("Images/UI/button")) {
                Position = new Vector2(200, 400),
                Text = "go to credits menu",
                NameMenuToSwitchTo = "credits",
            };
            creditsButton.Click += SwitchToMenu;

            var backButton = new Button(File.Load<Texture2D>("Images/UI/button")) {
                Position = new Vector2(200, 700),
                Text = "go back",
                NameMenuToSwitchTo = "main",
            };
            backButton.Click += SwitchToMenu;


            mainMenu.AddComponent(playButton);
            mainMenu.AddComponent(optionsButton);
            mainMenu.AddComponent(creditsButton);

            currentMenu = mainMenu;
            
            playMenu.AddComponent(testButton); playMenu.AddComponent(backButton); playMenu.active = false;
            optionsMenu.AddComponent(testSlider); optionsMenu.AddComponent(backButton); optionsMenu.active = false;
            creditsMenu.AddComponent(testTickBox); creditsMenu.AddComponent(backButton); creditsMenu.active = false;

            menuRect.Add(mainMenu.name, mainMenu);
            menuRect.Add(playMenu.name, playMenu);
            menuRect.Add(optionsMenu.name, optionsMenu);
            menuRect.Add(creditsMenu.name, creditsMenu);
        }

        private void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;
 
            //Transform goalTransform = camTransf[menu];
            if (currentMenu != null)
                currentMenu.active = false;

            currentMenu = menuRect[button.NameMenuToSwitchTo];
            currentMenu.active = true;
        }


            private void TestButtonFunction(object sender, EventArgs e) {
            Game1.instance.menuDisplay = false;
            currentMenu.active = false;
        }

        public void Update() {
            /*foreach (var component in test)
                component.Update();*/

            foreach (var go in GameObject.All)
                go.Update();
        }

        public void Draw() {
            //spriteBatch.Begin();
            /*foreach (var component in test)
                component.Draw(spriteBatch);*/

            foreach (var go in GameObject.All) 
                if (go.active)
                    foreach (var component in go.components)
                        component.Draw();
            

            //spriteBatch.End();
        }

        private void TestQuitFunction(object sender, EventArgs e) {
            Game1.instance.Exit();
        }


    }
}
