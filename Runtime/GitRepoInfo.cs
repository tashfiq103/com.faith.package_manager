namespace com.faith.packagemanager {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Dependencies {
        public string displayName;
        public string version;
        public string name;
        public string url;
    }

    [System.Serializable]
    public enum TypeOfVersionControl {
        git
    }

    [System.Serializable]
    public class Repository {
        public TypeOfVersionControl type = TypeOfVersionControl.git;
        public string url;
    }

    [System.Serializable]
    public class GitInfo {

        [HideInInspector]
        public bool isNewVersionAvailable = false;  

        public string name;
        public string displayName;
        public string version;
        public string unity;
        public string author;
        public string description;

        public List<Dependencies> unityDependencies;
        public List<Dependencies> internalDependencies;
        public List<Dependencies> externalDependencies;

        public Repository repository;
    }

    [System.Serializable]
    public class RemoteGitInfos {
        public List<GitInfo> gitInfos;
    }

    [CreateAssetMenu (fileName = "GitRepositoryInfo", menuName = "FAITH/GitRepositoryInfo", order = 1)]
    public class GitRepoInfo : ScriptableObject {

        #region Public Variables

#if UNITY_EDITOR

        public bool     showDeveloperPanel;
        public bool     showLocalRepositoryInfo;
        public string   nameOfFile = "remoteGitRepositoryInfos";
        public string   downloadURL = "https://faithstudio.org/jsonTest.json";

#endif

        [Space(5.0f)]
        public List<GitInfo> gitInfos;
        public RemoteGitInfos remoteGitInfos;

        #endregion

#if UNITY_EDITOR

        #region Configuretion

        private void OnJsonDownloadComplete(string t_RawJsonData){

            remoteGitInfos = JsonUtility.FromJson<RemoteGitInfos>(t_RawJsonData);
            
            int t_NumberOfRemoteGitInfos    = remoteGitInfos.gitInfos.Count;
            int t_NumberOfLocalGitInfos     = gitInfos.Count;
            
            for(int i = 0; i  < t_NumberOfRemoteGitInfos; i++){

                bool t_IsFoundAsLocalGitInfo = false;
                
                for(int j = 0 ; j < t_NumberOfLocalGitInfos; j++){

                    if(remoteGitInfos.gitInfos[i].name == gitInfos[j].name){

                        string[] t_VersionOnRemote  = remoteGitInfos.gitInfos[i].version.Split('.');
                        string[] t_VersionOnLocal   = gitInfos[j].version.Split('.');

                        int t_SubDivisionForVersionOnRemote = t_VersionOnRemote.Length;
                        int t_SubDivisionForVersionOnLocal  = t_VersionOnLocal.Length;

                        bool t_IsUpdateAvailable = false;
                        if(t_SubDivisionForVersionOnRemote == t_SubDivisionForVersionOnLocal)
                        {
                            for(int k = 0 ; k < t_SubDivisionForVersionOnRemote; k++){

                                if(Convert.ToInt32(t_VersionOnRemote[k]) > Convert.ToInt32(t_VersionOnLocal[k])){

                                    t_IsUpdateAvailable = true;
                                    break;
                                }
                            }
                        }else{

                            Debug.LogError("Invalid Version Format, Remote : " + remoteGitInfos.gitInfos[i].version + ", Local : " + gitInfos[j].version);
                        }

                        if(t_IsUpdateAvailable){

                            Debug.Log("UpdateAvailable : " + gitInfos[j].name + ", CurrentVersion (" + gitInfos[i].version + ") : NewVersion (" + remoteGitInfos.gitInfos[i].version + ")");
                            gitInfos[j].isNewVersionAvailable = true;
                        }else{

                            gitInfos[j].isNewVersionAvailable = false;
                        }

                        t_IsFoundAsLocalGitInfo = true;
                        break;
                    }
                }

                if(!t_IsFoundAsLocalGitInfo){

                    gitInfos.Add(remoteGitInfos.gitInfos[i]);
                    t_NumberOfLocalGitInfos++;

                    Debug.Log("NewRepositoryAdded : " + remoteGitInfos.gitInfos[i].name);
                    
                }
            }
        }

        #endregion

        #region Public Callback

        public void CheckForUpdate(){

            string t_URL = downloadURL + "/" + nameOfFile;
            DownloadManager.DownloadJsonFile(t_URL, OnJsonDownloadComplete);
        }

        public void SaveRespositoryInfoAsJson()
        {
            string t_JsonString = JsonUtility.ToJson(remoteGitInfos, true);
            System.IO.File.WriteAllText(Application.dataPath + "/" + nameOfFile, t_JsonString);
            UnityEditor.AssetDatabase.Refresh();

            gitInfos = new List<GitInfo>(remoteGitInfos.gitInfos);
        }

        public void UploadToJsonFileToWebURL() {

            string t_JsonString = JsonUtility.ToJson(remoteGitInfos, true);
            UploadManager.UploadJsonFile(t_JsonString, downloadURL, nameOfFile);
        }

        public void MarkRepositoryWithLatestVersion(string t_NameOfRepository) {

            bool t_IsUpdatedVersion = false;
            int t_NumberOfLocalGitInfos = gitInfos.Count;
            int t_NumberOfRemoteGitInfos    = remoteGitInfos.gitInfos.Count;
            for (int i = 0; i < t_NumberOfLocalGitInfos; i++) {

                for (int j = 0; j < t_NumberOfRemoteGitInfos; j++) {

                    if (gitInfos[i].name == remoteGitInfos.gitInfos[j].name) {

                        gitInfos[i].version = remoteGitInfos.gitInfos[j].version;
                        t_IsUpdatedVersion = true;
                        break;
                    }
                }

                if (t_IsUpdatedVersion) break;
            }
        }

        #endregion

#endif
    }
}