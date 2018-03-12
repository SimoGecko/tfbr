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
            GameObject moneyprefab = new GameObject("moneyPrefab", Content.Load<Model>("sphere"));
            moneyprefab.transform.Scale(.2f);
            moneyprefab.transform.SetStatic();
            moneyprefab.AddComponent(new Money());
            moneyprefab.AddComponent(new SphereCollider(Vector3.Zero, .5f));
            AddPrefab(moneyprefab);

            //make more money prefabs


            //crate prefab
            GameObject cratePrefab = new GameObject("cratePrefab", Content.Load<Model>("cube"));
            cratePrefab.transform.Scale(.5f);
            cratePrefab.transform.SetStatic();
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new BoxCollider(Vector3.Zero, Vector3.One*.5f));
            AddPrefab(cratePrefab);

        }

    }
}
