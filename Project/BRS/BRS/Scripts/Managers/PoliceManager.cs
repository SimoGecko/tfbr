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
        int totalNumPolicePerPlayer = 1;
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
        List<Vector3>[] recordedWaypoints;
        List<Vector3>[] staticWaypoints;

        private bool _isRecording;
        private bool _isSpawning;

        public static PoliceManager Instance;


        // --------------------- BASE METHODS ------------------
        public PoliceManager() {
            Instance = this;
        }

        public override void Start() {
            totalNumPolicePerPlayer = GameManager.lvlDifficulty;
        }

        public override void Update() {
            //DrawPoints();
        }

        public override void Reset() {
            foreach (Police police in _polices) {
                GameObject.Destroy(police.gameObject);
            }
        }

        // --------------------- CUSTOM METHODS ----------------





        // commands
        void SpawnPatrolPolice() {
            staticWaypoints = File.ReadPolicePaths("Load/PolicePaths.txt").ToArray();
            foreach (var listwp in staticWaypoints) {
                SpawnNewPolice(listwp, true);
            }
        }

        public void StartRound() {
            SetupRecording();

            _polices = new List<Police>();
            _totalSpawnedPerPlayer = 0;

            _spawningTime = ((float)RoundManager.RoundTime - startDelay) / totalNumPolicePerPlayer;
            new Timer(startDelay, CallSpawnPolice);

            SpawnPatrolPolice();
        }

        void SetupRecording() {
            targets = ElementManager.Instance.PlayerTransforms();
            numTargets = targets.Length;
            recordedWaypoints = new List<Vector3>[numTargets];

            for (int i = 0; i < numTargets; i++)
                recordedWaypoints[i] = new List<Vector3>();
            if (!_isRecording)
                RecordWaypoints();
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

        void SpawnNewPolice(List<Vector3> wp, bool loop = false) {
            if (!IsActive) return;

            Police pol = GameObject.Instantiate("policePrefab", Vector3.Zero, Quaternion.Identity).GetComponent<Police>();
            pol.StartFollowing(wp, loop);
            _polices.Add(pol);
        }

        void SpawnNewPolice(int pathToFollow) {
            SpawnNewPolice(recordedWaypoints[pathToFollow]);
        }


        // queries
        void DrawPoints() {
            for (int path = 0; path < numTargets; path++) {
                for (int i = 0; i < recordedWaypoints[path].Count; i++) {
                    Gizmos.DrawWireSphere(recordedWaypoints[path][i], 0.1f);
                }
            }
        }


        // other
        async void RecordWaypoints() {
            _isRecording = true;

            while (IsActive) {
                for (int i = 0; i < numTargets; i++) {
                    if (targets[i] != null) {
                        if (recordedWaypoints[i].Count < 2 || Vector3.DistanceSquared(targets[i].position, recordedWaypoints[i][recordedWaypoints[i].Count - 1]) > distThreshold * distThreshold) {
                            //  Vector3 pos = new Vector3(targets[i].position.X, 0.25f, targets[i].position.Z);
                            recordedWaypoints[i].Add(targets[i].position);
                        }
                    }
                }

                await Time.WaitForSeconds(recordRefreshTime);
            }

            _isRecording = false;
        }

        
    }
}