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
        public static ContentManager Content;
        static Dictionary<string, SoundEffect> sounds;
        static Dictionary<string, SoundEffectInstance> instance;
        static Dictionary<string, Song> songs;

        public static void Start() {
            sounds = new Dictionary<string, SoundEffect>();
            instance = new Dictionary<string, SoundEffectInstance>(); // allows to pause and stop
            songs = new Dictionary<string, Song>();


            BuildAudioLibrary();
            BuildSongLibrary();
        }

        public static void Update() {

        }

        //COMMANDS
        public static void GiveContent(ContentManager c) {
            Content = c;
        }


        //EFFECTS
        public static void Play(string name) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].Play();
        }
        public static void Play(string name, float volume, float pan) { // pan: -1=left, 0=center, +1=right; volume in [0,1]
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }
            sounds[name].Play(volume, 0, pan);
        }


        //INSTANCES
        public static void Pause() {

        }
        public static void Stop() {

        }

        public static void SetLoop(string name) {
            instance[name].IsLooped = true;
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

        



        //=============================================================================================================
        // AUDIO AND SONGS IN THE GAME
        static void BuildAudioLibrary() {
            //SoundEffect sound1 = Content.Load<SoundEffect>("sound1");
            //sounds.Add("sound1", sound1);
            //instance.Add("sound1", sound1.CreateInstance());
        }

        static void BuildSongLibrary() {
            //songs.Add("song1", Content.Load<Song>("song1"));
        }

        
    }
}
