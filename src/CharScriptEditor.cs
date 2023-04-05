using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEditor;

[CustomEditor(typeof(CharScript))]
[CanEditMultipleObjects]
public class CharScriptEditor : Editor
{
    string[] shaderSelections = { "Unlit", "UnityChanToonShader", "Original"};
    bool isOpenAnimation = true;
    bool isOpenModel = true;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var cs = target as CharScript;
        var selected = EditorGUILayout.Popup("シェーダー", Array.IndexOf(shaderSelections, cs.shaderType), shaderSelections);

        if (shaderSelections[selected] != cs.shaderType)
        {
            cs.shaderType = shaderSelections[selected];
            EditorUtility.SetDirty(cs);
        }

        isOpenModel = EditorGUILayout.BeginFoldoutHeaderGroup(isOpenModel, "Model");
        if (isOpenModel)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if ((GUILayout.Button("Load Anim")))
                {
                    cs.LoadAnim(cs.animID);
                }
                if (GUILayout.Button("Load Body"))
                {
                    cs.LoadBody(cs.bodyID);
                    cs.Deform(cs.bodyGO);
                    cs.ConfigureAllAccessories();
                }
                if ((GUILayout.Button("Load Face")))
                {
                    cs.LoadFace(cs.faceID);
                    cs.Deform(cs.faceGO);
                    cs.ConfigureAllAccessories();
                }

                if ((GUILayout.Button("Load Hair")))
                {
                    cs.LoadHair(cs.hairID);
                    cs.Deform(cs.hairGO);
                    cs.ConfigureAllAccessories();
                }
                if ((GUILayout.Button("Load Accessorys")))
                {
                    cs.LoadAccessory(cs.accessoryIDs);
                }
                if ((GUILayout.Button("Load Weapon")))
                {
                    cs.LoadWeapon(cs.weaponID);
                    cs.ConfigureWeapon(cs.weaponID);
                }
                if ((GUILayout.Button("Load Prop")))
                {
                    cs.LoadProp(cs.propID);
                }
            }
            EditorGUILayout.Space();
            if ((GUILayout.Button("Save Settings")))
            {
                cs.SaveSettings();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        isOpenAnimation = EditorGUILayout.BeginFoldoutHeaderGroup(isOpenAnimation, "Animation");
        if (isOpenAnimation)
        {
            using (var vs = new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button("Play"))
                {
                    if (cs.playableGraph.IsValid())
                    {
                        for (int i = 0; i < cs.playableGraph.GetRootPlayableCount(); i++)
                        {
                            var playable = cs.playableGraph.GetRootPlayable(i);
                            playable.SetTime(0.0d);
                            if (playable.GetPlayState() == PlayState.Paused)
                            {
                                playable.Play();
                            }

                        }
                        cs.playableGraph.Play();

                    }
                    else
                    {
                        cs.LoadAnim(cs.animID);
                        cs.playableGraph.Play();
                    }
                }
                if ((GUILayout.Button("Stop")))
                {
                    if (cs.playableGraph.IsValid())
                    {
                        cs.playableGraph.Stop();


                    }
                }
                if ((GUILayout.Button("Pause")))
                {
                    if (cs.playableGraph.IsValid())
                    {
                        for (int i = 0; i < cs.playableGraph.GetRootPlayableCount(); i++)
                        {
                            var playable = cs.playableGraph.GetRootPlayable(i);
                            if (playable.GetPlayState() == PlayState.Paused)
                            {
                                playable.Play();
                            }
                            else
                            {
                                playable.Pause();
                            }
                        }
                    }
                }
                if ((GUILayout.Button("Resume")))
                {
                    for (int i = 0; i < cs.playableGraph.GetRootPlayableCount(); i++)
                    {
                        var playable = cs.playableGraph.GetRootPlayable(i);

                        if (playable.GetPlayState() == PlayState.Paused)
                        {
                            playable.Play();
                        }

                    }

                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}

