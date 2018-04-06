// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Curve = BRS.Engine.Utilities.Curve;

namespace BRS.Scripts.PlayerScripts {
    class PlayerAttack : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private bool _attacking;
        private Vector3 _attackStartPos, _attackEndPos;
        private float _attackRefTime;
        private float _attackStartTime;
        private bool _hasAppliedDamage;

        //const
        public const float AttackDistance = 5;
        private const float AttackDuration = .2f;
        private const float AttackDistanceThreshold = 2f;
        private const float AttackDamage = 40;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() { _attacking = false; }
        public override void Update() { }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer && _attacking) {
                Player p = c.GameObject.GetComponent<Player>();
                DealWithAttack(p);
            }
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void BeginAttack() {
            Debug.Log(Time.CurrentTime);
            _attacking = true;
            _attackRefTime = 0;
            _attackStartPos = transform.position;
            _attackEndPos = transform.position + transform.Forward * AttackDistance;
            _hasAppliedDamage = false;
            _attackStartTime = Time.CurrentTime;
            Invoke(AttackDuration, () => _attacking = false);
        }

        public void AttackCoroutine() {
            if (_attackRefTime <= 1) {
                _attackRefTime += Time.DeltaTime / AttackDuration;
                float t = Curve.EvaluateSqrt(_attackRefTime);
                Vector3 newPosition = Vector3.LerpPrecise(_attackStartPos, _attackEndPos, t);
                
                // Apply new position to the rigid-body
                // Todo by Andy for Andy: can be surely written better :-)
                MovingRigidBody mrb = gameObject.GetComponent<MovingRigidBody>();
                mrb.RigidBody.Position = new JVector(newPosition.X, mrb.RigidBody.Position.Y, newPosition.Z);
            } else {
                _attacking = false;
            }
        }
        /*
        void EndAttack() {
            Debug.Log(Time.time);
            attacking = false;
        }*/

        void DealWithAttack(Player p) {
            PlayerAttack pa = p.gameObject.GetComponent<PlayerAttack>();
            if (!_hasAppliedDamage && (!pa._attacking || pa._attackStartTime > _attackStartTime)) {
                //if the other is not attacking or started attacking later
                p.TakeDamage(AttackDamage);
                _hasAppliedDamage = true;
            }
        }
        /*
        public void CheckCollision(Player otherPlayer) {
            if (!hasAppliedDamage && Vector3.DistanceSquared(transform.position, otherPlayer.transform.position) < attackDistanceThreshold) {
                otherPlayer.GetHit();
                hasAppliedDamage = true;
            }
        }*/


        // queries
        public bool AttackEnded { get { return !_attacking; } }
        public bool IsAttacking { get { return _attacking; } }


        // other
        
    }
}