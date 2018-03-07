using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

namespace BRS {
    static class Utility {
        //class that provides useful methods (mostly math and on existing objects) - Imitates Unity


        //VARIOUS METHODS
        public static T[] ShuffleArray<T>(T[] array, int seed) {
            System.Random prng = new System.Random(seed);
            for (int i = 0; i < array.Length - 1; i++) {
                int randomIndex = prng.Next(i, array.Length);
                T tempItem = array[randomIndex];
                array[randomIndex] = array[i];
                array[i] = tempItem;
            }
            return array;
        }



        //MATH METHODS
        public static int Log2(int x) {
            return (int)(Math.Log(x) / Math.Log(2));
        }


        public static float Clamp01(float v) {
            return v < 0 ? 0 : v > 1 ? 1 : v;
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed = float.MaxValue) {
            float deltaTime = Time.deltatime;

            smoothTime = Math.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = current - target;
            float num5 = target;
            float num6 = maxSpeed * smoothTime;
            num4 = MathHelper.Clamp(num4, -num6, num6);
            target = current - num4;
            float num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            float num8 = target + (num4 + num7) * num3;
            if (num5 - current > 0f == num8 > num5) {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }


        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed = float.MaxValue) { // TODO fix wrt currentvelocity
            //takes in degrees
            if(Math.Abs(current - target) > 180) {
                if (target < current) target += 360;
                else target -= 360;
            }
            float result = SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed);
            //return WrapAngle(result);
            return result;
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime) {
            return new Vector3(SmoothDamp(current.X, target.X, ref currentVelocity.X, smoothTime),
                                SmoothDamp(current.Y, target.Y, ref currentVelocity.Y, smoothTime),
                                SmoothDamp(current.Z, target.Z, ref currentVelocity.Z, smoothTime));
        }

        public static float WrapAngle(float angle) {//in degrees, return angle in range -180, 180
            if (angle < -180) while (angle < -180) angle += 360;
            if (angle >  180) while (angle >  180) angle -= 360;
            return angle;
        }

        public static float WrapAngle(float angle, float reference) {//in degrees, return angle in range -180, 180
            angle -= reference;
            if (angle < -180) while (angle < -180) angle += 360;
            if (angle > 180) while (angle > 180) angle -= 360;
            return angle + reference;
        }

        //TRANSORM METHODS
        /*
        public static Vector3 toEulerAngle(this Quaternion q){ // in degrees //NOT WORKING CORRECTLY
	        // roll (x-axis rotation)
	        double sinr = 2.0 * (q.W * q.X + q.Y * q.Z);
            double cosr = 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
            float roll = (float)Math.Atan2(sinr, cosr);

            // pitch (y-axis rotation)
            float pitch;
            double sinp = 2.0 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
                //pitch = copysign(M_PI / 2, sinp); // use 90 degrees if out of range
                pitch = (float)(Math.Sign(sinp) * Math.PI / 2.0);
            else
                pitch = (float)Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny = 2.0 * (q.W * q.Z + q.X * q.Y);
            double cosy = 1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
            float yaw = (float)Math.Atan2(siny, cosy);

            //set degrees
            pitch = MathHelper.ToDegrees(pitch);
            yaw   = MathHelper.ToDegrees(yaw);
            roll  = MathHelper.ToDegrees(roll);

            return new Vector3(pitch, -yaw, -roll); // Ensure it's right
        }*/

        /*
    public static Vector3 toEulerAngles(this Matrix m) { // IT WORKS!
        float yaw=0, pitch=0, roll=0;
        if (m.M11 == 1.0f) {
            yaw = (float)Math.Atan2(m.M13, m.M34);

        } else if (m.M11 == -1.0f) {
            yaw = (float)Math.Atan2(m.M13, m.M34);
        } else {
            yaw   = (float)Math.Atan2(-m.M31, m.M11);
            pitch = (float)Math.Asin(m.M21);
            roll  = (float)Math.Atan2(-m.M23, m.M22);
        }
        //set degrees
        pitch = MathHelper.ToDegrees(pitch);
        yaw   = MathHelper.ToDegrees(yaw);
        roll  = MathHelper.ToDegrees(roll);

        return new Vector3(pitch, yaw, -roll);
    }*/

        public static Quaternion Euler(Vector3 angles) {
            return Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(angles.Y), MathHelper.ToRadians(angles.X), MathHelper.ToRadians(angles.Z));
        }


        //DEEP COPY
        /*
        public static T DeepClone<T>(T obj) {
            using (var ms = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }*/



        //GRAPHICS METHODS
        public static void DrawModel(Model model, Matrix view, Matrix proj, Matrix world) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    //NOTE: lighting staff must be put here

                    effect.EnableDefaultLighting();
                    //effects
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = proj;
                }
                mesh.Draw(); // outside, not inside
            }
        }


        //EXTENSION METHODS
        public static Vector3 normalized(this Vector3 v) {
            if (v.Length() < 1e-5) return Vector3.Zero;
            return v / v.Length();
        }

        public static Vector3 GetPoint(this Ray ray, float t) {
            return ray.Position + t * ray.Direction.normalized();
        }

        public static string ToReadableString(this TimeSpan timeSpan) {
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            if (seconds < 10) return minutes + ":0" + seconds;
            return minutes + ":" + seconds;
        }

        public static Vector3 To3(this Vector2 v) {
            return new Vector3(v.X, 0, v.Y);
        }

    }


    static class MyRandom {
        static int seed = 101;
        static Random rand = new Random(seed);

        public static float Value { // random float in [0, 1[
            get { return (float)rand.NextDouble(); }
        }

        public static int Range(int min, int max) { // random int in [min, max[
            return min + rand.Next(max - min);
        }

        public static Vector2 InsideRectangle(Rectangle rect) {
            return new Vector2(rect.X + Value * rect.Width, rect.Y + Value * rect.Height);
        }

    }

    static class Debug {
        public static void Log(string s) {
            //Console.WriteLine(s);
            System.Diagnostics.Debug.WriteLine(s);

        }
        public static void LogError(string s) {
            //Console.WriteLine("//ERROR//: "+s);
            System.Diagnostics.Debug.WriteLine("//ERROR//: "+s);

        }
    }

    public class Curve {
        //represents a curve that can be evaulated in the range [0,1]
        public static float EvaluateSigmoid(float t) {
            return (float)(Math.Tanh((t - 0.5f) * 5.2f) + 1) / 2;
        }
        public static float EvaluateSqrt(float t) {
            return (float)Math.Sqrt(t*1.5f);
        }
        public static float EvaluatePingPong(float t) {
            return (-t * t + t) * 4;
        }
        public static float EvaluateUp(float t) {
            return t*t;
        }
        public static float EvaluateDown(float t) {
            return (float)Math.Pow(t, .2f);
        }

    }
}
