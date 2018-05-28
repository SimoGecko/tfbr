// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.PostProcessing;
using BRS.Scripts.Managers;
using BRS.Scripts.UI;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// player class that is a center hub for all things related to the player
    /// it manages the state, team, calls relative functions with input BUT NOTHING ELSE
    /// </summary>
    class Player : LivingEntity {
        // --------------------- VARIABLES ---------------------
        public enum PlayerState { Normal, Attack, Stun, Dead, Collided }

        //public
        public int PlayerIndex { get; private set; } // player index - to select input and camera
        public int TeamIndex { get; private set; } // to differentiate teams
        public string PlayerName;
        public Color PlayerColor;

        //HIT and STUN
        const float StunTime = 2f;
        const float StunDisabledTime = 1f;
        const float RespawnTime = 5f;
        public PlayerState State { get; set; } = PlayerState.Normal;

        float nextStunTime;

        //private
        Vector3 startPosition;

        //reference

        // Last collider which used the vibration-event
        private Collider _lastCollider;
        // Last time when the vibration-event was used
        private float _collidedAt;
        // Allows to vibrate for the same collider if time between is larger than this
        private const float TimeResetLastCollider = 2;

        //subcomponents
        public PlayerAttack pA    { get; private set; }
        public PlayerMovement pM  { get; private set; }
        public PlayerInventory pI { get; private set; }
        public PlayerPowerup pP   { get; private set; }
        public PlayerStamina pS   { get; private set; }

        private SteerableCollider _steerableCollider;
        public CameraController CamController;
        Player _other;


        // --------------------- BASE METHODS ------------------
        public Player(int playerIndex, int teamIndex, Vector3 startPos, string name = "Player") {
            PlayerIndex = playerIndex;
            TeamIndex = teamIndex;
            PlayerName = name + (playerIndex + 1).ToString();
            //PlayerColor = Graphics.ColorIndex(playerIndex);
            PlayerColor = playerIndex % 2 == 0
                ? ScenesCommunicationManager.TeamAColor
                : ScenesCommunicationManager.TeamBColor;

            startPosition = startPos;
        }

        public override void Start() {
            base.Start();

            _other = ElementManager.Instance.Enemy(TeamIndex);

            CamController = GameObject.FindGameObjectWithName("camera_" + PlayerIndex).GetComponent<CameraController>();
            //CamController.Start(); // why is start called on this?

            //subcomponents (shorten)
            pA = gameObject.GetComponent<PlayerAttack>();
            pM = gameObject.GetComponent<PlayerMovement>();
            pI = gameObject.GetComponent<PlayerInventory>();
            pP = gameObject.GetComponent<PlayerPowerup>();
            pS = gameObject.GetComponent<PlayerStamina>();

            MovingRigidBody mrb = gameObject.GetComponent<MovingRigidBody>();
            _steerableCollider = mrb.SteerableCollider;

            // Reset start position
            transform.position = startPosition;
            transform.rotation = Quaternion.Identity;

            if (_steerableCollider != null) {
                _steerableCollider.Speed = JVector.Zero;
                _steerableCollider.RotationY = 0;
                _steerableCollider.Position = Conversion.ToJitterVector(startPosition);
                _steerableCollider.Orientation = JMatrix.CreateRotationY(0);
            }
        }

        public override void Update() {
            UpdateUI();
            if (!GameManager.GameActive) {
                pM.Move(Vector3.Zero); // smooth stop
                return;
            }

            //only if game is running
            if (State == PlayerState.Normal) {
                bool boosting = BoostInput() ? pS.HasStaminaForBoost() : false;
                pM.Boosting = boosting;
                if (boosting) {
                    pS.UseStaminaForBoost();
                    //Input.Vibrate(.001f, .001f, PlayerIndex);
                }

                Vector2 moveInput;
                //if(true)//!CameraController.autoFollow)
                moveInput = MoveInput().Rotate(CamController.YRotation); // first input type
                //else
                    //moveInput = MoveInput().Rotate(transform.eulerAngles.Y); // input requested by nico
                pM.Move(moveInput.To3());

                if (PowerupInput()) pP.UsePowerup(this);
                if (DropCashInput()) pI.DropMoney();

                bool attack = AttackInput() ? pS.HasStaminaForAttack() : false;
                if (attack) {
                    State = PlayerState.Attack;
                    pS.UseStaminaForAttack();
                    pA.BeginAttack();
                    CamController.Shake(.5f);
                }
                
            } else if (State == PlayerState.Attack) {
                pA.AttackCoroutine();
                if (pA.AttackEnded) State = PlayerState.Normal;
            } else if (State == PlayerState.Stun) {
                _steerableCollider.Speed = JVector.Zero;
            }

            pS.UpdateStamina();
        }

        public override void Reset() {
            Start();
            pA.Reset();
            pM.Reset();
            pI.Reset();
            //pP.Reset();
            pS.Reset();
            UpdateUI();
            _steerableCollider.PostStep(0.0f);
        }

        public override void OnCollisionEnter(Collider c) {
            if (c.GameObject.tag == ObjectTag.StaticObstacle) {
                pM.SetSpeedPad(false);

                if (_lastCollider != c || _collidedAt + TimeResetLastCollider < Time.CurrentTime) {
                    Debug.Log("Vibration: " +(_collidedAt + TimeResetLastCollider )+ " < " + Time.CurrentTime);
                    Input.Vibrate(.05f, .1f, PlayerIndex);
                }

                _lastCollider = c;
                _collidedAt = Time.CurrentTime;
            }
        }


        // --------------------- CUSTOM METHODS ----------------

        // LIVING STUFF
        public override void TakeDamage(float damage) { // for bombs aswell
            base.TakeDamage(0); // don't override state => it's needed for effects

            if (!Dead) {
                if (Time.CurrentTime > nextStunTime) {
                    Input.Vibrate(.05f, .1f, PlayerIndex);
                    nextStunTime = Time.CurrentTime + StunDisabledTime + StunTime; // to avoid too frequent
                    State = PlayerState.Stun;
                    Audio.Play("stun", transform.position);
                    ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Stun, 1.2f);
                    PostProcessingManager.Instance.ActivateBlackAndWhite(PlayerIndex);
                    pI.LoseMoney();
                    ParticleUI.Instance.GiveOrder(transform.position+Vector3.Up*2, ParticleType.RotatingStars, .7f);
                    Timer t = new Timer(StunTime, () => { if (State == PlayerState.Stun) State = PlayerState.Normal; });
                }
            }
        }

        void UpdateUI() {
            bool playerInRange = false;
            if (_other != null) {
                playerInRange = Vector3.DistanceSquared(transform.position, _other.transform.position) <= Math.Pow(pA.AttackDistance, 2);
            }
            bool canAttack = pS.HasStaminaForAttackCheck() && playerInRange;
            PlayerUI.Instance.UpdatePlayerUI(PlayerIndex,
                pS.Stamina, pS.MaxStamina,
                pI.Capacity, pI.CarryingValue, pI.CarryingWeight,
                PlayerName, canAttack);
        }

        /// <summary>
        /// Start the collision-handling with the bounce-script
        /// </summary>
        /// <param name="other"></param>
        /// <param name="endPosition"></param>
        /// <param name="endAngle"></param>
        public void SetCollisionState(Collider other, Vector3 endPosition, float endAngle) {
            State = PlayerState.Normal;
            //_pM.ResetRotation(endAngle);
            pM.ResetSmoothMatnitude();
        }


        public bool IsAttacking() { return State == PlayerState.Attack; }
        public bool Full() { return gameObject.GetComponent<PlayerInventory>().IsFullCompletely(); }
        public bool Empty() { return gameObject.GetComponent<PlayerStamina>().TriedToUseUnsuccessfully; }

        //-------------------------------------------------------------------------------------------
        // INPUT queries
        //note: keyboard assumes max 2 players, gamepad works for 4

        Vector2 MoveInput() {
            if (PlayerIndex == 0)
                return new Vector2(Input.GetAxisRaw0(Input.Axis.Horizontal), Input.GetAxisRaw0(Input.Axis.Vertical));

            if (PlayerIndex == 1)
                return new Vector2(Input.GetAxisRaw1(Input.Axis.Horizontal), Input.GetAxisRaw1(Input.Axis.Vertical));

            return Input.GetThumbstick(Input.Stick.Left, PlayerIndex);
        }

        public bool AttackInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.Space) : Input.GetKeyDown(Keys.Enter))
                || Input.GetButtonDown(Buttons.A, PlayerIndex);
        }

        public bool DropCashInput() {
            return (PlayerIndex == 0 ? Input.GetKey(Keys.C) : Input.GetKey(Keys.M))
               || Input.GetButton(Buttons.B, PlayerIndex);
        }

        public bool PowerupInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.R) : Input.GetKeyDown(Keys.P))
               || Input.GetButtonDown(Buttons.X, PlayerIndex);
        }

        public bool BoostInput() {
            return (PlayerIndex == 0 ? Input.GetKey(Keys.LeftShift) : Input.GetKey(Keys.RightShift))
               || Input.GetButton(Buttons.RightShoulder, PlayerIndex) || Input.GetButton(Buttons.RightTrigger, PlayerIndex)
               || Input.GetButton(Buttons.LeftShoulder, PlayerIndex)  || Input.GetButton(Buttons.LeftTrigger, PlayerIndex);
        }

        // other
        
        

    }

}