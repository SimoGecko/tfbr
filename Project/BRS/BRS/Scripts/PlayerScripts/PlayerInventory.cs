// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using BRS.Engine;
using BRS.Scripts.Elements;
using BRS.Scripts.Managers;

namespace BRS.Scripts.PlayerScripts {
    class PlayerInventory : Component {
        ////////// knows about the valuables the player is currently carrying //////////

        // --------------------- VARIABLES ---------------------

        // const
        const float TimeBetweenDrops = .1f;
        const float DropcashRadius = .5f;
        const float LosecashRadius = 3f;

        //public
        public bool CanDeload = false;

        //private
        private int _capacity = 20;

        

        //MONEY
        int _carryingWeight = 0;
        int _carryingValue = 0;
        private bool _canDropMoney = true;
        private Stack<Money> _carryingMoney = new Stack<Money>();

        //reference
        public Action OnInventoryFull;

        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //INIT
            _carryingValue = _carryingWeight = 0;
            _carryingMoney = new Stack<Money>(); _carryingMoney.Clear();
            _canDropMoney = true;
        }
        public override void Update() { }

        public override void Reset() {
            Start();
        }


        // --------------------- CUSTOM METHODS ----------------


        // commands

        public void Collect(Money money) {
            if (CanPickUp(money)) {
                Input.Vibrate(.01f, .015f, gameObject.GetComponent<Player>().PlayerIndex);
                _carryingWeight += money.Weight;
                _carryingValue += money.Value;
                _carryingMoney.Push(money);
                if (IsAlmostFull()) {
                    OnInventoryFull?.Invoke();
                }
            }
        }

        //deload = leave in base (no spawning)
        public void DeloadAll() {
            _carryingWeight = 0;
            _carryingValue = 0;
            _carryingMoney.Clear();
        }

        public void DeloadOne() {
            Money m = _carryingMoney.Pop();
            _carryingWeight -= m.Weight;
            _carryingValue -= m.Value;
        }

        //drop = leave on ground by choice based on input
        public void DropMoney() {
            if (_canDropMoney && CarryingWeight>0) {
                Audio.Play("leave", transform.position);
                RemoveMoneyAmount(1, DropcashRadius);
                _canDropMoney = false;
                new Timer(TimeBetweenDrops, () => _canDropMoney = true);
            }
        }

        //lose = leave on ground by attack
        public void LoseMoney() {
            RemoveMoneyAmount(Math.Max(_carryingMoney.Count / 2, 5), LosecashRadius);
        }

        public void LoseAllMoney() {
            RemoveMoneyAmount(_carryingMoney.Count, LosecashRadius);
        }

        //general to remove
        void RemoveMoneyAmount(int amount, float radius) {
            amount = Math.Min(amount, _carryingMoney.Count);
            for (int i = 0; i < amount; i++) {
                Money money = _carryingMoney.Pop();
                //Spawn money somewhere
                Spawner.Instance.SpawnMoneyAround(transform.position, radius, money.type.ToString());
                _carryingValue -= money.Value;
                _carryingWeight -= money.Weight;
            }
        }

        public void UpdateCapacity(int amountToAdd) {
            _capacity += amountToAdd;
        }

        // queries
        public bool CanPickUp(Money money) {
            return _carryingWeight + money.Weight <= _capacity;
        }

        public bool IsAlmostFull() {
            return _carryingWeight >= _capacity - 3;
        }
        public bool IsFullCompletely() {
            return _carryingWeight >= _capacity;
        }

        /*
        bool CanDrop() {
            return canDropMoney && carryingMoney.Count > 0;
        }*/

        public float MoneyPercent { get { return (float)_carryingWeight / _capacity; } }
        public int CarryingValue { get { return _carryingValue; } }
        public int CarryingWeight { get { return _carryingWeight; } }
        public int Capacity { get { return _capacity; } }
        public int ValueOnTop { get { return _carryingMoney.Peek().Value; } }

        public void SetCapacity(int cap) { _capacity = cap; }

        // other

    }

}