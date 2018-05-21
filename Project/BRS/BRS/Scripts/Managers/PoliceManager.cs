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
        const float recordRefreshTime = .3f; // refresh rate to check for point
        const int startDelay = 20; // how much time to wait before first police spawn
        int[] patrolPolicePerDifficulty = new int[] { 1, 2, 3 }; // patrol = follow predefined path
        int[] followPolicePerDifficulty = new int[] { 0, 3, 6 }; // follow = chase player

        public static bool IsActive = true;

        //private
        int numTargets;

        //reference
        Transform[] targets;
        List<Vector3>[] recordedWaypoints;
        List<Vector3>[] staticWaypoints;
        private List<Vector3> _startPositions;

        private bool recordingInitialized;

        public static PoliceManager Instance;


        // --------------------- BASE METHODS ------------------
        public PoliceManager(List<Vector3> startPositions) {
            _startPositions = startPositions;
            Instance = this;
        }

        public override void Start() {
            staticWaypoints = new List<Vector3>[0];
            recordingInitialized = false;
        }

        public override void Update() {
            //DrawPoints();
        }

        public override void Reset() {
            foreach (Police police in ElementManager.Instance.Polices()) {
                GameObject.Destroy(police.gameObject);
            }
            recordingInitialized = false;
        }

        // --------------------- CUSTOM METHODS ----------------





        // commands
        

        public void StartRound() { // why not use the start method and then recall it on restart

            SetupRecording();
            SpawnPatrolPolice();
            SpawnFollowPoliceCoroutine();
        }

        void SetupRecording() {
            if (recordingInitialized) return;
            recordingInitialized = true;
            targets = ElementManager.Instance.PlayerTransforms();
            numTargets = targets.Length;
            recordedWaypoints = new List<Vector3>[numTargets];
            for (int i = 0; i < numTargets; i++)
                recordedWaypoints[i] = new List<Vector3> { _startPositions[i] };

            RecordWaypointsCoroutine();
        }

        void SpawnPatrolPolice() {
            staticWaypoints = File.ReadPolicePaths("Load/UnitySceneData/PolicePaths_level" + GameManager.LvlScene + ".txt").ToArray();
            int numPatrolPolice = MathHelper.Min(staticWaypoints.Length, patrolPolicePerDifficulty[GameManager.lvlDifficulty]);

            for (int i = 0; i < numPatrolPolice; i++) {
                SpawnNewPolice(staticWaypoints[i], true);
            }
        }

        async void SpawnFollowPoliceCoroutine() {
            int numFollowPolice = followPolicePerDifficulty[GameManager.lvlDifficulty];
            float spawnDelay = ((float)RoundManager.RoundTime - startDelay) / numFollowPolice;
            spawnDelay = MathHelper.Max(spawnDelay, 1f);

            await Time.WaitForSeconds(startDelay);

            for(int i=0; i<numFollowPolice; i++) {
                SpawnNewPolice(i % numTargets);
                await Time.WaitForSeconds(spawnDelay);
            }
        }

        void SpawnNewPolice(List<Vector3> wp, bool loop = false) {
            if (!IsActive) return;

            Police pol = GameObject.Instantiate("policePrefab", Vector3.Zero, Quaternion.Identity).GetComponent<Police>();
            pol.StartFollowing(wp, loop);
            ElementManager.Instance.Add(pol);
        }

        void SpawnNewPolice(int pathToFollow) {
            SpawnNewPolice(recordedWaypoints[pathToFollow]);
        }


        // queries


        void DrawPoints() {
            for (int i = 0; i < numTargets; i++) {
                DrawPath(recordedWaypoints[i]);
            }
            for(int i=0; i<staticWaypoints.Length; i++) {
                DrawPath(staticWaypoints[i]);
            }
        }

        void DrawPath(List<Vector3> path) {
            for (int i = 0; i < path.Count; i++) {
                Gizmos.DrawWireSphere(path[i], 0.1f);
            }
        }


        // other
        async void RecordWaypointsCoroutine() {
            while (IsActive) {
                for (int i = 0; i < numTargets; i++) {
                    if (targets[i] != null) {
                        if (recordedWaypoints[i].Count < 2 || Vector3.DistanceSquared(targets[i].position, recordedWaypoints[i][recordedWaypoints[i].Count - 1]) > distThreshold * distThreshold) {
                            Vector3 pos = new Vector3(targets[i].position.X, 0.01f, targets[i].position.Z);
                            recordedWaypoints[i].Add(pos);
                        }
                    }
                }
                await Time.WaitForSeconds(recordRefreshTime);
            }
        }


    }
}