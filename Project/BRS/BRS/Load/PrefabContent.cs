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
            GameObject moneyprefab = new GameObject("moneyprefab", Content.Load<Model>("sphere"));
            moneyprefab.transform.Scale(.2f);
            moneyprefab.AddComponent(new Money());
            moneyprefab.transform.SetStatic();
            moneyprefab.AddComponent(new SphereCollider(Vector3.Zero, .5f));
            AddPrefab(moneyprefab);

            //make more money prefabs
        }

    }
}
