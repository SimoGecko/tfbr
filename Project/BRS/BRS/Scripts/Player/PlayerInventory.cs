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
        const float dropcashRadius = .5f;
        const float losecashRadius = 2f;

        //private
        //MONEY
        int carryingWeight = 0;
        int carryingValue = 0;
        bool canDropMoney = true;
        Stack<Money> carryingMoney = new Stack<Money>();

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {
            //INIT
            carryingValue = carryingWeight = 0;
            carryingMoney = new Stack<Money>(); carryingMoney.Clear();
            canDropMoney = true;
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

        //deload = leave in base (no spawning)
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

        //drop = leave on ground by choice based on input
        public void DropMoney() {
            if (canDropMoney) {
                RemoveMoneyAmount(1, dropcashRadius);
                canDropMoney = false;
                new Timer(timeBetweenDrops, () => canDropMoney = true);
            }
        }

        //lose = leave on ground by attack
        public void LoseMoney() {
            RemoveMoneyAmount(Math.Max(carryingMoney.Count/2, 3), losecashRadius);
        }

        public void LoseAllMoney() {
            RemoveMoneyAmount(carryingMoney.Count, losecashRadius);
        }

        //general to remove
        void RemoveMoneyAmount(int amount, float radius) {
            amount = Math.Min(amount, carryingMoney.Count);
            for (int i = 0; i < amount; i++){
                Money money = carryingMoney.Pop();
                //Spawn money somewhere
                Spawner.instance.SpawnMoneyAround(transform.position, radius);
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

        public bool IsFull() {
            return carryingWeight >= capacity-3;
        }

        /*
        bool CanDrop() {
            return canDropMoney && carryingMoney.Count > 0;
        }*/

        public float MoneyPercent { get { return (float)carryingWeight / capacity; } }
        public int CarryingValue  { get { return carryingValue; } }
        public int CarryingWeight { get { return carryingWeight; } }
        public int Capacity       { get { return capacity; } }
        public int ValueOnTop     { get { return carryingMoney.Peek().Value; } }

        // other

    }

}