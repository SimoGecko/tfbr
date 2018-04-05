// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Scripts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRS {
    static class Time {
        ////////// static class that provides time functions and timers //////////

        public static GameTime gt;
        public static int frame = 0;

        public static List<Timer> timers = new List<Timer>();

        public static float time      { get { return (float)gt.TotalGameTime.TotalSeconds; } }
        public static float deltaTime { get { return (float)gt.ElapsedGameTime.TotalSeconds; } }
        public static int OneFrame  { get { return gt.ElapsedGameTime.Milliseconds; } }

        public static void Update(GameTime _gt) {
            gt = _gt;
            frame++;

            //process timers
            for(int i=0; i<timers.Count; i++) {
                if (!GameManager.GameActive && !timers[i].alwaysRun) continue;
                timers[i].span = timers[i].span.Subtract(gt.ElapsedGameTime);
                if(timers[i].span.TotalSeconds<0) {
                    timers[i].callback();
                    timers.RemoveAt(i--);
                }
            }
        }

        public static Task WaitForSeconds(float s) { // used in coroutines
            return Task.Delay((int)(s * 1000));
        }
        public static Task WaitForFrame() { // used in coroutines
            //THIS doesn't work, isn't smooth
            return Task.Delay(1);// gt.ElapsedGameTime.Milliseconds/2);
        }

    }

    //TIMERS - create them and they will automatically call the callback function once they expire
    class Timer {
        public TimeSpan span;
        public System.Action callback;
        public bool alwaysRun; // update the timer even outside of game playing
        public Timer(int minutes, int seconds, int milliseconds, System.Action _callback, bool _alwaysRun = false) {
            span = new TimeSpan(0, 0, minutes, seconds, milliseconds);
            callback = _callback;
            alwaysRun = _alwaysRun;
            Time.timers.Add(this);
        }
        //shorter constructors
        public Timer(int seconds, System.Action _callback, bool _alwaysRun = false) : this(0, seconds, 0, _callback, _alwaysRun) { }
        public Timer(int minutes, int seconds, System.Action _callback, bool _alwaysRun = false) : this(minutes, seconds, 0, _callback, _alwaysRun) { }
        public Timer(float seconds, System.Action _callback, bool _alwaysRun = false) : this(0, 0, (int)(1000*seconds), _callback, _alwaysRun) { }
    }
}
