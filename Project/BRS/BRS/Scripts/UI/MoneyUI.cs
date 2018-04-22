// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Utilities;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Scripts.UI {
    class MoneyUI : Component {
        ////////// draws UI when picking up cash //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float duration = 1f;
        const float displacement = 2f;


        //private
        List<DrawValuableOrder> orderList = new List<DrawValuableOrder>();

        //reference
        public static MoneyUI Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------

        public override void Draw(int index) {
            if (index == 0) return;
            index--;

            for (int i=0; i<orderList.Count; i++) {
                DrawValuableOrder dvo = orderList[i];
                if (dvo.index == index) {
                    dvo.percent += Time.DeltaTime * duration;
                    if (dvo.percent > 1) orderList.RemoveAt(i--);
                    Vector2 screenPos = Camera.GetCamera(index).WorldToScreenPoint(dvo.position + Vector3.Up * dvo.percent * displacement);
                    UserInterface.DrawString("+" + Utility.IntToMoneyString(dvo.value), screenPos, scale:.7f);//, col:Color.Lerp(Color.White, Color.White, dvo.percent));
                }
            }
        }

        public void PickedupValuable(Vector3 position, int value, int index) {
            orderList.Add(new DrawValuableOrder(position, value, index));
        }


        // commands
        

        // queries



        // other
        class DrawValuableOrder {
            public Vector3 position;
            public int value;
            public int index;
            public float percent;
            public DrawValuableOrder(Vector3 p, int v, int i) {
                position = p;
                value = v;
                index = i;
                percent = 0;
            }
        }

    }
}