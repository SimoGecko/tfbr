// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BRS.Scripts {
    class PoliceManager : Component {
        ////////// controls the police cars functions //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float distThreshold = .2f; // if player hasn't moved this much don't record
        const float recordRefreshTime = .3f;
        const int totalNumPolicePerPlayer = 3;
        const int startDelay = 5; // how much time to wait before first police spawn

        public static bool IsActive = true;

        //private
        //bool record = true;
        int numTargets;
        private List<Police> _polices;
        private int _totalSpawnedPerPlayer;
        private float _spawningTime;

        //reference
        Transform[] targets;
        List<Vector3>[] waypoints;

        private bool _isRecording;
        private bool _isSpawning;

        public static PoliceManager Instance;


        // --------------------- BASE METHODS ------------------
        public PoliceManager() {
            Instance = this;
        }

        public override void Start() {
        }

        public override void Update() {
            //DrawPoints();
        }

        public override void Reset() {
            foreach (Police police in _polices) {
                GameObject.Destroy(police.gameObject);
            }
        }

        public void StartRound() {
            targets = ElementManager.Instance.PlayerTransforms();
            numTargets = targets.Length;

            waypoints = new List<Vector3>[numTargets];
            for (int i = 0; i < numTargets; i++) waypoints[i] = new List<Vector3>();

            _totalSpawnedPerPlayer = 0;
            _polices = new List<Police>();

            if (!_isRecording) {
                RecordWaypoints();
            }

            _spawningTime = ((float)RoundManager.RoundTime - startDelay) / totalNumPolicePerPlayer;
            new Timer(startDelay, CallSpawnPolice);
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void SpawnNewPolice(int pathToFollow) {
            if (!IsActive)
                return;

            Police pol = GameObject.Instantiate("policePrefab", new Vector3(pathToFollow * 4, 1.0f, 0), Quaternion.Identity).GetComponent<Police>();
            pol.StartFollowing(waypoints[pathToFollow]);
            _polices.Add(pol);
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
            _isRecording = true;

            while (IsActive) {
                for (int i = 0; i < numTargets; i++) {
                    if (targets[i] != null) {
                        if (waypoints[i].Count < 2 || Vector3.DistanceSquared(targets[i].position, waypoints[i][waypoints[i].Count - 1]) > distThreshold * distThreshold) {
                            //  Vector3 pos = new Vector3(targets[i].position.X, 0.25f, targets[i].position.Z);
                            waypoints[i].Add(targets[i].position);
                        }
                    }
                }

                await Time.WaitForSeconds(recordRefreshTime);
            }

            _isRecording = false;
        }

        void CallSpawnPolice() {
            for (int i = 0; i < numTargets; i++) {
                SpawnNewPolice(i);
            }

            ++_totalSpawnedPerPlayer;

            if (_totalSpawnedPerPlayer < totalNumPolicePerPlayer) {
                new Timer(_spawningTime, CallSpawnPolice);
            }
        }
    }
}