// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine.Physics.Colliders;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BRS.Scripts.PlayerScripts;

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
            if (!IsActive) {
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
            if (!IsActive) {
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
                body1?.GameObject.OnCollisionEnter(body2);
                body2?.GameObject.OnCollisionEnter(body1);
            }
        }

        /// <summary>
        /// Event as soon as a contact is created for a collision.
        /// This event contains more information about the collision.
        /// </summary>
        /// <param name="obj"></param>
        private void Events_ContactCreated(Contact obj) {
            if (!IsActive) {
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

        private bool RaycastCallback(RigidBody body, JVector normal, float fraction) {
            return body.IsStatic && body.PureCollider == false;
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
        /// 
        /// </summary>
        /// <param name="rigidBody"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Vector3 DetectCollision(RigidBody rigidBody, Vector3 start, Vector3 end) {
            JVector jStart = Conversion.ToJitterVector(start);
            JVector jEnd = Conversion.ToJitterVector(end);
            JVector result = DetectCollision(rigidBody, jStart, jEnd);
            return Conversion.ToXnaVector(result);
        }

        public JVector DetectCollision(RigidBody rigidBody, JVector start, JVector end) {
            JVector p = rigidBody.Position;
            JVector d = end - start;

            Collider collider = rigidBody as Collider;

            RigidBody resBody;
            JVector hitNormal;
            float fraction;
            //ProjectOn(new JVector(2, 3, 1), new JVector(5, -2, 2));
            bool result = World.CollisionSystem.Raycast(p,
                d, RaycastCallback, out resBody,
                out hitNormal, out fraction);

            if (result && fraction <= 1.0f) {
                JVector collisionAt = p + fraction * d;
                return GetPosition(collider, collisionAt, hitNormal);
            }

            //try {
            //    GameObject.Destroy((resBody as Collider).GameObject);
            //} catch {
            //}

            return end;
        }

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

            // Direction of the forward-vector into the same as the hit-normal.
            if (JVector.Dot(forward, hitNormal) < 0.0f) {
                forward = -1 * forward;
            }

            // Direction of the right-vector into the same as the hit-normal.
            if (JVector.Dot(right, hitNormal) < 0.0f) {
                right = -1 * right;
            }

            // Direction of the d-vector into the same as the hit-normal.
            if (JVector.Dot(d, hitNormal) < 0.0f) {
                d = -1 * d;
            }

            // Calculate the projected length (half-length) of the player on the hit-normal.
            JVector lengthOnNormal = ProjectOn(forward, hitNormal);
            JVector widthOnNormal = ProjectOn(right, hitNormal);
            JVector projectedSize = lengthOnNormal + widthOnNormal;

            //// Get vector which gives the non-collided-part of the car projected on the hit-normal.
            //JVector distColToPos = collisionPoint - collider.Position;

            //if (JVector.Dot(distColToPos, hitNormal) < 0.0f) {
            //    distColToPos = -1 * distColToPos;
            //}

            //// Calculate the penetration-depth of the collision
            //JVector nonCollidedSize = ProjectOn(distColToPos, hitNormal);
            //JVector penetration = nonCollidedSize - projectedSize;

            JVector margin = ProjectOn(projectedSize, d);

            //Debug.Log("Collision detected! ");
            //Debug.Log("Margin: " + margin);
            //if (margin.IsNaN()) {
            //    Debug.Log("Should not happen");
            //}
            PhysicsDrawer.Instance.AddPointToDraw(Conversion.ToXnaVector(collisionPoint + margin));

            return collisionPoint + margin;
        }

        /// <summary>
        /// Get the position so that the object does not collide with the object anymore.
        /// </summary>
        /// <param name="collider">The collider which is the player</param>
        /// <param name="collisionPoint">Point of the collision in the scene</param>
        /// <param name="hitNormal">Collision-normal of the object with which the player collided</param>
        /// <returns></returns>
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

            // Direction of the forward-vector into the same as the hit-normal.
            if (JVector.Dot(forward, hitNormal) < 0.0f) {
                forward = -1 * forward;
            }

            // Direction of the right-vector into the same as the hit-normal.
            if (JVector.Dot(right, hitNormal) < 0.0f) {
                right = -1 * right;
            }

            // Calculate the projected length (half-length) of the player on the hit-normal.
            JVector lengthOnNormal = ProjectOn(forward, hitNormal);
            JVector widthOnNormal = ProjectOn(right, hitNormal);
            JVector projectedSize = lengthOnNormal + widthOnNormal;

            // Direction of the projected-vector into the same as the hit-normal.
            if (JVector.Dot(projectedSize, hitNormal) < 0.0f) {
                projectedSize = -1 * projectedSize;
            }

            // Get vector which gives the non-collided-part of the car projected on the hit-normal.
            JVector distColToPos = collisionPoint - collider.Position;

            if (JVector.Dot(distColToPos, hitNormal) < 0.0f) {
                distColToPos = -1 * distColToPos;
            }

            // Calculate the penetration-depth of the collision
            JVector nonCollidedSize = ProjectOn(distColToPos, hitNormal);
            JVector penetration = nonCollidedSize - projectedSize;

            // Todo: Visual debuggin might be removed in the end
            PhysicsDrawer.Instance.AddPointToDraw(Conversion.ToXnaVector(collider.Position - (1 + Epsilon) * penetration));

            // Calcualte the position for the player so it doesn't collide anymore
            return collider.Position - (1 + Epsilon) * penetration;
        }

        #endregion

        #region Collision handling

        private void HandlePlayerCollision(SteerableCollider player, Collider other, JVector collisionPoint, JVector normal) {
            // First, calculate the new position based on the orientation of the car
            //// First try
            //JVector newPosition1 = GetPosition(player.BoundingBoxSize, player.GameObject, normal,
            //    Conversion.ToJitterVector(player.GameObject.transform.Forward), collisionPoint);

            //// Second try
            //JVector newPosition = collisionPoint + ReflectOnNormal(Conversion.ToJitterVector(player.GameObject.transform.Forward),
            //                          normal);
            ////newPosition -= newPosition1;
            //newPosition += 0.1f * normal;

            //// Second, check if the new position is not projected into another obstacle
            //newPosition = DetectCollision(player, player.GameObject, player.Position, newPosition);

            //// Calculate the torque for the car
            //JVector right = Conversion.ToJitterVector(player.GameObject.transform.Right);
            ////JVector bounce = newPosition - collisionPoint;

            //float angle = JVector.Dot(right, normal) > 0.0f ? -15.0f : 15.0f;
            float angle = 0.0f;
            angle += MathHelper.ToDegrees(player.RotationY);

            JVector forward = Conversion.ToJitterVector(player.GameObject.transform.Forward);

            if (JVector.Dot(forward, normal) > 0.0f) {
                return;
            }
            JVector newPosition = ProjectToNonCollision(player, collisionPoint, normal);

            //Debug.Log(newPosition1);
            //Debug.Log(newPosition);
            player.GameObject.GetComponent<Player>().SetCollisionState(other, Conversion.ToXnaVector(newPosition), angle);
            player.Position = newPosition;
            //player.RotationY = MathHelper.ToRadians(angle);
            //player.Orientation = JMatrix.CreateRotationY(MathHelper.ToRadians(angle));
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

        #endregion
    }
}
