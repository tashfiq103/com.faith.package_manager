using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JsonConverter))]
public class JsonConverterEditor : Editor
{
    private JsonConverter Reference;

    private void OnEnable(){

        Reference = (JsonConverter) target;
    }

    public override void OnInspectorGUI(){


        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        {
            if(GUILayout.Button("Get:SerialiedObject")){

                Reference.SaveRespositoryInfoAsJson();
            }

            if(GUILayout.Button("Get:Download")){

                Reference.DownloadRespositoryInfoAsJson();
            }
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}
