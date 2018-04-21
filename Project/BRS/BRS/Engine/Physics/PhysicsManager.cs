// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

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
                HandlePlayerCollision(body1, body2, obj.Position2, -1 * obj.Normal);
            } else if (body2IsPlayer && !body1IsPureCollider && body1IsStatic && !body1IsGround) {
                HandlePlayerCollision(body2, body1, obj.Position1, obj.Normal);
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
        /// <param name="gameObject"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Vector3 DetectCollision(RigidBody rigidBody, GameObject gameObject, Vector3 start, Vector3 end) {
            JVector jStart = Conversion.ToJitterVector(start);
            JVector jEnd = Conversion.ToJitterVector(end);
            JVector result = DetectCollision(rigidBody, gameObject, jStart, jEnd);
            return Conversion.ToXnaVector(result);
        }

        public JVector DetectCollision(RigidBody rigidBody, GameObject gameObject, JVector start, JVector end) {
            JVector p = start + rigidBody.CenterOfMass;
            JVector d = end - start;

            JVector bbSize = rigidBody.BoundingBox.Max - rigidBody.BoundingBox.Min;

            RigidBody resBody;
            JVector hitNormal;
            float fraction;
            //ProjectOn(new JVector(2, 3, 1), new JVector(5, -2, 2));
            bool result = World.CollisionSystem.Raycast(p,
                d, RaycastCallback, out resBody,
                out hitNormal, out fraction);

            if (result && fraction <= 1.0f) {
                JVector collisionAt = p + fraction * d;
                return GetPosition(bbSize, gameObject, hitNormal, p, collisionAt);
            }

            //try {
            //    GameObject.Destroy((resBody as Collider).GameObject);
            //} catch {
            //}

            return end;
        }

        private JVector ProjectOn(JVector v, JVector u) {
            return JVector.Dot(v, u) / u.LengthSquared() * u;
        }

        private JVector GetPosition(JVector bbSize, GameObject gameObject, JVector hitNormal, JVector d, JVector end) {
            JVector forward = 0.5f * bbSize.X * Conversion.ToJitterVector(gameObject.transform.Forward);
            JVector right = 0.5f * bbSize.Z * Conversion.ToJitterVector(gameObject.transform.Right);
            d.Normalize();

            if (hitNormal.IsZero()) {
                return bbSize.Z * -0.5f * Conversion.ToJitterVector(gameObject.transform.Forward);
            }

            if (JVector.Dot(forward, hitNormal) < 0.0f) {
                forward = -1 * forward;
            }

            if (JVector.Dot(right, hitNormal) < 0.0f) {
                right = -1 * right;
            }

            JVector lengthOnNormal = ProjectOn(forward, hitNormal);
            JVector widthOnNormal = ProjectOn(right, hitNormal);
            JVector distanceToHit = lengthOnNormal + widthOnNormal;

            if (JVector.Dot(distanceToHit, d) < 0.0f) {
                distanceToHit = -1 * distanceToHit;
            }

            JVector margin = ProjectOn(distanceToHit, d);

            Debug.Log("Collision detected! ");
            Debug.Log("Margin: " + margin);
            if (margin.IsNaN()) {
                Debug.Log("Should not happen");
            }
            PhysicsDrawer.Instance.AddPointToDraw(Conversion.ToXnaVector(end - margin));

            return end - margin;
        }

        private JVector ReflectOnNormal(JVector input, JVector normal) {
            JVector normalNormalized = normal;
            normalNormalized.Normalize();
            JVector inputNormalized = input;
            inputNormalized.Normalize();

            return 2 * (inputNormalized - 2 * JVector.Dot(inputNormalized, normalNormalized) * normalNormalized);
        }


        #endregion

        #region Collision handling

        private void HandlePlayerCollision(Collider player, Collider other, JVector collisionPoint, JVector normal) {
            // First, calculate the new position based on the orientation of the car
            //JVector newPosition1 = GetPosition(player.BoundingBoxSize, player.GameObject, normal,
            //    Conversion.ToJitterVector(player.GameObject.transform.Forward), collisionPoint);
            JVector newPosition = collisionPoint + ReflectOnNormal(Conversion.ToJitterVector(player.GameObject.transform.Forward),
                                      normal);
            //newPosition -= newPosition1;
            newPosition += 0.1f * normal;

            // Second, check if the new position is not projected into another obstacle
            newPosition = DetectCollision(player, player.GameObject, player.Position, newPosition);

            // Calculate the torque for the car
            JVector right = Conversion.ToJitterVector(player.GameObject.transform.Right);
            //JVector bounce = newPosition - collisionPoint;

            float angle = JVector.Dot(right, normal) > 0.0f ? -45.0f : 45.0f;

            //Debug.Log(newPosition1);
            //Debug.Log(newPosition);
            player.GameObject.GetComponent<Player>().SetCollisionState(other, Conversion.ToXnaVector(newPosition), angle);
        }

        #endregion
    }
}
