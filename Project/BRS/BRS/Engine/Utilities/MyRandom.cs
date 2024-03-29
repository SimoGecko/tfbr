using System;
using Microsoft.Xna.Framework;

namespace BRS.Engine {
    static class MyRandom { // TODO find better name
        static bool sameSeed = false;
        static readonly int Seed = 315;
        static readonly Random Rand = sameSeed ? new Random(Seed) : new Random();

        public static float Value { // random float in [0, 1[
            get { return (float)Rand.NextDouble(); }
        }

        public static int Range(int min, int max) { // random int in [min, max[
            return min + Rand.Next(max - min);
        }
        public static float Range(float min, float max) { // random float in [min, max[
            return (float)(min + Rand.NextDouble() * (max - min));
        }

        public static Vector2 InsideRectangle(Rectangle rect) {
            return new Vector2(rect.X + Value * rect.Width, rect.Y + Value * rect.Height);
        }

        public static Vector2 InsideUnitSquare() {
            return new Vector2(Value * 2 - 1, Value * 2 - 1);
        }

        public static Vector3 InsideUnitCube() {
            return new Vector3(Value * 2 - 1, Value * 2 - 1, Value * 2 - 1);
        }

        public static Vector2 InsideUnitCircle() {
            double r = Math.Sqrt(Rand.NextDouble());
            double phi = Rand.NextDouble() * 2 * Math.PI;
            return new Vector2((float)(Math.Cos(phi) * r), (float)(Math.Sin(phi) * r));
        }

        public static Vector3 InsideUnitSphere() {
            Vector3 sample = new Vector3(Value * 2 - 1, Value * 2 - 1, Value * 2 - 1);
            while (sample.LengthSquared() > 1)
                sample = new Vector3(Value * 2 - 1, Value * 2 - 1, Value * 2 - 1);
            return sample;
        }

        public static Quaternion YRotation() {
            return Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(Value * 360));
        }

        public static Vector3 UpsideLinearVelocity() {
            return new Vector3(Range(-0.5f, 0.5f), Range(1.0f, 2.0f), Range(-0.5f, 0.5f));
        }

        public static Color ColorByte() {
            return new Color(
                (byte)Rand.Next(255),
                (byte)Rand.Next(255),
                (byte)Rand.Next(255),
                (byte)Rand.Next(255));
        }
    }
}