// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Engine.Physics {
    /// <summary>
    /// Conversions between the XNA- and Jitter-Framework.
    /// </summary>
    public static class Conversion {
        /// <summary>
        /// Convert a XNA-vector to a Jitter-vector
        /// </summary>
        /// <param name="vector">XNA-vector</param>
        /// <returns>Jitter-vector</returns>
        public static JVector ToJitterVector(Vector3 vector) {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Convert a Jitter-vector to a XNA-vector
        /// </summary>
        /// <param name="vector">Jitter-vector</param>
        /// <returns>XNA-vector</returns>
        public static Vector3 ToXnaVector(JVector vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Convert a XNA-matrix to a Jitter-matrix
        /// </summary>
        /// <param name="matrix">XNA-matrix</param>
        /// <returns>Jitter-matrix</returns>
        public static JMatrix ToJitterMatrix(Matrix matrix) {
            JMatrix result;
            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M13 = matrix.M13;
            result.M21 = matrix.M21;
            result.M22 = matrix.M22;
            result.M23 = matrix.M23;
            result.M31 = matrix.M31;
            result.M32 = matrix.M32;
            result.M33 = matrix.M33;
            return result;
        }

        /// <summary>
        /// Convert a Jitter-matrix to a XNA-matrix
        /// </summary>
        /// <param name="matrix">Jitter-matrix</param>
        /// <returns>XNA-matrix</returns>
        public static Matrix ToXnaMatrix(JMatrix matrix) {
            return new Matrix(matrix.M11,
                matrix.M12,
                matrix.M13,
                0.0f,
                matrix.M21,
                matrix.M22,
                matrix.M23,
                0.0f,
                matrix.M31,
                matrix.M32,
                matrix.M33,
                0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        public static Quaternion ToXnaQuaternion(JQuaternion quaternion) {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static JQuaternion ToJitterQuaternion(Quaternion quaternion) {
            return new JQuaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }
    }
}
