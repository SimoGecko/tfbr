// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace BRS {
    static class Audio {
        //static class that contains all audio in the game and allows playback
        //static Dictionary<string, SoundEffect> sounds;
        static Dictionary<string, SoundEffectInstance> sounds;
        static Dictionary<string, Song> songs;

        public static void Start() {
            //sounds    = new Dictionary<string, SoundEffect>();
            sounds = new Dictionary<string, SoundEffectInstance>(); // allows to pause and stop
            songs  = new Dictionary<string, Song>();

            BuildAudioLibrary();
            BuildSongLibrary();
        }

        public static void Update() {

        }

        //COMMANDS


        //EFFECTS
        /*
        public static void Play(string name) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].Play();
        }
        public static void Play(string name, float volume, float pan) { // pan: -1=left, 0=center, +1=right; volume in [0,1]
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].Play(volume, 0, pan);
        }*/


        //MASTER SETTINGS
        public static void SetMusicVolume(float v) {

        }
        public static void SetSoundsVolume(float v) {

        }


        //EFFECTS
        public static void Play(string name, Vector3 position) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }

            AudioEmitter em = new AudioEmitter();
            em.Position = position;

            sounds[name].Apply3D(Listener(), em);
            sounds[name].Play();
        }

        public static void Pause(string name) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].Pause();
        }
        public static void Resume(string name) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].Resume();
        }
        public static void Stop(string name) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].Stop();
        }
        public static void SetLoop(string name, bool b) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].IsLooped = b;
        }


        //SONGS
        public static void PlaySong(string name) {
            if (!songs.ContainsKey(name)) { Debug.LogError("No song " + name); return; }
            MediaPlayer.Play(songs[name]);
        }

        public static void SetSongLoop(bool b) {
            MediaPlayer.IsRepeating = b;
        }

        public static string SongPosition() {
            return MediaPlayer.PlayPosition.ToReadableString();
        }



        //queries
        static AudioListener Listener() {
            AudioListener result = new AudioListener();
            result.Forward  = Camera.main.transform.Forward;
            result.Up       = Camera.main.transform.Up;
            result.Position = Camera.main.transform.position;
            return result;
        }




        //=============================================================================================================
        // AUDIO AND SONGS IN THE GAME
        static void BuildAudioLibrary() {
            //extend this string array with all the sounds
            string[] soundsString = new string[] { "phi", "boing" };

            foreach(string s in soundsString) {
                SoundEffect soundEffect = File.Load<SoundEffect>("Audio/test/" + s);
                sounds.Add(s, soundEffect.CreateInstance());
            }
        }

        static void BuildSongLibrary() {
            string[] songsString = new string[] { };

            foreach (string s in songsString) {
                Song song = File.Load<Song>("Audio/test/" + s);
                songs.Add(s, song);
            }
        }

        
    }
}
