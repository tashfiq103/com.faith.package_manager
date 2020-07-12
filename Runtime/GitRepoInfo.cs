namespace com.faith.package_manager {
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Dependencies {
        public string displayName;
        public string version;
        public string name;
        public string url;
    }

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

        [Space (5.0f)]
        public List<Dependencies> unityDependencies;
        public List<Dependencies> internalDependencies;
        public List<Dependencies> externalDependencies;

        [Space (5.0f)]
        public Repository repository;
    }

    [CreateAssetMenu (fileName = "GitRepositoryInfo", menuName = "FAITH/GitRepositoryInfo", order = 1)]
    public class GitRepoInfo : ScriptableObject {
        public List<GitInfo> gitInfos;
    }
}