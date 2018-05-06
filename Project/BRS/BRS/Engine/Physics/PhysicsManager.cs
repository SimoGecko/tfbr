// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine.Physics.Colliders;
using BRS.Scripts.PlayerScripts;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BRS.Scripts.Managers;

namespace BRS.Engine.Physics {
    /// <summary>
    /// Updates the physics and takes care of the collision-events.
    /// </summary>
    public class PhysicsManager {

        #region Singleton

        // Singleton-instance
        private static PhysicsManager _instance;

        /// <summary>
        /// Singleton-getter
        /// </summary>
        public static PhysicsManager Instance => _instance ?? (_instance = new PhysicsManager());

        #endregion

        #region Properties and attributes

        private enum CollisionState { Propagate, SaveInList }

        // Current state of the collision-handling
        private CollisionState _status;

        // Collection of all colliders which are collected in a manual-collision-detection
        private List<Collider> _colliders;

        private const float Epsilon = 0.01f;

        /// <summary>
        /// Stores the physical world.
        /// </summary>
        public World World { get; }

        public bool IsActive { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the physics with the collision-setup
        /// </summary>
        private PhysicsManager() {
            CollisionSystem collision = new CollisionSystemPersistentSAP();

            // Initialization of physic-settings
            World = new World(collision) {
                AllowDeactivation = true,
                Gravity = new JVector(0, -20, 0)
            };

            // Event-handling
            World.Events.BodiesBeginCollide += Events_BodiesBeginCollide;
            World.Events.ContactCreated += Events_ContactCreated;
        }

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Start the physics-simulation from scratch.
        /// </summary>
        public void Start() {
            World.Clear();
        }

        /// <summary>
        /// Update the physics based on the time.
        /// </summary>
        /// <param name="gameTime">Current game-time</param>
        public void Update(GameTime gameTime) {
            if (!IsActive || !GameManager.GameActive) {
                return;
            }

            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (step > 1.0f / 100.0f) {
                step = 1.0f / 100.0f;
            }

            World.Step(step, false);
        }

        #endregion

        #region Event-handling

        /// <summary>
        /// Event as soon as two objects started the collision.
        /// </summary>
        /// <param name="arg1">Rigidbody 1</param>
        /// <param name="arg2">Rigidbody 2</param>
        private void Events_BodiesBeginCollide(RigidBody arg1, RigidBody arg2) {
            if (!IsActive || !GameManager.GameActive) {
                return;
            }

            Collider body1 = arg1 as Collider;
            Collider body2 = arg2 as Collider;

            if (Instance._status == CollisionState.SaveInList) {
                if (body1 != null && (body1.Tag is BodyTag && (BodyTag)body1.Tag != BodyTag.TestObject)) {
                    Instance._colliders.Add(body1);
                }

                if (body2 != null && (body2.Tag is BodyTag && (BodyTag)body2.Tag != BodyTag.TestObject)) {
                    Instance._colliders.Add(body2);
                }
            } else {
                if (body2 != null) body1?.GameObject.OnCollisionEnter(body2);
                if (body1 != null) body2?.GameObject.OnCollisionEnter(body1);
            }
        }

        /// <summary>
        /// Event as soon as a contact is created for a collision.
        /// This event contains more information about the collision.
        /// </summary>
        /// <param name="obj"></param>
        private void Events_ContactCreated(Contact obj) {
            if (!IsActive || !GameManager.GameActive) {
                return;
            }

            Collider body1 = obj.Body1 as Collider;
            Collider body2 = obj.Body2 as Collider;

            bool body1IsPlayer = body1?.GameObject?.tag == ObjectTag.Player;
            bool body2IsPlayer = body2?.GameObject?.tag == ObjectTag.Player;

            bool body1IsGround = body1?.GameObject?.tag == ObjectTag.Ground;
            bool body2IsGround = body2?.GameObject?.tag == ObjectTag.Ground;

            bool body1IsPureCollider = body1?.PureCollider == true;
            bool body2IsPureCollider = body2?.PureCollider == true;

            bool body1IsStatic = body1?.IsStatic == true;
            bool body2IsStatic = body2?.IsStatic == true;

            if (body1IsPlayer && !body2IsPureCollider && body2IsStatic && !body2IsGround) {
                SteerableCollider sc = body1 as SteerableCollider;
                HandlePlayerCollision(sc, body2, obj.Position2, -1 * obj.Normal);
            } else if (body2IsPlayer && !body1IsPureCollider && body1IsStatic && !body1IsGround) {
                SteerableCollider sc = body2 as SteerableCollider;
                HandlePlayerCollision(sc, body1, obj.Position1, obj.Normal);
            }
        }


        /// <summary>
        /// Raycast callback to define if a rigid body counts as collision for the ray.
        /// (Only static and non-pure-collider are counted as collisions)
        /// </summary>
        /// <param name="body">Body which is hit by the ray</param>
        /// <param name="normal">Normal of the collision</param>
        /// <param name="fraction">Fraction of the collision on the ray. (start + fraction * direction)</param>
        /// <returns>True if the body is considered as a hit.</returns>
        private bool RaycastCallback(RigidBody body, JVector normal, float fraction) {
            return body.IsStatic && body.PureCollider == false && !(body is SteerableCollider);
        }

        #endregion

        #region Manaual collision-detection


        /// <summary>
        /// Find all the colliders that intersect the given sphere.
        /// </summary>
        /// <param name="position">Absolute position of the sphere-center.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <param name="collisionTag">Only check for a specific tag.</param>
        /// <returns>List of all colliders which are contained in the given sphere.</returns>
        public static Collider[] OverlapSphere(Vector3 position, float radius, ObjectTag collisionTag = ObjectTag.Default) {
            SphereShape sphere = new SphereShape(radius);
            Collider rbSphere = new Collider(sphere) {
                Position = Conversion.ToJitterVector(position),
                PureCollider = true,
                Tag = BodyTag.TestObject
            };

            // Prepare the instance to handle the detected collisions correctly
            Instance._status = CollisionState.SaveInList;
            Instance._colliders = new List<Collider>();

            foreach (RigidBody rb in Instance.World.RigidBodies) {
                Collider c = rb as Collider;
                if (c != null && ((collisionTag != ObjectTag.Default && c.GameObject.tag != collisionTag) || !c.GameObject.active)) {
                    continue;
                }

                Instance.World.CollisionSystem.Detect(rbSphere, rb);
            }

            Instance._status = CollisionState.Propagate;

            return Instance._colliders.ToArray();
        }


        /// <summary>
        /// Detect a possible collision between the object and a planned end-position.
        /// </summary>
        /// <param name="rigidBody">Rigid-body to test for</param>
        /// <param name="start">Start of the test-ray</param>
        /// <param name="end">End of the test-ray</param>
        /// <returns>Position on the ray which is still possible to place the rigid-body without collision</returns>
        public Vector3 DetectCollision(RigidBody rigidBody, Vector3 start, Vector3 end) {
            JVector jStart = Conversion.ToJitterVector(start);
            JVector jEnd = Conversion.ToJitterVector(end);
            JVector result = DetectCollision(rigidBody, jStart, jEnd);
            return Conversion.ToXnaVector(result);
        }


        /// <summary>
        /// Detect a possible collision between the object and a planned end-position.
        /// </summary>
        /// <param name="rigidBody">Rigid-body to test for</param>
        /// <param name="start">Start of the test-ray</param>
        /// <param name="end">End of the test-ray</param>
        /// <returns>Position on the ray which is still possible to place the rigid-body without collision</returns>
        public JVector DetectCollision(RigidBody rigidBody, JVector start, JVector end) {
            JVector p = rigidBody.Position;
            JVector d = end - start;
            JVector d5 = 5 * d;

            Collider collider = rigidBody as Collider;

            RigidBody resBody;
            JVector hitNormal;
            float fraction;

            bool result = World.CollisionSystem.Raycast(p, d5, RaycastCallback,
                out resBody, out hitNormal, out fraction);

            if (result && resBody != rigidBody) {
                float maxLength = d.LengthSquared();
                JVector collisionAt = p + fraction * d5;
                JVector position = GetPosition(collider, collisionAt, hitNormal);

                float ncLength = (position - p).LengthSquared();

                //resBody.Tag = BodyTag.DrawMe;
                //Debug.Log(fraction);
                //Debug.Log((resBody as Collider).GameObject.name);
                //World.AddBody(new RigidBody(new CylinderShape(1.0f, .25f)) { Position = p, IsStatic = true, PureCollider = true });
                //World.AddBody(new RigidBody(new CylinderShape(2.5f, 0.5f)) { Position = end, IsStatic = true, PureCollider = true });
                ////World.AddBody(new RigidBody(new CylinderShape(1.0f, 1.0f)) { Position = p + d5, IsStatic = true, PureCollider = true });
                //World.AddBody(new RigidBody(new CylinderShape(1.0f, 1.0f)) { Position = collisionAt, IsStatic = true, PureCollider = true });

                return maxLength < ncLength ? end : position;
            }

            return end;
        }


        /// <summary>
        /// Get the position so that the object does not collide with the object anymore.
        /// Important: this is used raycasting checks, as this needs some more corner-cases.
        /// </summary>
        /// <param name="collider">The collider which is the player</param>
        /// <param name="collisionPoint">Point of the collision in the scene</param>
        /// <param name="hitNormal">Collision-normal of the object with which the player collided</param>
        /// <returns>Position which is possible for the collider so there is no collision.</returns>
        private JVector GetPosition(Collider collider, JVector collisionPoint, JVector hitNormal) {
            JVector bbSize = collider.BoundingBoxSize;
            GameObject gameObject = collider.GameObject;

            // Calculate the size of the forward and right vector based on the bounding-box.
            JVector forward = 0.5f * bbSize.Z * Conversion.ToJitterVector(gameObject.transform.Forward);
            JVector right = 0.5f * bbSize.X * Conversion.ToJitterVector(gameObject.transform.Right);
            JVector d = Conversion.ToJitterVector(gameObject.transform.Forward);

            if (hitNormal.IsZero()) {
                return bbSize.Z * -0.5f * Conversion.ToJitterVector(gameObject.transform.Forward);
            }

            // Correct directions
            TurnIntoDirectionOf(ref forward, hitNormal);
            TurnIntoDirectionOf(ref right, hitNormal);
            TurnIntoDirectionOf(ref d, hitNormal);

            // Calculate the projected length (half-length) of the player on the hit-normal.
            JVector lengthOnNormal = ProjectOn(forward, hitNormal);
            JVector widthOnNormal = ProjectOn(right, hitNormal);
            JVector projectedSize = lengthOnNormal + widthOnNormal;

            JVector margin = ProjectOn(projectedSize, d);
            JVector endPosition = collisionPoint + margin;

            // Check if it hit a corner and it could actually go closer
            JVector plane = new JVector(-hitNormal.Z, 0, hitNormal.X);
            TurnIntoDirectionOf(ref plane, -1 * forward);
            plane.Normalize();

            // Check if there is an intersection between the front-"plane" of the player and the collided object
            JVector intersection;

            JVector frontMiddle = endPosition + forward;
            JVector frontRight = endPosition + forward - right;
            JVector planeEnd = collisionPoint + MathHelper.Max(bbSize.X, bbSize.Z) * plane;

            if (DoIntersect(frontMiddle, frontRight, collisionPoint, planeEnd, out intersection)) {
                intersection.Y = collider.Position.Y;
                JVector colToIntersection = intersection - collisionPoint;

                JVector margin2 = ProjectOn(colToIntersection, d);

                margin = margin.LengthSquared() > margin2.LengthSquared() ? margin2 : margin;
                endPosition = collisionPoint + margin;
            }

            //// Todo: Visual debuggin might be removed in the end
            //PhysicsDrawer.Instance.ClearPointsToDraw();
            //PhysicsDrawer.Instance.AddPointToDraw(Conversion.ToXnaVector(collisionPoint));
            //PhysicsDrawer.Instance.AddPointToDraw(Conversion.ToXnaVector(endPosition));

            //return collider.Position;
            return endPosition;
        }


        /// <summary>
        /// Get the position so that the object does not collide with the object anymore.
        /// Important: this is used for the normal collision-handling.
        /// </summary>
        /// <param name="collider">The collider which is the player</param>
        /// <param name="collisionPoint">Point of the collision in the scene</param>
        /// <param name="hitNormal">Collision-normal of the object with which the player collided</param>
        /// <returns>Position which is possible for the collider so there is no collision.</returns>
        private JVector ProjectToNonCollision(Collider collider, JVector collisionPoint, JVector hitNormal) {
            JVector bbSize = collider.BoundingBoxSize;
            GameObject gameObject = collider.GameObject;

            // This should actually not happen
            if (hitNormal.IsZero()) {
                return bbSize.Z * -0.5f * Conversion.ToJitterVector(gameObject.transform.Forward);
            }

            // Calculate the size of the forward and right vector based on the bounding-box.
            JVector forward = 0.5f * bbSize.Z * Conversion.ToJitterVector(gameObject.transform.Forward);
            JVector right = 0.5f * bbSize.X * Conversion.ToJitterVector(gameObject.transform.Right);

            // Correct directions for calculations.
            TurnIntoDirectionOf(ref forward, hitNormal);
            TurnIntoDirectionOf(ref right, hitNormal);

            // Calculate the projected length (half-length) of the player on the hit-normal.
            JVector lengthOnNormal = ProjectOn(forward, hitNormal);
            JVector widthOnNormal = ProjectOn(right, hitNormal);
            JVector projectedSize = lengthOnNormal + widthOnNormal;

            // Get vector which gives the non-collided-part of the car projected on the hit-normal.
            JVector distColToPos = collisionPoint - collider.Position;

            // Correct directions for calculations.
            TurnIntoDirectionOf(ref projectedSize, hitNormal);
            TurnIntoDirectionOf(ref distColToPos, hitNormal);

            // Calculate the penetration-depth of the collision
            JVector nonCollidedSize = ProjectOn(distColToPos, hitNormal);
            JVector penetration = nonCollidedSize - projectedSize;

            // Calcualte the position for the player so it doesn't collide anymore
            return collider.Position - (1 + Epsilon) * penetration;
        }

        #endregion

        #region Collision handling

        private void HandlePlayerCollision(SteerableCollider player, Collider other, JVector collisionPoint, JVector normal) {
            //float angle = JVector.Dot(right, normal) > 0.0f ? -15.0f : 15.0f;
            float angle = 0.0f;
            angle += MathHelper.ToDegrees(player.RotationY);

            JVector forward = Conversion.ToJitterVector(player.GameObject.transform.Forward);

            // If the player is driving away of the wall there should be no collision-handling
            if (JVector.Dot(forward, normal) > 0.0f) {
                return;
            }

            JVector newPosition = ProjectToNonCollision(player, collisionPoint, normal);

            player.GameObject.GetComponent<Player>().SetCollisionState(other, Conversion.ToXnaVector(newPosition), angle);
            player.Position = newPosition;
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Project vector <paramref name="v"/> on vector <paramref name="u"/>
        /// </summary>
        /// <param name="v">Vector to project</param>
        /// <param name="u">Vector to project on</param>
        /// <returns>v projected on u</returns>
        private JVector ProjectOn(JVector v, JVector u) {
            return JVector.Dot(v, u) / u.LengthSquared() * u;
        }


        /// <summary>
        /// Reflect the <paramref name="input"/>-vector on the <paramref name="normal"/>-vector.
        /// </summary>
        /// <param name="input">Input-vector</param>
        /// <param name="normal">Reflector-normal</param>
        /// <returns>Vector which is the reflection of input on normal</returns>
        private JVector ReflectOnNormal(JVector input, JVector normal) {
            JVector normalNormalized = normal;
            normalNormalized.Normalize();
            JVector inputNormalized = input;
            inputNormalized.Normalize();

            return inputNormalized - 2 * JVector.Dot(inputNormalized, normalNormalized) * normalNormalized;
        }


        /// <summary>
        /// Turn the <paramref name="input"/>-vector in the same direction as the <paramref name="direction"/>.
        /// </summary>
        /// <param name="input">Input vector to adjust</param>
        /// <param name="direction">Given direction</param>
        private void TurnIntoDirectionOf(ref JVector input, JVector direction) {
            if (JVector.Dot(input, direction) < 0.0f) {
                input = -1 * input;
            }
        }


        /// <summary>
        /// Helper to calculate an intersection of two lines. Both lines are defined with two points on the line.
        /// </summary>
        /// <param name="startA">Point 1 on line A</param>
        /// <param name="endA">Point 2 on line A</param>
        /// <param name="startB">Point 1 on line B</param>
        /// <param name="endB">Point 2 on line B</param>
        /// <param name="intersection">Calculated intersection between the two lines</param>
        /// <returns>True if there is an intersection, false otherwise</returns>
        private bool DoIntersect(JVector startA, JVector endA, JVector startB, JVector endB, out JVector intersection) {
            // Parameter-form
            float a1 = endA.Z - startA.Z;
            float b1 = startA.X - endA.X;
            float c1 = a1 * startA.X + b1 * startA.Z;

            float a2 = endB.Z - startB.Z;
            float b2 = startB.X - endB.X;
            float c2 = a2 * startB.X + b2 * startB.Z;

            // Determinant of the vectors
            float det = a1 * b2 - a2 * b1;

            // If 0 => parallel => no intersection
            if (Math.Abs(det) < JMath.Epsilon) {
                intersection = JVector.Zero;
                return false;
            }

            // Otherwise calculate intersection
            float x = (b2 * c1 - b1 * c2) / det;
            float z = (a1 * c2 - a2 * c1) / det;
            intersection = new JVector(x, 0.0f, z);
            return true;
        }

        #endregion
    }
}
