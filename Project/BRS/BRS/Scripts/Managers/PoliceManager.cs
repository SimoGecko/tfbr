// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BRS.Engine;
using BRS.Scripts.Managers;
using BRS.Scripts.PlayerScripts;

namespace BRS.Scripts {
    class PoliceManager : Component {
        ////////// controls the police cars functions //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float distThreshold = .2f; // if player hasn't moved this much don't record
        const float recordRefreshTime = .3f;
        const int totNumPolice = 6;
        const int startDelay = 5; // how much time to wait before first police spawn

        //private
        bool record = true;
        int numTargets;

        //reference
        Transform[] targets;
        List<Vector3>[] waypoints;


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            targets = ElementManager.Instance.PlayerTransforms();
            numTargets = targets.Length;

            waypoints = new List<Vector3>[numTargets];
            for(int i=0; i< numTargets; i++)  waypoints[i] = new List<Vector3>();

            RecordWaypoints();
            CallSpawnPolice();
        }

        public override void Update() {
            //DrawPoints();
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SpawnNewPolice(int pathToFollow) {
            Police pol = GameObject.Instantiate("policePrefab").GetComponent<Police>();
            pol.StartFollowing(waypoints[pathToFollow]);
        }


        // queries
        void DrawPoints() {
            for (int path = 0; path < numTargets; path++) {
                for (int i = 0; i < waypoints[path].Count; i++) {
                    Gizmos.DrawWireSphere(waypoints[path][i], 0.1f);
                }
            }
        }


        // other
        async void RecordWaypoints() {
            while (record) {
                for (int i = 0; i < numTargets; i++) {
                    if (targets[i] !=null) {
                        if (waypoints[i].Count < 2 || Vector3.DistanceSquared(targets[i].position, waypoints[i][waypoints[i].Count - 1]) > distThreshold * distThreshold)
                            waypoints[i].Add(targets[i].position);
                    }
                }
                await Time.WaitForSeconds(recordRefreshTime);
            }
        }

        async void CallSpawnPolice() {
            await Time.WaitForSeconds(startDelay);
            float timeBetweenCalls = ((float)RoundManager.RoundTime - startDelay)/(totNumPolice/numTargets);
            int totSpawned = 0;
            while (totSpawned < totNumPolice) {
                for (int i = 0; i < numTargets; i++) {
                    SpawnNewPolice(i);
                    totSpawned++;
                }
                await Time.WaitForSeconds(timeBetweenCalls);
            }
        }
    }
}