﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using BRS.Engine.Physics.RigidBodies;
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
        public float AttackDistance = 5;
        //const
        private const float AttackDuration = .2f;
        private const float AttackDistanceThreshold = 2f;
        private const float AttackDamage = 40;

        //private
        private bool _attacking;
        private Vector3 _attackStartPos, _attackEndPos;
        private Vector3 _attackEndCollision;
        private float _attackRefTime;
        private float _attackStartTime;
        private bool _hasAppliedDamage;
        

        //reference
        public Action OnAttackBegin;
        public Action OnEnemyHit;
        private MovingRigidBody _rigidBody;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Reset();
            _attacking = false;
            _rigidBody = gameObject.GetComponent<MovingRigidBody>();
        }
        public override void Update() {

        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer && _attacking) {
                Player p = c.GameObject.GetComponent<Player>();
                DealWithAttack(p);
            }
            bool isPolice = c.GameObject.tag == ObjectTag.Police;
            if (isPolice && _attacking) {
                IDamageable dam = c.GameObject.GetComponent<IDamageable>();
                dam.TakeDamage(1);
            }
        }


        // --------------------- CUSTOM METHODS ----------------
        public override void Reset() {
            _attacking = _hasAppliedDamage = false;
            _attackStartPos = _attackEndPos = _attackEndCollision = Vector3.Zero;
            _attackRefTime = _attackStartTime = 0f;
        }

        // commands
        public void BeginAttack() {
            Audio.Play("dash", transform.position);
            Input.Vibrate(.04f, .1f, gameObject.GetComponent<Player>().PlayerIndex);
            OnAttackBegin?.Invoke();

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

                // Apply new position to the rigid-body
                _rigidBody.RigidBody.Position = new JVector(newPosition.X, _rigidBody.RigidBody.Position.Y, newPosition.Z);
            } else {
                _attacking = false;
            }
        }

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

        // queries
        public bool AttackEnded { get { return !_attacking; } }
        public bool IsAttacking { get { return _attacking; } }


        // other

    }
}