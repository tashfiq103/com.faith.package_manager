namespace com.faith.packagemanager
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(GitRepoInfo))]
    public class GitRepoInfoEditor : Editor
    {
        private GitRepoInfo Reference;

        private SerializedProperty m_SPShowDeveloperPanel;
        private SerializedProperty m_SPShowLocalRepositoryInfo;
        private SerializedProperty m_SPNameOfFile;
        private SerializedProperty m_SPDownloadURL;
        private SerializedProperty m_SPRemoteGitInfos;
        private SerializedProperty m_SPGitInfos;
        

        private void OnEnable()
        {
            Reference = (GitRepoInfo)target;

            if (Reference == null)
                return;

            m_SPShowDeveloperPanel = serializedObject.FindProperty("showDeveloperPanel");
            m_SPShowLocalRepositoryInfo = serializedObject.FindProperty("showLocalRepositoryInfo");
            m_SPNameOfFile = serializedObject.FindProperty("nameOfFile");
            m_SPDownloadURL = serializedObject.FindProperty("downloadURL");
            m_SPRemoteGitInfos = serializedObject.FindProperty("remoteGitInfos");
            m_SPGitInfos = serializedObject.FindProperty("gitInfos");
            

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            {
                m_SPShowDeveloperPanel.boolValue = EditorGUILayout.Foldout(
                    m_SPShowDeveloperPanel.boolValue,
                    "Developer Panel",
                    true
                );

                if (m_SPShowDeveloperPanel.boolValue && GUILayout.Button("GenerateJSON", GUILayout.Width(100f))) {

                    Reference.SaveRespositoryInfoAsJson();
                }

                if (m_SPShowDeveloperPanel.boolValue && GUILayout.Button("UploadJSON", GUILayout.Width(100f)))
                {

                    Reference.UploadToJsonFileToWebURL();
                }
            }
            EditorGUILayout.EndHorizontal();

            

            if (m_SPShowDeveloperPanel.boolValue) {

                EditorGUI.indentLevel += 1;

                EditorGUILayout.PropertyField(m_SPShowLocalRepositoryInfo);
                EditorGUILayout.PropertyField(m_SPNameOfFile);
                EditorGUILayout.PropertyField(m_SPDownloadURL);
                EditorGUILayout.PropertyField(m_SPRemoteGitInfos);

                EditorGUILayout.Space();

                EditorGUI.indentLevel -= 1;

            }

            if(m_SPShowLocalRepositoryInfo.boolValue)
                EditorGUILayout.PropertyField(m_SPGitInfos);

            serializedObject.ApplyModifiedProperties();
        }
    }
}



