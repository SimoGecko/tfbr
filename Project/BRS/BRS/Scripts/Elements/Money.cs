// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB


using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
<<<<<<< HEAD
using Microsoft.Xna.Framework;
=======
using BRS.Scripts.UI;
>>>>>>> develop

namespace BRS.Scripts.Elements {
    class Money : Pickup { // TODO rename valuable
        ////////// represents an amount of money that can be collected  //////////

        // --------------------- VARIABLES ---------------------
        //public
        public enum Type { Cash, Gold, Diamond };

        // Todo: Do we really want to have it that each money has different value? Doesn't it make it too unpredictable? (Andy)
        public const float randomizer = .0f; // how much to deviate from actual value


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
            value = (int)(Value*MyRandom.Range(1-randomizer, 1+randomizer));
        }

        public override void Update() {
            base.Update();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void DoPickup(Player p) {
            PlayerInventory pi = p.gameObject.GetComponent<PlayerInventory>();
            if (pi.CanPickUp(this)) {
                Audio.Play(type.ToString().ToLower()+ "_pickup", transform.position);
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