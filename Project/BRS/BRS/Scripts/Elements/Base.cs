// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BRS.Engine.Physics;
using BRS.Scripts.Managers;

namespace BRS.Scripts.Elements {
    class Base : LivingEntity {
        ////////// base in the game that has health and collects money //////////

        // --------------------- VARIABLES ---------------------

        //public
        public int TotalMoney { get; private set; }
        public int TotalMoneyPenalty { get; private set; }
        public Color BaseColor { get; private set; }
        // deload done
        public bool FullDeloadDone = false;

        //private
        private const float TimeBetweenUnloads = .03f;
        private const int MoneyPenaltyAmount = 1000;
        private const int _bundlesPerStack = 10;
        private const int _columnsPerRow = 2;
        private const float _margin = 1f;

        private int _shownMoneyStacks = 0;
        private readonly int _baseIndex = 0;

        private List<GameObject> _moneyGameObjects = new List<GameObject>();

        public System.Action OnBringBase;

        //reference


        // --------------------- BASE METHODS ------------------
        public Base(int baseIndex) {
            _baseIndex = baseIndex;
            BaseColor = Graphics.ColorIndex(baseIndex);
        }

        public override void Start() {
            base.Start();
            TotalMoney = 0;
            TotalMoneyPenalty = 0;

            _shownMoneyStacks = 0;

            // Todo: This causes currently a strange loop. Players are need UI to be started already, but the UI which contains the Suggestions uses the player and bases to be startet first!
            UpdateUI();
        }

        public override void Update() {
            //UpdateUI();
        }

        public override void Reset() {
            foreach (GameObject go in _moneyGameObjects) {
                GameObject.Destroy(go);
            }

            _moneyGameObjects.Clear();
            BaseUI.Instance.UpdateBaseUI(_baseIndex, 0);

            Start();
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;

            if (isPlayer) {
                Player p = c.GameObject.GetComponent<Player>();

                if (p.TeamIndex == _baseIndex) {
                    PlayerInventory pi = p.gameObject.GetComponent<PlayerInventory>();
                    pi.CanDeload = true;
                    DeloadPlayerProgression(pi);
                }
            }
        }

        public override void OnCollisionEnd(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer) {
                Player p = c.GameObject.GetComponent<Player>();
                if (p.TeamIndex == _baseIndex) {
                    p.gameObject.GetComponent<PlayerInventory>().CanDeload = false;
                }
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        void UpdateUI() {
            BaseUI.Instance.UpdateBaseUI(_baseIndex, TotalMoney);
        }

        public void NotifyRoundEnd() {
            foreach (var p in ElementManager.Instance.Team(_baseIndex)) {
                PlayerInventory pi = p.gameObject.GetComponent<PlayerInventory>();

                if (PlayArea.IsInsidePlayArea(p.transform.position)) { // PROXIMITY CHECK
                    //apply penalty (could happen twice)
                    TotalMoneyPenalty += MoneyPenaltyAmount;
                    RoundUI.instance.ShowEndRound(p.PlayerIndex, RoundUI.EndRoundCondition.Busted);
                }
            }
            UpdateUI();
        }


        // queries
       

        // other
        async void DeloadPlayerProgression(PlayerInventory pi) {
            bool wasDeloading = false;
            if (pi.CarryingValue > 0) {
                Audio.Play("leaving_cash_base", transform.position);
                OnBringBase?.Invoke();
                wasDeloading = true;
            }

            while (pi.CarryingValue > 0 && pi.CanDeload) {
                TotalMoney += pi.ValueOnTop;
                pi.DeloadOne();
                UpdateUI();
                UpdateMoneyStack();
                Input.Vibrate(.01f, .01f, pi.gameObject.GetComponent<Player>().PlayerIndex);
                await Time.WaitForSeconds(TimeBetweenUnloads);
            }

            if (wasDeloading) {
                FullDeloadDone = true;
                Timer t = new Timer(3, () => FullDeloadDone = false);
            }
        }

        void UpdateMoneyStack() {
            int totalStacksToShow = TotalMoney / 1000;

            while (_shownMoneyStacks < totalStacksToShow) {
                GameObject newBundle = GameObject.Instantiate("cashStack", transform.position + 0.5f * Vector3.Up, MyRandom.YRotation());
                _moneyGameObjects.Add(newBundle);

                Vector3 size = BoundingBoxHelper.CalculateSize(newBundle.Model, newBundle.transform.scale);

                int stackId = _shownMoneyStacks / _bundlesPerStack;
                int rowId = stackId % _columnsPerRow;
                int colId = stackId / _columnsPerRow;
                Vector3 up = (0.1f + (_shownMoneyStacks % _bundlesPerStack) * size.Y) * Vector3.Up;
                Vector3 right = (rowId * _margin + size.X * rowId) * Vector3.Right;
                Vector3 back = (colId * _margin + size.Z * colId) * Vector3.Backward;

                newBundle.transform.position = transform.position + up + right + back;

                ++_shownMoneyStacks;
            }
        }

    }

}