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
using BRS.Engine.Physics.RigidBodies;

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
        public static PhysicsManager Instance {
            get {
                if (_instance == null) {
                    _instance = new PhysicsManager();
                }

                return _instance;
            }
        }

        #endregion

        #region Properties and attributes

        private enum CollisionState { Propagate, SaveInList }

        // Current state of the collision-handling
        private CollisionState _status;

        // Collection of all colliders which are collected in a manual-collision-detection
        private List<JRigidBody> _colliders;

        /// <summary>
        /// Stores the physical world.
        /// </summary>
        public World World { get; }

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
        /// Update the physics based on the time.
        /// </summary>
        /// <param name="gameTime">Current game-time</param>
        public void Update(GameTime gameTime) {
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
            JRigidBody body1 = arg1 as JRigidBody;
            JRigidBody body2 = arg2 as JRigidBody;

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
            JRigidBody body1 = obj.Body1 as JRigidBody;
            JRigidBody body2 = obj.Body2 as JRigidBody;

            bool body1IsPlayer = body1?.GameObject?.tag == ObjectTag.Player;
            bool body2IsPLayer = body2?.GameObject?.tag == ObjectTag.Player;

            bool body1IsPureCollider = body1?.PureCollider == true;
            bool body2IsPureCollider = body2?.PureCollider == true;

            if (body1IsPlayer && !body2IsPureCollider) {
                //body1.AddForce(obj.Normal * 100);
                //body1.LinearVelocity -= obj.Normal * 5;
                //Debug.Log(obj.Normal, "Force to body 1: ");
                body1.Position -= obj.Normal * 0.1f;
            } else if (body2IsPLayer && !body1IsPureCollider) {
                //body2.AddForce(obj.Normal * -100);
                //body2.LinearVelocity += obj.Normal * 5;
                //Debug.Log(obj.Normal, "Force to body 2: ");
                body2.Position += obj.Normal * 0.1f;
            }
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
        public static JRigidBody[] OverlapSphere(Vector3 position, float radius, ObjectTag collisionTag = ObjectTag.Default) {
            SphereShape sphere = new SphereShape(radius);
            JRigidBody rbSphere = new JRigidBody(sphere) {
                Position = Conversion.ToJitterVector(position),
                PureCollider = true,
                Tag = BodyTag.TestObject
            };

            // Prepare the instance to handle the detected collisions correctly
            Instance._status = CollisionState.SaveInList;
            Instance._colliders = new List<JRigidBody>();

            foreach (RigidBody rb in Instance.World.RigidBodies) {
                JRigidBody c = rb as JRigidBody;
                if (c != null && ((collisionTag != ObjectTag.Default && c.GameObject.tag != collisionTag) || !c.GameObject.active)) {
                    continue;
                }

                Instance.World.CollisionSystem.Detect(rbSphere, rb);
            }

            Instance._status = CollisionState.Propagate;

            return Instance._colliders.ToArray();
        }

        #endregion
    }
}
