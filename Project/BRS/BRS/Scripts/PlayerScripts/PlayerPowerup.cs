﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.PowerUps;
using BRS.Scripts.UI;
using BRS.Scripts.Managers;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// deals with storing powerups and using them
    /// </summary>
    class PlayerPowerup : Component {

        // --------------------- VARIABLES ---------------------

        //public


        //private
        private List<Powerup> _carryingPowerup; // last collected is first to use -> LIFO
        //it's like a stack. if you add more that maxNumber, the one at position 0 is deleted

        // List of all powerups the player is in collision currently
        private List<Powerup> _collidedWith;

        // const
        const int MaxNumberPowerups = 1;
        public System.Action OnPowerupPickup;

        //reference
        private Player _player;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //_carryingPowerup = new List<Powerup>();
            //Reset();
            if (_carryingPowerup == null) _carryingPowerup = new List<Powerup>();
            _collidedWith = new List<Powerup>();

            _player = gameObject.GetComponent<Player>();
        }
        
        public override void Reset() { // this is called 4 times
            _carryingPowerup = new List<Powerup>(); _carryingPowerup.Clear();
            //int boughtPowerup = 0;
            if(BuyPowerupManager.Instance != null && _player != null)
                if (BuyPowerupManager.Instance.HasBoughtPowerup(_player.PlayerIndex)){
                    //create and add powerup
                    Powerup newPowerup = Spawner.Instance.SpawnPowerupToBuy();//Powerup.PowerupFromIndex(boughtPowerup);
                    newPowerup.DoPickup(_player);
                    //newPowerup.SetOwner(_player);
                    //_carryingPowerup.Clear();
                    //_carryingPowerup.Add(newPowerup);
                    //Collect(newPowerup); // TODO make sure this works
                }

            PowerupUI.Instance.UpdatePlayerPowerupUI(gameObject.GetComponent<Player>().PlayerIndex, CarryingPowerups());
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void UsePowerup(Player p) {
            //could implement selector here
            if (_carryingPowerup.Count > 0) {
                _carryingPowerup[_carryingPowerup.Count - 1].UsePowerup();
                _carryingPowerup.RemoveAt(_carryingPowerup.Count - 1);
                Input.Vibrate(.03f, .04f, gameObject.GetComponent<Player>().PlayerIndex);

                if (IsCollidedWithPowerup) {
                    UseFirstCollided();
                }
            }
            PowerupUI.Instance.UpdatePlayerPowerupUI(p.PlayerIndex, CarryingPowerups());
        }

        public void Collect(Powerup powerup) {
            if (_carryingPowerup.Count == MaxNumberPowerups) {
                _carryingPowerup.RemoveAt(0);
            }
            Input.Vibrate(.03f, .04f, _player.PlayerIndex);

            OnPowerupPickup?.Invoke();
            _carryingPowerup.Add(powerup);
            PowerupUI.Instance.UpdatePlayerPowerupUI(powerup.Owner.PlayerIndex, CarryingPowerups());
        }

        public bool CanPickUp(Powerup powerup) {
            //return true;
            return _carryingPowerup.Count < MaxNumberPowerups;
        }

        public void AddCollided(Powerup powerup) {
            _collidedWith.Add(powerup);
        }

        public void RemoveCollided(Powerup powerup) {
            _collidedWith.Remove(powerup);
        }

        public void UseFirstCollided() {
            _collidedWith[0].DoPickup(_player);
            _collidedWith.RemoveAt(0);
        }


        // queries
        public bool HasPowerup { get { return _carryingPowerup.Count > 0; } }

        public int[] CarryingPowerups() {
            List<int> result = new List<int>();
            foreach (var p in _carryingPowerup) result.Add((int)p.powerupType);
            return result.ToArray();
        }

        public bool IsCollidedWithPowerup => _collidedWith.Count > 0;

        // other

    }
}