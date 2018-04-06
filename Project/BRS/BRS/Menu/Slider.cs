using BRS.Engine;
using BRS.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BRS.Menu {
    class Slider : Component {
        ////////// class to create and display a slider object (with the bar texture => see UserInterface) //////////

        // --------------------- VARIABLES ---------------------
        private MouseState _currentMouse;
        private MouseState _previousMouse;

        public Button ButtonSlider;
        bool _moveButton;
        private float _offset;

        public Vector2 Position;

        // --------------------- BASE METHODS ------------------
        public Slider() {
            _moveButton = false;
        }

        public override void Update() {
            base.Update();

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed && _moveButton)
                _moveButton = false;

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            ButtonSlider.IsHovering = false;
            if (mouseRectangle.Intersects(ButtonSlider.Rectangle)) {
                ButtonSlider.IsHovering = true;
                if (_currentMouse.LeftButton == ButtonState.Pressed) {
                    _moveButton = true;
                    _offset = _currentMouse.Position.X - ButtonSlider.InitPos.X;
                }
            }

            if (_moveButton) {
                ButtonSlider.IsHovering = true;
                ButtonSlider.InitPos = new Vector2(_currentMouse.Position.X - _offset, ButtonSlider.InitPos.Y);
                if (ButtonSlider.InitPos.X + ButtonSlider.Texture.Width / 2 < Position.X)
                    ButtonSlider.InitPos = new Vector2(Position.X - ButtonSlider.Texture.Width / 2, ButtonSlider.InitPos.Y);
                if (ButtonSlider.InitPos.X > Position.X + UserInterface.BarBigWidth - ButtonSlider.Texture.Width / 2)
                    ButtonSlider.InitPos = new Vector2(Position.X + UserInterface.BarBigWidth - ButtonSlider.Texture.Width / 2, ButtonSlider.InitPos.Y);
            }
        }

        public override void Draw() {
            base.Draw();

            float percentPosButon =  ((ButtonSlider.InitPos.X - Position.X + ButtonSlider.Texture.Width/2) / UserInterface.BarBigWidth);
            UserInterface.Instance.DrawBarBig(new Vector2(Position.X, Position.Y) - ButtonSlider.OffsetTexture, percentPosButon,  Color.Yellow);

            ButtonSlider.Draw();
        }

        // --------------------- CUSTOM METHODS ----------------
    }
}
