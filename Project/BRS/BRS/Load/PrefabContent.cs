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

            //simple money prefab
            GameObject moneyPrefab = new GameObject("moneyPrefab", Content.Load<Model>("cash"));
            moneyPrefab.Transform.Scale(.5f);
            moneyPrefab.Transform.SetStatic();
            moneyPrefab.AddComponent(new Money(100, 1));
            moneyPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(moneyPrefab);

            //diamond
            GameObject diamondPrefab = new GameObject("diamondPrefab", Content.Load<Model>("diamond"));
            diamondPrefab.Transform.Scale(1f);
            diamondPrefab.Transform.SetStatic();
            diamondPrefab.AddComponent(new Money(300, 2));
            diamondPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(diamondPrefab);

            //gold
            GameObject goldPrefab = new GameObject("goldPrefab", Content.Load<Model>("gold"));
            goldPrefab.Transform.Scale(.5f);
            goldPrefab.Transform.SetStatic();
            goldPrefab.AddComponent(new Money(1000, 3));
            goldPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(goldPrefab);

            //make more money prefabs


            //crate prefab
            GameObject cratePrefab = new GameObject("cratePrefab", Content.Load<Model>("cube"));
            cratePrefab.Transform.Scale(.5f);
            cratePrefab.Transform.SetStatic();
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new BoxCollider(Vector3.Zero, Vector3.One*.5f));
            AddPrefab(cratePrefab);

        }

    }
}
