// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;

namespace BRS.Engine {
    ////////// TIMERS - create them and they will automatically call the callback function once they expire //////////
    class Timer {
        public TimeSpan Span;
        public readonly Action Callback;
        public readonly bool AlwaysRun; // update the timer even outside of game playing
        public readonly bool BoundToRound; // special timers which need to be reset if the round ends

        public Timer(int minutes, int seconds, int milliseconds, Action callback, bool alwaysRun = false, bool boundToRound = false) {
            Span = new TimeSpan(0, 0, minutes, seconds, milliseconds);
            Callback = callback;
            AlwaysRun = alwaysRun;
            BoundToRound = boundToRound;
            Time.timers.Add(this);
        }

        //shorter constructors
        public Timer(int seconds, Action callback, bool alwaysRun = false, bool boundToRound = false) : this(0, seconds, 0, callback, alwaysRun, boundToRound) { }
        public Timer(int minutes, int seconds, Action callback, bool alwaysRun = false, bool boundToRound = false) : this(minutes, seconds, 0, callback, alwaysRun, boundToRound) { }
        public Timer(float seconds, Action callback, bool alwaysRun = false, bool boundToRound = false) : this(0, 0, (int)(1000 * seconds), callback, alwaysRun, boundToRound) { }
    }
}
