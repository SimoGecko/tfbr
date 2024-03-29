﻿// (c) Simone Guggiari / Nicolas Huart 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Windows.Storage;
using static BRS.Scripts.UI.Menu;

namespace BRS.Engine {

    /// <summary>
    /// Class used to load files from disk and providing safeguard agains null files
    /// </summary>
    class File : Component {

        // --------------------- VARIABLES ---------------------

        //public


        //private
        static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        //reference
        public static ContentManager content;


        // --------------------- BASE METHODS ------------------
        public override void Start() {

        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        // Todo: Clean up
        // simo code
        public static T Load<T>(string s, bool check = false) {
            //TODO check first if file exists

            /*
            bool fileExists = false;// System.IO.File.Exists(fullPath);
            if (false && !fileExists && typeof(T) == typeof(Model)) {
                Debug.Log("File " + s + " doesn't exist!");
                //return content.Load<T>("Models/primitives/cube");
            }*/

            T result = content.Load<T>(s);
            return result;
        }

        /// <summary>
        /// Read the scene-file and build the game- and physic-objects for it
        /// </summary>
        /// <param name="pathName">Path to the scene-file</param>
        public static void ReadStatic(string pathName) {
            using (StreamReader reader = new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                string nameContent;
                while ((nameContent = reader.ReadLine()) != null) {
                    while (nameContent == "")
                        nameContent = reader.ReadLine();

                    if (nameContent == null)
                        break;

                    string tagName = reader.ReadLine().Split(' ')[1];
                    string prefabName = reader.ReadLine().Split(' ')[1];

                    string lineNoObj = reader.ReadLine();
                    int n = int.Parse(lineNoObj.Split(' ')[1]);

                    for (int i = 0; i < n; i++) {
                        string p = reader.ReadLine();
                        string r = reader.ReadLine();
                        string s = reader.ReadLine();

                        string[] pSplit = p.Split(' '); // pos: x y z in unity coord. system
                        string[] rSplit = r.Split(' '); // rot: x y z in unity coord. system
                        string[] sSplit = s.Split(' '); // sca: x y z in unity coord. system

                        Vector3 position = new Vector3(float.Parse(pSplit[3]), float.Parse(pSplit[2]), float.Parse(pSplit[1]));
                        Quaternion rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-float.Parse(rSplit[2])), MathHelper.ToRadians(float.Parse(rSplit[1])), MathHelper.ToRadians(float.Parse(rSplit[3])));
                        Vector3 scale = new Vector3(float.Parse(sSplit[3]), float.Parse(sSplit[2]), float.Parse(sSplit[1]));

                        GameObject go = new GameObject(tagName + "_" + i, Load<Model>("Models/primitives/" + prefabName));

                        go.transform.position = position;
                        go.transform.scale = scale;
                        go.transform.rotation = rotation;

                        try {
                            go.tag = (ObjectTag)Enum.Parse(typeof(ObjectTag), tagName, true);
                        } catch {
                            go.tag = ObjectTag.Default;
                        }

                        switch (go.tag) {
                            case ObjectTag.Base:
                                //go.AddComponent(new StaticRigidBody(physics, pureCollider: true));
                                break;
                            case ObjectTag.Ground:
                                go.AddComponent(new StaticRigidBody(isGround: true, shapeType: ShapeType.BoxInvisible));
                                break;
                            case ObjectTag.DynamicObstacle:
                                go.AddComponent(new DynamicRigidBody());
                                break;
                            default:
                                go.AddComponent(new StaticRigidBody(shapeType: ShapeType.BoxInvisible));
                                break;
                        }
                    }

                    nameContent = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// Read the scene-file and build the game- and physic-objects for it
        /// </summary>
        /// <param name="pathName">Path to the scene-file</param>
        public static void ReadDynamic(string pathName) {
            using (StreamReader reader = new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                string nameContent;
                while ((nameContent = reader.ReadLine()) != null) {
                    while (nameContent == "")
                        nameContent = reader.ReadLine();

                    if (nameContent == null)
                        break;

                    string tagName = reader.ReadLine().Split(' ')[1];
                    string prefabName = reader.ReadLine().Split(' ')[1];

                    string lineNoObj = reader.ReadLine();
                    int n = int.Parse(lineNoObj.Split(' ')[1]);

                    for (int i = 0; i < n; i++) {
                        string p = reader.ReadLine();
                        string r = reader.ReadLine();
                        string s = reader.ReadLine();

                        string[] pSplit = p.Split(' '); // pos: x y z in unity coord. system
                        string[] rSplit = r.Split(' '); // rot: x y z in unity coord. system
                        string[] sSplit = s.Split(' '); // sca: x y z in unity coord. system

                        Vector3 position = new Vector3(float.Parse(pSplit[3]), float.Parse(pSplit[2]), float.Parse(pSplit[1]));
                        Quaternion rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-float.Parse(rSplit[2])), MathHelper.ToRadians(float.Parse(rSplit[1])), MathHelper.ToRadians(float.Parse(rSplit[3])));
                        Vector3 scale = new Vector3(float.Parse(sSplit[3]), float.Parse(sSplit[2]), float.Parse(sSplit[1]));

                        try {
                            var tag = (ObjectTag)Enum.Parse(typeof(ObjectTag), tagName, true);

                            switch (tag) {
                                case ObjectTag.Chair:
                                case ObjectTag.Cart:
                                case ObjectTag.Plant:
                                    GameObject go = GameObject.Instantiate(tagName.ToLower(), position, rotation);
                                    go.transform.scale = scale;
                                    break;
                            }
                        } catch {
                            // ignored
                        }
                    }

                    nameContent = reader.ReadLine();
                }
            }
        }


        //simo code
        public static void ReadHeistScene(string pathName) {
            Vector3 offset = new Vector3(0, 0, 10); // Turn this off to avoid offset
            using (StreamReader reader = new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                string nameContent;
                while ((nameContent = reader.ReadLine()) != null) {
                    while (nameContent == "") nameContent = reader.ReadLine();
                    if (nameContent == null) break;
                    //<begin>

                    string meshName = reader.ReadLine().Split(' ')[1];

                    string amountLine = reader.ReadLine();
                    int n = int.Parse(amountLine.Split(' ')[1]);

                    for (int i = 0; i < n; i++) {
                        string p = reader.ReadLine();
                        string r = reader.ReadLine();
                        string s = reader.ReadLine();

                        string[] pSplit = p.Split(' '); // pos: x y z in unity coord. system
                        string[] rSplit = r.Split(' '); // rot: x y z in unity coord. system
                        string[] sSplit = s.Split(' '); // sca: x y z in unity coord. system

                        Vector3 pos = new Vector3(float.Parse(pSplit[1]), float.Parse(pSplit[2]), float.Parse(pSplit[3]));
                        Vector3 rot = new Vector3(float.Parse(rSplit[1]), float.Parse(rSplit[2]), float.Parse(rSplit[3]));
                        Vector3 sca = new Vector3(float.Parse(sSplit[1]), float.Parse(sSplit[2]), float.Parse(sSplit[3]));

                        GameObject go = new GameObject(meshName + "_" + i.ToString(), File.Load<Model>("Models/polygonheist/" + meshName, true));
                        //NOW DO CONVERSION
                        go.transform.position = new Vector3(pos.X, pos.Y, -pos.Z) + offset;
                        go.transform.eulerAngles = new Vector3(-rot.X, -rot.Y + 180, rot.Z); // +180 is probably due to not scaling with -1
                        go.transform.scale = new Vector3(sca.X, sca.Y, sca.Z);
                    }
                    // <end>
                    nameContent = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// Read the rankings and return a list of player's name with their highscore
        /// </summary>
        /// <param name="pathName">Path to the file</param>
        public static List<Tuple<string, string>> ReadRanking(string pathName) {
            try {
                List<Tuple<string, string>> listPerson = new List<Tuple<string, string>>();
                using (StreamReader reader =
                    new StreamReader(new FileStream(localFolder.Path + "/" + pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        if (line == "")
                            break;

                        string[] pSplit = line.Split(' ');

                        string namePlayer = "";
                        for (int i = 0; i < pSplit.Length-1; ++i)
                            namePlayer += pSplit[i];

                        listPerson.Add(new Tuple<string, string>(namePlayer, pSplit[pSplit.Length-1]));

                    }
                }
                return listPerson;
            } catch (Exception e) {
                Debug.LogError(e.Message);

                return new List<Tuple<string, string>>();
            }
        }

        /// <summary>
        /// Write the rankings to a file
        /// </summary>
        /// <param name="pathName">Path to the file</param>
        async public static void WriteRanking(string pathName, List<Tuple<string, string>> listPlayersNameScore, int maxElem) {
            try {
                StorageFile sampleFile = await localFolder.CreateFileAsync(pathName,
                    CreationCollisionOption.ReplaceExisting);

                int count = 0;
                foreach (var elem in listPlayersNameScore) {
                    await FileIO.AppendTextAsync(sampleFile, elem.Item1 + " " + elem.Item2 + "\n");
                    ++count;
                    if (count >= maxElem) break;
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Add text to file stream
        /// </summary>
        /// <param name="fs">Filestream</param>
        /// /// <param name="value">Text to add</param>
        private static void AddText(FileStream fs, string value) {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        /// <summary>
        /// Read the path of the police patrol
        /// </summary>
        /// <param name="pathName">Path to the file</param>
        public static List<List<Vector3>> ReadPolicePaths(string pathName) {
            List<List<Vector3>> policePaths = new List<List<Vector3>>();
            using (StreamReader reader = new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                string nameContent;
                int idPath = 0;
                while ((nameContent = reader.ReadLine()) != null) {
                    while (nameContent == "")
                        nameContent = reader.ReadLine();

                    if (nameContent == null)
                        break;

                    policePaths.Add(new List<Vector3>());

                    string lineNoObj = reader.ReadLine();
                    int n = int.Parse(lineNoObj.Split(' ')[1]);

                    for (int i = 0; i < n; i++) {
                        string p = reader.ReadLine();

                        string[] pSplit = p.Split(' '); // pos: x y z in unity coord. system

                        Vector3 position = new Vector3(float.Parse(pSplit[3]), float.Parse(pSplit[2]), float.Parse(pSplit[1]));
                        policePaths[idPath].Add(position);
                    }

                    nameContent = reader.ReadLine();
                    ++idPath;
                }
            }
            return policePaths;
        }

        /// <summary>
        /// Write a 2D array of int to a file
        /// </summary>
        /// <param name="pathName">Path to the file</param>
        /// <param name="values" fil>2D array of int</param>
        public static void Write2DArrayIntToFile(string pathName, int[,] values) {
            try {
                using (FileStream fs = System.IO.File.Open(pathName, FileMode.OpenOrCreate)) {
                    fs.Flush();

                    int width = values.GetLength(0);
                    int height = values.GetLength(1);

                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            AddText(fs, values[x,y] + " ");
                        }
                        AddText(fs, "\n");
                    }

                }
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Parse a menu panel describe by a file
        /// </summary>
        /// <param name="pathName">Path to the file</param>
        public static List<MenuStruct> ReadMenuPanel(string pathName) {
            List<MenuStruct> panel = new List<MenuStruct>();

            using (StreamReader reader = new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                string nameContent;
                while ((nameContent = reader.ReadLine()) != null) {
                    while (nameContent == "")
                        nameContent = reader.ReadLine();

                    if (nameContent == null)
                        break;

                    string line, key;
                    MenuStruct menuObject = new MenuStruct();
                    if (nameContent == "<Button>")
                        menuObject.menuType = MenuType.Button;
                    else if (nameContent == "<Text>")
                        menuObject.menuType = MenuType.Text;
                    else if (nameContent == "<Image>") 
                        menuObject.menuType = MenuType.Image;
                    else if (nameContent == "<Slider>")
                        menuObject.menuType = MenuType.Slider;
                    else if (nameContent == "<TickBox>")
                        menuObject.menuType = MenuType.TickBox;

                    menuObject.Active = true;

                    while ((!(line = reader.ReadLine()).Contains("</"))) {
                        key = line.Split(':')[0];
                        string[] values = line.Split(':')[1].Substring(1).Split(' ');

                        switch (key) {
                            case "Position":
                                menuObject.Position = new Vector2(float.Parse(values[0]), float.Parse(values[1]));
                                break;
                            case "Texture":
                                menuObject.TextureName = values[0];
                                break;
                            case "TextureInside":
                                menuObject.TextureInsideName = values[0];
                                break;
                            case "Text":
                                menuObject.Text = line.Split(':')[1].Substring(1);
                                break;
                            case "ScaleWidth":
                                menuObject.ScaleWidth = float.Parse(values[0]);
                                break;
                            case "ScaleHeight":
                                menuObject.ScaleHeight = float.Parse(values[0]);
                                break;
                            case "Color":
                                menuObject.Color = new Color(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
                                break;
                            case "ColorInside":
                                menuObject.ColorInside = new Color(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
                                break;
                            case "Transparency":
                                menuObject.transparency = int.Parse(values[0]);
                                break;
                            case "Functions":
                                menuObject.Functions = new List<string>();
                                foreach (var elem in values)
                                    menuObject.Functions.Add(elem);
                                break;
                            case "UniqueChoiceWith":
                                menuObject.UniqueChoiceButtonWith = new List<string>();
                                foreach (var elem in values)
                                    menuObject.UniqueChoiceButtonWith.Add(elem);
                                break;
                            case "NeighborsUpDownLeftRight":
                                menuObject.NeighborsUpDownLeftRight = new string[4];
                                for (int i = 0; i < 4; ++i)
                                    menuObject.NeighborsUpDownLeftRight[i] = values[i];
                                break;
                            case "NameIdentifier":
                                menuObject.Name = values[0];
                                break;
                            case "NameSwitchTo":
                                menuObject.NameToSwitchTo = values[0];
                                break;
                            case "ScaleWidthInside":
                                menuObject.ScaleWidthInside = float.Parse(values[0]);
                                break;
                            case "ScaleHeightInside":
                                menuObject.ScaleHeightInside = float.Parse(values[0]);
                                break;
                            case "Index":
                                menuObject.Index = Int32.Parse(values[0]);
                                break;
                            case "CurrentSelection":
                                menuObject.CurrentSelection = values[0] == "yes" ? true : false;
                                break;
                            case "IsClicked":
                                menuObject.IsClicked = values[0] == "yes" ? true : false;
                                break;
                            case "UseBigFont":
                                menuObject.UseBigFont = values[0] == "yes" ? true : false;
                                break;
                            case "DeSelectOnMove":
                                menuObject.deSelectOnMove = values[0] == "yes" ? true : false;
                                break;
                            case "Active":
                                menuObject.Active = values[0] == "no" ? false : true;
                                break;
                            default:
                                    Debug.LogError("key: " + key + "  Menu Panel not found !");
                                    break;
                        }
                    }
                    panel.Add(menuObject);                  
                }             
            }
            return panel;
        }
    }
}