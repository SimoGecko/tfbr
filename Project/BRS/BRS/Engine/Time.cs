// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using System.Threading.Tasks;
using BRS.Scripts;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;

namespace BRS.Engine {
    static class Time {
        ////////// static class that provides time functions and timers //////////

        public static GameTime Gt;
        public static int Frame = 0;

        public static List<Timer> timers = new List<Timer>();

        public static float CurrentTime      { get { return (float)Gt.TotalGameTime.TotalSeconds; } }
        public static float DeltaTime { get { return (float)Gt.ElapsedGameTime.TotalSeconds; } }
        public static int OneFrame  { get { return Gt.ElapsedGameTime.Milliseconds; } }

        public static void Update(GameTime gt) {
            Gt = gt;
            Frame++;

            //process timers
            for(int i=0; i<timers.Count; i++) {
                if (!GameManager.GameActive && !timers[i].AlwaysRun) continue;
                timers[i].Span = timers[i].Span.Subtract(Gt.ElapsedGameTime);
                if(timers[i].Span.TotalSeconds<0) {
                    timers[i].Callback();
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
}
