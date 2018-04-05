using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;


public class SceneObjectsSaver : MonoBehaviour {


    public string path;

	// Use this for initialization
	void Start () {
        ExportNico1();
    }
	
	void Update () {
		
	}

    /* FORMAT:
     *      #Objects
     *      position
     *      scale
     */
    void WriteToFile(string pathName, GameObject[] obj, string nameContent, string tagName, string prefabName) {
        using (StreamWriter file = new StreamWriter(@pathName, true))
        {
            file.WriteLine("<" + nameContent + ">");
            file.WriteLine("tag: " + tagName);
            file.WriteLine("prefabName: " + prefabName);
            file.WriteLine("amount: " + obj.Length.ToString());

            foreach (GameObject go in obj)
            {
                file.WriteLine("pos: "
                               + go.transform.position.x + " "
                               + go.transform.position.y + " "
                               + go.transform.position.z);

                file.WriteLine("rot: "
                               + go.transform.eulerAngles.x + " "
                               + go.transform.eulerAngles.y + " "
                               + go.transform.eulerAngles.z);

                file.WriteLine("sca: "
                               + go.transform.lossyScale.x + " "
                               + go.transform.lossyScale.y + " "
                               + go.transform.lossyScale.z);
            }

            file.WriteLine("</" + nameContent + ">");
            file.WriteLine("");
        }
    }

    public void ExportNico1() {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Ground");
        GameObject[] b = GameObject.FindGameObjectsWithTag("Base");
        GameObject[] so = GameObject.FindGameObjectsWithTag("StaticObstacle");
        GameObject[] dO = GameObject.FindGameObjectsWithTag("DynamicObstacle");
        GameObject[] bo = GameObject.FindGameObjectsWithTag("Boundary");

        WriteToFile("UnitySceneData/ObjectSceneUnity.txt", p, "Ground", "Ground", "gplane");
        WriteToFile("UnitySceneData/ObjectSceneUnity.txt", b, "Bases", "Base", "cube");
        WriteToFile("UnitySceneData/ObjectSceneUnity.txt", so, "StaticObstacles", "StaticObstacle", "cube");
        WriteToFile("UnitySceneData/ObjectSceneUnity.txt", dO, "DynamicObstacles", "DynamicObstacle", "cube");
        WriteToFile("UnitySceneData/ObjectSceneUnity.txt", bo, "Boundaries", "Boundary", "cube");
    }

    
}
