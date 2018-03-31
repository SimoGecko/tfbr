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
            //GameObject moneyPrefab = new GameObject("moneyPrefab", File.Load<Model>("Models/valuables/cash"));
            GameObject moneyPrefab = new GameObject("moneyPrefab", File.Load<Model>("Models/polygonheist/SM_Prop_Money_Note_05"));
            moneyPrefab.transform.Scale(2f);
            moneyPrefab.transform.SetStatic();
            moneyPrefab.AddComponent(new Money(100, 1, Money.Type.Cash));
            moneyPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(moneyPrefab);

            //diamond
            GameObject diamondPrefab = new GameObject("diamondPrefab", File.Load<Model>("Models/valuables/diamond"));
            diamondPrefab.transform.Scale(1f);
            diamondPrefab.transform.SetStatic();
            diamondPrefab.AddComponent(new Money(300, 2, Money.Type.Diamond));
            diamondPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(diamondPrefab);

            //gold
            GameObject goldPrefab = new GameObject("goldPrefab", File.Load<Model>("Models/valuables/gold"));
            goldPrefab.transform.Scale(.5f);
            goldPrefab.transform.SetStatic();
            goldPrefab.AddComponent(new Money(1000, 3, Money.Type.Gold));
            goldPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(goldPrefab);


            //crate prefab
            GameObject cratePrefab = new GameObject("cratePrefab", File.Load<Model>("Models/primitives/cube"));
            cratePrefab.transform.Scale(.5f);
            cratePrefab.transform.SetStatic();
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new BoxCollider(Vector3.Zero, Vector3.One*.5f));
            AddPrefab(cratePrefab);


            //POWERUPS

            //bomb
            GameObject bombPrefab = new GameObject("bombPrefab", File.Load<Model>("Models/powerups/bomb"));
            bombPrefab.transform.Scale(.3f);
            bombPrefab.AddComponent(new Bomb());
            bombPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //bombPrefab.transform.SetStatic();
            AddPrefab(bombPrefab);

            //capacity
            GameObject capacityPrefab = new GameObject("capacityPrefab", File.Load<Model>("Models/powerups/capacity"));
            capacityPrefab.transform.Scale(.3f);
            capacityPrefab.AddComponent(new CapacityBoost());
            capacityPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //capacityPrefab.transform.SetStatic();
            AddPrefab(capacityPrefab);

            //trap
            GameObject staminaPrefab = new GameObject("staminaPrefab", File.Load<Model>("Models/powerups/stamina"));
            staminaPrefab.transform.Scale(.3f);
            staminaPrefab.AddComponent(new StaminaPotion());
            staminaPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //staminaPrefab.transform.SetStatic();
            AddPrefab(staminaPrefab);

            //key
            GameObject keyPrefab = new GameObject("keyPrefab", File.Load<Model>("Models/powerups/key"));
            keyPrefab.transform.Scale(.3f);
            keyPrefab.AddComponent(new Key());
            keyPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //keyPrefab.transform.SetStatic();
            AddPrefab(keyPrefab);

            //health
            GameObject healthPrefab = new GameObject("healthPrefab", File.Load<Model>("Models/powerups/health"));
            healthPrefab.transform.Scale(.3f);
            healthPrefab.AddComponent(new HealthPotion());
            healthPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //healthPrefab.transform.SetStatic();
            AddPrefab(healthPrefab);

            //shield
            GameObject shieldPrefab = new GameObject("shieldPrefab", File.Load<Model>("Models/powerups/shield"));
            shieldPrefab.transform.Scale(.3f);
            shieldPrefab.AddComponent(new StaminaPotion());
            shieldPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //shieldPrefab.transform.SetStatic();
            AddPrefab(shieldPrefab);

            //speed
            GameObject speedPrefab = new GameObject("speedPrefab", File.Load<Model>("Models/powerups/speed"));
            speedPrefab.transform.Scale(.3f);
            speedPrefab.AddComponent(new SpeedBoost());
            speedPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //speedPrefab.transform.SetStatic();
            AddPrefab(speedPrefab);

            //trap
            GameObject trapPrefab = new GameObject("trapPrefab", File.Load<Model>("Models/powerups/trap"));
            trapPrefab.transform.Scale(.3f);
            trapPrefab.AddComponent(new Trap());
            trapPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //trapPrefab.transform.SetStatic();
            AddPrefab(trapPrefab);


            //GAME ELEMENTS

            //oil
            GameObject oilPrefab = new GameObject("oilPrefab", File.Load<Model>("Models/elements/oil_trap"));
            oilPrefab.transform.Scale(.6f);
            oilPrefab.transform.SetStatic();
            oilPrefab.AddComponent(new OilTrap());
            oilPrefab.AddComponent(new SphereCollider(Vector3.Zero, .6f));
            AddPrefab(oilPrefab);

            //speed boost
            GameObject speedpadPrefab = new GameObject("speedpadPrefab", File.Load<Model>("Models/elements/platform"));
            speedpadPrefab.transform.Scale(1f);
            speedpadPrefab.transform.SetStatic();
            speedpadPrefab.AddComponent(new SpeedPad());
            speedpadPrefab.AddComponent(new BoxCollider(Vector3.Zero, new Vector3(1, .5f, 1)));
            AddPrefab(speedpadPrefab);



        }

    }
}
