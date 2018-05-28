// (c) Simone Guggiari / Nicolas Huart (camera transition + rankings update only) 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BRS.Engine;
using BRS.Engine.Menu;
using BRS.Engine.PostProcessing;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;


namespace BRS.Scripts.Managers {
    class RoundManager : Component {
        ////////// deals with round stuff, ie time, current winner of rounds and score, ... (just for one round) //////////
        ////////// also stores the mode of the round, ie BOMBER / SPEEDER / SUPERCASH / POWERUP RUSH / ... //////////

        // --------------------- VARIABLES ---------------------

        //public
        public static int RoundTime = 120;
        public const int TimeBeforePolice = 15;
        public const int MoneyToWinRound = 20000;
        public const int NumRounds = 3;
        public const int TimeBetweenRounds = 3;

        public Action OnRoundStartAction;
        public Action OnRoundAlmostEndAction;
        public Action OnPoliceComingAction;
        public Action OnRoundEndAction;

        public int Winner { get; private set; }

        //private
        Timer roundTimer;

        static int[] teamWins;
        static int roundNumber;

        bool roundStarted;
        bool calledPolice;
        bool roundEnded;

        bool startingFirstTime = true;

        // Cam transition parameters
        public bool CamMoving;

        Vector3[] _velocityPos = { Vector3.Zero, Vector3.Zero };
        Vector3[] _velocityRot = { Vector3.Zero, Vector3.Zero };
        float[] _newTransitionTime = { 1.4f, 1.4f };

        Vector3[] _posCam;
        Vector3[] _rotCam;

        //reference
        public static RoundManager Instance;


        // --------------------- BASE METHODS ------------------
        public override void Start() { // done only once at beginning
            Instance = this;
            if (startingFirstTime) {
                startingFirstTime = false;
                teamWins = new int[GameManager.NumTeams];
                roundNumber = 0;
            }

            RestartRound();
        }

        void RestartRound() { // done at beginning of every round
            SetUpStartCamTransition();

            UpdateLvlAllPlayers();

            roundTimer = new Timer(RoundTime, OnRoundEnd, boundToRound:true);
            roundNumber++;
            roundStarted = calledPolice = roundEnded = false;
            RoundUI.instance.ShowEndRound(false);

            GameUI.Instance.StartMatch(roundTimer);
            GameManager.state = GameManager.State.Ended;

            CountDown();
        }

        public override void Update() {
            if (roundEnded) {
                //check for input to restart
                if (InputRestart()) TryRestartRound();
            }

            if (CamMoving)
                CamTransitionForCountDown();
        }

        // --------------------- CUSTOM METHODS ---------b -------

        void UpdateLvlAllPlayers() {
            for (int i = 0; i < GameManager.NumPlayers; i++) {
                // update lvl
                Player p = ElementManager.Instance.Player(i);
                p.PlayerLvl = GetLvlSinglePlayer(p);

                // update capacity according to level
                int currIdModel = 0;
                if (ScenesCommunicationManager.Instance != null) 
                    if (ScenesCommunicationManager.Instance.PlayersInfo.ContainsKey("player_" + i))
                        currIdModel = ScenesCommunicationManager.Instance.PlayersInfo["player_" + i].Item2;

                float coeffMult = 1 + (float)((float)p.PlayerLvl / (float)Player.PlayermaxLvl);
                p.gameObject.GetComponent<PlayerInventory>().SetCapacity((int)(ScenesCommunicationManager.ValuesStats[currIdModel].Capacity * coeffMult));
                p.gameObject.GetComponent<PlayerAttack>().AttackDistance = (int)(ScenesCommunicationManager.ValuesStats[currIdModel].AttackDistance * coeffMult);
                p.gameObject.GetComponent<PlayerMovement>().SetMaxSpeed((int)(ScenesCommunicationManager.ValuesStats[currIdModel].MaxSpeed * coeffMult));
                p.gameObject.GetComponent<PlayerMovement>().SetMinSpeed((int)(ScenesCommunicationManager.ValuesStats[currIdModel].MinSpeed * coeffMult));
            }
        }

        int GetLvlSinglePlayer(Player p) {
            string[] RankingPlayersText = { "1P", "2P", "4P" };
            string[] RankingDurationText = { "2 min", "3 min", "5 min", "10 min" };
            float[] coeffMult = { 1, 1.5f, 3 };

            int highscore = 0;
            int count = 0;

            foreach (var noPlayers in RankingPlayersText) {
                foreach (var durationRound in RankingDurationText) {
                    List<Tuple<string, string>> rankinglist = File.ReadRanking("ranking" + durationRound + noPlayers + ".txt"); //File.ReadRanking("Load /Rankings/ranking" + durationRound + noPlayers + ".txt");

                    // Text component with player's name and score
                    foreach (var aPerson in rankinglist) {
                        if (aPerson.Item1 == p.PlayerName) {
                            highscore += (int)(int.Parse(aPerson.Item2) * float.Parse(noPlayers[0].ToString()) / int.Parse(durationRound.Split(' ')[0]));
                            count++;
                            break;
                        }
                    }
                }
            }

            highscore += p.highscore;
            if (count != 0)
                highscore /= (count + p.highscore == 0 ? 0 : 1);
            p.highscore = highscore;

            int level = highscore / 3000;

            if (level > Player.PlayermaxLvl)
                level = Player.PlayermaxLvl;
            if (level < 0)
                level = 0;

            return level;
        }


        /// <summary>
        /// Set up the start information for the 3-2-1 count down camera transition
        /// </summary>
        void SetUpStartCamTransition() {
            // Start position and rotation for the cameras
            for (int i = 0; i < Screen.Cameras.Length; ++i) {
                Screen.Cameras[i].transform.position = new Vector3(0, 90, 30) + ((i % 2) == 0 ? -1 : 1) * new Vector3(5, 0, 0);
                Screen.Cameras[i].transform.eulerAngles = new Vector3(-85, 0, 0);
            }

            // Target of the transition
            _posCam = new Vector3[Screen.Cameras.Length];
            _rotCam = new Vector3[Screen.Cameras.Length];

            for (int i = 0; i < Screen.Cameras.Length; ++i) {
                _posCam[i] = Screen.Cameras[i].gameObject.GetComponent<CameraController>().GetPlayerPosition() + CameraController.Offset /*+ ((i % 2) == 0 ? 1 : -1) *new Vector3(5, 0, 0)*/;
                _rotCam[i] = CameraController.StartAngle;
            }

            // Start transition
            CamMoving = true;
        }

        /// <summary>
        /// Update the camera during the camera transition
        /// </summary>
        void CamTransitionForCountDown() {
            // Update camera position and rotation
            for (int i = 0; i < Screen.Cameras.Length; ++i) {
                Screen.Cameras[i].transform.position = Utility.SmoothDamp(Screen.Cameras[i].transform.position, _posCam[i],ref  _velocityPos[i%2], _newTransitionTime[i % 2]);
                Screen.Cameras[i].transform.eulerAngles = Utility.SmoothDampAngle(Screen.Cameras[i].transform.eulerAngles, _rotCam[i], ref _velocityRot[i%2], _newTransitionTime[i % 2]);
            }
        }

        bool InputRestart() {
            bool Apressed = false;
            for (int i = 0; i < GameManager.NumPlayers; i++) Apressed = Apressed || Input.GetButtonDown(Buttons.A, i);
            return Input.GetKeyDown(Keys.Space) || Input.GetKeyDown(Keys.Space) || Apressed;
        }

        async void CountDown() {
            for (int i = 3; i >= 0; i--) {
                await Time.WaitForSeconds(1f);
                RoundUI.instance.ShowCountDown(i);
                if(i>0) Audio.Play("start321", Vector3.Zero);
                else Audio.Play("start0", Vector3.Zero);
                if (i == 0) {
                    foreach (Camera c in Screen.Cameras) {
                        //c.transform.position = c.gameObject.GetComponent<CameraController>().GetPlayerPosition() + CameraController.Offset;
                        //c.transform.eulerAngles = CameraController.StartAngle;
                        c.gameObject.GetComponent<CameraController>().Reset();
                    }

                    CamMoving = false;

                    OnRoundStart();
                }
            }
            await Time.WaitForSeconds(1f);
            RoundUI.instance.ShowCountDown(-1);//disables it
        }

        void OnRoundStart() {
            GameManager.state = GameManager.State.Playing;
            roundStarted = true;
            OnRoundStartAction?.Invoke();
            PoliceManager.Instance.StartRound();
            new Timer(RoundTime- 60, () => OnRoundAlmostEnd(), boundToRound:true);
            new Timer(RoundTime-TimeBeforePolice, () => OnPoliceComing(), boundToRound:true);
        }

        void OnRoundAlmostEnd() {
            OnRoundAlmostEndAction?.Invoke();
        }

        void OnPoliceComing() {
            calledPolice = true;
            OnPoliceComingAction?.Invoke();
            //Audio.SetLoop("police", true);
            PlayPoliceSoundLoop();
            GameUI.Instance.UpdatePoliceComing();
        }

        async void PlayPoliceSoundLoop() {
            float duration = Audio.GetDuration("police")-.2f;
            int numRep = (int)System.Math.Round(TimeBeforePolice / duration);

            for(int i=0; i<numRep; i++) {
                if (GameManager.GameEnded) return;
                Audio.Play("police", Vector3.Zero, .0005f);
                await Time.WaitForSeconds(duration);
            }
        }

        

        void OnRoundEnd() {
            roundEnded = true;
            GameManager.state = GameManager.State.Ended;

            //FIND WINNER
            //reset // TODO reorganize
            for (int i = 0; i < GameManager.NumPlayers; i++) {
                RoundUI.instance.ShowEndRound(i, RoundUI.EndRoundCondition.Timesup);
            }

            // Remove all dynamic shaders
            PostProcessingManager.Instance.RemoveShader(PostprocessingType.BlackAndWhite);
            PostProcessingManager.Instance.RemoveShader(PostprocessingType.ShockWave);
            PostProcessingManager.Instance.RemoveShader(PostprocessingType.Wave);

            // Cleanup timers
            Time.ClearRoundTimers();

            foreach (Base b in ElementManager.Instance.Bases()) b.NotifyRoundEnd();

            Winner = FindWinner();
            GameUI.Instance.UpdateGameWinnerUI(Winner);
            teamWins[Winner]++;
            BaseUI.Instance.UpdateBaseUIWins(Winner);

            bool finalRound = roundNumber == NumRounds;

            for (int i = 0; i < GameManager.NumPlayers; i++) {
                bool busted = RoundUI.instance.Busted(i);
                if (!busted) {
                    if (ElementManager.Instance.Player(i).TeamIndex == Winner)
                        RoundUI.instance.ShowEndRound(i, finalRound ? RoundUI.EndRoundCondition.Youwon : RoundUI.EndRoundCondition.Success);
                }
            }


            //ready to restart
            OnRoundEndAction?.Invoke();
            //new Timer(TimeBetweenRounds, () => TryRestartRound(), true);
        }

        void TryRestartRound() {
            UpdateRanking();
            //Heatmap.instance.SaveHeatMap();
            bool oneAlreadyWon2Rounds = false;
            for (int i = 0; i < GameManager.NumTeams; i++)
                oneAlreadyWon2Rounds = oneAlreadyWon2Rounds || teamWins[i] >= 2;
            if (RoundNumber < NumRounds /*&& !oneAlreadyWon2Rounds*/) {
                GameManager.RestartCustom();
                RestartRound();
            } else {
                OnGameEnd();
            }
        }


        void OnGameEnd() {
            //save scores
            //UpdateRanking();
            //return to menu
            SceneManager.LoadScene("LevelMenu");
        }

        public void UpdateRanking() {
            List<Tuple<string, string>> rankinglist = File.ReadRanking("ranking" + RoundTime / 60 + " min" + GameManager.NumPlayers + "P.txt");

            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                Base b = ElementManager.Instance.Base(i % 2);
                Player p = ElementManager.Instance.Player(i);
                p.highscore += b.TotalMoney;

                rankinglist.Add(new Tuple<string, string>(PlayerUI.Instance.GetPlayerName(i), b.TotalMoney.ToString()));
            }
            rankinglist.Sort((x, y) => -1 * Int32.Parse(x.Item2).CompareTo(Int32.Parse(y.Item2)));
            File.WriteRanking("ranking" + RoundTime / 60 + " min" + GameManager.NumPlayers + "P.txt", rankinglist, 10);
        }

        // queries
        int FindWinner() { // TODO deal with tie
            Base[] bases = ElementManager.Instance.Bases();
            int winner = 0;
            int maxCash = bases[0].TotalMoney;

            for (int i = 1; i < bases.Length; i++) {
                int totmoney = bases[i].TotalMoney;

                if (totmoney > maxCash) {
                    winner = i;
                    maxCash = totmoney;
                }
            }
            return winner;
        }

        int BaseCash(int baseIndex) {
            return ElementManager.Instance.Bases()[baseIndex].TotalMoney;
        }

        public static int GetRank(int teamIndex) {
            int team = ElementManager.Instance.Base(teamIndex).TotalMoney;
            int enemyTeam = ElementManager.Instance.EnemyBase(teamIndex).TotalMoney;
            return (team > enemyTeam) ? 1 : (enemyTeam > team) ? 2 : 0;
        }

        public static string RankToString(int rank) {
            return rank == 1 ? "1." : rank == 2 ? "2." : "-";
        }

        public static int NumWins(int TeamIndex) { return teamWins[TeamIndex]; }

        // other


        public static int RoundNumber { get { return roundNumber; } }


    }
}