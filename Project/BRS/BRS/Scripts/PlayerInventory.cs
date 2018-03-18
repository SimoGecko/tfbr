// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS.Scripts {
    class PlayerInventory : Component {
        ////////// knows about the valuables the player is currently carrying //////////

        // --------------------- VARIABLES ---------------------

        //public
        const float timeBetweenDrops = .1f;
        int capacity = 20;

        //private
        //MONEY
        int carryingWeight = 0;
        int carryingValue = 0;
        bool canDropMoneyTimer = true;
        Stack<Money> carryingMoney = new Stack<Money>();

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //INIT
            carryingValue = 0;
            carryingWeight = 0;
            carryingMoney = new Stack<Money>();
            canDropMoneyTimer = true;
        }
        public override void Update() { }


        // --------------------- CUSTOM METHODS ----------------


        // commands
        
        public void Collect(Money money) {
            if (CanPickUp(money)) {
                carryingWeight += money.Weight;
                carryingValue += money.Value;
                carryingMoney.Push(money);
            }
        }

        public void DeloadAll() {
            carryingWeight = 0;
            carryingValue = 0;
            carryingMoney.Clear();
        }

        public void DeloadOne() {
            Money m = carryingMoney.Pop();
            carryingWeight -= m.Weight;
            carryingValue -= m.Value;
        }

        public void DropMoney() {
            if (canDropMoneyTimer) {
                DropMoneyAmount(1);
                canDropMoneyTimer = false;
                new Timer(timeBetweenDrops, () => canDropMoneyTimer = true);
            }
        }

        public void LoseMoney() {
            DropMoneyAmount(carryingMoney.Count/3);
        }

        public void LoseAllMoney() {
            DropMoneyAmount(carryingMoney.Count);
        }

        void DropMoneyAmount(int amount) {
            amount = Math.Min(amount, carryingMoney.Count);
            for (int i = 0; i < amount; i++){
                Money money = carryingMoney.Pop();
                //Spawn money somewhere
                Spawner.instance.SpawnMoneyAround(transform.position);
                carryingValue -= money.Value;
                carryingWeight -= money.Weight;
            }
        }

        public void UpdateCapacity(int amountToAdd) {
            capacity += amountToAdd;
        }

        // queries
        public bool CanPickUp(Money money) {
            return carryingWeight + money.Weight <= capacity;
        }

        bool CanDrop() {
            return canDropMoneyTimer && carryingMoney.Count > 0;
        }

        public float MoneyPercent { get { return (float)carryingWeight / capacity; } }
        public int CarryingValue  { get { return carryingValue; } }
        public int CarryingWeight { get { return carryingWeight; } }
        public int Capacity { get { return capacity; } }

        public int ValueOnTop { get { return carryingMoney.Peek().Value; } }

        // other

    }

}