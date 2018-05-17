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
using BRS.Engine.Rendering;
using BRS.Engine.Utilities;
using BRS.Scripts.Elements.Lighting;


namespace BRS.Engine {
    static class PrefabContent {
        ////////// builds all the prefabs and gives them to Prefabs //////////
        static bool builtAlready = false;

        static string[] powerupName = new string[] { "bomb", "capacity", "stamina", "key", "health", "shield", "speed", "trap", "explodingbox", "weight", "magnet" };
        static Powerup[] powerupcomponents = new Powerup[] { new Bomb(), new CapacityBoost(), new StaminaPotion(), new Key(), new HealthPotion(), new ShieldPotion(), new SpeedBoost(), new Trap(), new ExplodingBox(), new Weight(), new Magnet() };
        static string[] dynamicElements = new string[] { "chair", "plant", "cart" };

        static float powerupScale = 2.2f;


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
            Material playerMat = new Material(File.Load<Texture2D>("Images/textures/player_colors_p1"), File.Load<Texture2D>("Images/lightmaps/elements"));

            // Initialize the models for hardware instanciating
            // Important: it doesn't matter when we load the modelds for the 
            Graphics.InitializeModel(ModelType.Cash, File.Load<Model>("Models/elements/cash"), elementsMat);
            Graphics.InitializeModel(ModelType.Gold, File.Load<Model>("Models/elements/gold"), powerupMat);
            Graphics.InitializeModel(ModelType.Diamond, File.Load<Model>("Models/elements/diamond"), powerupMat);
            Graphics.InitializeModel(ModelType.Police, File.Load<Model>("Models/vehicles/police"), policeMat);
            Graphics.InitializeModel(ModelType.Crate, File.Load<Model>("Models/elements/crate"), powerupMat);
            Graphics.InitializeModel(ModelType.Oil, File.Load<Model>("Models/elements/oil"), powerupMat);
            Graphics.InitializeModel(ModelType.Bomb, File.Load<Model>("Models/powerups/bomb"), powerupMat);
            Graphics.InitializeModel(ModelType.Weight, File.Load<Model>("Models/powerups/weight"), powerupMat);
            Graphics.InitializeModel(ModelType.Magnet, File.Load<Model>("Models/powerups/magnet"), powerupMat);
            Graphics.InitializeModel(ModelType.Speedpad, File.Load<Model>("Models/elements/speedpad"), elementsMat);
            Graphics.InitializeModel(ModelType.Stack, File.Load<Model>("Models/elements/stack"), elementsMat);
            Graphics.InitializeModel(ModelType.Shadow, File.Load<Model>("Models/primitives/plane"), shadowMat);
            Graphics.InitializeModel(ModelType.YellowLight, File.Load<Model>("Models/primitives/plane"), lightPlayerMat);
            Graphics.InitializeModel(ModelType.BlueLight, File.Load<Model>("Models/primitives/plane"), lightBlueMat);
            Graphics.InitializeModel(ModelType.RedLight, File.Load<Model>("Models/primitives/plane"), lightRedMat);
            Graphics.InitializeModel(ModelType.WheelFl, File.Load<Model>("Models/vehicles/wheel_fl"), playerMat);
            Graphics.InitializeModel(ModelType.WheelBz, File.Load<Model>("Models/vehicles/wheel_bz"), playerMat);
            Graphics.InitializeModel(ModelType.WheelPolice, File.Load<Model>("Models/vehicles/wheel_police"), policeMat);
            Graphics.InitializeModel(ModelType.Vault, File.Load<Model>("Models/elements/vault"), elementsMat);

            for (int i = 0; i < powerupName.Length; i++) {
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), powerupName[i], true);
                Graphics.InitializeModel(modelType, File.Load<Model>("Models/powerups/" + powerupName[i]), powerupMat);
            }

            foreach (string s in dynamicElements) {
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), s, true);
                Graphics.InitializeModel(modelType, File.Load<Model>("Models/elements/" + s), elementsMat);
            }

            //-------------------VALUABLES-------------------
            //cash
            GameObject cashPrefab = new GameObject("cashPrefab", ModelType.Cash, false);
            cashPrefab.transform.Scale(2.5f);
            cashPrefab.AddComponent(new Money(100, 1, Money.Type.Cash));
            cashPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box, pureCollider: true, size: new Vector3(1.5f, 3.0f, 3.0f)));
            Prefabs.AddPrefab(cashPrefab);

            //gold
            GameObject goldPrefab = new GameObject("goldPrefab", ModelType.Gold, false);
            goldPrefab.transform.Scale(2f);
            goldPrefab.AddComponent(new Money(500, 1, Money.Type.Gold));
            goldPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Box, pureCollider: true, size: new Vector3(3.0f, 3.0f, 1.5f)));
            Prefabs.AddPrefab(goldPrefab);

            //diamond
            GameObject diamondPrefab = new GameObject("diamondPrefab", ModelType.Diamond, false);
            diamondPrefab.transform.Scale(1.5f);
            diamondPrefab.AddComponent(new Money(2000, 1, Money.Type.Diamond));
            diamondPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true, size: 1.5f));
            Prefabs.AddPrefab(diamondPrefab);



            //-------------------POWERUPS-------------------
            //expand these two arrays to add new powerups with a particular name and a powerup script
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

            for (int i = 0; i < powerupName.Length; i++) {
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), powerupName[i], true);

                GameObject powerupPrefab = new GameObject(powerupName[i] + "Prefab", modelType, false);
                powerupPrefab.transform.Scale(powerupScale);
                powerupPrefab.AddComponent(powerupcomponents[i]);
                powerupPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.Sphere, pureCollider: true));
                powerupPrefab.AddComponent(new PowerUpEffect(powerupcomponents[i].powerupColor));
                Prefabs.AddPrefab(powerupPrefab);
            }

            //-------------------ELEMENTS-------------------

            //police car
            GameObject police = new GameObject("policePrefab", ModelType.Police, false);
            police.tag = ObjectTag.Police;
            police.transform.Scale(1f);
            police.AddComponent(new Police());
            police.AddComponent(new AnimatedRigidBody(shapeType: ShapeType.Box, pureCollider: true));
            police.AddComponent(new DynamicShadow());
            police.AddComponent(new AlarmLight(FollowerType.LightRed, new Vector3(-0.2f, 0.850f, 0.035f),
                FollowerType.LightBlue, new Vector3(0.2f, 0.851f, 0.035f)));
            police.AddComponent(new FrontLight(FrontLight.Type.FrontAndBack, new Vector3(0.27f, 0.35f, -0.97f), new Vector3(-0.27f, 0.35f, 0.93f)));
            police.AddComponent(new AnimatedWheels(AnimatedWheels.Type.FrontOnly, 20, 3));
            Prefabs.AddPrefab(police);

            //crate
            GameObject cratePrefab = new GameObject("cratePrefab", ModelType.Crate, false);
            cratePrefab.transform.Scale(1.5f);
            cratePrefab.AddComponent(new Crate());
            cratePrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: false));
            cratePrefab.AddComponent(new DynamicShadow());
            Prefabs.AddPrefab(cratePrefab);

            //oil
            GameObject oilPrefab = new GameObject("oilPrefab", ModelType.Oil, false);
            oilPrefab.transform.Scale(1f);
            oilPrefab.transform.SetStatic();
            oilPrefab.AddComponent(new OilTrap());
            oilPrefab.AddComponent(new StaticRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            Prefabs.AddPrefab(oilPrefab);

            //planted bomb
            GameObject plantedBombPrefab = new GameObject("plantedBombPrefab", ModelType.Bomb, false);
            plantedBombPrefab.transform.Scale(powerupScale);
            plantedBombPrefab.transform.SetStatic();
            plantedBombPrefab.AddComponent(new PlantedBomb());
            plantedBombPrefab.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            Prefabs.AddPrefab(plantedBombPrefab);

            //falling weight
            GameObject fallingWeight = new GameObject("fallingWeightPrefab", ModelType.Weight, false);
            fallingWeight.transform.Scale(powerupScale);
            fallingWeight.AddComponent(new FallingWeight());
            fallingWeight.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            fallingWeight.AddComponent(new DynamicShadow());
            Prefabs.AddPrefab(fallingWeight);

            //planted magnet
            GameObject plantedMagnet = new GameObject("plantedMagnetPrefab", ModelType.Weight, false);
            plantedMagnet.transform.Scale(powerupScale);
            plantedMagnet.AddComponent(new PlantedMagnet());
            plantedMagnet.AddComponent(new DynamicRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            Prefabs.AddPrefab(plantedMagnet);

            //speed boost
            GameObject speedpadPrefab = new GameObject("speedpadPrefab", ModelType.Speedpad, false);
            speedpadPrefab.transform.Scale(1f);
            speedpadPrefab.transform.SetStatic();
            speedpadPrefab.AddComponent(new SpeedPad());
            speedpadPrefab.AddComponent(new StaticRigidBody(shapeType: ShapeType.BoxUniform, pureCollider: true));
            Prefabs.AddPrefab(speedpadPrefab);


            //-------------------DYNAMIC OBJECTS-------------------
            foreach (string s in dynamicElements) {
                ModelType modelType = (ModelType)Enum.Parse(typeof(ModelType), s, true);

                GameObject dynamicElement = new GameObject(s, modelType, false);
                dynamicElement.AddComponent(new DynamicRigidBody());
                dynamicElement.AddComponent(new DynamicShadow());
                Prefabs.AddPrefab(dynamicElement);
            }

            // cash-stacks for the base
            GameObject cashStack = new GameObject("cashStack", ModelType.Stack, false);
            cashStack.transform.Scale(2f);
            Prefabs.AddPrefab(cashStack);


            // dynamic shadow
            GameObject dynamicShadow = new GameObject(FollowerType.DynamicShadow.GetDescription(), ModelType.Shadow, false);
            dynamicShadow.tag = ObjectTag.Lighting;
            Prefabs.AddPrefab(dynamicShadow);


            // dynamic lights
            GameObject lightPlayer = new GameObject(FollowerType.LightYellow.GetDescription(), ModelType.YellowLight, false);
            lightPlayer.tag = ObjectTag.Lighting;
            Prefabs.AddPrefab(lightPlayer);

            GameObject lightBlue = new GameObject(FollowerType.LightBlue.GetDescription(), ModelType.BlueLight, false);
            lightBlue.tag = ObjectTag.Lighting;
            Prefabs.AddPrefab(lightBlue);

            GameObject lightRed = new GameObject(FollowerType.LightRed.GetDescription(), ModelType.RedLight, false);
            lightRed.tag = ObjectTag.Lighting;
            Prefabs.AddPrefab(lightRed);

            // Wheels for the player-models
            GameObject wheelType1 = new GameObject("wheelType1", ModelType.WheelFl, false);
            Prefabs.AddPrefab(wheelType1);

            GameObject wheelType2 = new GameObject("wheelType2", ModelType.WheelBz, false);
            Prefabs.AddPrefab(wheelType2);

            GameObject wheelPolice = new GameObject("wheelPolice", ModelType.WheelPolice, false);
            Prefabs.AddPrefab(wheelPolice);
        }
    }
}
