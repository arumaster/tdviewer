using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
[UnityEditor.InitializeOnLoad]
#endif
public class GM : MonoBehaviour
{
    public static string assetBundleDir;
    public float timeScale = 1;
    void Awake()
    {
    
        if (assetBundleDir == null)
        {
            var primDirTxtPath = Path.Combine(Application.dataPath, "prim_dir.txt");
            if (File.Exists(primDirTxtPath))
            {
                assetBundleDir = File.ReadAllText(primDirTxtPath).TrimEnd();
            }
            else
            {
                var selectedDir = EditorUtility.OpenFolderPanel("ゲームデータのあるディレクトリ(prim)を選択してください", "", "");
                if (string.IsNullOrEmpty(selectedDir))
                {
                    throw new DirectoryNotFoundException("prim ディレクトリを指定してください。");
                }
                else
                {
                    assetBundleDir = selectedDir;
                    if (Directory.Exists(assetBundleDir))
                    {

                        File.WriteAllText(primDirTxtPath, selectedDir);
                    }
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

[CustomEditor(typeof(GM))]

class GMEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set TimeScale"))
        {
           
            Time.timeScale = (target as GM).timeScale;
        }
        
    }

}