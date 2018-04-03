﻿using System;
using System.Collections.Generic;
using BRS.Engine.Physics.Primitives3D;
using BRS.Load;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BRS.Engine.Physics {
    public class PhysicsManager {
        public static PhysicsManager Instance { get; private set; }

        public static void SetUpPhysics(DebugDrawer debugDrawer, Display display, GraphicsDevice graphicsDevice) {
            Instance = new PhysicsManager(debugDrawer, display, graphicsDevice);
        }

        public enum CollisionState { Propagate, SaveInList }

        private CollisionState Status { get; set; }
        private List<Collider> _colliders;

        // Stores the physical world
        public World World { private set; get; }

        // Reference for drawing additional information on the screen
        public DebugDrawer DebugDrawer { private set; get; }

        // Written debug information
        public Display Display { private set; get; }

        private int _activeBodies;
        public BasicEffect BasicEffect { private set; get; }

        private GeometricPrimitive[] _primitives = new GeometricPrimitive[5];

        private bool _doDrawings = false;

        /// <summary>
        /// Initialize the physics with the collision-setup
        /// </summary>
        private PhysicsManager(DebugDrawer debugDrawer, Display display, GraphicsDevice graphicsDevice) {
            CollisionSystem collision = new CollisionSystemPersistentSAP();

            World = new World(collision);
            World.AllowDeactivation = true;
            World.Gravity = new JVector(0, -20, 0);
            //World.ContactSettings.BreakThreshold = 0.5f;
            //World.ContactSettings.AllowedPenetration = -0.1f;
            //World.ContactSettings.MinimumVelocity = 2.0f;

            World.Events.BodiesBeginCollide += Events_BodiesBeginCollide;
            World.Events.ContactCreated += Events_ContactCreated;

            DebugDrawer = debugDrawer;
            Display = display;

            _primitives[(int)PrimitiveTypes.Box] = new BoxPrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Capsule] = new CapsulePrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Cone] = new ConePrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Cylinder] = new CylinderPrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Sphere] = new SpherePrimitive(graphicsDevice);
            //_primitives[(int)PrimitiveTypes.Convex] = new ConvexHullPrimitive(graphicsDevice);

            BasicEffect = new BasicEffect(graphicsDevice);
            BasicEffect.EnableDefaultLighting();
            BasicEffect.PreferPerPixelLighting = true;
        }

        public void Update(GameTime gameTime) {
            UpdateDisplayText(gameTime);

            if (Input.GetKeyDown(Keys.F1)) {
                _doDrawings = !_doDrawings;
            }

            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (step > 1.0f / 100.0f) {
                step = 1.0f / 100.0f;
            }

            World.Step(step, false);
        }

        /// <summary>
        /// Find all the colliders that intersect the given sphere.
        /// </summary>
        /// <param name="position">Absolute position of the sphere-center.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <param name="collisionTag">Only check for a specific tag.</param>
        /// <returns>List of all colliders which are contained in the given sphere.</returns>
        public static Collider[] OverlapSphere(Vector3 position, float radius, ObjectTag collisionTag = ObjectTag.Default) {
            SphereShape sphere = new SphereShape(radius);
            RigidBody rbSphere = new RigidBody(sphere) {
                Position = Conversion.ToJitterVector(position),
                PureCollider = true
            };

            // Prepare the instance to handle the detected collisions correctly
            Instance.Status = CollisionState.SaveInList;
            Instance._colliders = new List<Collider>();

            foreach (RigidBody rb in Instance.World.RigidBodies) {
                Collider c = rb as Collider;
                if (c != null && ((collisionTag != ObjectTag.Default && c.GameObject.tag != collisionTag) || !c.GameObject.active)) {
                    continue;
                }

                // Todo: this yields a problem
                Instance.World.CollisionSystem.Detect(rbSphere, rb);
            }

            Instance.Status = CollisionState.Propagate;


            //foreach (Collider c in Collider.allcolliders) { // TODO implement more efficient method (prune eg Octree)
            //    if ((collisionTag != ObjectTag.Default && c.gameObject.tag != collisionTag) || !c.gameObject.active) continue;

            //    if (c.Intersects(sphere)) result.Add(c);
            //}
            return Instance._colliders.ToArray();
        }

        public void Draw(Camera camera) {
            if (_doDrawings == false) {
                return;
            }

            BasicEffect.View = camera.View;
            BasicEffect.Projection = camera.Proj;

            BasicEffect.PreferPerPixelLighting = true;
            BasicEffect.LightingEnabled = true;
            _activeBodies = 0;

            // Draw all shapes
            foreach (RigidBody body in World.RigidBodies) {
                if (body.IsActive) _activeBodies++;
                if (body.Tag is int || body.IsParticle) continue;
                AddBodyToDrawList(body);
            }


            foreach (GeometricPrimitive prim in _primitives) {
                prim.Draw(BasicEffect);
            }

            BasicEffect.PreferPerPixelLighting = false;
            BasicEffect.LightingEnabled = false;
            DebugDrawer.SetBasicEffect(BasicEffect);
            //DrawIslands();
            DrawDebugInfo();
        }

        private void Events_BodiesBeginCollide(RigidBody arg1, RigidBody arg2) {
            Collider body1 = arg1 as Collider;
            Collider body2 = arg2 as Collider;

            if (Instance.Status == CollisionState.SaveInList) {
                if (body1 != null) {
                    Instance._colliders.Add(body1);
                }

                if (body2 != null) {
                    Instance._colliders.Add(body2);
                }
            } else {
                body1?.GameObject.OnCollisionEnter(body2);
                body2?.GameObject.OnCollisionEnter(body1);
            }
        }

        private void Events_ContactCreated(Contact obj) {
            Collider body1 = obj.Body1 as Collider;
            Collider body2 = obj.Body2 as Collider;

            bool body1IsPlayer = body1?.GameObject.tag == ObjectTag.Player;
            bool body2IsPLayer = body2?.GameObject.tag == ObjectTag.Player;

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

        /// <summary>
        /// 
        /// </summary>
        public void DrawDebugInfo() {
            int cc = 0;

            foreach (Constraint constr in World.Constraints) {
                constr.DebugDraw(DebugDrawer);
            }

            foreach (RigidBody body in World.RigidBodies) {
                DebugDrawer.Color = DebugDrawer.RandomColors[cc % DebugDrawer.RandomColors.Length];
                body.DebugDraw(DebugDrawer);
                cc++;
            }
        }

        //public void DrawIslands() {
        //    foreach (CollisionIsland island in World.Islands) {
        //        JBBox box = JBBox.SmallBox;

        //        foreach (RigidBody body in island.Bodies) {
        //            box = JBBox.CreateMerged(box, body.BoundingBox);
        //        }

        //        DebugDrawer.DrawAabb(box.Min, box.Max, island.IsActive() ? Color.Green : Color.Yellow);
        //    }
        //}

        #region add draw matrices to the different primitives
        private void AddShapeToDrawList(Shape shape, JMatrix ori, JVector pos) {
            GeometricPrimitive primitive = null;
            Matrix scaleMatrix = Matrix.Identity;

            if (shape is BoxShape) {
                primitive = _primitives[(int)PrimitiveTypes.Box];
                scaleMatrix = Matrix.CreateScale(Conversion.ToXnaVector((shape as BoxShape).Size));
            } else if (shape is SphereShape) {
                primitive = _primitives[(int)PrimitiveTypes.Sphere];
                scaleMatrix = Matrix.CreateScale((shape as SphereShape).Radius);
            } else if (shape is CylinderShape) {
                primitive = _primitives[(int)PrimitiveTypes.Cylinder];
                CylinderShape cs = shape as CylinderShape;
                scaleMatrix = Matrix.CreateScale(cs.Radius, cs.Height, cs.Radius);
            } else if (shape is CapsuleShape) {
                primitive = _primitives[(int)PrimitiveTypes.Capsule];
                CapsuleShape cs = shape as CapsuleShape;
                scaleMatrix = Matrix.CreateScale(cs.Radius * 2, cs.Length, cs.Radius * 2);
            } else if (shape is ConeShape) {
                ConeShape cs = shape as ConeShape;
                scaleMatrix = Matrix.CreateScale(cs.Radius, cs.Height, cs.Radius);
                primitive = _primitives[(int)PrimitiveTypes.Cone];
            } else if (shape is ConvexHullShape) {
                ConvexHullShape cs = shape as ConvexHullShape;
                primitive = _primitives[(int)PrimitiveTypes.Box];
                scaleMatrix = Matrix.CreateScale(Conversion.ToXnaVector(cs.BoundingBox.Max - cs.BoundingBox.Min));
            }

            if (primitive != null)
                primitive.AddWorldMatrix(scaleMatrix * Conversion.ToXnaMatrix(ori) *
                                         Matrix.CreateTranslation(Conversion.ToXnaVector(pos)));
        }

        private void AddBodyToDrawList(RigidBody rb) {
            if (rb.Tag is BodyTag && ((BodyTag)rb.Tag) == BodyTag.DontDrawMe) return;

            Collider c = rb as Collider;
            //if (c != null && (c.GameObject.tag == ObjectTag.Ground || c.GameObject.tag == ObjectTag.Obstacle)) return;

            bool isCompoundShape = (rb.Shape is CompoundShape);

            if (!isCompoundShape) {
                AddShapeToDrawList(rb.Shape, rb.Orientation, rb.Position);
            } else {
                CompoundShape cShape = rb.Shape as CompoundShape;
                JMatrix orientation = rb.Orientation;
                JVector position = rb.Position;

                foreach (var ts in cShape.Shapes) {
                    JVector pos = ts.Position;
                    JMatrix ori = ts.Orientation;

                    JVector.Transform(ref pos, ref orientation, out pos);
                    JVector.Add(ref pos, ref position, out pos);

                    JMatrix.Multiply(ref ori, ref orientation, out ori);

                    AddShapeToDrawList(ts.Shape, ori, pos);
                }

            }

        }
        #endregion

        #region update the display text informations

        private float _accUpdateTime;

        private void UpdateDisplayText(GameTime time) {
            _accUpdateTime += (float)time.ElapsedGameTime.TotalSeconds;
            if (_accUpdateTime < 0.1f) return;

            _accUpdateTime = 0.0f;

            int contactCount = 0;

            foreach (Arbiter ar in World.ArbiterMap.Arbiters) {
                contactCount += ar.ContactList.Count;
            }

            Display.DisplayText[1] = World.CollisionSystem.ToString();

            //Display.DisplayText[0] = "Current Scene: " + PhysicScenes[currentScene].ToString();
            //
            Display.DisplayText[2] = "Arbitercount: " + World.ArbiterMap.Arbiters.Count + ";" + " Contactcount: " + contactCount;
            Display.DisplayText[3] = "Islandcount: " + World.Islands.Count;
            Display.DisplayText[4] = "Bodycount: " + World.RigidBodies.Count + " (" + _activeBodies + ")";
            //Display.DisplayText[5] = (multithread) ? "Multithreaded" : "Single Threaded";

            int entries = (int)World.DebugType.Num;
            double total = 0;

            for (int i = 0; i < entries; i++) {
                World.DebugType type = (World.DebugType)i;

                Display.DisplayText[8 + i] = type + ": " + World.DebugTimes[i].ToString("0.00");

                total += World.DebugTimes[i];
            }

            Display.DisplayText[8 + entries] = "------------------------------";
            Display.DisplayText[9 + entries] = "Total Physics Time: " + total.ToString("0.00");
            Display.DisplayText[10 + entries] = "Physics Framerate: " + (1000.0d / total).ToString("0") + " fps";

            Display.DisplayText[6] = "gen0: " + GC.CollectionCount(0).ToString() +
                "  gen1: " + GC.CollectionCount(1).ToString() +
                "  gen2: " + GC.CollectionCount(2).ToString();
        }
        #endregion
    }
}
