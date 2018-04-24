// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts;
using BRS.Scripts.Elements;
using BRS.Scripts.PowerUps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Partial classes have to have the same namespace. Could maybe be solved with inheritance which would be nicer?
// ReSharper disable once CheckNamespace
namespace BRS.Engine {
    static class PrefabContent {
        ////////// builds all the prefabs and gives them to Prefabs //////////

        //==============================================================================================
        // create all prefabs - PUT YOUR CODE HERE
        public static void BuildPrefabs() {

            //-------------------MATERIALS-------------------
            Material powerupMat = new Material(File.Load<Texture2D>("Images/textures/powerups"));
            Material elementsMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/elements"));
            Material policeMat = new Material(File.Load<Texture2D>("Images/textures/Vehicle_Police"), File.Load<Texture2D>("Images/lightmaps/elements"));


            //-------------------VALUABLES-------------------
            //cash
            GameObject cashPrefab = new GameObject("cashPrefab", File.Load<Model>("Models/elements/cash"));
            cashPrefab.transform.Scale(2f);
            cashPrefab.material = elementsMat;
            cashPrefab.AddComponent(new Money(100, 1, Money.Type.Cash));
            cashPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box, pureCollider: true, size: new Vector3(1.5f, 3.0f, 3.0f)));
            Prefabs.AddPrefab(cashPrefab);

            //gold
            GameObject goldPrefab = new GameObject("goldPrefab", File.Load<Model>("Models/elements/goldP"));////SM_Prop_Jewellery_Necklace_02
            goldPrefab.transform.Scale(2f);
            goldPrefab.material = elementsMat;
            goldPrefab.AddComponent(new Money(1000, 1, Money.Type.Gold));
            goldPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box, pureCollider: true, size: new Vector3(3.0f, 3.0f, 1.5f)));
            Prefabs.AddPrefab(goldPrefab);

            //diamond
            GameObject diamondPrefab = new GameObject("diamondPrefab", File.Load<Model>("Models/elements/diamond"));
            diamondPrefab.transform.Scale(5f);
            diamondPrefab.material = powerupMat;
            diamondPrefab.AddComponent(new Money(3000, 1, Money.Type.Diamond));
            diamondPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true, size: 1.5f));
            Prefabs.AddPrefab(diamondPrefab);

           

            //-------------------POWERUPS-------------------
            //expand these two arrays to add new powerups with a particular name and a powerup script
            string[] powerupName = new string[] { "bomb", "capacity", "stamina", "key", "health", "shield", "speed", "trap", "explodingbox", "weight", "magnet" };
            Powerup[] powerupcomponents = new Powerup[] { new Bomb(), new CapacityBoost(), new StaminaPotion(), new Key(), new  HealthPotion(), new ShieldPotion(), new SpeedBoost(), new Trap(), new ExplodingBox(), new Weight(), new Magnet()};


            for (int i=0; i<powerupName.Length; i++) {
                GameObject powerupPrefab = new GameObject(powerupName[i]+"Prefab", File.Load<Model>("Models/powerups/"+powerupName[i]));
                powerupPrefab.transform.Scale(1.5f);
                powerupPrefab.AddComponent(powerupcomponents[i]);
                powerupPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Sphere, pureCollider: true));
                powerupPrefab.material = powerupMat;
                Prefabs.AddPrefab(powerupPrefab);
            }

            //-------------------ELEMENTS-------------------

            //police car
            GameObject police = new GameObject("policePrefab", File.Load<Model>("Models/vehicles/police"));
            police.AddComponent(new Police());
            police.transform.Scale(1f);
            police.material = policeMat;
            police.AddComponent(new AnimatedRigidBody(shapeType: ShapeType.Box, pureCollider: true));
            Prefabs.AddPrefab(police);

            //crate
            GameObject cratePrefab = new GameObject("cratePrefab", File.Load<Model>("Models/elements/crate"));
            cratePrefab.transform.Scale(1.5f);
            cratePrefab.material = powerupMat;
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            Prefabs.AddPrefab(cratePrefab);

            //oil
            GameObject oilPrefab = new GameObject("oilPrefab", File.Load<Model>("Models/elements/oil"));
            oilPrefab.transform.Scale(1f);
            oilPrefab.transform.SetStatic();
            oilPrefab.material = elementsMat;
            oilPrefab.AddComponent(new OilTrap());
            oilPrefab.AddComponent(new StaticRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            Prefabs.AddPrefab(oilPrefab);

            //planted bomb
            GameObject plantedBombPrefab = new GameObject("plantedBombPrefab", File.Load<Model>("Models/powerups/bomb"));
            plantedBombPrefab.transform.Scale(1.5f);
            plantedBombPrefab.transform.SetStatic();
            plantedBombPrefab.AddComponent(new PlantedBomb());
            plantedBombPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            plantedBombPrefab.material = powerupMat;
            Prefabs.AddPrefab(plantedBombPrefab);

            //falling weight
            GameObject fallingWeight = new GameObject("fallingWeightPrefab", File.Load<Model>("Models/powerups/weight"));
            fallingWeight.transform.Scale(1.5f);
            fallingWeight.AddComponent(new FallingWeight());
            fallingWeight.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            fallingWeight.material = powerupMat;
            Prefabs.AddPrefab(fallingWeight);

            //planted magnet
            GameObject plantedMagnet = new GameObject("plantedMagnetPrefab", File.Load<Model>("Models/powerups/magnet"));
            plantedMagnet.transform.Scale(1.5f);
            plantedMagnet.AddComponent(new PlantedMagnet());
            plantedMagnet.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            plantedMagnet.material = powerupMat;
            Prefabs.AddPrefab(plantedMagnet);

            //speed boost
            GameObject speedpadPrefab = new GameObject("speedpadPrefab", File.Load<Model>("Models/elements/speedpad"));
            speedpadPrefab.transform.Scale(1f);
            speedpadPrefab.transform.SetStatic();
            speedpadPrefab.material = elementsMat;
            speedpadPrefab.AddComponent(new SpeedPad());
            speedpadPrefab.AddComponent(new StaticRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            Prefabs.AddPrefab(speedpadPrefab);


            //-------------------DYNAMIC OBJECTS-------------------
            string[] dynamicElements = new string[] { "chair", "plant", "cart" };
            foreach(string s in dynamicElements) {
                GameObject dynamicElement = new GameObject(s, File.Load<Model>("Models/elements/" + s));
                dynamicElement.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box));
                dynamicElement.material = elementsMat;
                Prefabs.AddPrefab(dynamicElement);
            }
        }
    }
}
