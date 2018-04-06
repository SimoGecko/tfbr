using System;

namespace BRS {

    /// <summary>
    /// TIMERS - create them and they will automatically call the callback function once they expire
    /// </summary>
    class Timer {
        public TimeSpan Span;
        public readonly Action Callback;
        public readonly bool AlwaysRun; // update the timer even outside of game playing

        public Timer(int minutes, int seconds, int milliseconds, Action callback, bool alwaysRun = false) {
            Span = new TimeSpan(0, 0, minutes, seconds, milliseconds);
            Callback = callback;
            AlwaysRun = alwaysRun;
            Time.timers.Add(this);
        }

        //shorter constructors
        public Timer(int seconds, Action callback, bool alwaysRun = false) : this(0, seconds, 0, callback, alwaysRun) { }
        public Timer(int minutes, int seconds, Action callback, bool alwaysRun = false) : this(minutes, seconds, 0, callback, alwaysRun) { }
        public Timer(float seconds, Action callback, bool alwaysRun = false) : this(0, 0, (int)(1000 * seconds), callback, alwaysRun) { }
    }
}
