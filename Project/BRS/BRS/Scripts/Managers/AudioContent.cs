// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;

namespace BRS.Scripts {
    class AudioContent {
        ////////// fills the Audio class with all the necessary files //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference


        // --------------------- BASE METHODS ------------------
        public static void Start() {
#if !DEBUG
            Audio.PlaySong("Happy Happy Game Show");
#endif

            Audio.SetMusicVolume(.005f);
            Audio.SetSoundVolume(1f);
            Audio.TansitionToRandomSong();
        }




        // --------------------- CUSTOM METHODS ----------------


        // commands
        public static string[] SongString() {
            return new string[] { "Happy Happy Game Show", "fun", "lively" };
        }

        public static string[] SoundString() {
            //extend this string array with all the sounds (use correct names)
            return new string[] {//effects from mary
                "attack/dash", "attack/enemy_hit", "attack/stun",
                "collisions/bomb_explosion", "collisions/bomb_timer", "collisions/wall_hit",
                "elements/active_magnet", "elements/catched_trap", "elements/falling_weight", "elements/speedpad",
                "game/end_lose", "game/end_win", "game/police",
                "menu/button_press_A", "menu/button_press_B", "menu/characters_popup","menu/menu_change",
                "movement/braking_tires", "movement/damaged_car", "movement/full_car", "movement/vehicle_engine_normal",//none used
                "other/crate_cracking", "other/leaving_cash_base", "other/vault_opening",
                "powerup/pickup", "powerup/use_bomb", "powerup/use_key", "powerup/use_stamina", "powerup/use_trap", "powerup/use_various",
                "valuables/leave", "valuables/pickup_cash", "valuables/pickup_diamond", "valuables/pickup_gold"
                //not called yet: wall_hit, end_lose/win, button_press_A/B, menu_change
            };
        }



        // queries



        // other

    }
}