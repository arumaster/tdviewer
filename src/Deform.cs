using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Animations;
public class Deform
{
    public float scale = 1f;
    string deformDirPath = null;
    string name;
    JObject dict;
	public Deform(string name, float scale = 1.0f)
	{
        this.name = name;
        this.scale = scale;
        deformDirPath = Path.Combine(Application.dataPath, "CharacterSettings/Deform");
        var filepath = Path.Combine(deformDirPath, name + ".json");
       
        if (File.Exists(filepath) == false)
        {
            return;
        }
        using (StreamReader reader = File.OpenText(filepath))
        {
            dict = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
        }

    }
    public void DeformModel(GameObject root) 
	{

        //Debug.Log(kv.Key);
        if (dict != null)
        {
            var ts = root.GetComponentsInChildren<Transform>();
            Transform sConstTransform = root.transform.parent.Find("ScaleConst");
            if (sConstTransform == null)
            {
                var sConstGObjRoot = new GameObject("ScaleConst");
                sConstTransform = sConstGObjRoot.transform;
                sConstTransform.parent = root.transform.parent;
            }
            foreach (Transform t in ts)
            {
                //Debug.Log(t.name);
                if (dict.ContainsKey(t.name))
                {
                    JArray f3 = (JArray)dict[t.name];
                    var vec = new Vector3(f3[0].Value<float>(), f3[1].Value<float>(), f3[2].Value<float>());
                    vec.Scale(new Vector3(scale, scale, scale));
                    var sConst = t.gameObject.AddComponent<ScaleConstraint>();
                    var gobj = new GameObject("ScaleConst_"+ t.name);
                    gobj.transform.localScale = vec;
                    gobj.transform.parent = sConstTransform;
                    var cSource = new ConstraintSource();
                    cSource.sourceTransform = gobj.transform;
                    cSource.weight = 1f;
                
                    sConst.AddSource(cSource);
                    sConst.constraintActive = true;
                }


            }
        }
        //root.transform.Find
	}
}
