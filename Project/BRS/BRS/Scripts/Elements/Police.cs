// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;
using BRS.Engine.Physics;
using BRS.Engine.Physics.Colliders;

namespace BRS.Scripts {
    /// <summary>
    /// Records a player position and then follows it smoothly
    /// </summary>
    class Police : Component {

        // --------------------- VARIABLES ---------------------

        //public
        const float speed = 4f;
        const float changeThreshold = .5f;
        const float turnSmoothTime = .2f;
        const float bustRadius = .5f;

        //private
        bool following;
        int wpIndex;
        float angleRefVelocity;


        //reference
        List<Vector3> waypoints;


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {
            if (following) {
                bool wait = false;

                while (Vector3.DistanceSquared(transform.position, waypoints[wpIndex]) < changeThreshold * changeThreshold && !wait) {
                    if (wpIndex < waypoints.Count - 1) {
                        wpIndex++;
                    } else {
                        wait = true;
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

                //ghost.LookAt(target);
                //ghost.Translate(Vector3.Forward * speed * Time.DeltaTime);

                //float percent = Vector3.Distance(ghost.position, waypoints[wpIndex - 1]) / (Vector3.Distance(waypoints[wpIndex], waypoints[wpIndex - 1])+.01f);
                //percent = Utility.Clamp01(percent);
                //smoothTarget = Utility.SmoothDamp(smoothTarget, target , ref refTarget, .2f);
                //transform.LookAt(smoothTarget);

            }

            //CheckCollision();
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer) {
                Player p = c.GameObject.GetComponent<Player>();
                p.TakeDamage(20);
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void CheckCollision() {
            foreach(Player p in ElementManager.Instance.Players()) {
                if(Vector3.DistanceSquared(transform.position, p.transform.position) < bustRadius * bustRadius) {
                    p.TakeDamage(20);
                }
            }
        }

        public void StartFollowing(List<Vector3> _wp) {
            following = true;
            waypoints = _wp;
            wpIndex = 0;
            transform.position = waypoints[0];
        }

        



        // queries



        // other
        

    }
}