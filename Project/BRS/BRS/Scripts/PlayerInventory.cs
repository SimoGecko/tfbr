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
        int capacity = 20;
        const float timeBetweenDrops = .1f;

        //private
        //MONEY
        int carryingWeight = 0;
        int carryingValue = 0;
        bool canDropMoney = true;
        Stack<Money> carryingMoney = new Stack<Money>();

        //reference


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands        
        public void Collect(Money money) {
            //carryingWeight += Math.Min(amount, capacity - carryingWeight);
            carryingWeight += money.Weight;
            carryingValue += money.Value;
            carryingMoney.Push(money);
        }

        public void Deload() {
            carryingWeight = 0;
            carryingValue = 0;
        }

        public void DropMoney() {
            if (canDropMoney) {
                DropMoneyAmount(1);
                canDropMoney = false;
                new Timer(timeBetweenDrops, () => canDropMoney = true);
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
        public float MoneyPercent { get { return (float)carryingWeight / capacity; } }
        public int CarryingValue { get { return carryingValue; } }
        public int Capacity { get { return capacity; } }
        public int CarryingWeight { get { return carryingWeight; } }


        // other

    }

}