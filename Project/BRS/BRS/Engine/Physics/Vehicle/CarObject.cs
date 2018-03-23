#region Using Statements

using BRS.Load;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace BRS.Engine.Physics.Vehicle {
    public class CarObject : DrawableGameComponent {
        private Model chassisModel = null;
        private Model tireModel = null;

        public DefaultCar carBody = null;

        private World _world;

        public CarObject(Game game, PhysicsManager physics)
            : base(game) {
            _world = physics.World;
            LoadContent();
            BuildCar();
        }

        private void BuildCar() {
            CompoundShape.TransformedShape lower = new CompoundShape.TransformedShape(
                new BoxShape(2.5f, 1f, 6.0f), JMatrix.Identity, JVector.Zero);

            CompoundShape.TransformedShape upper = new CompoundShape.TransformedShape(
                new BoxShape(2.0f, 0.5f, 3.0f), JMatrix.Identity, JVector.Up * 0.75f + JVector.Backward * 1.0f);

            CompoundShape.TransformedShape[] subShapes = { lower, upper };

            Shape chassis = new CompoundShape(subShapes);

            //chassis = new BoxShape(2.5f, 1f, 6.0f);

            carBody = new DefaultCar(_world, chassis);

            // use the inertia of the lower box.

            // adjust some driving values
            carBody.SteerAngle = 30; carBody.DriveTorque = 155;
            carBody.AccelerationRate = 10;
            carBody.SteerRate = 2f;
            carBody.AdjustWheelValues();

            carBody.Tag = BodyTag.DontDrawMe;
            carBody.AllowDeactivation = false;

            // place the car two units above the ground.
            carBody.Position = new JVector(0, 5, 0);

            _world.AddBody(carBody);
        }

        public override void Update(GameTime gameTime) {
            KeyboardState keyState = Keyboard.GetState();

            float steer, accelerate;
            if (keyState.IsKeyDown(Keys.Up)) accelerate = 1.0f;
            else if (keyState.IsKeyDown(Keys.Down)) accelerate = -1.0f;
            else accelerate = 0.0f;

            if (keyState.IsKeyDown(Keys.Left)) steer = 1;
            else if (keyState.IsKeyDown(Keys.Right)) steer = -1;
            else steer = 0.0f;

            carBody.SetInput(accelerate, steer);

            base.Update(gameTime);
        }

        #region Draw Wheels
        private void DrawWheels() {
            for (int i = 0; i < carBody.Wheels.Length; i++) {
                Wheel wheel = carBody.Wheels[i];

                Vector3 position = Conversion.ToXnaVector(wheel.GetWorldPosition());

                foreach (ModelMesh mesh in tireModel.Meshes) {
                    foreach (BasicEffect effect in mesh.Effects) {
                        Matrix addOrienation;

                        if (i % 2 != 0) addOrienation = Matrix.CreateRotationX(MathHelper.Pi);
                        else addOrienation = Matrix.Identity;

                        effect.World =
                            addOrienation *
                            Matrix.CreateRotationZ(MathHelper.PiOver2) *
                            Matrix.CreateRotationX(MathHelper.ToRadians(-wheel.WheelRotation)) *
                            Matrix.CreateRotationY(MathHelper.ToRadians(wheel.SteerAngle)) *
                            Conversion.ToXnaMatrix(carBody.Orientation) *
                            Matrix.CreateTranslation(position);

                        effect.EnableDefaultLighting();
                        effect.View = Screen.cameras[0].View;
                        effect.Projection = Screen.cameras[0].Proj;
                    }
                    mesh.Draw();
                }

            }

        }
        #endregion

        private void DrawChassis() {
            foreach (ModelMesh mesh in chassisModel.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    Matrix matrix = Conversion.ToXnaMatrix(carBody.Orientation);
                    matrix.Translation = Conversion.ToXnaVector(carBody.Position) -
                        Vector3.Transform(new Vector3(0, 1.0f, 0), matrix);

                    effect.EnableDefaultLighting();
                    effect.World = matrix;
                    effect.View = Screen.cameras[0].View;
                    effect.Projection = Screen.cameras[0].Proj;
                }
                mesh.Draw();
            }
        }

        protected override void LoadContent() {
            chassisModel = File.Load<Model>("Models/vehicles/car");
            tireModel = File.Load<Model>("Models/vehicles/wheel");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime) {
            DrawWheels();
            DrawChassis();
            base.Draw(gameTime);
        }


    }
}
