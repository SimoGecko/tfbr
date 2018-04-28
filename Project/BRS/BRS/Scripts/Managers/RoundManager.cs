// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace BRS.Scripts.Managers {
    class RoundManager : Component {
        ////////// deals with round stuff, ie time, current winner of rounds and score, ... (just for one round) //////////
        ////////// also stores the mode of the round, ie BOMBER / SPEEDER / SUPERCASH / POWERUP RUSH / ... //////////

        // --------------------- VARIABLES ---------------------

        //public
        public static int RoundTime = 15;
        public const int TimeBeforePolice = 5;
        public const int MoneyToWinRound = 20000;
        public const int NumRounds = 3;

        public Action OnRoundStartAction;
        public Action OnRoundAlmostEndAction;
        public Action OnRoundEndAction;

        public int Winner { get; private set; }

        //private
        Timer roundTimer;

        int[] teamWins;
        static int roundNumber;

        bool roundStarted;
        bool calledPolice;
        bool roundEnded;

        //reference
        public static RoundManager Instance;



        // --------------------- BASE METHODS ------------------
        public override void Start() { // done only once at beginning
            Instance = this;
            teamWins = new int[2];
            roundNumber = 0;
            RestartRound();
        }

        void RestartRound() { // done at beginning of every round
            roundTimer = new Timer(0, RoundTime, OnRoundEnd);
            roundNumber++;
            roundStarted = calledPolice = roundEnded = false;

            GameUI.Instance.StartMatch(roundTimer);
            GameManager.state = GameManager.State.Finished;

            CountDown();
        }

        public override void Update() {
            /*if (started) {
                if (roundTimer.Span.TotalSeconds < TimeBeforePolice && !calledPolice) {
                    OnPoliceComing();
                }
            }*/
            if (roundEnded) {
                if (Input.GetKeyDown(Keys.Space)) {
                    if (RoundNumber < NumRounds) {
                        GameManager.RestartCustom();
                        RestartRound();
                    } else OnGameEnd();
                }
            }
        }



        // --------------------- CUSTOM METHODS ---------b -------


        // commands
        async void CountDown() {
            for (int i = 3; i >= 0; i--) {
                await Time.WaitForSeconds(1f);
                RoundUI.instance.ShowCountDown(i);
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
            new Timer(RoundTime-TimeBeforePolice, () => OnPoliceComing());
        }

        void OnPoliceComing() {
            calledPolice = true;
            OnRoundAlmostEndAction?.Invoke();
            //Audio.SetLoop("police", true);
            Audio.Play("police", Vector3.Zero);
            GameUI.Instance.UpdatePoliceComing();
        }

        void OnRoundEnd() {
            //Audio.Stop("police");
            //Audio.SetLoop("police", false);
            roundEnded = true;
            GameManager.state = GameManager.State.Finished;

            //FIND WINNER
            //reset // TODO reorganize
            for (int i=0; i<GameManager.NumPlayers; i++)
                RoundUI.instance.ShowEndRound(i, RoundUI.EndRoundCondition.Success);

            foreach (Base b in ElementManager.Instance.Bases()) b.NotifyRoundEnd();

            Winner = FindWinner();
            GameUI.Instance.UpdateGameWinnerUI(Winner);
            teamWins[Winner]++;
            BaseUI.Instance.UpdateBaseUIWins(Winner);

            //ready to restart
            OnRoundEndAction?.Invoke();
            //new Timer(5, GameManager.RestartCustom, true);
        }

        

        void OnGameEnd() {
            //save scores
            UpdateRanking();
            //return to menu
            SceneManager.LoadScene("Level2");
        }


        void UpdateRanking() {//@nico move somewhere else
            List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/ranking" + (RoundTime / 60) + "min.txt");
            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                Base b = ElementManager.Instance.Base(i % 2);
                rankinglist.Add(new Tuple<string, string>(PlayerUI.Instance.GetPlayerName(i), b.TotalMoney.ToString()));
            }
            rankinglist.Sort((x,y) => -1* Int32.Parse(x.Item2).CompareTo(Int32.Parse(y.Item2)));
            File.WriteRanking("Load/Rankings/ranking" + (RoundTime / 60) + "min.txt", rankinglist, 5);
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
            int enemyTeam = ElementManager.Instance.Base(1 - teamIndex).TotalMoney;
            return (team > enemyTeam) ? 1 : (enemyTeam > team) ? 2 : 0;
        }

        public static string RankToString(int rank) {
            return rank == 1 ? "1." : rank == 2 ? "2." : "-";
        }

        // other


        public static int RoundNumber { get { return roundNumber; } }


    }
}