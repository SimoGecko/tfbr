// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.PostProcessing;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
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
            roundTimer = new Timer(RoundTime, OnRoundEnd, boundToRound:true);
            roundNumber++;
            roundStarted = calledPolice = roundEnded = false;
            RoundUI.instance.ShowEndRound(false);

            GameUI.Instance.StartMatch(roundTimer);
            GameManager.state = GameManager.State.Ended;

            CountDown();
        }

        public override void Update() {
        }



        // --------------------- CUSTOM METHODS ---------b -------


        // commands


        async void CountDown() {
            for (int i = 3; i >= 0; i--) {
                await Time.WaitForSeconds(1f);
                RoundUI.instance.ShowCountDown(i);
                if(i>0) Audio.Play("start321", Vector3.Zero);
                else Audio.Play("start0", Vector3.Zero);
                if (i == 0) OnRoundStart();
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
            //Audio.Stop("police");
            //Audio.SetLoop("police", false);
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
            new Timer(TimeBetweenRounds, () => TryRestartRound(), true);
        }

        void TryRestartRound() {
            UpdateRanking();
            Heatmap.instance.SaveHeatMap();
            bool oneAlreadyWon2Rounds = false;
            for (int i = 0; i < GameManager.NumTeams; i++)
                oneAlreadyWon2Rounds = oneAlreadyWon2Rounds || teamWins[i] >= 2;
            if (RoundNumber < NumRounds && !oneAlreadyWon2Rounds) {
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


        void UpdateRanking() {//@nico move somewhere else
            List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/ranking" + RoundTime / 60 + " min" + GameManager.NumPlayers + "P.txt");
            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                Base b = ElementManager.Instance.Base(i % 2);
                rankinglist.Add(new Tuple<string, string>(PlayerUI.Instance.GetPlayerName(i), b.TotalMoney.ToString()));
            }
            rankinglist.Sort((x, y) => -1 * Int32.Parse(x.Item2).CompareTo(Int32.Parse(y.Item2)));
            File.WriteRanking("Load/Rankings/ranking" + RoundTime / 60 + " min" + GameManager.NumPlayers + "P.txt", rankinglist, 10);
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