// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Scripts;
using BRS.Scripts.Elements;
using BRS.Scripts.PlayerScripts;
using BRS.Scripts.PowerUps;
using BRS.Scripts.Particles3D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements.Lighting;


namespace BRS.Engine {
    static class PrefabContent {
        ////////// builds all the prefabs and gives them to Prefabs //////////
        static bool builtAlready = false;

        //==============================================================================================
        // create all prefabs - PUT YOUR CODE HERE
        public static void BuildPrefabs() {
            Debug.Assert(!builtAlready, "Calling buildprefab a second time");
            if (builtAlready) return;
            builtAlready = true;
            //-------------------MATERIALS-------------------
            Material powerupMat = new Material(File.Load<Texture2D>("Images/textures/powerups"));
            Material shadowMat = new Material(File.Load<Texture2D>("Images/textures/shadow"), true);
            Material lightPlayerMat = new Material(File.Load<Texture2D>("Images/textures/player_light"), true, true);
            Material lightBlueMat = new Material(File.Load<Texture2D>("Images/textures/police_blue"), true, true);
            Material lightRedMat = new Material(File.Load<Texture2D>("Images/textures/police_red"), true, true);
            Material elementsMat = new Material(File.Load<Texture2D>("Images/textures/polygonHeist"), File.Load<Texture2D>("Images/lightmaps/elements"));
            Material policeMat = new Material(File.Load<Texture2D>("Images/textures/Vehicle_Police"), File.Load<Texture2D>("Images/lightmaps/elements"));

            float powerupScale = 2.2f;

            //-------------------VALUABLES-------------------
            //cash
            GameObject cashPrefab = new GameObject("cashPrefab", File.Load<Model>("Models/elements/cash"));
            cashPrefab.transform.Scale(2.5f);
            cashPrefab.material = elementsMat;
            cashPrefab.AddComponent(new Money(100, 1, Money.Type.Cash));
            cashPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box, pureCollider: true, size: new Vector3(1.5f, 3.0f, 3.0f)));
            Prefabs.AddPrefab(cashPrefab);

            //gold
            GameObject goldPrefab = new GameObject("goldPrefab", File.Load<Model>("Models/elements/gold"));////SM_Prop_Jewellery_Necklace_02
            goldPrefab.transform.Scale(2f);
            goldPrefab.material = powerupMat;
            goldPrefab.AddComponent(new Money(500, 1, Money.Type.Gold));
            goldPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box, pureCollider: true, size: new Vector3(3.0f, 3.0f, 1.5f)));
            Prefabs.AddPrefab(goldPrefab);

            //diamond
            GameObject diamondPrefab = new GameObject("diamondPrefab", File.Load<Model>("Models/elements/diamond"));
            diamondPrefab.transform.Scale(1.5f);
            diamondPrefab.material = powerupMat;
            diamondPrefab.AddComponent(new Money(2000, 1, Money.Type.Diamond));
            diamondPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true, size: 1.5f));
            Prefabs.AddPrefab(diamondPrefab);



            //-------------------POWERUPS-------------------
            //expand these two arrays to add new powerups with a particular name and a powerup script
            string[] powerupName = new string[] { "bomb", "capacity", "stamina", "key", "health", "shield", "speed", "trap", "explodingbox", "weight", "magnet" };
            Powerup[] powerupcomponents = new Powerup[] { new Bomb(), new CapacityBoost(), new StaminaPotion(), new Key(), new  HealthPotion(), new ShieldPotion(), new SpeedBoost(), new Trap(), new ExplodingBox(), new Weight(), new Magnet()};
            /*var colorMapping = new Dictionary<string, Color>()
                {
                    { "bomb", Color.Red},
                    { "capacity", Color.Green},
                    { "stamina", Color.Green},
                    { "key", Color.Yellow},
                    { "health", Color.Green},
                    { "shield", Color.Green},
                    { "speed", Color.Yellow},
                    { "trap", Color.Red},
                    { "explodingbox", Color.Red},
                    { "weight", Color.Red},
                    { "magnet", Color.Purple},
                };*/

            for (int i=0; i<powerupName.Length; i++) {
                GameObject powerupPrefab = new GameObject(powerupName[i]+"Prefab", File.Load<Model>("Models/powerups/"+powerupName[i]));
                powerupPrefab.transform.Scale(powerupScale);
                powerupPrefab.AddComponent(powerupcomponents[i]);
                powerupPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Sphere, pureCollider: true));
                
                powerupPrefab.AddComponent(new PowerUpEffect(powerupcomponents[i].powerupColor));
                powerupPrefab.material = powerupMat;
                Prefabs.AddPrefab(powerupPrefab);
            }

            //-------------------ELEMENTS-------------------

            //police car
            GameObject police = new GameObject("policePrefab", File.Load<Model>("Models/vehicles/police"));
            police.AddComponent(new Police());
            police.tag = ObjectTag.Police;
            police.transform.Scale(1f);
            police.material = policeMat;
            police.AddComponent(new AnimatedRigidBody(shapeType: ShapeType.Box, pureCollider: true));
            police.AddComponent(new DynamicShadow());
            police.AddComponent(new AlarmLight(FollowerType.LightRed, new Vector3(-0.2f, 0.850f, 0.035f),
                FollowerType.LightBlue, new Vector3(0.2f, 0.851f, 0.035f)));
            police.AddComponent(new FrontLight(FrontLight.Type.FrontAndBack, new Vector3(0.27f, 0.35f, -0.97f), new Vector3(-0.27f, 0.35f, 0.93f)));
            //police.AddComponent(new AnimatedWheels(AnimatedWheels.Type.FrontAndBack, 20, 3, "wheel_fl"));
            Prefabs.AddPrefab(police);

            //crate
            GameObject cratePrefab = new GameObject("cratePrefab", File.Load<Model>("Models/elements/crate"));
            cratePrefab.transform.Scale(1.5f);
            cratePrefab.material = powerupMat;
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: false));
            cratePrefab.AddComponent(new DynamicShadow());
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
            plantedBombPrefab.transform.Scale(powerupScale);
            plantedBombPrefab.transform.SetStatic();
            plantedBombPrefab.AddComponent(new PlantedBomb());
            plantedBombPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            plantedBombPrefab.material = powerupMat;
            Prefabs.AddPrefab(plantedBombPrefab);

            //falling weight
            GameObject fallingWeight = new GameObject("fallingWeightPrefab", File.Load<Model>("Models/powerups/weight"));
            fallingWeight.transform.Scale(powerupScale);
            fallingWeight.AddComponent(new FallingWeight());
            fallingWeight.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            fallingWeight.AddComponent(new DynamicShadow());
            fallingWeight.material = powerupMat;
            Prefabs.AddPrefab(fallingWeight);

            //planted magnet
            GameObject plantedMagnet = new GameObject("plantedMagnetPrefab", File.Load<Model>("Models/powerups/magnet"));
            plantedMagnet.transform.Scale(powerupScale);
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
            foreach (string s in dynamicElements) {
                GameObject dynamicElement = new GameObject(s, File.Load<Model>("Models/elements/" + s));
                dynamicElement.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box));
                dynamicElement.AddComponent(new DynamicShadow());
                dynamicElement.material = elementsMat;
                Prefabs.AddPrefab(dynamicElement);
            }

            // cash-stacks for the base
            GameObject cashStack = new GameObject("cashStack", File.Load<Model>("Models/elements/stack"));
            cashStack.transform.Scale(2f);
            cashStack.material = elementsMat;
            Prefabs.AddPrefab(cashStack);


            // dynamic shadow
            GameObject dynamicShadow = new GameObject(FollowerType.DynamicShadow.GetDescription(), File.Load<Model>("Models/primitives/plane"));
            dynamicShadow.material = shadowMat;
            Prefabs.AddPrefab(dynamicShadow);


            // dynamic lights
            GameObject lightPlayer = new GameObject(FollowerType.LightYellow.GetDescription(), File.Load<Model>("Models/primitives/plane"));
            lightPlayer.material = lightPlayerMat;
            Prefabs.AddPrefab(lightPlayer);

            GameObject lightBlue = new GameObject(FollowerType.LightBlue.GetDescription(), File.Load<Model>("Models/primitives/plane"));
            lightBlue.material = lightBlueMat;
            Prefabs.AddPrefab(lightBlue);

            GameObject lightRed = new GameObject(FollowerType.LightRed.GetDescription(), File.Load<Model>("Models/primitives/plane"));
            lightRed.material = lightRedMat;
            Prefabs.AddPrefab(lightRed);

            // Wheels for the player-models
            GameObject wheelType1 = new GameObject("wheelType1", File.Load<Model>("Models/vehicles/wheel_fl"));
            wheelType1.material = new Material(File.Load<Texture2D>("Images/textures/player_colors_p1"), File.Load<Texture2D>("Images/lightmaps/elements"));
            Prefabs.AddPrefab(wheelType1);

            GameObject wheelType2 = new GameObject("wheelType2", File.Load<Model>("Models/vehicles/wheel_bz"));
            wheelType2.material = new Material(File.Load<Texture2D>("Images/textures/player_colors_p1"), File.Load<Texture2D>("Images/lightmaps/elements"));
            Prefabs.AddPrefab(wheelType2);

            //builtAlready = true;

        }
    }
}
