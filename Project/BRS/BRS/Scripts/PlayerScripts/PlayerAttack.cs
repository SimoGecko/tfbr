// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Curve = BRS.Engine.Utilities.Curve;

namespace BRS.Scripts.PlayerScripts {
    /// <summary>
    /// Deals with the attack of the player.
    /// </summary>
    class PlayerAttack : Component {

        // --------------------- VARIABLES ---------------------

        //public

        //private
        private bool _attacking;
        private Vector3 _attackStartPos, _attackEndPos;
        private Vector3 _attackEndCollision;
        private float _attackRefTime;
        private float _attackStartTime;
        private bool _hasAppliedDamage;

        //const
        public const float AttackDistance = 5;
        private const float AttackDuration = .2f;
        private const float AttackDistanceThreshold = 2f;
        private const float AttackDamage = 40;

        //reference
        public Action OnAttackBegin;
        public Action OnEnemyHit;
        private MovingRigidBody _rigidBody;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            _attacking = false;
            _rigidBody = gameObject.GetComponent<MovingRigidBody>();
        }
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
            Audio.Play("dash", transform.position);
            OnAttackBegin?.Invoke();

            //Debug.Log(Time.CurrentTime);
            _attacking = true;
            _attackRefTime = 0;
            _attackStartPos = transform.position;
            _attackEndPos = transform.position + transform.Forward * AttackDistance;
            _hasAppliedDamage = false;
            _attackStartTime = Time.CurrentTime;

            _attackEndCollision = PhysicsManager.Instance.DetectCollision(_rigidBody.RigidBody, _attackStartPos, _attackEndPos);
            _attackEndPos = _attackEndCollision;

            Invoke(AttackDuration, () => _attacking = false);
        }

        public void AttackCoroutine() {
            if (_attackRefTime <= 1) {
                _attackRefTime += Time.DeltaTime / AttackDuration;
                float t = Curve.EvaluateSqrt(_attackRefTime);
                Vector3 newPosition = Vector3.LerpPrecise(_attackStartPos, _attackEndPos, t);

                // Todo: Fix attack-end
                //if (newPosition.X - _attackStartPos.X > _attackEndCollision.X - _attackStartPos.X) {
                //    newPosition = _attackEndCollision;
                //}

                // Apply new position to the rigid-body
                // Todo by Andy for Andy: can be surely written better :-)
                _rigidBody.RigidBody.Position = new JVector(newPosition.X, _rigidBody.RigidBody.Position.Y, newPosition.Z);
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
                OnEnemyHit?.Invoke();
                Audio.Play("enemy_hit", transform.position);

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