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


        public void LoadContent() {
            var testButton = new Button(File.Load<Texture2D>("Images/UI/button"), File.Load<SpriteFont>("Other/font/font1")) {
                Position = new Vector2(200, 200),
                Text = "testButton",
                index = 1,
            };
            testButton.Click += TestButtonFunction;

            var testQuitButton = new Button(File.Load<Texture2D>("Images/UI/button"), File.Load<SpriteFont>("Other/font/font1")) {
                Position = new Vector2(200, 300),
                Text = "testQuitButton",
            };
            testQuitButton.Click += TestQuitFunction;

            var testTickBox = new TickBox(File.Load<Texture2D>("Images/UI/tickbox_notclicked"), File.Load<Texture2D>("Images/UI/tickbox_clicked"), File.Load<SpriteFont>("Other/font/font1")) {
                Position = new Vector2(200, 400),
            };

            Texture2D textureButtonSlider = File.Load<Texture2D>("Images/UI/sliderButton");
            var testSlider = new Slider(File.Load<Texture2D>("Images/UI/progress_bar")) {
                Position = new Vector2(200, 500),
                
                buttonSlider = new Button(textureButtonSlider, File.Load<SpriteFont>("Other/font/font1")) {
                    Position = new Vector2(200 - textureButtonSlider.Width/2, 500 - (textureButtonSlider.Height - Slider.BARHEIGHT)/2),
                }
            };

            test = new List<Component>() {
                testButton,
                testQuitButton,
                testTickBox,
                testSlider,
            };

            var playButton = new Button(File.Load<Texture2D>("Images/UI/button"), File.Load<SpriteFont>("Other/font/font1")) {
                Position = new Vector2(200, 200),
                Text = "go to play menu",
                NameMenuToSwitchTo = "play",
                index = 0,
            };
            playButton.Click += SwitchToMenu;

            var optionsButton = new Button(File.Load<Texture2D>("Images/UI/button"), File.Load<SpriteFont>("Other/font/font1")) {
                Position = new Vector2(200, 300),
                Text = "go to options menu",
                NameMenuToSwitchTo = "options",
                index = 1,
            };
            optionsButton.Click += SwitchToMenu;

            var creditsButton = new Button(File.Load<Texture2D>("Images/UI/button"), File.Load<SpriteFont>("Other/font/font1")) {
                Position = new Vector2(200, 400),
                Text = "go to credits menu",
                NameMenuToSwitchTo = "credits",
                index = 2,
            };
            creditsButton.Click += SwitchToMenu;

            var backButton = new Button(File.Load<Texture2D>("Images/UI/button"), File.Load<SpriteFont>("Other/font/font1")) {
                Position = new Vector2(200, 700),
                Text = "go back",
                NameMenuToSwitchTo = "main",
                index = 2,
            };
            backButton.Click += SwitchToMenu;


            mainMenu.AddComponent(playButton);
            mainMenu.AddComponent(optionsButton);
            mainMenu.AddComponent(creditsButton);

            currentMenu = mainMenu;
            
            playMenu.AddComponent(testButton); playMenu.AddComponent(backButton); playMenu.Active = false;
            optionsMenu.AddComponent(testSlider); optionsMenu.AddComponent(backButton); optionsMenu.Active = false;
            creditsMenu.AddComponent(testTickBox); creditsMenu.AddComponent(backButton); creditsMenu.Active = false;

            menuRect.Add(mainMenu.Name, mainMenu);
            menuRect.Add(playMenu.Name, playMenu);
            menuRect.Add(optionsMenu.Name, optionsMenu);
            menuRect.Add(creditsMenu.Name, creditsMenu);
        }

        private void SwitchToMenu(object sender, EventArgs e) {
            Button button = (Button)sender;
 
            //Transform goalTransform = camTransf[menu];
            if (currentMenu != null)
                currentMenu.Active = false;

            currentMenu = menuRect[button.NameMenuToSwitchTo];
            currentMenu.Active = true;
        }

        private void SwitchToMenu(string menuName) {

        }

            private void TestButtonFunction(object sender, EventArgs e) {
            Game1.instance.menuDisplay = false;
        }

        public void Update() {
            /*foreach (var component in test)
                component.Update();*/

            foreach (var go in GameObject.All)
                go.Update();
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();
            /*foreach (var component in test)
                component.Draw(spriteBatch);*/

            foreach (var go in GameObject.All) 
                if (go.Active)
                    foreach (var component in go.components)
                        component.Draw(spriteBatch);
            

            spriteBatch.End();
        }

        private void TestQuitFunction(object sender, EventArgs e) {
            Game1.instance.Exit();
        }


    }
}
