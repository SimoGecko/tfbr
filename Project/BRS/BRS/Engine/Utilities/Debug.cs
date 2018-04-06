// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;

namespace BRS.Engine.Utilities {
    static class Debug {
        public static void Log(string s) {
            //Console.WriteLine(s);
            System.Diagnostics.Debug.WriteLine(s);
        }
        public static void Log(Object o) {
            System.Diagnostics.Debug.WriteLine(o.ToString());
        }

        public static void LogError(string s) {
            //Console.WriteLine("//ERROR//: "+s);
            System.Diagnostics.Debug.WriteLine("//ERROR//: "+s);
        }

        public static void Assert(bool b, string s) {
            if (!b) System.Diagnostics.Debug.WriteLine("//ASSERTION FAIL//: " + s);
        }
    }
}
