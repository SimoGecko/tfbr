// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts.UI {
    class TutorialUI : Component {
        ////////// shows tutorial for the first time playing //////////

        // --------------------- VARIABLES ---------------------

        //public
        delegate bool Predicate();
        const float timeBetweenTut = 3f;

        //private
        bool showTutorial = false;

        private int _index;
        Tut[] tutorial;
        int tutIndex; // in the tutorial
        float nextTutTime;
        bool isDisplaying;

        //to display
        string displayString;
        XboxButtons displayButton;

        Rectangle buttonDest;

        //reference
        Player player; // every instance has its own index


        // --------------------- BASE METHODS ------------------
        public TutorialUI(int idx) { _index = idx; }

        public override void Start() {
            SetupTutorial();

            player = ElementManager.Instance.Player(_index);

            tutIndex = 0;
            nextTutTime = Time.CurrentTime + timeBetweenTut + 4f ; // initial countdown
            isDisplaying = false;

            displayString = "";
            displayButton = XboxButtons.Null;
        }

        public override void Update() {
            if (!showTutorial) return;

            if (tutIndex < tutorial.Length) {
                if (!isDisplaying) {
                    if(Time.CurrentTime > nextTutTime)
                        CheckForTutorialStart();
                } else {
                    CheckForTutorialEnd();
                }
            } else {
                showTutorial = false; // don't display anymore
            }

            if (GameManager.GameActive) {
                if (isDisplaying && displayButton != XboxButtons.Null) {
                    ButtonsUI.Instance.GiveCommand(_index, buttonDest, displayButton, Align.TopLeft);
                }
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SetupTutorial() {
            tutorial = new Tut[] {
                new Tut(XboxButtons.Null, "Collect cash\nand gold", True, Has3Cash),
                new Tut(XboxButtons.R, "to rotate\nthe camera", True, UsedR),
                new Tut(XboxButtons.RT, "for turbo", True, UsedBoost),
                new Tut(XboxButtons.A, "to attack\nenemies and crates", True, UsedAttack),
                new Tut(XboxButtons.X, "to use powerup", HasPowerup, UsedPowerup),
                new Tut(XboxButtons.Null, "Bring the cash\nto your base!", IsFull, IsEmpty),
                new Tut(XboxButtons.B, "to drop cash", Has1Cash, UsedDrop),
            };
        }

        void CheckForTutorialStart() {
            if (tutorial[tutIndex].initiation()) {
                isDisplaying = true;
                //show
                displayString = tutorial[tutIndex].textToDisplay;
                displayButton = tutorial[tutIndex].button;
            }
        }

        void CheckForTutorialEnd() {
            if (tutorial[tutIndex].termination()) {
                isDisplaying = false;
                tutIndex++;
                nextTutTime = Time.CurrentTime + timeBetweenTut;
                //hide (not needed)
                displayString = "";
                displayButton = XboxButtons.Null;
            }
        }

        public override void Draw2D(int i) {
            if (i != _index || !GameManager.GameActive) return;
            if (isDisplaying) {
                Vector2 screenPosition = Camera.GetCamera(_index).WorldToScreenPoint(player.transform.position);

                buttonDest = new Rectangle(screenPosition.ToPoint() + new Point(70, -40), new Point(40, 40));

                //draw string
                Rectangle notificationRect = new Rectangle(screenPosition.ToPoint() + new Point(50, -20), new Point(150, 40));
                UserInterface.DrawString(displayString, notificationRect, Align.TopLeft, Align.TopLeft, Align.Left, scale: .7f);
            }
        }


        // queries

        bool True() { return true; }

        bool Has1Cash() { return player.pI.CarryingWeight>=1; }
        bool Has3Cash() { return player.pI.CarryingWeight>=3; }
        bool IsEmpty()  { return player.pI.CarryingWeight==0; }
        bool IsFull()   { return player.pI.IsAlmostFull(); }
        bool HasPowerup() { return player.pP.HasPowerup; }

        //pressed
        bool UsedR() { return player.CamController.InputIsGreaterThanDeadZone(); } // camera input

        bool UsedAttack() { return player.AttackInput(); }
        bool UsedDrop()   { return player.DropCashInput(); }
        bool UsedPowerup(){ return player.PowerupInput(); }
        bool UsedBoost()  { return player.BoostInput(); }



        // other
        struct Tut {
            public XboxButtons button;
            public string textToDisplay;
            public Predicate initiation;
            public Predicate termination;
            public Tut(XboxButtons b, string s, Predicate i, Predicate t) { button = b; textToDisplay = s; initiation = i; termination = t; }
        }

    }
}