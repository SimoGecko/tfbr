﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.PowerUps;
using BRS.Scripts.UI;

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

        // const
        const int MaxNumberPowerups = 1;
        public System.Action OnPowerupPickup;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _carryingPowerup = new List<Powerup>();

        }
        public override void Update() { }

        public override void Reset() {
            _carryingPowerup = new List<Powerup>(); _carryingPowerup.Clear();
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
            }
            PowerupUI.Instance.UpdatePlayerPowerupUI(p.PlayerIndex, CarryingPowerups());
        }

        public void Collect(Powerup powerup) {
            int playerIndex = gameObject.GetComponent<Player>().PlayerIndex;
            if (_carryingPowerup.Count == MaxNumberPowerups) {
                _carryingPowerup.RemoveAt(0);
            }
            Input.Vibrate(.03f, .04f, playerIndex);

            OnPowerupPickup?.Invoke();
            _carryingPowerup.Add(powerup);
            PowerupUI.Instance.UpdatePlayerPowerupUI(powerup.Owner.PlayerIndex, CarryingPowerups());
            PowerupUI.Instance.SetBackgroundColor(powerup.powerupColor, playerIndex);
        }

        public bool CanPickUp(Powerup powerup) {
            //return true;
            return _carryingPowerup.Count < MaxNumberPowerups;
        }

        // queries
        bool HasPowerup { get { return _carryingPowerup.Count > 0; } }

        public int[] CarryingPowerups() {
            List<int> result = new List<int>();
            foreach (var p in _carryingPowerup) result.Add((int)p.PowerupType);
            return result.ToArray();
        }

        // other

    }
}