﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerAttack : Component {
        ////////// deals with the attack of the player //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float attackDuration = .2f;
        const float attackDistance = 5;
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
            bool isPlayer = c.gameObject.myTag == "player";
            if (isPlayer && attacking) {
                Player p = c.gameObject.GetComponent<Player>();
                DealWithAttack(p);
            }
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void BeginAttack() {
            attacking = true;
            attackRefTime = 0;
            attackStartPos = transform.position;
            attackEndPos = transform.position + transform.Forward * attackDistance;
            hasAppliedDamage = false;
            attackStartTime = Time.time;
        }

        public void AttackCoroutine() {
            if (attackRefTime <= 1) {
                attackRefTime += Time.deltatime / attackDuration;
                float t = Curve.EvaluateSqrt(attackRefTime);
                transform.position = Vector3.LerpPrecise(this.attackStartPos, attackEndPos, t);
            } else {
                EndAttack();
            }
        }

        void EndAttack() {
            attacking = false;
        }

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