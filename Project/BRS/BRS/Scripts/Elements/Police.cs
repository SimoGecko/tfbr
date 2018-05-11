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
        const float speed = 4f;
        const float changeThreshold = .8f;
        const float turnSmoothTime = .2f;
        const float bustRadius = .5f;
        const float stunTime = 2f;


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

                while (Vector3.DistanceSquared(transform.position, waypoints[wpIndex]) < changeThreshold * changeThreshold && !wait) {
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
                    float smoothY = Utility.SmoothDampAngle(nowY, targetY, ref angleRefVelocity, turnSmoothTime);
                    transform.eulerAngles = new Vector3(0, smoothY, 0);
                    transform.Translate(Vector3.Forward * speed * Time.DeltaTime);
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
            if (isPlayer) {
                state = State.Chasing;
            }

            if (c.GameObject.tag == ObjectTag.Police) {
                state = State.Chasing;
            }
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        /*
        void CheckCollision() {
            foreach(Player p in ElementManager.Instance.Players()) {
                if(Vector3.DistanceSquared(transform.position, p.transform.position) < bustRadius * bustRadius) {
                    p.TakeDamage(20);
                }
            }
        }*/

        public void StartFollowing(List<Vector3> _wp, bool loop = false) {
            following = true;
            waypoints = _wp;
            wpIndex = 0;
            transform.position = waypoints[0];
            loopWaypoints = loop;
        }

        public void TakeDamage(float damage) {
            state = State.Stun;
            endStunTime = Time.CurrentTime + stunTime;
            ParticleUI.Instance.GiveOrder(transform.position + Vector3.Up * 2, ParticleType.RotatingStars, .7f);

        }





        // queries



        // other


    }

}