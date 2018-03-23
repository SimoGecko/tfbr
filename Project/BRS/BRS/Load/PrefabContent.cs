// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BRS.Scripts;

namespace BRS {
    static partial class Prefabs {
        //static class that contains all GameObjects stored as prefabs
       

        //==============================================================================================
        // create all prefabs - PUT YOUR CODE HERE
        static void BuildPrefabs() {

            //VALUABLES
            //cash
            GameObject moneyPrefab = new GameObject("moneyPrefab", Content.Load<Model>("Models/valuables/cash"));
            moneyPrefab.transform.Scale(.5f);
            moneyPrefab.transform.SetStatic();
            moneyPrefab.AddComponent(new Money(100, 1, Money.Type.Cash));
            moneyPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(moneyPrefab);

            //diamond
            GameObject diamondPrefab = new GameObject("diamondPrefab", Content.Load<Model>("Models/valuables/diamond"));
            diamondPrefab.transform.Scale(1f);
            diamondPrefab.transform.SetStatic();
            diamondPrefab.AddComponent(new Money(300, 2, Money.Type.Diamond));
            diamondPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(diamondPrefab);

            //gold
            GameObject goldPrefab = new GameObject("goldPrefab", Content.Load<Model>("Models/valuables/gold"));
            goldPrefab.transform.Scale(.5f);
            goldPrefab.transform.SetStatic();
            goldPrefab.AddComponent(new Money(1000, 3, Money.Type.Gold));
            goldPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(goldPrefab);


            //crate prefab
            GameObject cratePrefab = new GameObject("cratePrefab", Content.Load<Model>("Models/primitives/cube"));
            cratePrefab.transform.Scale(.5f);
            cratePrefab.transform.SetStatic();
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new BoxCollider(Vector3.Zero, Vector3.One*.5f));
            AddPrefab(cratePrefab);


            //POWER UPS

            //bomb
            GameObject bombPrefab = new GameObject("bombPrefab", Content.Load<Model>("Models/powerups/bomb"));
            bombPrefab.transform.Scale(.3f);
            bombPrefab.AddComponent(new Bomb());
            bombPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(bombPrefab);

            //capacity
            GameObject capacityPrefab = new GameObject("capacityPrefab", Content.Load<Model>("Models/powerups/capacity"));
            capacityPrefab.transform.Scale(.3f);
            capacityPrefab.AddComponent(new CapacityBoost());
            capacityPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(capacityPrefab);

            //key
            GameObject keyPrefab = new GameObject("keyPrefab", Content.Load<Model>("Models/powerups/key"));
            keyPrefab.transform.Scale(.3f);
            keyPrefab.AddComponent(new Key());
            keyPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(keyPrefab);

            //health
            GameObject healthPrefab = new GameObject("healthPrefab", Content.Load<Model>("Models/powerups/health"));
            healthPrefab.transform.Scale(.3f);
            healthPrefab.AddComponent(new HealthPotion());
            healthPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(healthPrefab);

            //shield
            GameObject shieldPrefab = new GameObject("shieldPrefab", Content.Load<Model>("Models/powerups/shield"));
            shieldPrefab.transform.Scale(.3f);
            shieldPrefab.AddComponent(new StaminaPotion());
            shieldPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(shieldPrefab);

            //speed
            GameObject speedPrefab = new GameObject("speedPrefab", Content.Load<Model>("Models/powerups/speed"));
            speedPrefab.transform.Scale(.3f);
            speedPrefab.AddComponent(new SpeedBoost());
            speedPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(speedPrefab);

        }

    }
}
