// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;

namespace BRS.Scripts {
    class AudioContent : Component {
        ////////// fills the Audio class with all the necessary files //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Audio.PlaySong("Happy Happy Game Show");
            Audio.SetMusicVolume(.01f);
            Audio.SetSoundVolume(1f);
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public static string[] SongString() {
            return new string[] { "Happy Happy Game Show" };
        }

        public static string[] SoundString() {
            //extend this string array with all the sounds (use correct names)
            return new string[] {
                "attacks/attack", "attacks/break", "attacks/stun",
                "elements/bomb_timer", "elements/explosion", "elements/speedpad",
                "game/police",
                "powerups/bomb_pickup", "powerups/capacity_pickup", "powerups/health_pickup","powerups/key_pickup", "powerups/shield_pickup", "powerups/speed_pickup",
                "powerups/key_use", "powerups/shield_use",
                "valuables/cash_pickup", "valuables/gold_pickup", "valuables/diamond_pickup", };
        }



        // queries



        // other

    }
}