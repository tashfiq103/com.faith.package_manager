﻿namespace com.faith.package_manager {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class PackageManagerEditorWindow : EditorWindow {

        #region CustomVariables

        private struct PackageInfo {
            public string packageName;
            public string packageURL;
        }

        #endregion

        #region Private Variables

        private static PackageManagerEditorWindow m_PackageManagerEditorWindow;
        private static GitRepoInfo GitRepositoryInfo;
        private bool IS_IN_DEVELOPMENT_MODE;

        private Color m_DefaultGUIBackgroundColor;
        private Color m_ColorForPackageInfo = new Color (0.7254902f, 0.7254902f, 0.7254902f, 1f);
        private Color m_OnSelectedGUIBackgroundColorForButtonInPackageList = new Color (0.2431373f, 0.4901961f, 0.9058824f, 1f);

        private GUIStyle m_DefaultGUIStyleForButtonInPackageList;
        private GUIStyle m_OnSelectedGUIStyleForButtonInPackageList;

        private Texture2D m_IconForTickMark;
        private Texture2D m_IconForDownload;
        private Texture2D m_BackgroundTextureOfPackageList;
        private Texture2D m_BackgroundTextureOfPackageDescription;
        private Texture2D m_BackgroundTextureOfHeader;
        private Texture2D m_BackgroundTextureOfFooter;

        private Vector2 m_ScrollPositionAtPackageList;
        private Vector2 m_ScrollPositionAtPackageDescription;

        private string m_NameOfManifestDirectory = "Packages";
        private string m_PackageName;
        private string m_RepositoryLink;

        private bool[] m_IsPackageLoaded;

        private int m_SelectedPackageIndex = 0;

        #endregion

        #region Public Callback :   Static

        [MenuItem ("FAITH/PackageManager")]
        public static void ShowWindow () {

            m_PackageManagerEditorWindow = (PackageManagerEditorWindow) GetWindow<PackageManagerEditorWindow> ("Package Manager", typeof (PackageManagerEditorWindow));

            m_PackageManagerEditorWindow.minSize = new Vector2 (360f, 240f);
            m_PackageManagerEditorWindow.Show ();
        }

        public static void HideWindow () {

        }

        #endregion

        #region EditorWindow

        private void OnEnable () {

            Initialization ();
        }

        private void OnGUI () {

            DrawHeader ();
            DrawPackageList ();
            DrawPackageDescription ();
            DrawFooter ();
        }

        #endregion

        #region Custom OnGUI

        private void Initialization () {

            IS_IN_DEVELOPMENT_MODE = IsRepositoryInAssetFolder ("com.faith.package_manager");

            string[] m_GUIDOfGitRepositoryInfo = AssetDatabase.FindAssets ("GitRepositoryInfo");

            if (m_GUIDOfGitRepositoryInfo.Length > 0) {

                string t_AssetPath = AssetDatabase.GUIDToAssetPath (m_GUIDOfGitRepositoryInfo[0]);
                GitRepositoryInfo = (GitRepoInfo) AssetDatabase.LoadAssetAtPath (t_AssetPath, typeof (GitRepoInfo));
            }

            m_SelectedPackageIndex = 0;

            UpdatePackageLoadedInfo ();

            if (IS_IN_DEVELOPMENT_MODE) {

                m_IconForTickMark = GetTexture ("Icon_TickMark", "Assets/com.faith.package_manager/Icons");
                m_IconForDownload = GetTexture ("Icon_Download", "Assets/com.faith.package_manager/Icons");
            } else {

                m_IconForTickMark = GetTexture ("Icon_TickMark", GetFolderName ("com.faith.package_manager") + "/Icons");
                m_IconForDownload = GetTexture ("Icon_Download", GetFolderName ("com.faith.package_manager") + "/Icons");
            }

            m_BackgroundTextureOfHeader = new Texture2D (1, 1);
            m_BackgroundTextureOfHeader.SetPixel (
                0,
                0,
                new Color (
                    0.8352942f,
                    0.8352942f,
                    0.8352942f,
                    1f)
            );
            m_BackgroundTextureOfHeader.Apply ();

            m_BackgroundTextureOfPackageList = new Texture2D (1, 1);
            m_BackgroundTextureOfPackageList.SetPixel (
                0,
                0,
                m_ColorForPackageInfo
            );
            m_BackgroundTextureOfPackageList.Apply ();

            m_BackgroundTextureOfPackageDescription = new Texture2D (1, 1);
            m_BackgroundTextureOfPackageDescription.SetPixel (
                0,
                0,
                m_ColorForPackageInfo
            );
            m_BackgroundTextureOfPackageDescription.Apply ();

            m_BackgroundTextureOfFooter = new Texture2D (1, 1);
            m_BackgroundTextureOfFooter.SetPixel (
                0,
                0,
                new Color (
                    0.8352942f,
                    0.8352942f,
                    0.8352942f,
                    1f)
            );
            m_BackgroundTextureOfFooter.Apply ();

            m_DefaultGUIBackgroundColor = GUI.backgroundColor;

            m_DefaultGUIStyleForButtonInPackageList = new GUIStyle () {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft
            };
            Texture2D t_DefaultBackgroundTexture = new Texture2D (1, 1);
            t_DefaultBackgroundTexture.SetPixel (1, 1, m_ColorForPackageInfo);
            t_DefaultBackgroundTexture.Apply ();
            m_DefaultGUIStyleForButtonInPackageList.normal.background = t_DefaultBackgroundTexture;
            m_DefaultGUIStyleForButtonInPackageList.normal.textColor = Color.black;

            m_OnSelectedGUIStyleForButtonInPackageList = new GUIStyle () {
                richText = true,
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft
            };
            Texture2D t_OnSelectedBackgroundTexture = new Texture2D (1, 1);
            t_OnSelectedBackgroundTexture.SetPixel (1, 1, m_OnSelectedGUIBackgroundColorForButtonInPackageList);
            t_OnSelectedBackgroundTexture.Apply ();
            m_OnSelectedGUIStyleForButtonInPackageList.normal.background = t_OnSelectedBackgroundTexture;
            m_OnSelectedGUIStyleForButtonInPackageList.normal.textColor = Color.white;
        }

        private void DrawHeader () {
            Vector2 t_PanelOriginOfHeaderPanel = new Vector2 (0, 1);
            Rect t_RectTransformOfHeaderPanel = new Rect (
                t_PanelOriginOfHeaderPanel.x,
                t_PanelOriginOfHeaderPanel.y,
                Screen.width,
                18f
            );
            GUI.DrawTexture (t_RectTransformOfHeaderPanel, m_BackgroundTextureOfHeader);
        }
        private void DrawPackageList () {
            Vector2 t_PanelOriginOfPackageListPanel = new Vector2 (0, 20);
            Rect t_RectTransformOfPackageListPanel = new Rect (
                t_PanelOriginOfPackageListPanel.x,
                t_PanelOriginOfPackageListPanel.y,
                180,
                Screen.height - (t_PanelOriginOfPackageListPanel.y * 3.5f)
            );

            GUI.DrawTexture (t_RectTransformOfPackageListPanel, m_BackgroundTextureOfPackageList);

            GUILayout.BeginArea (t_RectTransformOfPackageListPanel);
            m_ScrollPositionAtPackageList = GUILayout.BeginScrollView (
                m_ScrollPositionAtPackageList,
                false,
                false
            ); {

                int t_NumberOfGitRepositoryInfo = GitRepositoryInfo.gitInfos.Count;

                for (int i = 0; i < t_NumberOfGitRepositoryInfo; i++) {

                    GUIStyle t_GUIStyleForButton;
                    GUIStyle t_GUIStyleForLabel = GUI.skin.label;
                    if (i == m_SelectedPackageIndex) {
                        t_GUIStyleForButton = new GUIStyle (m_OnSelectedGUIStyleForButtonInPackageList);
                        t_GUIStyleForLabel.normal.textColor = Color.white;
                    } else {
                        t_GUIStyleForButton = new GUIStyle (m_DefaultGUIStyleForButtonInPackageList);
                        t_GUIStyleForLabel.normal.textColor = Color.black;
                    }

                    if (GUI.Button (new Rect (0, 28 * i, 180, 25), "", t_GUIStyleForButton)) {

                        m_SelectedPackageIndex = i;
                    }

                    GUI.Label (new Rect (0, 28 * i, 160, 25), "  " + GitRepositoryInfo.gitInfos[i].displayName, t_GUIStyleForLabel);
                    GUI.Label (
                        new Rect (160, 30 * i, 20, 20),
                        m_IsPackageLoaded[i] ? m_IconForTickMark : m_IconForDownload);
                    // GUILayout.BeginHorizontal (GUI.skin.textArea); {

                    //     if (GUILayout.Button (m_GitRepositoryInfo.gitInfos[i].displayName, m_GUIStyleForButtonInPackageList, GUILayout.Height (25))) {

                    //         m_SelectedPackageIndex = i;
                    //     }
                    //     GUILayout.Label (m_IsPackageLoaded[i] ? m_IconForTickMark : new Texture2D (1, 1), GUILayout.Width (20), GUILayout.Height (20));
                    // }
                    // GUILayout.EndHorizontal ();

                }
            }
            EditorGUILayout.EndScrollView ();
            GUILayout.EndArea ();
        }
        private void DrawPackageDescription () {

            Vector2 t_PanelOriginOfPackageDescriptionPanel = new Vector2 (185, 20);
            GUI.DrawTexture (new Rect (
                    t_PanelOriginOfPackageDescriptionPanel.x,
                    t_PanelOriginOfPackageDescriptionPanel.y,
                    Screen.width - 185,
                    Screen.height - (t_PanelOriginOfPackageDescriptionPanel.y * 3.5f)
                ),
                m_BackgroundTextureOfPackageDescription);

            Rect t_RectTransformOfPackageListPanel = new Rect (
                t_PanelOriginOfPackageDescriptionPanel.x,
                t_PanelOriginOfPackageDescriptionPanel.y,
                360f,
                Screen.height - (t_PanelOriginOfPackageDescriptionPanel.y * 3.5f)
            );

            GUILayout.BeginArea (t_RectTransformOfPackageListPanel); {
                m_ScrollPositionAtPackageDescription = GUILayout.BeginScrollView (m_ScrollPositionAtPackageDescription); {
                    EditorGUILayout.BeginVertical (); {
                        EditorGUILayout.LabelField (
                            GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].displayName + " : v" + GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].version,
                            EditorStyles.boldLabel);
                        EditorGUILayout.LabelField (
                            "Unity" + GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unity,
                            EditorStyles.helpBox,
                            GUILayout.Width (70));

                        EditorGUILayout.Space ();
                        EditorGUILayout.LabelField (
                            "Description",
                            EditorStyles.boldLabel);
                        EditorGUILayout.LabelField (
                            GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].description);

                        int t_NumberOfUnityDependencies = GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unityDependencies.Count;
                        if (t_NumberOfUnityDependencies > 0) {

                            EditorGUILayout.Space ();
                            EditorGUILayout.LabelField (
                                "Unity Dependencies",
                                EditorStyles.boldLabel);

                            EditorGUI.indentLevel += 1;
                            for (int i = 0; i < t_NumberOfUnityDependencies; i++) {
                                EditorGUILayout.LabelField (
                                    "(" + (i + 1) + ") " +
                                    GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unityDependencies[i].name +
                                    " v" +
                                    GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unityDependencies[i].version);
                            }
                            EditorGUI.indentLevel -= 1;
                        }

                        int t_NumberOfInternalDependencies = GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].internalDependencies.Count;
                        if (t_NumberOfInternalDependencies > 0) {

                            EditorGUILayout.Space ();
                            EditorGUILayout.LabelField (
                                "Internal Dependencies",
                                EditorStyles.boldLabel);

                            EditorGUI.indentLevel += 1;
                            for (int i = 0; i < t_NumberOfInternalDependencies; i++) {
                                EditorGUILayout.LabelField (
                                    "(" + (i + 1) + ") " +
                                    GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].internalDependencies[i].name +
                                    " v" +
                                    GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].internalDependencies[i].version);
                            }
                            EditorGUI.indentLevel -= 1;
                        }

                        int t_NumberOfExternalDependencies = GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].externalDependencies.Count;
                        if (t_NumberOfExternalDependencies > 0) {

                            EditorGUILayout.Space ();
                            EditorGUILayout.LabelField (
                                "External Dependencies",
                                EditorStyles.boldLabel);

                            EditorGUI.indentLevel += 1;
                            for (int i = 0; i < t_NumberOfExternalDependencies; i++) {
                                EditorGUILayout.LabelField (
                                    "(" + (i + 1) + ") " +
                                    GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].externalDependencies[i].name +
                                    " v" +
                                    GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].externalDependencies[i].version);
                            }
                            EditorGUI.indentLevel -= 1;
                        }
                    }
                    EditorGUILayout.EndVertical ();
                }
                GUILayout.EndScrollView ();
            }
            GUILayout.EndArea ();
        }
        private void DrawFooter () {

            Vector2 t_PanelOriginOfFooterPanel = new Vector2 (0, Screen.height - 45f);
            Rect t_RectTransformOfFooterPanel = new Rect (
                t_PanelOriginOfFooterPanel.x,
                t_PanelOriginOfFooterPanel.y,
                Screen.width,
                25
            );
            GUI.DrawTexture (t_RectTransformOfFooterPanel, m_BackgroundTextureOfFooter);

            GUILayout.BeginArea (t_RectTransformOfFooterPanel); {

                EditorGUILayout.BeginHorizontal (); {
                    EditorGUILayout.BeginHorizontal (GUILayout.Width (180)); {
                        GUILayout.Label (
                            "Last update " + System.DateTime.Now,
                            GUILayout.Width (150)
                        );

                        GUILayout.Button (
                            m_IconForTickMark,
                            GUILayout.Width (20),
                            GUILayout.Height (20)
                        );
                    }
                    EditorGUILayout.EndHorizontal ();

                    EditorGUILayout.BeginHorizontal (GUILayout.Width (t_RectTransformOfFooterPanel.width - 360f)); {
                        EditorGUILayout.LabelField ("");
                    }
                    EditorGUILayout.EndHorizontal ();

                    EditorGUILayout.BeginHorizontal (GUILayout.Width (170)); {

                        if (m_IsPackageLoaded[m_SelectedPackageIndex]) {
                            if (GUILayout.Button ("Remove")) {
                                List<PackageInfo> t_ListOfPackageInfo = GetPackageURLs (m_SelectedPackageIndex);
                                List<string> t_ListOfPackageName = new List<string> ();
                                int t_NumberOfPackageInfo = t_ListOfPackageInfo.Count;
                                for (int i = 0; i < t_NumberOfPackageInfo; i++) {
                                    t_ListOfPackageName.Add (t_ListOfPackageInfo[i].packageName);
                                }
                                RemoveRepositoriesFromManifestJSON (t_ListOfPackageName);
                            }
                        } else {

                            if (GUILayout.Button ("Import")) {

                                List<PackageInfo> t_ListOfPackageInfo = GetPackageURLs (m_SelectedPackageIndex);
                                int t_NumberOfPackageInfo = t_ListOfPackageInfo.Count;
                                for (int i = 0; i < t_NumberOfPackageInfo; i++) {

                                    if (IsRepositoryInAssetFolder (t_ListOfPackageInfo[i].packageName)) {
                                        t_ListOfPackageInfo.RemoveAt (i);
                                        t_ListOfPackageInfo.TrimExcess ();
                                        t_NumberOfPackageInfo--;
                                        i--;
                                    }
                                }

                                if (t_NumberOfPackageInfo > 0)
                                    AddNewRepositoriesToManifestJSON (t_ListOfPackageInfo);
                            }

                        }

                    }
                    EditorGUILayout.EndHorizontal ();
                }
                EditorGUILayout.EndHorizontal ();

            }
            GUILayout.EndArea ();
        }

        #endregion

        #region Configuretion 

        private Texture2D GetTexture (string t_Filter, string t_SearchInFolder = "") {

            string[] m_GUIDOfIcon;
            if (t_SearchInFolder == "") {
                m_GUIDOfIcon = AssetDatabase.FindAssets (t_Filter);
            } else {
                m_GUIDOfIcon = AssetDatabase.FindAssets (t_Filter, new string[] { t_SearchInFolder });
            }

            if (m_GUIDOfIcon.Length > 0) {

                string t_AssetPath = AssetDatabase.GUIDToAssetPath (m_GUIDOfIcon[0]);
                return (Texture2D) AssetDatabase.LoadAssetAtPath (t_AssetPath, typeof (Texture2D));
            }
            return null;
        }

        private void UpdatePackageLoadedInfo () {

            int t_NumberOfGitInfo = GitRepositoryInfo.gitInfos.Count;
            m_IsPackageLoaded = new bool[t_NumberOfGitInfo];
            for (int i = 0; i < t_NumberOfGitInfo; i++) {

                m_IsPackageLoaded[i] = IsRepositoryInAssetFolder (GitRepositoryInfo.gitInfos[i].name);
            }
        }

        public bool IsRepositoryInAssetFolder (string t_PackageName) {

            string[] t_Directories = Directory.GetDirectories (Application.dataPath, t_PackageName + "*");
            if (t_Directories.Length > 0) {
                return true;
            }
            return false;
        }

        public bool IsRepositoryInPackageFolder (string t_PackageName) {

            string t_ModifiedDataPath = "";
            string[] t_PathSplit = Application.dataPath.Split ('/');
            int t_NumberOfSplitedPath = t_PathSplit.Length;
            for (int i = 0; i < t_NumberOfSplitedPath - 1; i++) {
                t_ModifiedDataPath += t_PathSplit[i] + "/";
            }
            t_ModifiedDataPath += "Library/PackageCache/";
            string[] t_Directories = Directory.GetDirectories (Application.dataPath, t_PackageName + "*");
            if (t_Directories.Length > 0) {
                return true;
            }
            return false;
        }

        public string GetFolderName (string t_FolderName) {

            string t_ModifiedDataPath = Application.dataPath;
            if (!IS_IN_DEVELOPMENT_MODE) {
                string[] t_PathSplit = Application.dataPath.Split ('/');
                int t_NumberOfSplitedPath = t_PathSplit.Length;
                for (int i = 0; i < t_NumberOfSplitedPath - 1; i++) {
                    t_ModifiedDataPath += t_PathSplit[i] + "/";
                }
                t_ModifiedDataPath += "Library/PackageCache/";
            }

            if (Directory.GetDirectories (t_ModifiedDataPath, t_FolderName + "*").Length > 0) {
                return Directory.GetDirectories (t_ModifiedDataPath, t_FolderName + "*") [0];
            }
            return "NoFolderFound";
        }

        private string RemoveUnwantedChar (string t_String) {

            string t_ConcatinatedString = "";
            List<char> t_Converted = t_String.ToList ();
            int t_NumberOfCharacter = t_Converted.Count;
            for (int j = 0; j < t_NumberOfCharacter;) {

                int t_Ascii = System.Convert.ToInt32 (t_Converted[j]);
                if (t_Ascii >= 0 && t_Ascii <= 31 ||
                    t_Converted[j] == ' ' ||
                    t_Converted[j] == '{' ||
                    t_Converted[j] == '}' ||
                    t_Converted[j] == '\t' ||
                    t_Converted[j] == '\n') {

                    t_Converted.RemoveAt (j);
                    t_Converted.TrimExcess ();

                    t_NumberOfCharacter--;
                } else {
                    t_ConcatinatedString += t_Converted[j];
                    j++;
                }
            }

            return t_ConcatinatedString;
        }

        private bool IsValidPackageIndex (int t_PackageIndex) {

            if (t_PackageIndex >= 0 && t_PackageIndex < GitRepositoryInfo.gitInfos.Count)
                return true;

            Debug.LogError ("Invalid package index : " + t_PackageIndex);
            return false;
        }

        private bool IsValidPackageName (string t_PackageName) {

            int t_NumberOfGitReposityory = GitRepositoryInfo.gitInfos.Count;
            for (int i = 0; i < t_NumberOfGitReposityory; i++) {

                if (t_PackageName == GitRepositoryInfo.gitInfos[i].name)
                    return true;
            }

            Debug.LogError ("Invalid package name : " + t_PackageName);
            return false;
        }

        private List<PackageInfo> GetPackageURLs (string t_PackageName) {

            if (IsValidPackageName (t_PackageName)) {
                int t_NumberOfGitReposityory = GitRepositoryInfo.gitInfos.Count;
                for (int i = 0; i < t_NumberOfGitReposityory; i++) {

                    if (t_PackageName == GitRepositoryInfo.gitInfos[i].name)
                        return GetPackageURLs (i);
                }
            }

            return null;
        }

        private List<PackageInfo> GetPackageURLs (int t_PackageIndex) {

            if (IsValidPackageIndex (t_PackageIndex)) {

                List<PackageInfo> t_ListOfPackageInfo = new List<PackageInfo> ();

                int t_NumberOfInternalDependencies = GitRepositoryInfo.gitInfos[t_PackageIndex].internalDependencies.Count;
                for (int i = 0; i < t_NumberOfInternalDependencies; i++) {
                    t_ListOfPackageInfo.Add (new PackageInfo () {
                        packageName = GitRepositoryInfo.gitInfos[t_PackageIndex].internalDependencies[i].name,
                            packageURL = GitRepositoryInfo.gitInfos[t_PackageIndex].internalDependencies[i].url
                    });
                }

                int t_NumberOfExternalDependencies = GitRepositoryInfo.gitInfos[t_PackageIndex].externalDependencies.Count;
                for (int i = 0; i < t_NumberOfExternalDependencies; i++) {
                    t_ListOfPackageInfo.Add (new PackageInfo () {
                        packageName = GitRepositoryInfo.gitInfos[t_PackageIndex].externalDependencies[i].name,
                            packageURL = GitRepositoryInfo.gitInfos[t_PackageIndex].externalDependencies[i].url
                    });
                }

                t_ListOfPackageInfo.Add (new PackageInfo () {
                    packageName = GitRepositoryInfo.gitInfos[t_PackageIndex].name,
                        packageURL = GitRepositoryInfo.gitInfos[t_PackageIndex].repository.url
                });

                return t_ListOfPackageInfo;
            }

            return null;
        }

        #endregion

        #region Configuretion   :   Reading/Writing manifest.json

        private void AddNewRepositoriesToManifestJSON (List<PackageInfo> t_ListOfPackageInfo) {

            string t_StreamingAssetPath = Application.streamingAssetsPath;
            string[] t_Split = t_StreamingAssetPath.Split ('/');
            string t_ManifestPath = "";

            int t_NumberOfSplit = t_Split.Length - 2;
            for (int i = 0; i < t_NumberOfSplit; i++) {

                t_ManifestPath += t_Split[i];
                t_ManifestPath += "/";
            }
            t_ManifestPath += m_NameOfManifestDirectory;
            t_ManifestPath += "/";
            t_ManifestPath += "manifest.json"; //"manifest.json"; 

            //Extracting    :   Package
            string t_Result = System.IO.File.ReadAllText (t_ManifestPath);
            string[] t_SplitByComa = t_Result.Split (',');
            t_NumberOfSplit = t_SplitByComa.Length;
            List<PackageInfo> t_CurrentPackageInfo = new List<PackageInfo> ();

            for (int i = 0; i < t_NumberOfSplit; i++) {

                string t_CleanString = RemoveUnwantedChar (t_SplitByComa[i]);
                string[] t_SplitByColon = t_CleanString.Split (':');
                if (i == 0) {
                    string t_PackageVersion = "";
                    for (int k = 2; k < t_SplitByColon.Length; k++) {
                        t_PackageVersion += ((k > 2 ? ":" : "") + t_SplitByColon[k]);
                    }
                    t_CurrentPackageInfo.Add (new PackageInfo () {
                        packageName = RemoveUnwantedChar (t_SplitByColon[1]),
                            packageURL = RemoveUnwantedChar (t_PackageVersion)
                    });
                } else {
                    string t_PackageVersion = "";
                    for (int k = 1; k < t_SplitByColon.Length; k++) {
                        t_PackageVersion += ((k > 1 ? ":" : "") + t_SplitByColon[k]);
                    }
                    t_CurrentPackageInfo.Add (new PackageInfo () {
                        packageName = RemoveUnwantedChar (t_SplitByColon[0]),
                            packageURL = RemoveUnwantedChar (t_PackageVersion)
                    });
                }
            }

            //WritingPackage
            using (StreamWriter streamWrite = new StreamWriter (t_ManifestPath)) {

                bool t_IsRepositoryAlreadyAdded = false;

                streamWrite.WriteLine ("{");
                streamWrite.WriteLine ("\t\"dependencies\":{");

                int t_NumberOfPackageToBeAdded = t_ListOfPackageInfo.Count;
                int t_NumberOfPackage = t_CurrentPackageInfo.Count;
                for (int i = 0; i < t_NumberOfPackage; i++) {

                    for (int j = 0; j < t_NumberOfPackageToBeAdded; j++) {

                        if (t_CurrentPackageInfo[i].packageName.Contains (t_ListOfPackageInfo[j].packageName)) {

                            t_ListOfPackageInfo.RemoveAt (j);
                            t_NumberOfPackageToBeAdded--;
                            break;
                        }
                    }

                    streamWrite.WriteLine (
                        "\t\t" +
                        t_CurrentPackageInfo[i].packageName +
                        " : " +
                        t_CurrentPackageInfo[i].packageURL +
                        ((i == (t_NumberOfPackage - 1)) ? (t_IsRepositoryAlreadyAdded ? "" : ",") : ","));
                }

                for (int i = 0; i < t_NumberOfPackageToBeAdded; i++) {

                    streamWrite.WriteLine (
                        "\t\t" +
                        "\"" +
                        t_ListOfPackageInfo[i].packageName +
                        "\"" +
                        " : " +
                        "\"git+" +
                        t_ListOfPackageInfo[i].packageURL +
                        "\"" +
                        ((i == (t_NumberOfPackageToBeAdded - 1)) ? "" : ","));
                }

                streamWrite.WriteLine ("\t}");
                streamWrite.WriteLine ("}");
            }
            AssetDatabase.Refresh ();

            UpdatePackageLoadedInfo ();
        }

        private void RemoveRepositoriesFromManifestJSON (List<string> t_ListOfPackageName) {

            string t_StreamingAssetPath = Application.streamingAssetsPath;
            string[] t_Split = t_StreamingAssetPath.Split ('/');
            string t_ManifestPath = "";

            int t_NumberOfSplit = t_Split.Length - 2;
            for (int i = 0; i < t_NumberOfSplit; i++) {

                t_ManifestPath += t_Split[i];
                t_ManifestPath += "/";
            }
            t_ManifestPath += m_NameOfManifestDirectory;
            t_ManifestPath += "/";
            t_ManifestPath += "manifest.json"; //"manifest.json"; 

            string t_Result = System.IO.File.ReadAllText (t_ManifestPath);

            //Extracting    :   Package
            string[] t_SplitByComa = t_Result.Split (',');
            t_NumberOfSplit = t_SplitByComa.Length;
            List<PackageInfo> t_CurrentPackageInfo = new List<PackageInfo> ();

            for (int i = 0; i < t_NumberOfSplit; i++) {

                string t_CleanString = RemoveUnwantedChar (t_SplitByComa[i]);
                string[] t_SplitByColon = t_CleanString.Split (':');
                if (i == 0) {
                    string t_PackageVersion = "";
                    for (int k = 2; k < t_SplitByColon.Length; k++) {
                        t_PackageVersion += ((k > 2 ? ":" : "") + t_SplitByColon[k]);
                    }
                    t_CurrentPackageInfo.Add (new PackageInfo () {
                        packageName = RemoveUnwantedChar (t_SplitByColon[1]),
                            packageURL = RemoveUnwantedChar (t_PackageVersion)
                    });
                } else {
                    string t_PackageVersion = "";
                    for (int k = 1; k < t_SplitByColon.Length; k++) {
                        t_PackageVersion += ((k > 1 ? ":" : "") + t_SplitByColon[k]);
                    }
                    t_CurrentPackageInfo.Add (new PackageInfo () {
                        packageName = RemoveUnwantedChar (t_SplitByColon[0]),
                            packageURL = RemoveUnwantedChar (t_PackageVersion)
                    });
                }

            }

            //WritingPackage
            using (StreamWriter streamWrite = new StreamWriter (t_ManifestPath)) {

                string t_NewManifest = "{\n";
                t_NewManifest += "\t\"dependencies\":{\n";

                int t_NumberOfPackage = t_CurrentPackageInfo.Count;
                int t_NumberOfPackageToBeRemoved = t_ListOfPackageName.Count;
                for (int i = 0; i < t_NumberOfPackage; i++) {

                    bool t_IsThisPackageToBeRemoved = false;
                    for (int j = 0; j < t_NumberOfPackageToBeRemoved; j++) {

                        if (t_CurrentPackageInfo[i].packageName.Contains (t_ListOfPackageName[j])) {

                            t_ListOfPackageName.RemoveAt (j);
                            t_ListOfPackageName.TrimExcess ();
                            t_NumberOfPackageToBeRemoved--;

                            t_IsThisPackageToBeRemoved = true;
                            break;
                        }
                    }

                    if (!t_IsThisPackageToBeRemoved) {

                        t_NewManifest +=
                            "\t\t" +
                            t_CurrentPackageInfo[i].packageName +
                            " : " +
                            t_CurrentPackageInfo[i].packageURL +
                            ",\n";
                    }
                }

                List<char> t_ConvertionFromStringToChar = t_NewManifest.ToList ();
                int t_NumberOfCharacter = t_ConvertionFromStringToChar.Count;
                t_ConvertionFromStringToChar.RemoveAt (t_NumberOfCharacter - 2);
                t_ConvertionFromStringToChar.TrimExcess ();
                t_NewManifest = new string (t_ConvertionFromStringToChar.ToArray ());

                t_NewManifest += "\t}\n}\n";
                streamWrite.Write (t_NewManifest);
            }
            AssetDatabase.Refresh ();
        }

        #endregion

        #region Editor Module

        private void DrawHorizontalLine () {
            EditorGUILayout.LabelField ("", GUI.skin.horizontalSlider);
        }

        private void DrawHorizontalLineOnGUI (Rect rect) {
            EditorGUI.LabelField (rect, "", GUI.skin.horizontalSlider);
        }

        #endregion
    }
}