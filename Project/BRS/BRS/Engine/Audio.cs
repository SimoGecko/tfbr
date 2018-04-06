// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace BRS {
    static class Audio {
        //static class that contains all audio in the game and allows playback
        //static Dictionary<string, SoundEffect> sounds;
        static Dictionary<string, SoundEffectInstance> sounds;
        static Dictionary<string, Song> songs;

        //static Transform listener = GameObject.FindGameObjectWithName("player_0").transform;
        static Transform listenerTransf = Camera.Main.transform;
        static AudioListener listener = new AudioListener();

        static AudioEmitter em = new AudioEmitter();

        //-------------------------------------------------------------------------------

        public static void Start() {
            //sounds    = new Dictionary<string, SoundEffect>();
            sounds = new Dictionary<string, SoundEffectInstance>(); // allows to pause and stop
            songs  = new Dictionary<string, Song>();

            BuildAudioLibrary();
            BuildSongLibrary();

            //PlaySong("Happy Happy Game Show");
            SetMusicVolume(.05f);
            SetSoundVolume(1f);
        }


        public static void Update() {
            sounds["mono/phi"].Apply3D(Listener(), em);

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
        public static void SetSoundVolume(float v) {
            SoundEffect.MasterVolume = v;
        }
        public static void SetMusicVolume(float v) {
            MediaPlayer.Volume = v;
        }


        //EFFECTS
        public static void Play(string name, Vector3 position, float volume = 1) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }

            em = new AudioEmitter();
            em.Position = position;
            em.Forward = Vector3.Forward;
            em.Up = Vector3.Up;

            sounds[name].Apply3D(Listener(), em);
            //sounds[name].Pan = -sounds[name].Pan;
            //sounds[name].Volume = volume;
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

        public static void PauseSong()  { MediaPlayer.Pause(); }
        public static void ResumeSong() { MediaPlayer.Resume(); }
        public static void StopSong()   { MediaPlayer.Stop(); }

        public static void SetSongLoop(bool b) {
            MediaPlayer.IsRepeating = b;
        }

        public static string SongPosition() {
            return MediaPlayer.PlayPosition.ToReadableString();
        }


        //queries
        static AudioListener Listener() {
            listener.Forward  = listenerTransf.Forward;
            listener.Up       = listenerTransf.Up;
            listener.Position = listenerTransf.position;
            return listener;
        }




        //=============================================================================================================
        // AUDIO AND SONGS IN THE GAME
        static void BuildAudioLibrary() {
            //extend this string array with all the sounds (use correct names)
            string[] soundsString = new string[] { "mono/phi", "boing" };

            foreach(string s in soundsString) {
                SoundEffect soundEffect = File.Load<SoundEffect>("Audio/test/" + s);
                sounds.Add(s, soundEffect.CreateInstance());
                //sounds[s].IsLooped = true;
            }
        }

        static void BuildSongLibrary() {
            string[] songsString = new string[] { "Happy Happy Game Show" };

            foreach (string s in songsString) {
                Song song = File.Load<Song>("Audio/songs/" + s);
                songs.Add(s, song);
            }
        }

        
    }
}
