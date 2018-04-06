using System;

namespace BRS.Engine.Utilities {
    public class Curve {
        //represents a curve that can be evaulated in the range [0,1]
        public static float EvaluateSigmoid(float t) {
            return (float)(Math.Tanh((t - 0.5f) * 5.2f) + 1) / 2;
        }

        public static float EvaluateSqrt(float t) {
            return (float)Math.Sqrt(t * 1.5f);
        }

        public static float EvaluatePingPong(float t) {
            return (-t * t + t) * 4;
        }

        public static float EvaluateUp(float t) {
            return t * t;
        }

        public static float EvaluateDown(float t) {
            return (float)Math.Pow(t, .2f);
        }

        //see notes on notebook for shape
        public static float EvaluateA(float t) { return t * t; }

        public static float EvaluateB(float t) { return (float)Math.Sqrt(t); }

        public static float EvaluateC(float t) { return (float)Math.Sin((t - .5f) * Math.PI) / 2 + .5f; }
    }
}