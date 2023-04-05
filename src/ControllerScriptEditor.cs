using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControllerScript))]
public class ControllerScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Play All"))
        {
            var cs = target as ControllerScript;
            cs.PlayAll();
        }
        if (GUILayout.Button("Stop All"))
        {
            var cs = target as ControllerScript;
            cs.StopAll();
        }
        if (GUILayout.Button("Pause All"))
        {
            var cs = target as ControllerScript;
            cs.PauseAll();
        }
        if (GUILayout.Button("Resume All"))
        {
            var cs = target as ControllerScript;
            cs.ResumeAll();
        }
    }
}
