// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace BRS.Scripts {
    /// <summary>
    /// Records a player position and then follows it smoothly
    /// </summary>
    class Police : Component, IDamageable {

        // --------------------- VARIABLES ---------------------
        enum State { Chasing, Stun, Collided }
        State state;

        //public
        const float Speed = 4f;
        const float ChangeThreshold = .8f;
        const float TurnSmoothTime = .2f;
        const float BustRadius = .5f;
        const float StunTime = 2f;


        //private
        bool following;
        int wpIndex;
        float angleRefVelocity;
        float endStunTime;

        //reference
        List<Vector3> waypoints;
        bool loopWaypoints = false;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            state = State.Chasing;
        }

        public override void Update() {
            if (GameManager.GameActive && following && state == State.Chasing) {
                bool wait = false;

                while (Vector3.DistanceSquared(transform.position, waypoints[wpIndex]) < ChangeThreshold * ChangeThreshold && !wait) {
                    if (wpIndex < waypoints.Count - 1) {
                        wpIndex++;
                    } else {
                        if (loopWaypoints) wpIndex = 0;
                        else wait = true;
                    }
                }

                if (!wait) {
                    Vector3 target = waypoints[wpIndex];
                    float nowY = transform.eulerAngles.Y;
                    transform.LookAt(target);
                    float targetY = transform.eulerAngles.Y;
                    float smoothY = Utility.SmoothDampAngle(nowY, targetY, ref angleRefVelocity, TurnSmoothTime);
                    transform.eulerAngles = new Vector3(0, smoothY, 0);
                    transform.Translate(Vector3.Forward * Speed * Time.DeltaTime);
                }
            }

            if (state == State.Stun && Time.CurrentTime > endStunTime) {
                state = State.Chasing;
            }

        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer) {
                Player p = c.GameObject.GetComponent<Player>();

                if (state == State.Chasing && !p.IsAttacking()) {
                    p.TakeDamage(20);
                    state = State.Collided;
                }
            }

            if (c.GameObject.tag == ObjectTag.Police) {
                state = State.Collided;
            }
        }

        public override void OnCollisionEnd(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            bool isPolice = c.GameObject.tag == ObjectTag.Police;

            if ((isPlayer|| isPolice) && Time.CurrentTime > endStunTime) {
                state = State.Chasing;
            }
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void StartFollowing(List<Vector3> _wp, bool loop = false) {
            following = true;
            waypoints = _wp;
            wpIndex = 0;
            transform.position = waypoints[0];
            loopWaypoints = loop;
        }

        public void TakeDamage(float damage) {
            state = State.Stun;
            endStunTime = Time.CurrentTime + StunTime;
            ParticleUI.Instance.GiveOrder(transform.position + Vector3.Up * 2, ParticleType.RotatingStars, .7f);
        }


        // queries



        // other


    }

}