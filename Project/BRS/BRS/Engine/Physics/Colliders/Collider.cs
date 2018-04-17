// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace BRS.Engine.Physics.Colliders {
    /// <summary>
    /// Represents the rigid body extension for the physics to have access to the game-object during simulation.
    /// </summary>
    public class Collider : RigidBody {
        public GameObject GameObject { get; set; }

        public float Length => BoundingBoxSize.X;
        public float Height => BoundingBoxSize.Y;
        public float Width => BoundingBoxSize.Z;
        public JVector BoundingBoxSize { get; }

        public bool IsAnimated { get; set; }

        public Collider(Shape shape) : base(shape) {
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
        }

        public Collider(Shape shape, Material material) : base(shape, material) {
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
        }

        public Collider(Shape shape, Material material, bool isParticle) : base(shape, material, isParticle) {
            BoundingBoxSize = BoundingBox.Max - BoundingBox.Min;
        }

        public override void PreStep(float timestep) {
            if (IsAnimated) {
                Position = Conversion.ToJitterVector(GameObject.transform.position) + CenterOfMass;
                Orientation = JMatrix.CreateFromQuaternion(Conversion.ToJitterQuaternion(GameObject.transform.rotation));
            }
        }

        public override void PostStep(float timestep) {
            if (!IsStatic && !IsAnimated) {
                GameObject.transform.position = Conversion.ToXnaVector(Position - CenterOfMass);
                GameObject.transform.rotation = Conversion.ToXnaQuaternion(JQuaternion.CreateFromMatrix(Orientation));
            }

            base.PostStep(timestep);
        }
    }
}
