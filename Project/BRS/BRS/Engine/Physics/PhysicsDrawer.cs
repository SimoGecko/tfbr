// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics.Primitives3D;
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
    /// <summary>
    /// This class is only for visual-debugging purpose and can be removed in a later state.
    /// Todo: (for Andy) Remove when not used anymore
    /// </summary>
    class PhysicsDrawer {

        #region Singleton

        /// <summary>
        /// Singleton-instance
        /// </summary>
        public static PhysicsDrawer Instance { get; private set; }

        /// <summary>
        /// Initialize the singleton.
        /// </summary>
        /// <param name="game">Instance of the game</param>
        /// <param name="graphicsDevice">Instance of the graphics-device</param>
        public static void Initialize(Game1 game, GraphicsDevice graphicsDevice) {
            Instance = new PhysicsDrawer(game, graphicsDevice);
        }

        #endregion

        #region Properties and attributes

        // Written debug information
        private readonly Display _display;

        // Reference for drawing additional information on the screen
        private readonly DebugDrawer _debugDrawer;

        // Effect of the camera-drawing
        private readonly BasicEffect _basicEffect;

        // Primitives used for drawing the debug-information in the 3D-space
        private readonly GeometricPrimitive[] _primitives = new GeometricPrimitive[5];

        // Direct reference to the physics-world
        private World World => PhysicsManager.Instance.World;

        /// <summary>
        /// Flag if physics-debug-information is drawn or not.
        /// </summary>
        public bool DoDrawings { get; private set; }

        private const int CollisionPointsToDraw = 20;
        private readonly List<Vector3> _pointsToDraw = new List<Vector3>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize drawer
        /// </summary>
        /// <param name="game">Instance of the game</param>
        /// <param name="graphicsDevice">Instance of the graphics-device</param>
        private PhysicsDrawer(Game1 game, GraphicsDevice graphicsDevice) {
            // Add the display-information and debug-drawer to the components
            _display = new Display(game);
            _debugDrawer = new DebugDrawer(game);

            game.Components.Add(_display);
            //game.Components.Add(_debugDrawer);

            // Initialize the primitives with the graphics device for drawing on the screen
            _primitives[(int)PrimitiveTypes.Box] = new BoxPrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Capsule] = new CapsulePrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Cone] = new ConePrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Cylinder] = new CylinderPrimitive(graphicsDevice);
            _primitives[(int)PrimitiveTypes.Sphere] = new SpherePrimitive(graphicsDevice);

            // Initialize the effect which is used for the drawing
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.EnableDefaultLighting();
            _basicEffect.PreferPerPixelLighting = true;
        }

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Handle input for switching the drawing on and off and update the instances
        /// with the needed information.
        /// </summary>
        /// <param name="gameTime">Current game-time</param>
        public void Update(GameTime gameTime) {
            if (Input.GetKeyDown(Keys.F1)) {
                DoDrawings = !DoDrawings;
            }

            _display.Update(gameTime);
            _display.UpdatePhysicsInformation(gameTime, PhysicsManager.Instance.World);
        }

        /// <summary>
        /// Draw the information on the camera
        /// </summary>
        /// <param name="camera">Current camera of the split-screen</param>
        public void Draw(Camera camera) {
            if (DoDrawings == false) {
                return;
            }

            _basicEffect.View = camera.View;
            _basicEffect.Projection = camera.Proj;

            _basicEffect.PreferPerPixelLighting = true;
            _basicEffect.LightingEnabled = true;

            foreach (Vector3 vector3 in _pointsToDraw) {
                Gizmos.DrawWireSphere(vector3, 1.0f);
            }

            // Draw all shapes
            foreach (RigidBody body in World.RigidBodies) {
                if (body.Tag is int || body.IsParticle) continue;
                AddBodyToDrawList(body);
            }

            foreach (GeometricPrimitive prim in _primitives) {
                prim.Draw(_basicEffect);
            }

            _basicEffect.PreferPerPixelLighting = false;
            _basicEffect.LightingEnabled = false;
            _debugDrawer.SetBasicEffect(_basicEffect);

            //DrawIslands();
            DrawDebugInfo();
        }

        #endregion

        #region Drawing-methods

        /// <summary>
        /// Draw the debug information of each constraint and rigid-body.
        /// </summary>
        private void DrawDebugInfo() {
            int cc = 0;

            foreach (Constraint constr in World.Constraints) {
                constr.DebugDraw(_debugDrawer);
            }

            foreach (RigidBody body in World.RigidBodies) {
                _debugDrawer.Color = _debugDrawer.RandomColors[cc % _debugDrawer.RandomColors.Length];
                body.DebugDraw(_debugDrawer);
                cc++;
            }
        }

        /// <summary>
        /// Draw the islands of the physics.
        /// </summary>
        public void DrawIslands() {
            foreach (CollisionIsland island in World.Islands) {
                JBBox box = JBBox.SmallBox;

                foreach (RigidBody body in island.Bodies) {
                    box = JBBox.CreateMerged(box, body.BoundingBox);
                }

                _debugDrawer.DrawAabb(box.Min, box.Max, island.IsActive() ? Color.Green : Color.Yellow);
            }
        }

        /// <summary>
        /// Add shape for debug-information on the screen
        /// </summary>
        /// <param name="shape">Shape of the object</param>
        /// <param name="ori">Orientation</param>
        /// <param name="pos">Position</param>
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

            if (primitive != null) {
                primitive.AddWorldMatrix(scaleMatrix * Conversion.ToXnaMatrix(ori) *
                                        Matrix.CreateTranslation(Conversion.ToXnaVector(pos)));
            }
        }

        /// <summary>
        /// Add rigid-body to the draw-list to draw on the screen.
        /// Only rigid-bodies with "Tag" = "DrawMe" are drawn.
        /// </summary>
        /// <param name="rb">Rigidbody</param>
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

        #region Other methods

        public void AddPointToDraw(Vector3 point) {
            _pointsToDraw.Add(point);

            if (_pointsToDraw.Count > CollisionPointsToDraw) {
                _pointsToDraw.RemoveAt(0);
            }
        }

        #endregion
    }
}
