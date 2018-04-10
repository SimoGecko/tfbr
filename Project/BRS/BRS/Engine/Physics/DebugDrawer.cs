using System;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {

    /// <summary>
    /// Draw axis aligned bounding boxes, points and lines.
    /// </summary>
    public class DebugDrawer : DrawableGameComponent, Jitter.IDebugDrawer {

        #region Properties and attributes

        /// <summary>
        /// Effect for drawing
        /// </summary>
        BasicEffect _basicEffect;

        /// <summary>
        /// Current color to draw
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Stores the vertex-positions for the triangles to draw.
        /// </summary>
        private VertexPositionColor[] _triangleList = new VertexPositionColor[99];

        /// <summary>
        /// Stores the vertex-positions for the lines to draw.
        /// </summary>
        private VertexPositionColor[] _lineList = new VertexPositionColor[50];

        /// <summary>
        /// Index of the line to draw.
        /// </summary>
        private int _lineIndex;

        /// <summary>
        /// Index of the triangle to draw.
        /// </summary>
        private int _triangleIndex;

        /// <summary>
        /// Stores all colors which are used for the drawings.
        /// </summary>
        public Color[] RandomColors { get; private set; }

        #endregion

        #region Getter and setter

        /// <summary>
        /// Override the effect for the drawings.
        /// </summary>
        /// <param name="basicEffect"></param>
        public void SetBasicEffect(BasicEffect basicEffect) {
            _basicEffect = basicEffect;
        }


        /// <summary>
        /// Set the specified element of the vector.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="index">Element to update</param>
        /// <param name="value">Value to update</param>
        private void SetElement(ref JVector v, int index, float value) {
            if (index == 0)
                v.X = value;
            else if (index == 1)
                v.Y = value;
            else if (index == 2)
                v.Z = value;
            else
                throw new ArgumentOutOfRangeException(nameof(index));
        }


        /// <summary>
        /// Get the specified element of the vector.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="index">Element to get the value of</param>
        /// <returns></returns>
        private float GetElement(JVector v, int index) {
            if (index == 0)
                return v.X;
            if (index == 1)
                return v.Y;
            if (index == 2)
                return v.Z;

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the debugdrawer.
        /// </summary>
        /// <param name="game">Instance of the game</param>
        public DebugDrawer(Game game)
            : base(game) {
            // Initialize randomly 20 colors
            Random rr = new Random();
            RandomColors = new Color[20];

            for (int i = 0; i < 20; i++) {
                RandomColors[i] = new Color((float)rr.NextDouble(), (float)rr.NextDouble(), (float)rr.NextDouble());
            }
        }

        #endregion

        #region Monogame-methods (DrawableGameComponent)

        /// <summary>
        /// Initialization of the class.
        /// </summary>
        public override void Initialize() {
            _basicEffect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };

            base.Initialize();
        }


        /// <summary>
        /// Draw method of monogame.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime) {
            Camera camera = Screen.Cameras[0];

            _basicEffect.View = camera.View;
            _basicEffect.Projection = camera.Proj;
            _basicEffect.Alpha = 0.1f;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes) {
                pass.Apply();

                if (_lineIndex > 0)
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineList, _lineList, 0, _lineIndex / 2);

                if (_triangleIndex > 0)
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleList, _triangleList, 0, _triangleIndex / 3);
            }

            _lineIndex = 0;
            _triangleIndex = 0;

            base.Draw(gameTime);
        }

        #endregion

        #region IDebugDraw

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void DrawLine(JVector start, JVector end) {
            DrawLine(start, end, Color.Black);
        }


        /// <summary>
        /// Draws a point.
        /// TODO: Not implemented
        /// </summary>
        /// <param name="pos"></param>
        public void DrawPoint(JVector pos) {
            //DrawPoint(pos, Color.Red);
        }


        /// <summary>
        /// Draws a triangle.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pos3"></param>
        public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3) {
            DrawTriangle(pos1, pos2, pos3, Color);
        }

        #endregion

        #region Draw-methods

        /// <summary>
        /// Draws a line on the screen.
        /// </summary>
        /// <param name="p0">Start position</param>
        /// <param name="p1">End position</param>
        /// <param name="color">Color of the line</param>
        public void DrawLine(JVector p0, JVector p1, Color color) {
            _lineIndex += 2;

            if (_lineIndex == _lineList.Length) {
                VertexPositionColor[] temp = new VertexPositionColor[_lineList.Length + 50];
                _lineList.CopyTo(temp, 0);
                _lineList = temp;
            }

            _lineList[_lineIndex - 2].Color = color;
            _lineList[_lineIndex - 2].Position = Conversion.ToXnaVector(p0);

            _lineList[_lineIndex - 1].Color = color;
            _lineList[_lineIndex - 1].Position = Conversion.ToXnaVector(p1);
        }


        /// <summary>
        /// Draws a triangle on the screen.
        /// </summary>
        /// <param name="p0">Vertex 1</param>
        /// <param name="p1">Vertex 2</param>
        /// <param name="p2">Vertex 3</param>
        /// <param name="color">Color of the triangle</param>
        public void DrawTriangle(JVector p0, JVector p1, JVector p2, Color color) {
            _triangleIndex += 3;

            if (_triangleIndex == _triangleList.Length) {
                VertexPositionColor[] temp = new VertexPositionColor[_triangleList.Length + 300];
                _triangleList.CopyTo(temp, 0);
                _triangleList = temp;
            }

            _triangleList[_triangleIndex - 2].Color = color;
            _triangleList[_triangleIndex - 2].Position = Conversion.ToXnaVector(p0);

            _triangleList[_triangleIndex - 1].Color = color;
            _triangleList[_triangleIndex - 1].Position = Conversion.ToXnaVector(p1);

            _triangleList[_triangleIndex - 3].Color = color;
            _triangleList[_triangleIndex - 3].Position = Conversion.ToXnaVector(p2);
        }


        /// <summary>
        /// Draws an axis-aligned-bounding-box on the screen forvisualization.
        /// </summary>
        /// <param name="from">Min-point</param>
        /// <param name="to">Max-point</param>
        /// <param name="color">Color of lines</param>
        public void DrawAabb(JVector from, JVector to, Color color) {
            JVector halfExtents = (to - from) * 0.5f;
            JVector center = (to + from) * 0.5f;

            JVector edgecoord = new JVector(1f, 1f, 1f), pa, pb;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 3; j++) {
                    pa = new JVector(edgecoord.X * halfExtents.X, edgecoord.Y * halfExtents.Y,
                        edgecoord.Z * halfExtents.Z);
                    pa += center;

                    int othercoord = j % 3;
                    SetElement(ref edgecoord, othercoord, GetElement(edgecoord, othercoord) * -1f);
                    pb = new JVector(edgecoord.X * halfExtents.X, edgecoord.Y * halfExtents.Y,
                        edgecoord.Z * halfExtents.Z);
                    pb += center;

                    DrawLine(pa, pb, color);
                }
                edgecoord = new JVector(-1f, -1f, -1f);
                if (i < 3)
                    SetElement(ref edgecoord, i, GetElement(edgecoord, i) * -1f);
            }
        }

        #endregion

    }
}
