using System;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine.Physics {

    /// <summary>
    /// Draw axis aligned bounding boxes, points and lines.
    /// </summary>
    public class DebugDrawer : DrawableGameComponent, Jitter.IDebugDrawer {
        BasicEffect _basicEffect;
        public void SetBasicEffect(BasicEffect basicEffect) {
            _basicEffect = basicEffect;
        }

        public Color[] RandomColors { private set; get; }

        public DebugDrawer(Game game)
            : base(game) {

            Random rr = new Random();
            RandomColors = new Color[20];

            for (int i = 0; i < 20; i++) {
                RandomColors[i] = new Color((float)rr.NextDouble(), (float)rr.NextDouble(), (float)rr.NextDouble());
            }
        }

        public override void Initialize() {
            base.Initialize();
            _basicEffect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };
        }

        public void DrawLine(JVector p0, JVector p1, Color color) {
            _lineIndex += 2;

            if (_lineIndex == LineList.Length) {
                VertexPositionColor[] temp = new VertexPositionColor[LineList.Length + 50];
                LineList.CopyTo(temp, 0);
                LineList = temp;
            }

            LineList[_lineIndex - 2].Color = color;
            LineList[_lineIndex - 2].Position = Conversion.ToXnaVector(p0);

            LineList[_lineIndex - 1].Color = color;
            LineList[_lineIndex - 1].Position = Conversion.ToXnaVector(p1);
        }

        public void DrawTriangle(JVector p0, JVector p1, JVector p2, Color color) {
            _triangleIndex += 3;

            if (_triangleIndex == TriangleList.Length) {
                VertexPositionColor[] temp = new VertexPositionColor[TriangleList.Length + 300];
                TriangleList.CopyTo(temp, 0);
                TriangleList = temp;
            }

            TriangleList[_triangleIndex - 2].Color = color;
            TriangleList[_triangleIndex - 2].Position = Conversion.ToXnaVector(p0);

            TriangleList[_triangleIndex - 1].Color = color;
            TriangleList[_triangleIndex - 1].Position = Conversion.ToXnaVector(p1);

            TriangleList[_triangleIndex - 3].Color = color;
            TriangleList[_triangleIndex - 3].Position = Conversion.ToXnaVector(p2);
        }

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

        private float GetElement(JVector v, int index) {
            if (index == 0)
                return v.X;
            if (index == 1)
                return v.Y;
            if (index == 2)
                return v.Z;

            throw new ArgumentOutOfRangeException(nameof(index));
        }

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

        public VertexPositionColor[] TriangleList = new VertexPositionColor[99];
        public VertexPositionColor[] LineList = new VertexPositionColor[50];

        private int _lineIndex = 0;
        private int _triangleIndex = 0;

        public override void Draw(GameTime gameTime) {
            Camera camera = Screen.cameras[0];

            _basicEffect.View = camera.View;
            _basicEffect.Projection = camera.Proj;
            _basicEffect.Alpha = 0.1f;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes) {
                pass.Apply();

                if (_lineIndex > 0)
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineList, LineList, 0, _lineIndex / 2);

                if (_triangleIndex > 0)
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleList, TriangleList, 0, _triangleIndex / 3);
            }

            _lineIndex = 0;
            _triangleIndex = 0;

            base.Draw(gameTime);
        }


        public void DrawLine(JVector start, JVector end) {
            DrawLine(start, end, Color.Black);
        }

        public void DrawPoint(JVector pos) {
            // DrawPoint(pos, Color.Red);
        }

        public Color Color { get; set; }

        public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3) {
            DrawTriangle(pos1, pos2, pos3, Color);
        }
    }
}
