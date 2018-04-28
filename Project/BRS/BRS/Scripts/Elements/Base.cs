// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine;
using BRS.Engine.Physics.Colliders;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BRS.Engine.Physics;

namespace BRS.Scripts.Elements {
    class Base : LivingEntity {
        ////////// base in the game that has health and collects money //////////

        // --------------------- VARIABLES ---------------------

        //public
        public int TotalMoney { get; private set; }
        public Color BaseColor { get; private set; }

        //private
        private const float DeloadDistanceThreshold = 4f;
        private const float TimeBetweenUnloads = .03f;
        private const float MoneyPenalty = .5f; // percent
        private readonly int _baseIndex = 0;

        private int _shownMoneyStacks = 0;
        private int _bundlesPerStack = 10;
        private float _margin = 0.05f;

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

            // Todo: This causes currently a strange loop. Players are need UI to be started already, but the UI which contains the Suggestions uses the player and bases to be startet first!
            UpdateUI();
        }

        public override void Update() {
            //UpdateUI();
        }

        public override void Reset() {
            Start();
        }

        public override void OnCollisionEnter(Collider c) {
            bool isPlayer = c.GameObject.tag == ObjectTag.Player;
            if (isPlayer) {
                Player p = c.GameObject.GetComponent<Player>();
                if (p.TeamIndex == _baseIndex) {
                    //DeloadPlayer(p.gameObject.GetComponent<PlayerInventory>());
                    DeloadPlayerProgression(p.gameObject.GetComponent<PlayerInventory>());
                }
            }
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public void DeloadPlayer(PlayerInventory pi) {
            TotalMoney += pi.CarryingValue;
            pi.DeloadAll();
            UpdateUI();
        }

        void UpdateUI() {
            BaseUI.Instance.UpdateBaseUI(_baseIndex, Health, StartingHealth, TotalMoney);
        }

        protected override void Die() {
            //TODO show gameover because base exploded
        }

        public override void TakeDamage(float damage) {
            //base.TakeDamage(damage);
            //UpdateUI();
        }

        public void NotifyRoundEnd() {
            foreach (var p in TeamPlayers()) {
                if (!PlayerInsideRange(gameObject)) {
                    //apply penalty (could happen twice)
                    TotalMoney -= (int)(TotalMoney * MoneyPenalty);
                    RoundUI.instance.ShowEndRound(p.PlayerIndex, RoundUI.EndRoundCondition.Busted);
                }
            }
            //SHOW money penalty (BUSTED!)
            UpdateUI();
        }


        // queries
        bool PlayerInsideRange(GameObject p) {
            return (p.transform.position - transform.position).LengthSquared() <= DeloadDistanceThreshold * DeloadDistanceThreshold;
        }

        Player[] TeamPlayers() {
            List<Player> result = new List<Player>();
            GameObject[] players = GameObject.FindGameObjectsWithTag(ObjectTag.Player);
            foreach (var player in players) {
                Player p = player.GetComponent<Player>();
                if (p.TeamIndex == _baseIndex) result.Add(p);
            }
            return result.ToArray();
        }


        // other
        async void DeloadPlayerProgression(PlayerInventory pi) {
            if (pi.CarryingValue > 0) {
                Audio.Play("leaving_cash_base", transform.position);
                OnBringBase?.Invoke();
            }

            while (pi.CarryingValue > 0 && PlayerInsideRange(pi.gameObject)) {
                TotalMoney += pi.ValueOnTop;
                pi.DeloadOne();
                UpdateUI();
                await Time.WaitForSeconds(TimeBetweenUnloads);
            }

            int totalStacksToShow = TotalMoney / 1000;

            while (_shownMoneyStacks < totalStacksToShow) {
                GameObject newBundle = GameObject.Instantiate("cashStack", transform.position + 0.5f * Vector3.Up, Quaternion.Identity);
                Vector3 size = BoundingBoxHelper.CalcualteSize(newBundle.Model, transform.scale);

                Vector3 up = (0.1f + (_shownMoneyStacks % _bundlesPerStack) * size.Y) * Vector3.Up;
                Vector3 right = size.X * (_margin + _shownMoneyStacks / _bundlesPerStack) * Vector3.Right;

                newBundle.transform.position = transform.position + up + right;

                ++_shownMoneyStacks;
            }
        }

    }

}