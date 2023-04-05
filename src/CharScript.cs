using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables; 
using UnityEditor.Animations;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine.Experimental.Animations;

//[ExecuteAlways]
public class CharScript : MonoBehaviour
{
    string[] meshNames = new string[] { "o_low_main", "o_low_parts", "o_up_main", "o_up_parts" };
    static string assetBundleDir = null;
    string settingsDir = null;
    string settingFilepath = null;
    [HideInInspector]
    public PlayableGraph playableGraph;
    public string animID = null;
    public string bodyID = null;
    public string faceID = null;
    public string hairID = null;
    public string weaponID = null;
    public string faceTextureID = null;
    public string[] accessoryIDs;
    public string propID = null;
    Dictionary<string, string> settingDict = new Dictionary<string, string>();
    Dictionary<string, string> accessorAttachDict = new Dictionary<string, string>();
    [HideInInspector]
    public GameObject bodyGO = null;
    [HideInInspector]
    public GameObject faceGO = null;
    [HideInInspector]
    public GameObject hairGO = null;
    [HideInInspector]
    public List<GameObject> accessoryGOs;
    [HideInInspector]
    public GameObject weaponGO = null;
    [HideInInspector]
    public AnimationClip animClip = null;
    [HideInInspector]
    public Texture2D faceTexture = null;
    TDShaderUtil shaderutil;
    TDMatUtil matutil;
    float scale = 1f;
    [HideInInspector]
    public string shaderType = "Unlit";
    private Deform deform;
  
    AnimationPlayableOutput bodyOutput;
    AnimationPlayableOutput faceOutput;
    AnimationPlayableOutput hairOutput;
    AnimationPlayableOutput weaponOutput;
    readonly Regex numbersIDRegex = new Regex("(c010)|(c011)|(c012)");
    readonly Regex angelsIDRegex = new Regex("(c802)|(c803)|(c805)");
    void LoadSettings(string filepath)
    {
        JObject o = null;
        if (File.Exists(filepath) == false)
        {
            filepath = settingFilepath;
        }
        using (StreamReader reader = File.OpenText(filepath)) {
            o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
        }
        
        this.animID = (string) o["anim"];
        this.bodyID = (string) o["body"];
        this.faceID = (string) o["face"];
        this.hairID = (string) o["hair"];
                
        IEnumerable<KeyValuePair<string,JToken>> accessoryQuery = from KeyValuePair<string, JToken> qkv in o
                                                     where qkv.Key.Contains("accy")
                                                     select qkv;
                var kvArray = accessoryQuery.ToArray<KeyValuePair<string, JToken>>();
                if (kvArray.Length > 0)
                {
                    Array.Resize<string>(ref this.accessoryIDs, kvArray.Length);
                    for (var i = 0; i < kvArray.Length; i++)
                    {
                        this.accessoryIDs[i] = (string) kvArray[i].Value;
                    }
                }
         
        try
        {
            this.faceTextureID = (string) o["faceTexture"];
        }
        catch (KeyNotFoundException) { this.faceTextureID = null; }

        try
        {
            weaponID = (string)o["weapon"];
        }
        catch (KeyNotFoundException) { weaponID = null; }
    }
    public void SaveSettings()
    {
        settingDict["anim"] = this.animID;
        settingDict["body"] = this.bodyID;
        settingDict["face"] = this.faceID;
        settingDict["hair"] = this.hairID;
        settingDict["weapon"] = this.weaponID;
        
        
        settingDict["faceTexture"] = this.faceTextureID;
        
        
       
        for(int i=0; i < accessoryIDs.Length; i++)
        {
            settingDict["accy" + i.ToString()] = accessoryIDs[i];
        }
        foreach (string key in settingDict.Keys)
        {
            if (settingDict[key] == null) { settingDict.Remove(key); }
        }
        var jsonStr = JsonConvert.SerializeObject(settingDict);
        var outputPath = Path.Combine(settingsDir, this.gameObject.name + ".json");
         using (var fileSt = new StreamWriter( File.Create(outputPath)))
        {
            fileSt.Write(jsonStr);
        }
        
    }
   
    public void LoadBody(string bodyID)
    {
        if (this.bodyGO != null)
        {
            DestroyImmediate(bodyGO);
            this.bodyGO = null;
        }
        var filepath = Path.Combine(assetBundleDir, bodyID);
        if (Path.GetExtension(filepath) != ".abap")
        {
            filepath = Path.ChangeExtension(filepath, ".abap");
        }
        var bodyAb = AssetBundle.LoadFromFile(filepath);
        try
        {
            var names = bodyAb.GetAllAssetNames();
            foreach (var name in names)
            {
                var item = bodyAb.LoadAsset(name);

                if (item is GameObject)
                {
                    var gobj = item as GameObject;
                 
                    gobj = Instantiate(gobj);
                    // 名前を戻す
                    gobj.name = item.name;
                    //animator = gobj.GetComponent<Animator>();
                    var skMeshRs = gobj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var skMeshR in skMeshRs)
                    {
                        if (matutil.GetType() !=  typeof(OrigialMatUtil)){
                            var mat = matutil.GetMaterial(skMeshR.material);
                            mat.shader = shaderutil.GetShader(mat.shader);
                            skMeshR.material = mat;


                        }
                        if (faceTextureID != null & skMeshR.material.name.Contains("skin"))
                        {

                            string textureName = "";
                            if (numbersIDRegex.IsMatch(faceTextureID) | angelsIDRegex.IsMatch(faceTextureID))
                            {
                                textureName = "v02_b0000c000s000_d";
                            }
                            else if (faceTextureID.Contains("c701"))
                            {
                                textureName = "v02_b0000c701s000_d";
                            }
                            if (textureName != "")
                            {
                                AssetBundle ab = null;
                                try
                                {
                                    ab = AssetBundle.LoadFromFile(Path.Combine(assetBundleDir, textureName + ".abap"));
                                    var tex2d = ab.LoadAsset<Texture2D>(textureName);
                                    skMeshR.material.mainTexture = tex2d;
                                        }
                                finally
                                {
                                    if (ab != null)
                                    {
                                        ab.Unload(false);
                                    }
                                }
                            }
                            
                        }
                    }
                    bodyGO = gobj;
                }
                if (item is Animator)
                {
                    //animator = item as Animator;

                }

            }


            this.bodyGO.transform.SetParent(this.gameObject.transform, false);
            if (playableGraph.IsValid())
            {
                ConfigurePlayable(ref bodyOutput, bodyGO.GetComponent<Animator>());
 
            }
        }

        finally
        {
            bodyAb.Unload(false);
            
        }
        //if (accessoryGOs.Count > 0)
        //{
        //    ConfigureAllAccessories();
        //}

    }
    public void LoadWeapon(string id) {
        if (this.weaponGO != null)
        {
            DestroyImmediate(weaponGO);
            this.weaponGO = null;
        }
        var filepath = Path.Combine(assetBundleDir, id);
        if (Path.GetExtension(filepath) != ".abap")
        {
            filepath = Path.ChangeExtension(filepath, ".abap");
        }
        var weaponAb = AssetBundle.LoadFromFile(filepath);
        try
        {
            var names = weaponAb.GetAllAssetNames();
            foreach (var name in names)
            {
                var item = weaponAb.LoadAsset(name);
                if (item is GameObject)
                {
                    var gobj = item as GameObject;
                    gobj = Instantiate(gobj);
                    // 名前を戻す
                    gobj.name = item.name;
                    var skMeshRs = gobj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var skMeshR in skMeshRs)
                    {
                        skMeshR.material.shader = shaderutil.GetShader(skMeshR.material.shader);
 
                    }
                    weaponGO = gobj;
                }
                if (item is Animator)
                {
                    //animator = item as Animator;

                }

            }


            weaponGO.transform.SetParent(this.gameObject.transform, false);
            if (playableGraph.IsValid())
            {
                ConfigurePlayable(ref weaponOutput, weaponGO.GetComponent<Animator>());

            }
        }

        finally
        {
            weaponAb.Unload(false);

        }
    }
    void ConfigurePlayable(ref AnimationPlayableOutput output, Animator animator)
    {
        output.SetTarget(animator);
        playableGraph.Evaluate(0f);
    }
    public void Deform(GameObject part)
    {
        var charIDRegex = new Regex(@"(c\d\d\d)");
       
        if (charIDRegex.IsMatch(this.faceTextureID))
        {
            if (deform == null)
            {
                deform = new Deform(charIDRegex.Match(this.faceTextureID).Value);
                deform.scale = 1f;
            }
            var roots = part.transform.GetComponentsInChildren<Transform>().Where(t => t.name == "root");
            foreach(var r in roots)
            {
                deform.DeformModel(r.gameObject);
            }
            
        }
    }
 
    public void LoadAnim(string animID)
    {
        if (playableGraph.IsValid() )
        {
            playableGraph.Destroy();
        }
        var filepath = Path.Combine(assetBundleDir, animID);
        if (Path.GetExtension(filepath) != ".abap")
        {
            filepath = Path.ChangeExtension(filepath, ".abap");
        }
        var animAb = AssetBundle.LoadFromFile(filepath);
        try
        {
            foreach (var item in animAb.LoadAllAssets())
            {
                if (item is AnimationClip)
                {

                    animClip = (item as AnimationClip);

                }
            }
         
            var bodyAnimator = bodyGO.GetComponent<Animator>();
            var faceAnimator = faceGO.GetComponent<Animator>();
            var hairAnimator = hairGO.GetComponent<Animator>();
            Animator weaponAnimator = null;
            if (weaponGO != null)
            {
               weaponAnimator = weaponGO.GetComponent<Animator>();
            }
            playableGraph = PlayableGraph.Create();
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            var bodyAnimOutput = AnimationPlayableOutput.Create(playableGraph, "body", bodyAnimator);
            var faceAnimOutput = AnimationPlayableOutput.Create(playableGraph, "face", faceAnimator);
            var hairAnimOutput = AnimationPlayableOutput.Create(playableGraph, "hair", hairAnimator);
            
            if (weaponGO != null)
            {
                AnimationPlayableOutput weaponAnimOutput;
                weaponAnimOutput = AnimationPlayableOutput.Create(playableGraph, "weapon", weaponAnimator);
                weaponAnimOutput.SetSourcePlayable(AnimationClipPlayable.Create(playableGraph, animClip));
                weaponOutput = weaponAnimOutput;
            }
            bodyAnimOutput.SetSourcePlayable(AnimationClipPlayable.Create(playableGraph, animClip));
            faceAnimOutput.SetSourcePlayable(AnimationClipPlayable.Create(playableGraph, animClip));
            hairAnimOutput.SetSourcePlayable(AnimationClipPlayable.Create(playableGraph, animClip));
            bodyOutput = bodyAnimOutput;
            faceOutput = faceAnimOutput;
            hairOutput = hairAnimOutput;            
        }
        finally
        {
            animAb.Unload(false);
        }
    }
    
    public void LoadFace(string faceID)
    {
        if (this.faceGO != null)
        {
            DestroyImmediate(faceGO);
            this.faceGO = null;
        }
        var filepath = Path.Combine(assetBundleDir, faceID);
        if (Path.GetExtension(filepath) != ".abap")
        {
            filepath = Path.ChangeExtension(filepath, ".abap");
        }
        var faceAb = AssetBundle.LoadFromFile(filepath);
        try
        {
            foreach (var item in faceAb.LoadAllAssets<GameObject>())
            {
                var gobj = Instantiate(item);
                gobj.name = item.name;
                faceGO = gobj;
                var skMeshRs = gobj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skMeshR in skMeshRs)
                {

                    var mat = matutil.GetMaterial(skMeshR.material);
                    mat.shader = shaderutil.GetShader(mat.shader);
                    skMeshR.material = mat;
  
                    if (faceTextureID != null)
                    {
                        if (skMeshR.material.mainTexture.name != faceTextureID)
                        {
                            LoadFaceTexture(faceTextureID);
                            skMeshR.material.mainTexture = faceTexture;
                        }
                    }
    
                }

            }
            this.faceGO.transform.SetParent(this.gameObject.transform, false);
            if (playableGraph.IsValid())
            {
                ConfigurePlayable(ref faceOutput, faceGO.GetComponent<Animator>());
            }
        }
        finally
        {
            faceAb.Unload(false);
        }
    }
    void LoadFaceTexture(string faceTextureID)
    {
        if (faceTexture != null)
        {
            
            faceTexture = null;
        }
        AssetBundle faceTextureAb;
        if (!(faceTextureID == null))
        {
            var filepath = Path.Combine(assetBundleDir, faceTextureID);
            if (Path.GetExtension(filepath) != ".abap")
            {
                filepath = Path.ChangeExtension(filepath, ".abap");
            }
            faceTextureAb = AssetBundle.LoadFromFile(filepath);
            try
            {
                foreach (var item in faceTextureAb.LoadAllAssets<Texture2D>())
                {

                    faceTexture = (item);
                }
            }
            finally
            {
                faceTextureAb.Unload(false);
            }
        }
    }
    public void LoadHair(string hairID)
    {
        if (this.hairGO != null)
        {
            DestroyImmediate(hairGO);
            this.hairGO = null;
        }
        var filepath = Path.Combine(assetBundleDir, hairID);
        if (Path.GetExtension(filepath) != ".abap")
        {
            filepath = Path.ChangeExtension(filepath, ".abap");
        }
        var hairAb = AssetBundle.LoadFromFile(filepath);
        try
        {
            foreach (var item in hairAb.LoadAllAssets<GameObject>())
            {
                var gobj = Instantiate(item);
                gobj.name = item.name;
                hairGO = gobj;
                var skMeshRs = gobj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skMeshR in skMeshRs)
                {
                    var mat = matutil.GetMaterial(skMeshR.material);
                    mat.shader = shaderutil.GetShader(mat.shader);
                    skMeshR.material = mat;
                }
                hairGO = gobj;
            }
            this.hairGO.transform.SetParent(this.gameObject.transform, false);
            if (playableGraph.IsValid())
            {
                ConfigurePlayable(ref hairOutput, hairGO.GetComponent<Animator>());
            }
        }
        finally
        {     
            hairAb.Unload(false);
        }
    }
    public void ConfigureAllAccessories()
    {
        foreach (var id in accessoryIDs) { ConfgiureAccessory(id); }
    }
    void ConfgiureAccessory(string id)
    {
        string parentName;
        accessorAttachDict.TryGetValue(id, out parentName);
        if (parentName == null)
        {
            if (id[0] == 'g' | id[0] == 'e')
            {
                parentName = "j_base_head";
            }
        }
       
        var accessoryGO = accessoryGOs.Find(go => go.name == id);
        if (accessoryGO == null) { return; }
        var roots = this.gameObject.transform.GetComponentsInChildren<Transform>().Where(t => t.name == "root");
        foreach (var root in roots)
        {
            var parent = root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == parentName);
            if (parent != null)
            {
                var pConst = accessoryGO.GetComponent<ParentConstraint>();
                if (pConst != null) { DestroyImmediate(pConst); }
              
                pConst = accessoryGO.AddComponent<ParentConstraint>();  
                var srcConst = new ConstraintSource
                {
                    sourceTransform = parent,
                    weight = 1f
                };
                pConst.AddSource(srcConst);

                if (id[0] == 'g' | id[0] == 'e')
                {
                    var rOffset = pConst.GetRotationOffset(0);
                    rOffset.y = -90;
                    pConst.SetRotationOffset(0, rOffset);
                }
                pConst.constraintActive = true;
                var sConst = accessoryGO.GetComponent<ScaleConstraint>();
                if (sConst != null) { DestroyImmediate(sConst); }               
                sConst = accessoryGO.AddComponent<ScaleConstraint>();
                
                var srcSConst = new ConstraintSource { sourceTransform = parent, weight = 1f };
                sConst.AddSource(srcSConst);
                sConst.constraintActive = true;
                break;
            }


        }
        accessoryGO.transform.SetParent(gameObject.transform, false);
    }
    public void ConfigureWeapon(string id) {
        if (weaponGO == null) { return; }
        string attachID;
        if (id.Contains("_l_")) { attachID = "j_base_weapon_L"; }
        else { attachID = "j_base_weapon_R"; }
       
        
        var roots = this.gameObject.transform.GetComponentsInChildren<Transform>().Where(t => t.name == "root");
        var body = roots.First(t => t.parent.name.StartsWith("b"));
        var attachGO = body.GetComponentsInChildren<Transform>().First(t => t.name == attachID).gameObject;
        
          
        
        var pConst = weaponGO.GetComponent<ParentConstraint>();
        if (pConst != null) { DestroyImmediate(pConst); }

        pConst = weaponGO.AddComponent<ParentConstraint>();
        var srcConst = new ConstraintSource
        {
            sourceTransform = attachGO.transform,
            weight = 1f
        };
        pConst.AddSource(srcConst);
        pConst.constraintActive = true;
    }
    public void LoadAccessory(String[] accessoryIDs)
    {
        if (accessoryGOs.Count > 0)
        {
            foreach(var gobj in accessoryGOs)
            {
                if (gobj != null) Destroy(gobj);
            }
            accessoryGOs.Clear();
        }
        foreach(var id in accessoryIDs)
        {
            LoadAccessory(id);
        }
        for (var i = 0; i < accessoryIDs.Length; i++)
        {
            var id = accessoryIDs[i];
            ConfgiureAccessory(id);
        }
    }
    public void LoadAccessory(String accessoryID)
    {
        if (accessoryID != null & accessoryID != "")
        {
            var filepath = Path.Combine(assetBundleDir, accessoryID);
            if (Path.GetExtension(filepath) != ".abap")
            {
                filepath = Path.ChangeExtension(filepath, ".abap");
            }
            var ab = AssetBundle.LoadFromFile(filepath);
            try
            {
                foreach (var item in ab.LoadAllAssets<GameObject>())
                {
                    var gobj = Instantiate(item);
                    gobj.name = item.name;
                    this.accessoryGOs.Add(gobj);
                   
                    var skMeshRs = gobj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var skMeshR in skMeshRs)
                    {
                        var mat = matutil.GetMaterial(skMeshR.material);
                        mat.shader = shaderutil.GetShader(mat.shader);
                        skMeshR.material = mat;
                    }
                    var meshRs = gobj.GetComponentsInChildren<MeshRenderer>();
                    foreach (var meshR in meshRs)
                    {
                        var mat = matutil.GetMaterial(meshR.material);
                        mat.shader = shaderutil.GetShader(mat.shader);
                        meshR.material = mat;
                    }

                }
            }
            finally
            {
                ab.Unload(false);
            }
        }
    }
    public void LoadProp(String propID)
    {
        if (propID != null & propID != "")
        {
            var filepath = Path.Combine(assetBundleDir, propID);
            if (Path.GetExtension(filepath) != ".abap")
            {
                filepath = Path.ChangeExtension(filepath, ".abap");
            }
            var ab = AssetBundle.LoadFromFile(filepath);
            try
            {
                foreach (var item in ab.LoadAllAssets<GameObject>())
                {
                    var gobj = Instantiate(item);
                    gobj.name = item.name;
                    var skMeshRs = gobj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var skMeshR in skMeshRs)
                    {
                        var mat = matutil.GetMaterial(skMeshR.material);
                        mat.shader = shaderutil.GetShader(mat.shader);
                        skMeshR.material = mat;
                    }
                    var meshRs = gobj.GetComponentsInChildren<MeshRenderer>();
                    foreach (var meshR in meshRs)
                    {
                        var mat = matutil.GetMaterial(meshR.material);
                        mat.shader = shaderutil.GetShader(mat.shader);
                        meshR.material = mat;
                    }

                }
            }
            finally
            {
                ab.Unload(false);
            }
        }
    }
        // Start is called before the first frame update
        void Start()
    {
        if (shaderType == "UnityChanToonShader")
        {
            shaderutil = new UTSShaderUtil();
            matutil = new UTSMatUtil();
        }
        else if (shaderType == "Unlit")
        {
            shaderutil = new UnlitShaderUtil();
            matutil = new UnlitMatUtil();
        }
        else if (shaderType == "Original")
        {
            shaderutil = new OriginalShaderUtil();
            matutil = new OrigialMatUtil();
        }
        if (CharScript.assetBundleDir == null)
        {
            var primDirTxtPath = Path.Combine(Application.dataPath, "prim_dir.txt");
            if (File.Exists(primDirTxtPath))
            {
                CharScript.assetBundleDir = File.ReadAllText(primDirTxtPath).TrimEnd();
            }
            else
            {
                var selectedDir = EditorUtility.OpenFolderPanel("ゲームデータのあるディレクトリ(prim)を選択してください", "", "");
                if (string.IsNullOrEmpty(selectedDir))
                {
                    throw new DirectoryNotFoundException("ゲームデータのあるディレクトリ(prim)が選択されていません。");
                }
                else
                {
                    CharScript.assetBundleDir = selectedDir;
                    if (Directory.Exists(CharScript.assetBundleDir))
                    {

                        File.WriteAllText(primDirTxtPath, selectedDir);
                    }
                }
            }
        }
        this.settingsDir = Path.Combine(Application.dataPath, "CharacterSettings");
        this.settingFilepath = Path.Combine(settingsDir, "Default.json");
        LoadSettings(Path.Combine(settingsDir , this.gameObject.name + ".json"));
        var accessoryDicPath = Path.Combine(settingsDir, "accessory_dic.json");
        JObject o = null;
        if (File.Exists(accessoryDicPath) )
        {
            var filepath = accessoryDicPath;

            using (StreamReader reader = File.OpenText(filepath))
            {
                o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }
        }
        else { Debug.Log(accessoryDicPath); }
        accessorAttachDict = o.ToObject<Dictionary<string, string>>();
        LoadFace(faceID);
        LoadHair(hairID);
        LoadBody(bodyID);
        LoadAccessory(accessoryIDs);
        LoadProp(propID);
        var charGO = this.gameObject;
        charGO.transform.localScale = new Vector3(scale, scale, scale);
        Deform(faceGO);
        Deform(bodyGO);
        Deform(hairGO);
    }

    
    private void OnDestroy()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }
    }
}
