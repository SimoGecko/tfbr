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

        //subcomponents
        PlayerAttack _pA;
        PlayerMovement _pM;
        PlayerInventory _pI;
        PlayerPowerup _pP;
        PlayerStamina _pS;
        PlayerLift _pL;
        private PlayerCollider _pC;
        private SteerableCollider _steerableCollider;

        public CameraController CamController;
        Player _other;


        // --------------------- BASE METHODS ------------------
        public Player(int playerIndex, int teamIndex, Vector3 startPos, string name = "Player") {
            PlayerIndex = playerIndex;
            TeamIndex = teamIndex;
            PlayerName = name + (playerIndex + 1).ToString();
            PlayerColor = Graphics.ColorIndex(playerIndex);

            startPosition = startPos;

            // TODO make mesh have this color
        }
        public override void Start() {
            base.Start();

            GameObject po = GameObject.FindGameObjectWithName("player_" + (1 - PlayerIndex));
            if (po != null) _other = po.GetComponent<Player>();

            CamController = GameObject.FindGameObjectWithName("camera_" + PlayerIndex).GetComponent<CameraController>();
            CamController.Start();

            //subcomponents (shorten)
            _pA = gameObject.GetComponent<PlayerAttack>();
            _pM = gameObject.GetComponent<PlayerMovement>();
            _pI = gameObject.GetComponent<PlayerInventory>();
            _pP = gameObject.GetComponent<PlayerPowerup>();
            _pS = gameObject.GetComponent<PlayerStamina>();
            _pL = gameObject.GetComponent<PlayerLift>();
            _pC = gameObject.GetComponent<PlayerCollider>();

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
                _pM.Move(Vector3.Zero); // smooth stop
                return;
            }

            //only if game is running
            if (State == PlayerState.Normal) {
                bool boosting = BoostInput() ? _pS.HasStaminaForBoost() : false;
                _pM.Boosting = boosting;
                if (boosting) {
                    _pS.UseStaminaForBoost();
                    //Input.Vibrate(.001f, .001f, PlayerIndex);
                }

                Vector2 moveInput;
                //if(true)//!CameraController.autoFollow)
                moveInput = MoveInput().Rotate(CamController.YRotation); // first input type
                //else
                    //moveInput = MoveInput().Rotate(transform.eulerAngles.Y); // input requested by nico
                _pM.Move(moveInput.To3());

                if (PowerupInput()) _pP.UsePowerup(this);
                if (DropCashInput()) _pI.DropMoney();

                bool attack = AttackInput() ? _pS.HasStaminaForAttack() : false;
                if (attack) {
                    State = PlayerState.Attack;
                    _pS.UseStaminaForAttack();
                    _pA.BeginAttack();
                    CamController.Shake(.5f);
                }
                
                if (LiftInput()) {
                    _pL.Lift();
                }
            } else if (State == PlayerState.Attack) {
                _pA.AttackCoroutine();
                if (_pA.AttackEnded) State = PlayerState.Normal;
            } else if (State == PlayerState.Stun) {
                _steerableCollider.Speed = JVector.Zero;
            }

            _pS.UpdateStamina();
        }

        public override void Reset() {
            Start();
            _pA.Reset();
            _pM.Reset();
            _pI.Reset();
            _pP.Reset();
            _pS.Reset();
            _pL.Reset();
            _pC.Reset();
            UpdateUI();
        }

        public override void OnCollisionEnter(Collider c) {
            if (c.GameObject.tag == ObjectTag.StaticObstacle) {
                _pM.SetSpeedPad(false);
                // CamController.Shake(.3f);
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
                    _pI.LoseMoney();
                    ParticleUI.Instance.GiveOrder(transform.position+Vector3.Up*2, ParticleType.RotatingStars, .7f);
                    Timer t = new Timer(StunTime, () => { if (State == PlayerState.Stun) State = PlayerState.Normal; });
                }

                
            }
        }

        /*
        protected override void Die() {
            base.Die();
            _state = PlayerState.Dead;
            _pI.LoseAllMoney();
            Timer timer = new Timer(RespawnTime, Respawn);
        }*/

        protected override void Respawn() {
            base.Respawn();
            State = PlayerState.Normal;
            transform.position = new Vector3(-5 + 10 * PlayerIndex, 0, 0); // store base position
        }

        void UpdateUI() {
            //Base ba = GameObject.FindGameObjectWithName("Base_" + playerIndex).GetComponent<Base>();
            // WHY SHOULD THE PLAYER KNOW ABOUT THE BASE??
            bool playerInRange = false;
            if (_other != null) {
                playerInRange = Vector3.DistanceSquared(transform.position, _other.transform.position) <= Math.Pow(_pA.AttackDistance, 2);
            }
            bool canAttack = /*_pS.HasStaminaForAttack() &&*/ playerInRange;
            PlayerUI.Instance.UpdatePlayerUI(PlayerIndex,
                Health, StartingHealth,
                _pS.Stamina, _pS.MaxStamina,
                _pI.Capacity, _pI.CarryingValue, _pI.CarryingWeight, PlayerName, canAttack);//, ba.Health, ba.startingHealth);
        }

        /// <summary>
        /// Start the collision-handling with the bounce-script
        /// </summary>
        /// <param name="other"></param>
        /// <param name="endPosition"></param>
        /// <param name="endAngle"></param>
        public void SetCollisionState(Collider other, Vector3 endPosition, float endAngle) {
            State = PlayerState.Normal;
            //_pC.Begin(other, endPosition, endAngle);
            //_pM.ResetRotation(endAngle);
            _pM.ResetSmoothMatnitude();
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

        bool AttackInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.Space) : Input.GetKeyDown(Keys.Enter))
                || Input.GetButtonDown(Buttons.A, PlayerIndex);
        }

        bool DropCashInput() {
            return (PlayerIndex == 0 ? Input.GetKey(Keys.C) : Input.GetKey(Keys.M))
               || Input.GetButton(Buttons.B, PlayerIndex);
        }

        bool PowerupInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.R) : Input.GetKeyDown(Keys.P))
               || Input.GetButtonDown(Buttons.X, PlayerIndex);
        }

        bool LiftInput() {
            return (PlayerIndex == 0 ? Input.GetKeyDown(Keys.F) : Input.GetKeyDown(Keys.L))
               || Input.GetButtonDown(Buttons.Y, PlayerIndex);
        }

        bool BoostInput() {
            return (PlayerIndex == 0 ? Input.GetKey(Keys.LeftShift) : Input.GetKey(Keys.RightShift))
               || Input.GetButton(Buttons.RightShoulder, PlayerIndex) || Input.GetButton(Buttons.RightTrigger, PlayerIndex)
               || Input.GetButton(Buttons.LeftShoulder, PlayerIndex) || Input.GetButton(Buttons.LeftTrigger, PlayerIndex);
        }

        // other


    }

}