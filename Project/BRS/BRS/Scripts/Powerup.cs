// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class Powerup : Pickup {
        ////////// base class for all powerups in the game //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float rotSpeed = 90;


        //private
        protected string powerupName;
        protected bool destroyOnUse = true;
        protected bool rotate = true;

        //reference
        protected Player owner;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            base.Start();
            destroyOnUse = rotate = true;
            transform.rotation = MyRandom.YRotation();
        }

        public override void Update() {
            base.Update();

            if(rotate)
                transform.Rotate(Vector3.Up, rotSpeed * Time.deltatime);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        protected override void DoPickup(Player p) {
            PlayerPowerup pp = p.gameObject.GetComponent<PlayerPowerup>();
            if (pp.CanPickUp(this)) {
                pp.Collect(this);
                owner = p;

                Elements.instance.Remove(this);
                UserInterface.instance.UpdatePlayerPowerupUI(p.PlayerIndex, powerupName, true);

                if(!destroyOnUse) gameObject.Active = false;
                else GameObject.Destroy(gameObject);
            }
            
        }

        public virtual void UsePowerup() {
            UserInterface.instance.UpdatePlayerPowerupUI(owner.PlayerIndex, powerupName, false);
        }

        // queries
        public virtual bool CanUse() {
            //fill 
            return true;
        }


        // other

    }
    
}