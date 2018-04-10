// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts.Elements;
using BRS.Scripts.PowerUps;
using Microsoft.Xna.Framework.Graphics;

// Partial classes have to have the same namespace. Could maybe be solved with inheritance which would be nicer?
// ReSharper disable once CheckNamespace
namespace BRS.Engine {
    static partial class Prefabs {
        //static class that contains all GameObjects stored as prefabs


        //==============================================================================================
        // create all prefabs - PUT YOUR CODE HERE
        static void BuildPrefabs() {
            PhysicsManager physics = PhysicsManager.Instance;

            ShapeType valuableShapeType = ShapeType.Box;
            ShapeType powerupShapeType = ShapeType.Sphere;

            //VALUABLES
            //cash
            //GameObject moneyPrefab = new GameObject("moneyPrefab", File.Load<Model>("Models/valuables/cash"));
            string[] models = new string[] { "SM_Prop_Money_Note_07", "SM_Prop_Money_Stack_02", "SM_Prop_Money_Stack_03" }; // ../polygonheist2/SM_Prop_Money_Stack_04
            int[] values = new int[] { 1, 3, 10 };
            for (int i=0; i<3; i++) {
                GameObject moneyPrefab = new GameObject("money"+values[i]+"Prefab", File.Load<Model>("Models/polygonheist/" + models[i]));
                moneyPrefab.transform.Scale(2f);
                moneyPrefab.transform.SetStatic();
                moneyPrefab.AddComponent(new Money(100*values[i], values[i], Money.Type.Cash));
                moneyPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: valuableShapeType, pureCollider: true, size: 2.0f));
                AddPrefab(moneyPrefab);
            }
            

            //diamond
            /*
            GameObject diamondPrefab = new GameObject("diamondPrefab", File.Load<Model>("Models/valuables/diamond"));
            diamondPrefab.transform.Scale(1f);
            diamondPrefab.AddComponent(new Money(300, 2, Money.Type.Diamond));
            diamondPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(diamondPrefab);*/

            //gold

            GameObject goldPrefab = new GameObject("goldPrefab", File.Load<Model>("Models/polygonheist/SM_Prop_GoldBar_01"));
            goldPrefab.transform.Scale(2f);
            goldPrefab.AddComponent(new Money(3000, 3, Money.Type.Gold));
            goldPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: valuableShapeType, pureCollider: true));
            AddPrefab(goldPrefab);


            //crate prefab
            GameObject cratePrefab = new GameObject("cratePrefab", File.Load<Model>("Models/primitives/cube"));
            cratePrefab.transform.Scale(.5f);
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new DynamicRigidBody(physics, shapeType: valuableShapeType, pureCollider: true));
            //cratePrefab.AddComponent(new BoxCollider(Vector3.Zero, Vector3.One*.5f));
            AddPrefab(cratePrefab);


            //POWERUPS
            //expand these two arrays to add new powerups with a particular name and a powerup script
            string[] powerupName = new string[] { "bomb", "capacity", "stamina", "key", "health", "shield", "speed", "trap", "explodingbox", "weight", "magnet" };
            Powerup[] powerupcomponents = new Powerup[] { new Bomb(), new CapacityBoost(), new StaminaPotion(), new Key(), new  HealthPotion(), new ShieldPotion(), new SpeedBoost(), new Trap(), new ExplodingBox(), new Weight(), new Magnet()};

            for(int i=0; i<powerupName.Length; i++) {
                GameObject powerupPrefab = new GameObject(powerupName[i]+"Prefab", File.Load<Model>("Models/powerups/"+powerupName[i]));
                powerupPrefab.transform.Scale(.3f);
                powerupPrefab.AddComponent(powerupcomponents[i]);
                powerupPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true, updateRotation: false));
                AddPrefab(powerupPrefab);
            }
            /*
            //bomb
            GameObject bombPrefab = new GameObject("bombPrefab", File.Load<Model>("Models/powerups/bomb"));
            bombPrefab.transform.Scale(.3f);
            bombPrefab.AddComponent(new Bomb());
            bombPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true));
            //bombPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //bombPrefab.transform.SetStatic();
            AddPrefab(bombPrefab);

            //capacity
            GameObject capacityPrefab = new GameObject("capacityPrefab", File.Load<Model>("Models/powerups/capacity"));
            capacityPrefab.transform.Scale(.3f);
            capacityPrefab.AddComponent(new CapacityBoost());
            capacityPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true));
            //capacityPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
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
            keyPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true));
            //keyPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(keyPrefab);

            //health
            GameObject healthPrefab = new GameObject("healthPrefab", File.Load<Model>("Models/powerups/health"));
            healthPrefab.transform.Scale(.3f);
            healthPrefab.AddComponent(new HealthPotion());
            healthPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true));
            //healthPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(healthPrefab);

            //shield
            GameObject shieldPrefab = new GameObject("shieldPrefab", File.Load<Model>("Models/powerups/shield"));
            shieldPrefab.transform.Scale(.3f);
            shieldPrefab.AddComponent(new StaminaPotion());
            shieldPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true));
            //shieldPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(shieldPrefab);

            //speed
            GameObject speedPrefab = new GameObject("speedPrefab", File.Load<Model>("Models/powerups/speed"));
            speedPrefab.transform.Scale(.3f);
            speedPrefab.AddComponent(new SpeedBoost());
            speedPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true));
            //speedPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(speedPrefab);

            //trap
            GameObject trapPrefab = new GameObject("trapPrefab", File.Load<Model>("Models/powerups/trap"));
            trapPrefab.transform.Scale(.3f);
            trapPrefab.AddComponent(new Trap());
            trapPrefab.AddComponent(new DynamicRigidBody(physics, shapeType: powerupShapeType, pureCollider: true));
            //trapPrefab.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            AddPrefab(trapPrefab);

            //explodingbox
            GameObject explodingbox = new GameObject("explodingboxPrefab", File.Load<Model>("Models/powerups/box"));
            explodingbox.transform.Scale(.3f);
            explodingbox.AddComponent(new ExplodingBox());
            explodingbox.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //trapPrefab.transform.SetStatic();
            AddPrefab(explodingbox);

            //weight
            GameObject weight = new GameObject("weightPrefab", File.Load<Model>("Models/powerups/weight"));
            weight.transform.Scale(.3f);
            weight.AddComponent(new Weight());
            weight.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //trapPrefab.transform.SetStatic();
            AddPrefab(weight);

            //magnet
            GameObject magnet = new GameObject("magnetPrefab", File.Load<Model>("Models/powerups/magnet"));
            magnet.transform.Scale(.3f);
            magnet.AddComponent(new Magnet());
            magnet.AddComponent(new SphereCollider(Vector3.Zero, .2f));
            //trapPrefab.transform.SetStatic();
            AddPrefab(magnet);
            */

            //GAME ELEMENTS

            //oil
            GameObject oilPrefab = new GameObject("oilPrefab", File.Load<Model>("Models/elements/oil_trap"));
            oilPrefab.transform.Scale(.6f);
            oilPrefab.transform.SetStatic();
            oilPrefab.AddComponent(new OilTrap());
            oilPrefab.AddComponent(new StaticRigidBody(physics, shapeType: ShapeType.BoxUniform, pureCollider: true));
            //oilPrefab.AddComponent(new SphereCollider(Vector3.Zero, .6f));
            AddPrefab(oilPrefab);

            //planted bomb
            GameObject plantedBombPrefab = new GameObject("plantedBombPrefab", File.Load<Model>("Models/elements/bomb"));
            plantedBombPrefab.transform.Scale(.3f);
            plantedBombPrefab.transform.SetStatic();
            plantedBombPrefab.AddComponent(new PlantedBomb());
            plantedBombPrefab.AddComponent(new StaticRigidBody(physics, shapeType: ShapeType.BoxUniform, pureCollider: true));
            AddPrefab(plantedBombPrefab);

            //falling weight
            GameObject fallingWeight = new GameObject("fallingWeightPrefab", File.Load<Model>("Models/elements/weight"));
            fallingWeight.transform.Scale(.5f);
            fallingWeight.AddComponent(new FallingWeight());
            fallingWeight.AddComponent(new DynamicRigidBody(physics, shapeType: ShapeType.BoxUniform, pureCollider: true));
            AddPrefab(fallingWeight);

            //planted magnet
            GameObject plantedMagnet = new GameObject("plantedMagnetPrefab", File.Load<Model>("Models/elements/magnet"));
            plantedMagnet.transform.Scale(.3f);
            plantedMagnet.AddComponent(new PlantedMagnet());
            plantedMagnet.AddComponent(new StaticRigidBody(physics, shapeType: ShapeType.BoxUniform, pureCollider: true));
            AddPrefab(plantedMagnet);

            //speed boost
            GameObject speedpadPrefab = new GameObject("speedpadPrefab", File.Load<Model>("Models/elements/platform"));
            speedpadPrefab.transform.Scale(1f);
            speedpadPrefab.transform.SetStatic();
            speedpadPrefab.AddComponent(new SpeedPad());
            speedpadPrefab.AddComponent(new StaticRigidBody(physics, shapeType: ShapeType.BoxUniform, pureCollider: true));
            //speedpadPrefab.AddComponent(new BoxCollider(Vector3.Zero, new Vector3(1, .5f, 1)));
            AddPrefab(speedpadPrefab);



        }

    }
}
