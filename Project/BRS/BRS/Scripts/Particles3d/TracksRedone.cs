using BRS.Engine;

namespace BRS.Scripts.Particles3D {
    public class TracksRedone : Component {
        private bool _fadeOut = false;

        public override void Start() {
            new Timer(1, () => _fadeOut = true);
        }

        public override void Update() {
            if (_fadeOut) {
                gameObject.Alpha -= 0.01f;

                if (gameObject.Alpha <= 0.0f)
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
}
