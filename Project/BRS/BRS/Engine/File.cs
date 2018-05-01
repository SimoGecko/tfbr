// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BRS.Engine.Physics;
using BRS.Engine.Physics.RigidBodies;
using BRS.Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {
    class File : Component {
        ////////// class used to load files from disk and providing safeguard agains null files //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


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


        // queries



        // other

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

                    Debug.Log(string.Format("{0}: {1}", tagName, n));

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


        public static List<Tuple<string, string>> ReadRanking(string pathName) {
            try {
                List<Tuple<string, string>> listPerson = new List<Tuple<string, string>>();
                using (StreamReader reader =
                    new StreamReader(new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        if (line == "")
                            break;

                        //string aPerson = reader.ReadLine();

                        string[] pSplit = line.Split(' ');
                        listPerson.Add(new Tuple<string, string>(pSplit[0], pSplit[1]));

                    }
                }
                return listPerson;
            } catch (Exception e) {
                Debug.LogError(e.Message);

                return new List<Tuple<string, string>>();
            }
        }

        public static void WriteRanking(string pathName, List<Tuple<string, string>> listPlayersNameScore, int maxElem) {
            try {
                using (FileStream fs = System.IO.File.Open(pathName, FileMode.OpenOrCreate)) {
                    int count = 0;
                    foreach (var elem in listPlayersNameScore) {
                        AddText(fs, elem.Item1 + " " + elem.Item2 + "\n");
                        ++count;
                        if (count >= maxElem) break;
                    }
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        private static void AddText(FileStream fs, string value) {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

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
    }

}