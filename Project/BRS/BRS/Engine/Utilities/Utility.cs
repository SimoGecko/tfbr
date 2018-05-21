// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Scripts.PlayerScripts;
using Microsoft.Xna.Framework;

namespace BRS.Engine {
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
            float deltaTime = Time.DeltaTime;

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

            if (CameraController.autoFollow) {
                while (result < 0  ) result += 360;
                while (result > 360) result -= 360;
            }

            return result;
        }

        public static Vector3 SmoothDampAngle(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime) {
            return new Vector3(SmoothDampAngle(current.X, target.X, ref currentVelocity.X, smoothTime),
                                SmoothDampAngle(current.Y, target.Y, ref currentVelocity.Y, smoothTime),
                                SmoothDampAngle(current.Z, target.Z, ref currentVelocity.Z, smoothTime));
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

        public static int[,] Flip(int[,] a) {
            int d0 = a.GetLength(0);
            int d1 = a.GetLength(1);
            int[,] result = new int[d1, d0];
            for (int x = 0; x < d0; x++) {
                for (int y = 0; y < d1; y++) {
                    result[y, x] = a[x,y];
                }
            }
            return result;
        }

        public static string IntToMoneyString(int value) {
            return "CHF " + value.ToString("N0") + ".-";//"$" + value.ToString("N0")
        }
        public static string IntToMoneyStringSimple(int value) {
            return value.ToString("N0") + ".-";//"$" + value.ToString("N0")
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
                if (Y == 1) return MathHelper.PiOver2;
                else        return -MathHelper.PiOver2;
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



        

        //==============================================================
        //EXTENSION METHODS
        public static Vector3 Normalized(this Vector3 v) {
            if (v.Length() < 1e-5) return Vector3.Zero;
            return v / v.Length();
        }

        public static Vector3 GetPoint(this Ray ray, float t) {
            return ray.Position + t * ray.Direction.Normalized();
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
            float cos = Vector3.Dot(a.Normalized(), b.Normalized());
            return MathHelper.ToDegrees((float) Math.Acos(cos));
        }

        public static Vector2 Project(this Rectangle rect, Vector2 p) {
            if (rect.Contains(p)) return p;
            p.X = Clamp(p.X, rect.Left, rect.Right);
            p.Y = Clamp(p.Y, rect.Top, rect.Bottom);
            return p;
        }


    }
}
