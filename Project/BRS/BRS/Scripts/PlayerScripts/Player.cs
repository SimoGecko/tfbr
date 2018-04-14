// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Utilities;
using BRS.Scripts.Managers;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// player class that is a center hub for all things related to the player
    /// it manages the state, team, calls relative functions with input BUT NOTHING ELSE
    /// </summary>
    class Player : LivingEntity {
        // --------------------- VARIABLES ---------------------
        enum State { Normal, Attack, Stun, Dead };

        //public
        public int PlayerIndex { get; private set; } // player index - to select input and camera
        public int TeamIndex { get; private set; } // to differentiate teams
        public string PlayerName;
        public Color PlayerColor;

        //HIT and STUN
        const float StunTime = 2f;
        const float RespawnTime = 5f;

        //private
        State _state = State.Normal;
        Vector3 startPosition;

        //reference

        //subcomponents
        PlayerAttack _pA;
        PlayerMovement _pM;
        PlayerInventory _pI;
        PlayerPowerup _pP;
        PlayerStamina _pS;
        PlayerLift _pL;

        public CameraController CamController;
        Player _other;


        // --------------------- BASE METHODS ------------------
        public Player(int playerIndex, int teamIndex, Vector3 startPos, string name = "Player") {
            PlayerIndex = playerIndex;
            TeamIndex = teamIndex;
            PlayerName = name + (playerIndex + 1).ToString();
            PlayerColor = Graphics.ColorIndex(playerIndex);

            startPosition = startPos;
            //TODO make mesh have this color
        }
        public override void Start() {
            base.Start();
            transform.position = startPosition;
            transform.rotation = Quaternion.Identity;

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
        }

        public override void Update() {
            UpdateUI();
            if (!GameManager.GameActive) {
                _pM.Move(Vector3.Zero); // smooth stop
                return;
            }

            //only if game is running
            if (_state == State.Normal) {
                bool boosting = BoostInput() && _pS.HasStaminaForBoost();
                _pM.Boosting = boosting;
                if (boosting) {
                    _pS.UseStaminaForBoost();
                    Audio.Play("useSpeed",Vector3.Zero);
                }

                Vector2 moveInput = MoveInput().Rotate(CamController.YRotation);
                _pM.Move(moveInput.To3());

                if (PowerupInput()) _pP.UsePowerup(this);
                if (DropCashInput()) {
                    _pI.DropMoney();
                    Audio.Play("dropCash",Vector3.Zero);
                }

                if (AttackInput() && _pS.HasStaminaForAttack()) {
                    _state = State.Attack;
                    _pS.UseStaminaForAttack();
                    _pA.BeginAttack();
                    CamController.Shake(.5f);
                }

                if (LiftInput()) {
                    _pL.Lift();
                }

                // Todo: Can be removed, just here till this is finally tested
                //if (Input.GetKeyDown(Keys.V)) {
                //    Collider[] test = PhysicsManager.OverlapSphere(transform.position, 10);


                //    string tmp = "Contained: ";
                //    foreach (Collider collider in test) {
                //        tmp += collider.GameObject.tag + ",";
                //    }
                //    Debug.Log(tmp);
                //}
            } else if (_state == State.Attack) {
                _pA.AttackCoroutine();
                if (_pA.AttackEnded) _state = State.Normal;
            }

            _pS.UpdateStamina();
        }

        public override void OnCollisionEnter(Collider c) {
            if (c.IsStatic) {
                CamController.Shake(.3f);
            }
        }


        // --------------------- CUSTOM METHODS ----------------

        // LIVING STUFF
        public override void TakeDamage(float damage) { // for bombs aswell
            //base.TakeDamage(damage); // don't override state

            if (!Dead) {
                _state = State.Stun;
                Audio.Play("stun", transform.position);
                ParticleUI.Instance.GiveOrder(transform.position, ParticleType.Stun);
                _pI.LoseMoney();
                Timer t = new Timer(StunTime, () => { if (_state == State.Stun) _state = State.Normal; });
            }
        }

        /*
        protected override void Die() {
            base.Die();
            _state = State.Dead;
            _pI.LoseAllMoney();
            Timer timer = new Timer(RespawnTime, Respawn);
        }*/

        protected override void Respawn() {
            base.Respawn();
            _state = State.Normal;
            transform.position = new Vector3(-5 + 10 * PlayerIndex, 0, 0); // store base position
        }

        void UpdateUI() {
            //Base ba = GameObject.FindGameObjectWithName("Base_" + playerIndex).GetComponent<Base>();
            // WHY SHOULD THE PLAYER KNOW ABOUT THE BASE??
            bool playerInRange = false;
            if (_other != null) {
                playerInRange = Vector3.DistanceSquared(transform.position, _other.transform.position) <= Math.Pow(PlayerAttack.AttackDistance, 2);
            }
            bool canAttack = _pS.HasStaminaForAttack() && playerInRange;
            PlayerUI.Instance.UpdatePlayerUI(PlayerIndex,
                Health, StartingHealth,
                _pS.Stamina, _pS.MaxStamina,
                _pI.Capacity, _pI.CarryingValue, _pI.CarryingWeight, PlayerName, canAttack);//, ba.Health, ba.startingHealth);
        }

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