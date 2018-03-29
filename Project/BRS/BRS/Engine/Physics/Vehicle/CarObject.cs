#region Using Statements

using Windows.Devices.PointOfService;
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

        private JVector _centerOfMass;

        public CarObject(Game game, PhysicsManager physics)
            : base(game) {
            _world = physics.World;
            LoadContent();
            BuildCar();
        }

        private void BuildCar() {
            float size = 0.5f;
            JVector lowerSize = new JVector(2.5f, 1.0f, 6.0f);
            //CompoundShape.TransformedShape lower = new CompoundShape.TransformedShape(
            //    new BoxShape(lowerSize * size), JMatrix.Identity, JVector.Zero);

            //CompoundShape.TransformedShape upper = new CompoundShape.TransformedShape(
            //    new BoxShape(2.0f, 0.5f, 3.0f), JMatrix.Identity, JVector.Up * 0.75f + JVector.Backward * 1.0f);
            ////CompoundShape.TransformedShape upper = new CompoundShape.TransformedShape(
            ////    new BoxShape(0.4f, 0.1f, 0.6f), JMatrix.Identity, (JVector.Up * 0.75f + JVector.Backward * 1.0f) * 0.5f);

            ////CompoundShape.TransformedShape[] subShapes = { lower, upper };
            //CompoundShape.TransformedShape[] subShapes = { lower };

            //Shape chassis = new CompoundShape(subShapes);

            //chassis = new BoxShape(2.5f, 1f, 6.0f);
            Model model = chassisModel;
            BoundingBox bb = BoundingBoxHelper.Calculate(model);
            JVector bbSize = Conversion.ToJitterVector(bb.Max - bb.Min);
            bbSize = JVector.Transform(bbSize, JMatrix.CreateRotationZ(MathHelper.PiOver2) *
                                               JMatrix.CreateRotationY(MathHelper.Pi) *
                                               JMatrix.CreateRotationX(MathHelper.PiOver2));

            //JMatrix tmp = JMatrix.CreateRotationZ(MathHelper.PiOver2) *
            //              JMatrix.CreateRotationY(MathHelper.Pi) *
            //              JMatrix.CreateRotationX(MathHelper.PiOver2);
            Shape chassis = new BoxShape(bbSize);
            chassis.CalculateMassInertia();
            JVector support;
            chassis.SupportCenter(out support);

            _centerOfMass = 0.5f * Conversion.ToJitterVector(bb.Max + bb.Min);

            carBody = new DefaultCar(_world, chassis, size);

            // use the inertia of the lower box.

            // adjust some driving values
            carBody.SteerAngle = 45;
            carBody.DriveTorque = .1f;
            carBody.AccelerationRate = 1;
            carBody.SteerRate = 5f;
            //carBody.Mass = 100;

            carBody.AdjustWheelValues();

            carBody.Tag = BodyTag.DrawMe;
            carBody.AllowDeactivation = false;

            // place the car two units above the ground.
            //carBody.Position = new JVector(0, 5, 0);

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
            Debug.Log(carBody.Position);
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

                        if (i % 2 == 0) addOrienation = Matrix.CreateRotationX(MathHelper.Pi);
                        else addOrienation = Matrix.Identity;

                        //effect.World =
                        //    addOrienation *
                        //    Matrix.CreateRotationZ(MathHelper.PiOver2) *
                        //    Matrix.CreateRotationX(MathHelper.ToRadians(-wheel.WheelRotation)) *
                        //    Matrix.CreateRotationY(MathHelper.ToRadians(wheel.SteerAngle)) *
                        //    Conversion.ToXnaMatrix(carBody.Orientation) *
                        //    Matrix.CreateTranslation(position);
                        effect.World =
                            addOrienation *
                            Matrix.CreateRotationX(MathHelper.PiOver2) *
                            Matrix.CreateRotationZ(MathHelper.ToRadians(-wheel.WheelRotation)) *
                            Matrix.CreateRotationY(MathHelper.ToRadians(wheel.SteerAngle + 90)) *
                            Conversion.ToXnaMatrix(carBody.Orientation) *
                            Matrix.CreateTranslation(position);

                        //effect.EnableDefaultLighting();
                        effect.Alpha = 1.0f;
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
                    Matrix matrix =
                        Matrix.CreateRotationZ(MathHelper.PiOver2) *
                        Matrix.CreateRotationY(MathHelper.Pi) *
                        Matrix.CreateRotationX(MathHelper.PiOver2) *
                        Conversion.ToXnaMatrix(carBody.Orientation);
                    matrix.Translation = Conversion.ToXnaVector(carBody.Position+_centerOfMass);

                    //effect.EnableDefaultLighting();
                    effect.Alpha = 1.0f;
                    effect.World = matrix;
                    effect.View = Screen.cameras[0].View;
                    effect.Projection = Screen.cameras[0].Proj;
                }
                mesh.Draw();
            }
        }

        protected override void LoadContent() {
            chassisModel = this.Game.Content.Load<Model>("forklift_chassis");
            tireModel = this.Game.Content.Load<Model>("forklift_wheel");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime) {
            DrawWheels();
            DrawChassis();
            base.Draw(gameTime);
        }


    }
}
