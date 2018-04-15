// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Utilities;
using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;

namespace BRS.Engine {
    static class Audio {
        ////////// static class that contains all audio in the game and allows playback //////////

        static Dictionary<string, SoundEffect> sounds;
        static Dictionary<string, Song> songs;

        static List<SoundEmit> currentlyPlayingEffects = new List<SoundEmit>();

        static AudioListener listener = new AudioListener();
        static Transform listenerTransf = Camera.Main.transform;

        //-------------------------------------------------------------------------------

        public static void Start() {
            sounds = new Dictionary<string, SoundEffect>(); // allows to pause and stop
            songs  = new Dictionary<string, Song>();

            BuildAudioLibrary(AudioContent.SoundString());
            BuildSongLibrary(AudioContent.SongString());
        }


        public static void Update() {
            Apply3DPositionToPlayingSounds();
            RemoveFinishedEffects();
        }

        static void Apply3DPositionToPlayingSounds() {
            listener = Listener();
            foreach (SoundEmit se in currentlyPlayingEffects) {
                se.soundInstance.Apply3D(listener, se.emitter);
            }
        }

        static void RemoveFinishedEffects() {
            for (int i = 0; i < currentlyPlayingEffects.Count; i++) {
                if (Time.CurrentTime > currentlyPlayingEffects[i].endTime) {
                    currentlyPlayingEffects.RemoveAt(i--);
                }
            }
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
            v = Utility.Clamp01(v);
            SoundEffect.MasterVolume = v;
        }
        public static void SetMusicVolume(float v) {
            v = Utility.Clamp01(v);
            MediaPlayer.Volume = v;
        }


        //-----------------------------EFFECTS-----------------------------
        public static void Play(string name, Vector3 position, float volume = 1, bool changePitch = false) {
            if (!sounds.ContainsKey(name)) { Debug.LogError("No sound " + name); return; }

            AudioEmitter em = new AudioEmitter();
            em.Position = position;
            em.Forward = Vector3.Forward;
            em.Up = Vector3.Up;

            SoundEffectInstance soundInstance = sounds[name].CreateInstance();
            float duration = (float)sounds[name].Duration.TotalSeconds;
            currentlyPlayingEffects.Add(new SoundEmit(soundInstance, em, duration));
            if (changePitch) {
                soundInstance.Pitch = MyRandom.Range(-1f, 1f);
            }
            //soundInstance.Volume = volume;
            //soundInstance.Pan = -sounds[name].Pan;
            soundInstance.Apply3D(Listener(), em);
            soundInstance.Play();
        }

        /*
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
        }*/


        //-----------------------------SONGS-----------------------------
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
            listener.Position = listenerTransf.position;
            listener.Forward  = listenerTransf.Forward;
            listener.Up       = listenerTransf.Up;
            return listener;
        }




        //=============================================================================================================
        // AUDIO AND SONGS IN THE GAME
        static void BuildAudioLibrary(string[] soundsString) {
            foreach(string s in soundsString) {
                SoundEffect soundEffect = File.Load<SoundEffect>("Audio/effects/" + s);
                string name = s.Split('/')[1];
                sounds.Add(name, soundEffect);
            }
        }

        static void BuildSongLibrary(string[] songsString) {
            foreach (string s in songsString) {
                Song song = File.Load<Song>("Audio/songs/" + s);
                songs.Add(s, song);
            }
        }

        public class SoundEmit {
            public SoundEffectInstance soundInstance;
            public AudioEmitter emitter;
            public float endTime;
            public SoundEmit(SoundEffectInstance _si, AudioEmitter _em, float duration) {
                soundInstance = _si;
                emitter = _em;
                endTime = Time.CurrentTime + duration;
            }
        }

        
    }
}
