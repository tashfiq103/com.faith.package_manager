using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using com.faith.packagemanager;


public class JsonConverter : MonoBehaviour
{
    #region Public Variables
    [Header("Reference  :   External")]
    public GitRepoInfo gitRepoInfo;

    [Header("Configuretion")]
    public string nameOfFile = "jsonDataOfGitRepositoryInfo";
    public string downloadLink;

    [Header("Debugging")]
    public JsonWrapperOfGitInfos output;
    #endregion

    #region Configuretion

    private IEnumerator DownloadJsonFileFromURL() {

        UnityWebRequest t_NewWebRequest = new UnityWebRequest(downloadLink);
        yield return t_NewWebRequest;
    }

    #endregion

    #region Pubic Callback

    public void SaveRespositoryInfoAsJson() {

        gitRepoInfo.gitTestInfos.gitInfos = gitRepoInfo.gitInfos;

        string t_JsonString = JsonUtility.ToJson(gitRepoInfo.gitTestInfos, true);
        System.IO.File.WriteAllText(Application.dataPath + "/" + nameOfFile + "(Local).json", t_JsonString);

        output = JsonUtility.FromJson<JsonWrapperOfGitInfos>(t_JsonString);
    }

    public void DownloadRespositoryInfoAsJson()
    {

        gitRepoInfo.gitTestInfos.gitInfos = gitRepoInfo.gitInfos;

        string t_JsonString = JsonUtility.ToJson(gitRepoInfo.gitTestInfos, true);
        System.IO.File.WriteAllText(Application.dataPath + "/" + nameOfFile + "(Local).json", t_JsonString);

        output = JsonUtility.FromJson<JsonWrapperOfGitInfos>(t_JsonString);
    }


    #endregion
}
