// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using Jitter.LinearMath;

namespace BRS.Engine {
    static class Debug {
        public static void Log(string s) {
            //Console.WriteLine(s);
            System.Diagnostics.Debug.WriteLine(s);
        }

        public static void Log(Object o) {
            System.Diagnostics.Debug.WriteLine(o.ToString());
        }


        /// <summary>
        /// Log a matrix from the jitter-phisics
        /// (Andy: Please do not remove this, also when unusused since it might come handy to quickly debug something, thanks)
        /// </summary>
        /// <param name="m">Matrix to log on the console</param>
        /// <param name="info">Additional info to print in front</param>
        public static void Log(JMatrix m, string info = "") {
            string s = $"{info}:\n{m.M11} {m.M12} {m.M13}\n{m.M21} {m.M22} {m.M23}\n{m.M31} {m.M32} {m.M33}";
            Log(s);
        }


        /// <summary>
        /// Log a vector from the jitter-phisics
        /// (Andy: Please do not remove this, also when unusused since it might come handy to quickly debug something, thanks)
        /// </summary>
        /// <param name="v">Vector to log on the console</param>
        /// <param name="info">Additional info to print in front</param>
        public static void Log(JVector v, string info = "") {
            string s = $"{info}:\n{v.X} {v.Y} {v.Z}";
            Log(s);
        }


        public static void LogError(string s) {
            //Console.WriteLine("//ERROR//: "+s);
            System.Diagnostics.Debug.WriteLine("//ERROR//: " + s);
        }

        public static void Assert(bool b, string s) {
            if (!b)
                System.Diagnostics.Debug.WriteLine("//ASSERTION FAIL//: " + s);
        }
    }
}
