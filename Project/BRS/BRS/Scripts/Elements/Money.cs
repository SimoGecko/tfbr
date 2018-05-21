// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB


using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;

namespace BRS.Scripts.Elements {
    class Money : Pickup { // TODO rename valuable
        ////////// represents an amount of money that can be collected  //////////

        // --------------------- VARIABLES ---------------------
        //public
        public enum Type { Cash, Gold, Diamond };



        //private
        int value = 1;
        int weight = 1;
        public Type type;

        //reference


        // --------------------- BASE METHODS ------------------
        public Money(int _value, int _weight, Type _type) {
            value = _value; weight = _weight;
            type = _type;
        }

        public override void Start() {
            base.Start();
        }

        public override void Update() {
            base.Update();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public override void DoPickup(Player p) {
            PlayerInventory pi = p.gameObject.GetComponent<PlayerInventory>();
            if (pi.CanPickUp(this)) {
                Audio.Play("pickup_" + type.ToString().ToLower(), transform.position);
                pi.Collect(this);
                ElementManager.Instance.Remove(this);
                MoneyUI.Instance.PickedupValuable(transform.position, value, p.PlayerIndex);
                GameObject.Destroy(gameObject);
            }
        }


        // queries
        public int Value  { get { return value; } }
        public int Weight { get { return weight; } }


        // other

    }

}