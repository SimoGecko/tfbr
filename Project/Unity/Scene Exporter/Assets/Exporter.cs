// (c) Simone Guggiari 2017

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


////////// exporter that deals with multiple objects having the same mesh //////////

public class Exporter : MonoBehaviour {
    // --------------------- VARIABLES ---------------------

    //public
    public string fileName = "export1.txt";
    public string modelFilePath = "Assets/PolygonHeist/model_list.txt";

    //private
    Dictionary<string, List<Transform>> sceneList;
    List<string> modelList;


    //references
    public Transform rootScene;


	// --------------------- BASE METHODS ------------------
	void Start () {

	}
	
	void Update () {
		
	}



    // --------------------- CUSTOM METHODS ----------------


    // commands
    [ContextMenu("Export")]
    void Export() {
        modelList = new List<string>();
        ReadModelFile();
        
        sceneList = new Dictionary<string, List<Transform>>();
        DFS(rootScene);
        WriteToFile();
    }

    //fills list with recursive DFS
    void DFS(Transform root) {
        for(int i=0; i<root.childCount; i++) {
            Transform t = root.GetChild(i);
            if(t.gameObject.GetComponent<MeshFilter>() != null) {
                string filterName = t.gameObject.GetComponent<MeshFilter>().sharedMesh.name;
                string meshName = filterName.Substring(filterName.IndexOf(' ') + 1);
                if (!sceneList.ContainsKey(meshName))
                    sceneList.Add(meshName, new List<Transform>());
                sceneList[meshName].Add(t);
            }
            DFS(t);
        }
    }

    //note it doesn't work with composite meshes





    // queries



    // other
    void WriteToFile() {
        using (StreamWriter file = new StreamWriter(@fileName, true)) {
            foreach(var elem in sceneList) {
            
                //check if must skip
                if (!modelList.Contains(elem.Key)) {
                    Debug.Log("skipped file " + elem.Key);
                    continue;
                }

                file.WriteLine("<begin>");
                file.WriteLine("prefabName: " + elem.Key);
                file.WriteLine("amount: " + elem.Value.Count);
                file.WriteLine("");

                foreach (Transform t in elem.Value) {
                    file.WriteLine("pos: " + t.position.x +    " " + t.position.y +    " " + t.position.z);
                    file.WriteLine("rot: " + t.eulerAngles.x + " " + t.eulerAngles.y + " " + t.eulerAngles.z);
                    file.WriteLine("sca: " + t.lossyScale.x +  " " + t.lossyScale.y +  " " + t.lossyScale.z);
                    file.WriteLine("");
                }

                file.WriteLine("<end>");
                file.WriteLine("");
            }
        }
    }
   
    public void ReadModelFile() {
        string[] lines = System.IO.File.ReadAllLines(modelFilePath);
        foreach (string line in lines) {
            string meshName = line.Substring(0, line.Length - 4);//cut last 4 chars away (.fbx)
            modelList.Add(meshName);
        }
    }

}