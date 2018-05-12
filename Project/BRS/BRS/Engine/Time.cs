// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using BRS.Scripts;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;

namespace BRS.Engine {
    static class Time {
        ////////// static class that provides time functions //////////

        public static GameTime Gt = new GameTime();
        public static int Frame = 0;

        public static List<Timer> timers = new List<Timer>();

        public static float CurrentTime { get { return (float)Gt.TotalGameTime.TotalSeconds; } }
        public static float DeltaTime { get { return (float)Gt.ElapsedGameTime.TotalSeconds; } }
        public static int OneFrame { get { return Gt.ElapsedGameTime.Milliseconds; } }
        public static float FrameRate { get { return 1 / (float)Gt.ElapsedGameTime.TotalSeconds; } } // TODO draw this on screen


        public static void Update(GameTime gt) {
            Gt = gt;
            Frame++;

            //Debug.Log("Start with " + timers.Count + " timers");
            //process timers
            for (int i = 0; i < timers.Count; i++) {
                if (!GameManager.GameActive && !timers[i].AlwaysRun) continue; // knows about gamemanager
                timers[i].Span = timers[i].Span.Subtract(Gt.ElapsedGameTime);
                if (timers[i].Span.TotalSeconds < 0) {
                    timers[i].Callback();
                    timers.RemoveAt(i--);
                }
            }
            //Debug.Log("End with " + timers.Count + " timers");
        }

        public static Task WaitForSeconds(float s) { // used in coroutines
            return Task.Delay((int)(s * 1000));
        }
        public static Task WaitForFrame() { // used in coroutines
            //THIS doesn't work, isn't smooth
            return Task.Delay(1);// gt.ElapsedGameTime.Milliseconds/2);
        }

        // Todo: Think about this one in more detail
        public static void ClearTimers() {
            for (int i = 0; i < timers.Count; ++i) {
                if (!timers[i].AlwaysRun) {
                    timers.RemoveAt(i--);
                }
            }
            //lock (new object())
            //{
            //    for (int i = 0; i < timers.Count; ++i)
            //    {
            //        if (!timers[i].AlwaysRun)
            //        {
            //            timers.RemoveAt(i--);
            //        }
            //    }
            //}

            //var newList = new List<Timer>();

            //foreach (Timer timer in timers)
            //{
            //    if (timer.AlwaysRun)
            //    {
            //        newList.Add(timer);
            //    }
            //}

            //timers = newList;

        }

    }
}
