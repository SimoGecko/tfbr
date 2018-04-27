// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;


namespace BRS.Scripts.Managers {
    class RoundManager : Component {
        ////////// deals with round stuff, ie time, current winner of rounds and score, ... (just for one round) //////////
        ////////// also stores the mode of the round, ie BOMBER / SPEEDER / SUPERCASH / POWERUP RUSH / ... //////////

        // --------------------- VARIABLES ---------------------

        //public
        public static int RoundTime = 150;
        public const int TimeBeforePolice = 10;
        public const int MoneyToWinRound = 20000;

        //private
        private Timer _rt;
        private Base[] _bases;
        bool calledPolice = false;
        public int Winner {get; private set;}
        bool started = false;
        //reference
        public static RoundManager Instance;
        public Action OnRoundStartAction;
        public Action OnRoundAlmostEndAction;
        public Action OnRoundEndAction;




        // --------------------- BASE METHODS ------------------
        public override void Start() {
            Instance = this;
            _rt = new Timer(0, RoundTime, OnRoundEnd);
            GameUI.Instance.StartMatch(_rt);
            GameManager.state = GameManager.State.Finished;

            CountDown();
        }

        public override void Update() {
            if (started) {
                if (_rt.Span.TotalSeconds < TimeBeforePolice && !calledPolice) {
                    calledPolice = true;
                    //Audio.SetLoop("police", true);
                    Audio.Play("police", Vector3.Zero);
                    GameUI.Instance.UpdatePoliceComing();
                }
            }
        }



        // --------------------- CUSTOM METHODS ---------b -------


        // commands

        void OnRoundStart() {
            GameManager.state = GameManager.State.Playing;
            OnRoundStartAction?.Invoke();

            PoliceManager.Instance.StartRound();

            new Timer(RoundTime * .8f, () => OnRoundAlmostEndAction?.Invoke());
            started = true;
        }

        void OnRoundEnd() {
            //Audio.Stop("police");
            //Audio.SetLoop("police", false);

            //reset
            for(int i=0; i<GameManager.NumPlayers; i++)
                RoundUI.instance.ShowEndRound(i, RoundUI.EndRoundCondition.Success);


            NotifyBases();

            Tuple<int, int> winner = FindWinner();
            OnRoundEndAction?.Invoke();
            Winner = winner.Item1;

            GameUI.Instance.UpdateGameWinnerUI(winner.Item1);
            //UpdateRanking();
            GameManager.Instance.OnRoundEnd(winner.Item1);
        }

        void UpdateRanking() {
            List<Tuple<string, string>> rankinglist = File.ReadRanking("Load/Rankings/ranking" + (RoundTime / 60) + "min.txt");

            for (int i = 0; i < GameManager.NumPlayers; ++i) {
                rankinglist.Add(new Tuple<string, string>(PlayerUI.Instance.GetPlayerName(i), _bases[i%2].TotalMoney.ToString()));
            }

            rankinglist.Sort((x,y) => -1* Int32.Parse(x.Item2).CompareTo(Int32.Parse(y.Item2)));

            File.WriteRanking("Load/Rankings/ranking" + (RoundTime / 60) + "min.txt", rankinglist, 5);
        }

        /*
        void FindBases() {
            //find bases
            GameObject[] basesObject = GameObject.FindGameObjectsWithTag(ObjectTag.Base);
            if (basesObject.Length < 1) {
                Debug.LogError("could not find the bases"); // avoids tag messup
            } else {
                _bases = new Base[basesObject.Length];
                for (int i = 0; i < _bases.Length; i++)
                    _bases[i] = basesObject[i].GetComponent<Base>();
            }
        }*/

        void NotifyBases() {
            //for (int i = 0; i < bases.Length; i++)
            //bases[i].NotifyRoundEnd();
            foreach (Base b in ElementManager.Instance.Bases()) b.NotifyRoundEnd();
        }

        public static int GetRank(int teamIndex) {
            int team = ElementManager.Instance.Base(teamIndex).TotalMoney;
            int enemyTeam = ElementManager.Instance.Base(1-teamIndex).TotalMoney;
            return (team > enemyTeam) ? 1 : (enemyTeam > team) ? 2 : 0; 
        }
        public static string RankToString(int rank) {
            return rank == 1 ? "1." : rank == 2 ? "2." : "-";
        }

        // queries
        Tuple<int, int> FindWinner() {
            Base[] bases = ElementManager.Instance.Bases();
            int winner = 0;
            int maxCash = bases[0].TotalMoney;

            for (int i = 1; i < bases.Length; i++) {
                int totmoney = bases[i].TotalMoney;

                if (totmoney > maxCash) { // TODO deal with tie
                    winner = i;
                    maxCash = totmoney;
                }
            }

            return new Tuple<int, int>(winner, maxCash);
        }


        // other
        async void CountDown() {
            for(int i=3; i>=0; i--) {
                RoundUI.instance.ShowCountDown(i);
                if(i==0)
                    OnRoundStart();
                await Time.WaitForSeconds(1f);
            }
            RoundUI.instance.ShowCountDown(-1);//disables it
        }

    }
}