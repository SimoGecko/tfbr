// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.IO;

namespace BRS {
    static class Utility {
        ////////// class that provides useful methods (mostly math and on existing objects) - Imitates Unity //////////

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
            //formula taken from Unity
            float deltaTime = Time.deltaTime;

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
            return new Vector3( SmoothDamp(current.X, target.X, ref currentVelocity.X, smoothTime),
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


        public static float InverseCDF(float x, float a) {
            return (x-(float)Math.Sqrt(-4*a*x+4+a+x*x)) / (2*(x-1));
        }

       public static string EvaluateDistribution(Dictionary<string, float> distrib) {
            float sum = 0;
            foreach (var entry in distrib) sum += entry.Value;

            float val = MyRandom.Value*sum;
            foreach(var entry in distrib) {
                if (val <= entry.Value) return entry.Key;
                val -= entry.Value;
            }
            Debug.LogError("distribution doesn't sum to 1");
            return "";
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

            //return new Vector3(pitch, -yaw, -roll); // Ensure it's right
            return new Vector3(roll, -pitch, yaw); // Ensure it's right
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
    }

        public static Quaternion Euler(Vector3 angles) {
            return Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(angles.Y), MathHelper.ToRadians(angles.X), MathHelper.ToRadians(angles.Z));
        }*/


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

        //========================================================0

        //In a 2D grid, returns the angle to a specified point from the +X axis
        public static float ArcTanAngle(float X, float Y) {
            if (X == 0) {
                if (Y == 1) return (float)MathHelper.PiOver2;
                else        return (float)-MathHelper.PiOver2;
            } else if (X > 0) return (float)Math.Atan(Y / X);
            else if (X < 0) {
                if (Y > 0) return (float)Math.Atan(Y / X) + MathHelper.Pi;
                else       return (float)Math.Atan(Y / X) - MathHelper.Pi;
            } else return 0;
        }

        //returns Euler angles that point from one point to another
        public static Vector3 AngleTo(Vector3 from, Vector3 location) {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);
            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = ArcTanAngle(-v3.Z, -v3.X);
            return angle;
        }

        //converts a Quaternion to Euler angles (X = pitch, Y = yaw, Z = roll)
        public static Vector3 ToEuler(this Quaternion rotation) {
            Vector3 rotationaxes = new Vector3();

            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            rotationaxes = AngleTo(new Vector3(), forward);
            if (rotationaxes.X == MathHelper.PiOver2) {
                rotationaxes.Y = ArcTanAngle(up.Z, up.X);
                rotationaxes.Z = 0;
            } else if (rotationaxes.X == -MathHelper.PiOver2) {
                rotationaxes.Y = ArcTanAngle(-up.Z, -up.X);
                rotationaxes.Z = 0;
            } else {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));
                rotationaxes.Z = ArcTanAngle(up.Y, -up.X);
            }
            return new Vector3(MathHelper.ToDegrees(rotationaxes.X), MathHelper.ToDegrees(rotationaxes.Y), MathHelper.ToDegrees(rotationaxes.Z));
        }



        //==============================================================



        //GRAPHICS METHODS
        public static void DrawModel(Model model, Matrix view, Matrix proj, Matrix world, EffectMaterial mat =null) {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    if (mat == null) {
                        //default settings
                        effect.EnableDefaultLighting();
                    } else {
                        effect.EnableDefaultLighting();
                        //effect.LightingEnabled = mat.lit;
                        //effect.DiffuseColor = mat.diffuse.ToVector3();
                        effect.Alpha = mat.diffuse.A;
                        //effect.CurrentTechnique = EffectTechnique
                        //effect.Texture
                    }
                    //effect.Alpha = .5f;
                    //effect.di
                    //effect.EnableDefaultLighting();

                    //effects
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = proj;
                }
                mesh.Draw(); // outside, not inside
            }
        }

        public static Color[,] TextureTo2DArray(Texture2D texture) {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];
            return colors2D;
        }
        /*
        public static Vector3 ColorTo3(this Color c) {
            return new Vector3((float)c.R/255, (float)c.G/255, (float)c.B/255);
        }*/

        //==============================================================
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

        public static Vector2 Rotate(this Vector2 v, float angle) {
            float cos = (float)Math.Cos(MathHelper.ToRadians(angle));
            float sin = (float)Math.Sin(MathHelper.ToRadians(angle));
            return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }

        public static Vector2 Evaluate(this Rectangle rect, Vector2 v) {
            return new Vector2(rect.X + v.X * rect.Width, rect.Y + v.Y * rect.Height);
        }
        public static Vector2 GetCenter(this Rectangle rect) {
            return new Vector2(rect.X + .5f*rect.Width, rect.Y + .5f*rect.Height);
        }

        public static Vector2 Round(this Vector2 v) { // Makes it Point2
            return new Vector2((int)v.X, (int)v.Y);
        }
        public static float Clamp(this float f, float min, float max) {
            return f < min ? min : f > max ? max : f;
        }

        public static float Angle(this Vector3 a, Vector3 b) {
            //returns angle in degree between a and b
            float cos = Vector3.Dot(a.normalized(), b.normalized());
            return MathHelper.ToDegrees((float) Math.Acos(cos));
        }


    }


    static class MyRandom { // TODO find better name
        static int seed = 102;
        static Random rand = new Random(seed);

        public static float Value { // random float in [0, 1[
            get { return (float)rand.NextDouble(); }
        }

        public static int Range(int min, int max) { // random int in [min, max[
            return min + rand.Next(max - min);
        }
        public static float Range(float min, float max) { // random float in [min, max[
            return (float)(min + rand.NextDouble()*(max - min));
        }

        public static Vector2 InsideRectangle(Rectangle rect) {
            return new Vector2(rect.X + Value * rect.Width, rect.Y + Value * rect.Height);
        }

        public static Vector2 insideUnitSquare() {
            return new Vector2(Value*2-1, Value * 2 - 1);
        }
        public static Vector3 insideUnitCube() {
            return new Vector3(Value * 2 - 1, Value * 2 - 1, Value * 2 - 1);
        }

        public static Vector2 insideUnitCircle() {
            double r = Math.Sqrt(rand.NextDouble());
            double phi = rand.NextDouble() * 2 * Math.PI;
            return new Vector2((float)(Math.Cos(phi) * r), (float)(Math.Sin(phi) * r));
        }
        public static Vector3 insideUnitSphere() {
            Vector3 sample = new Vector3(Value*2-1, Value*2-1, Value*2-1);
            while(sample.LengthSquared()>1)
                sample = new Vector3(Value*2-1, Value*2-1, Value*2-1);
            return sample;
        }
        

        public static Quaternion YRotation() {
            return Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(Value * 360));
        }

    }

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

        //see notes on notebook for shape
        public static float EvaluateA(float t) { return t * t; }
        public static float EvaluateB(float t) { return (float)Math.Sqrt(t); }
        public static float EvaluateC(float t) { return (float)Math.Sin((t-.5f)*Math.PI)/2+.5f; }
    }
}
