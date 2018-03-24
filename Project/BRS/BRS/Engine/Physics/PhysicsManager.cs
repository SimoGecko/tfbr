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

namespace BRS.Engine.Physics {
    public class PhysicsManager {
        // Stores the physical world
        public World World { private set; get; }

        // Reference for drawing additional information on the screen
        public DebugDrawer DebugDrawer { private set; get; }

        // Written debug information
        public Display Display { private set; get; }

        private int _activeBodies;
        public BasicEffect BasicEffect { private set; get; }

        private Primitives3D.GeometricPrimitive[] _primitives = new Primitives3D.GeometricPrimitive[5];

        /// <summary>
        /// Initialize the physics with the collision-setup
        /// </summary>
        public PhysicsManager(DebugDrawer debugDrawer, Display display, GraphicsDevice graphicsDevice) {
            CollisionSystem collision = new CollisionSystemPersistentSAP();

            World = new World(collision);
            World.AllowDeactivation = true;
            World.Gravity = new JVector(0, -20, 0);

            World.Events.BodiesBeginCollide += Events_BodiesBeginCollide;

            DebugDrawer = debugDrawer;
            Display = display;

            _primitives[(int)PrimitiveTypes.Box] = new Primitives3D.BoxPrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Capsule] = new Primitives3D.CapsulePrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Cone] = new Primitives3D.ConePrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Cylinder] = new Primitives3D.CylinderPrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Sphere] = new Primitives3D.SpherePrimitive(graphicsDevice);
            //_primitives[(int)PrimitiveTypes.Convex] = new ConvexHullPrimitive(graphicsDevice);

            BasicEffect = new BasicEffect(graphicsDevice);
            BasicEffect.EnableDefaultLighting();
            BasicEffect.PreferPerPixelLighting = true;
        }

        public void Update(GameTime gameTime) {
            UpdateDisplayText(gameTime);

            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (step > 1.0f / 100.0f) {
                step = 1.0f / 100.0f;
            }

            World.Step(step, true);
        }

        public void Draw() {
            Camera camera = Screen.cameras[0];
            BasicEffect.GraphicsDevice.Viewport = Screen.fullViewport;
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


            foreach (Primitives3D.GeometricPrimitive prim in _primitives) {
                prim.Draw(BasicEffect);
            }

            BasicEffect.PreferPerPixelLighting = false;
            BasicEffect.LightingEnabled = false;
            DebugDrawer.SetBasicEffect(BasicEffect);
            DrawIslands();
            DrawDebugInfo();
        }

        private void Events_BodiesBeginCollide(RigidBody arg1, RigidBody arg2) {
            //GameObject body1 = arg1 as GameObject;
            //GameObject body2 = arg2 as GameObject;

            //if (body1 == null || body2 == null) {
            //    return;
            //}

            //GameObject player;
            //GameObject gameObject;

            //if (body1.Type == ObjectType.Player || body2.Type == ObjectType.Player) {

            //}
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

        public void DrawIslands() {
            foreach (CollisionIsland island in World.Islands) {
                JBBox box = JBBox.SmallBox;

                foreach (RigidBody body in island.Bodies) {
                    box = JBBox.CreateMerged(box, body.BoundingBox);
                }

                DebugDrawer.DrawAabb(box.Min, box.Max, island.IsActive() ? Color.Green : Color.Yellow);
            }
        }

        #region add draw matrices to the different primitives
        private void AddShapeToDrawList(Shape shape, JMatrix ori, JVector pos) {
            Primitives3D.GeometricPrimitive primitive = null;
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

#if (WINDOWS)
                    Display.DisplayText[6] = "gen0: " + GC.CollectionCount(0).ToString() +
                        "  gen1: " + GC.CollectionCount(1).ToString() +
                        "  gen2: " + GC.CollectionCount(2).ToString();
#endif

        }
        #endregion
    }
}
