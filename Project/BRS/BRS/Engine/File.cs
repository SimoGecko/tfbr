// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace BRS {
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
        public static T Load<T>(string s) {
            //TODO check first if file exists
            T result = content.Load<T>(s);
            if (result == null) Debug.LogError("incorrect path to file: " + s);
            return result;
        }


        // queries



        // other
        public static void ReadFile(string pathName) {
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
                        Quaternion rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(float.Parse(pSplit[2])), MathHelper.ToRadians(float.Parse(pSplit[1])), MathHelper.ToRadians(float.Parse(pSplit[3])));
                        Vector3 scale = new Vector3(float.Parse(sSplit[3]), float.Parse(sSplit[2]), float.Parse(sSplit[1]));

                        GameObject go = new GameObject(tagName + "_" + i.ToString(), File.Load<Model>("Models/primitives/" + prefabName));

                        go.transform.position = position;
                        go.transform.scale = scale;
                        //go.transform.rotation = rotation; // rotation not parsed correctly // <- of course it doesn't parse correctly, you use pSplit instead of rSplit!

                        if (tagName == "Ground") go.tag = ObjectTag.Ground;
                        else if (tagName == "Base") go.tag = ObjectTag.Base;
                        else if (tagName == "Obstacle") go.tag = ObjectTag.Obstacle;
                        else if (tagName == "Boundary") go.tag = ObjectTag.Boundary;
                        else if (tagName == "VaultDoor") go.tag = ObjectTag.Vault;
                    }
                    nameContent = reader.ReadLine();
                }
            }
        }

        public static void ReadHeistScene(string pathName) {
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

                        Vector3 position   = new Vector3(float.Parse(pSplit[1]), float.Parse(pSplit[2]), -float.Parse(pSplit[3]));
                        Vector3 eulerAngle = new Vector3(float.Parse(rSplit[1]), float.Parse(rSplit[2]),  float.Parse(rSplit[3]));
                        Quaternion rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(float.Parse(rSplit[2])), MathHelper.ToRadians(float.Parse(rSplit[1])), MathHelper.ToRadians(float.Parse(rSplit[3])));
                        Vector3 scale = new Vector3(float.Parse(sSplit[1]), float.Parse(sSplit[2]), float.Parse(sSplit[3]));

                        GameObject go = new GameObject(meshName + "_" + i.ToString(), File.Load<Model>("Models/polygonheist/" + meshName));

                        go.transform.position = position + new Vector3(0, 0, 30);
                        go.transform.scale = scale;
                        go.transform.rotation = rotation; // rotation not parsed correctly?
                    }
                    nameContent = reader.ReadLine(); // <end>
                }
            }
        }

    }

}