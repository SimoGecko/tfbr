// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using BRS.Scripts;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using System;

namespace BRS.Engine {
    static class Time {
        ////////// static class that provides time functions //////////

        public static GameTime Gt = new GameTime();
        public static int Frame = 0;
        static bool needClearTimers = false;
        static float TimeScale = 1; // speedUp/slowdown
        static float timeChange = .5f;

        public static List<Timer> timers = new List<Timer>();

        public static float CurrentTime { get { return (float)Gt.TotalGameTime.TotalSeconds; } }
        public static float DeltaTime { get { return (float)Gt.ElapsedGameTime.TotalSeconds*TimeScale; } }
        public static int OneFrame { get { return Gt.ElapsedGameTime.Milliseconds; } }
        public static float FrameRate { get { return 1 / (float)Gt.ElapsedGameTime.TotalSeconds; } } // TODO draw this on screen


        public static void Update(GameTime gt) {
            Gt = gt;
            Frame++;

            CheckTimescaleInput();
            DisplayInfo();
            

            //Debug.Log("Start with " + timers.Count + " timers");
            //process timers
            //if (myownLock) return;
            for (int i = 0; i < timers.Count; i++) {
                if (!GameManager.GameActive && !timers[i].AlwaysRun) continue; // knows about gamemanager
                TimeSpan toSubtract = new TimeSpan(0, 0, 0, 0, (int)Math.Round(gt.ElapsedGameTime.Milliseconds * TimeScale));
                timers[i].Span = timers[i].Span.Subtract(toSubtract);//Gt.ElapsedGameTime);
                if (timers[i].Span.TotalSeconds < 0) {
                    timers[i].Callback();
                    timers.RemoveAt(i--);
                }
            }
            //if (needClearTimers) ActualClearTimers();
            //Debug.Log("End with " + timers.Count + " timers");
        }

        static void CheckTimescaleInput() {
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Z)) TimeScale += timeChange;
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.U)) TimeScale = MathHelper.Max(TimeScale - timeChange, .25f);
        }

        static void DisplayInfo() {
            Debug.Log("timescale: " + TimeScale);
        }

        public static Task WaitForSeconds(float s) { // used in coroutines
            int millisec = MathHelper.Max(1, (int)(s * 1000/TimeScale));
            return Task.Delay(millisec);
        }
        public static Task WaitForFrame() { // used in coroutines
            //THIS doesn't work, isn't smooth
            return Task.Delay(1);// gt.ElapsedGameTime.Milliseconds/2);
        }

        // Todo: Think about this one in more detail
        public static void ClearTimers() {
            needClearTimers = true;
            
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

        static void ActualClearTimers() {
            for (int i = 0; i < timers.Count; ++i) {
                if (!timers[i].AlwaysRun) {
                    timers.RemoveAt(i--);
                }
            }
            needClearTimers = false;
        }

    }
}
