// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;

namespace BRS.Scripts.Particles3D {
    public class Tracks : Component {

        #region Properties and attributes

        private bool _fadeOut;
        private const float FadeOutSpeed = 0.01f;

        #endregion

        #region Monogame-structure

        /// <summary>
        /// Start the component.
        /// Fading out starts 1 second after
        /// </summary>
        public override void Start() {
            _fadeOut = false;
            new Timer(1, () => _fadeOut = true);
        }


        /// <summary>
        /// Update-routine
        /// </summary>
        public override void Update() {
            // If fade-out is enabled simply decrease the alpha-value
            // and remove the track as soon as it's not visible anymore
            if (_fadeOut) {
                gameObject.Alpha -= FadeOutSpeed;

                if (gameObject.Alpha <= 0.0f) {
                    GameObject.Destroy(gameObject);
                }
            }
        }

        #endregion
    }
}
