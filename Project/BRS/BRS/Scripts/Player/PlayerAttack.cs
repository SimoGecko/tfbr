// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerAttack : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float attackDuration = .2f;
        public const float attackDistance = 5;
        const float attackDistanceThreshold = 2f;
        const float attackDamage = 40;

        //private
        bool attacking;
        Vector3 attackStartPos, attackEndPos;
        float attackRefTime;
        float attackStartTime;
        bool hasAppliedDamage;

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() { attacking = false; }
        public override void Update() { }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer && attacking) {
                Player p = c.GameObject.GetComponent<Player>();
                DealWithAttack(p);
            }
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void BeginAttack() {
            Debug.Log(Time.time);
            attacking = true;
            attackRefTime = 0;
            attackStartPos = transform.position;
            attackEndPos = transform.position + transform.Forward * attackDistance;
            hasAppliedDamage = false;
            attackStartTime = Time.time;
            Invoke(attackDuration, () => attacking = false);
        }

        public void AttackCoroutine() {
            if (attackRefTime <= 1) {
                attackRefTime += Time.deltaTime / attackDuration;
                float t = Curve.EvaluateSqrt(attackRefTime);
                Vector3 newPosition = Vector3.LerpPrecise(attackStartPos, attackEndPos, t);
                
                // Apply new position to the rigid-body
                // Todo by Andy for Andy: can be surely written better :-)
                MovingRigidBody mrb = gameObject.GetComponent<MovingRigidBody>();
                mrb.RigidBody.Position = new JVector(newPosition.X, mrb.RigidBody.Position.Y, newPosition.Z);
            } else {
                attacking = false;
            }
        }
        /*
        void EndAttack() {
            Debug.Log(Time.time);
            attacking = false;
        }*/

        void DealWithAttack(Player p) {
            PlayerAttack pa = p.gameObject.GetComponent<PlayerAttack>();
            if (!hasAppliedDamage && (!pa.attacking || pa.attackStartTime > attackStartTime)) {
                //if the other is not attacking or started attacking later
                p.TakeDamage(attackDamage);
                hasAppliedDamage = true;
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
        public bool AttackEnded { get { return !attacking; } }
        public bool IsAttacking { get { return attacking; } }


        // other
        
    }
}