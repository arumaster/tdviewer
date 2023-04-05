using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class BackgroundScript: MonoBehaviour
{
    public string bgFilename = "bg_1007.abap";
    public Image image;
    public Sprite LoadBgSprite(string filename) {
        var abFilepath = Path.Combine(GM.assetBundleDir, filename);
        if (Path.GetExtension(abFilepath) != ".abap")
        {
            abFilepath = abFilepath + ".abap";
        }
        var ab = AssetBundle.LoadFromFile(abFilepath);
        Sprite sp;
        try
        {
            sp = Instantiate(ab.LoadAsset<Sprite>(Path.GetFileNameWithoutExtension(filename)));
        }
        finally
        {
            ab.Unload(false);
        }
        return sp;
    }
    public void SetupBackground()
    {
   
        var canvas = gameObject.GetComponent<Canvas>();
        GameObject bgImage = null;
        if (canvas == null) {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 13;
            if (gameObject.transform.Find("BgImage") == null)
            {
                bgImage = new GameObject("BgImage");
                bgImage.transform.SetParent(gameObject.transform, false);

            }
            else
            {
                bgImage = gameObject.transform.Find("BgImage").gameObject;
            }
            var fitter = bgImage.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = 1.33333f;

        }
     
        image = bgImage.GetComponent<UnityEngine.UI.Image>();
        if (image == null)
        {
            image = bgImage.AddComponent<UnityEngine.UI.Image>();   
            image.type = Image.Type.Simple;
        }

    }
    void Start()
    {
        SetupBackground();
        var bgSprite = LoadBgSprite(bgFilename);
        
        image.sprite = bgSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CustomEditor(typeof(BackgroundScript))]
public class BackgroundScriptEditor : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var bgs = target as BackgroundScript;
        if (GUILayout.Button("Load Bg")){
            var sprite = bgs.LoadBgSprite(bgs.bgFilename);
            bgs.image.sprite = sprite;
        }
    }
}