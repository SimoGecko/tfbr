// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    static class Time {
        //static class that contains all gameobjects in the scene
        public static GameTime gt;
        public static int frame = 0;

        public static List<Timer> timers = new List<Timer>();

        public static float time      { get { return (float)gt.TotalGameTime.TotalSeconds; } }
        public static float deltatime { get { return (float)gt.ElapsedGameTime.TotalSeconds; } }
        public static int OneFrame  { get { return gt.ElapsedGameTime.Milliseconds; } }

        public static void Update(GameTime _gt) {
            frame++;
            gt = _gt;

            for(int i=0; i<timers.Count; i++) {
                timers[i].span = timers[i].span.Subtract(gt.ElapsedGameTime);
                if(timers[i].span.TotalSeconds<0) {
                    //timers[i].expired = true;
                    timers[i].callback();
                    timers.RemoveAt(i--);
                }
            }
        }

        public static Task WaitForSeconds(float s) { // used in coroutines
            return Task.Delay((int)(s * 1000));
            //using System.Threading.Tasks;
        }
        public static Task WaitForFrame() { // used in coroutines
            return Task.Delay(1);// gt.ElapsedGameTime.Milliseconds/2);
        }

    }

    //timers -- create them and they will automatically call the callback function once they expire
    class Timer {
        public TimeSpan span;
        public System.Action callback;
        public Timer(int minutes, int seconds, int milliseconds, System.Action _callback) {
            span = new TimeSpan(0, 0, minutes, seconds, milliseconds);
            callback = _callback;
            Time.timers.Add(this);
        }
        //shorter versions
        public Timer(int seconds, System.Action _callback) : this(0, seconds, 0, _callback) { }
        public Timer(int minutes, int seconds, System.Action _callback) : this(minutes, seconds, 0, _callback) { }
        public Timer(float seconds, System.Action _callback) : this(0, 0, (int)(1000*seconds), _callback) { }
    }
}
