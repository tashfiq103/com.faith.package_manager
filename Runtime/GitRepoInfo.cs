namespace com.faith.packagemanager {
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
    public class GitTestInfo {
        public List<GitInfo> gitInfos;
    }

    [CreateAssetMenu (fileName = "GitRepositoryInfo", menuName = "FAITH/GitRepositoryInfo", order = 1)]
    public class GitRepoInfo : ScriptableObject {
        
        public List<GitInfo> gitInfos;
        [SerializeField]
        public GitTestInfo gitTestInfos;
    }
}