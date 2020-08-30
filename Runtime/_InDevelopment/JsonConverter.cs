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
    public RemoteGitInfos output;
    #endregion

    #region Private Variables


    #endregion

    #region Mono Behaviour

    

    #endregion

    #region Configuretion

    private void OnDataRecieved(string t_Data){

        Debug.Log(t_Data);
    }

    private IEnumerator DownloadJsonFileFromURL() {

        UnityWebRequest t_NewWebRequest = UnityWebRequest.Get(downloadLink);
        yield return t_NewWebRequest.SendWebRequest();
        if(t_NewWebRequest.isDone){

            System.IO.File.WriteAllText(Application.dataPath + "/" + nameOfFile + "(Local).json", t_NewWebRequest.downloadHandler.text);
            output = JsonUtility.FromJson<RemoteGitInfos>(t_NewWebRequest.downloadHandler.text);

        }else{


            Debug.Log(t_NewWebRequest.error);
        }
    }

    #endregion

    #region Pubic Callback

    public void SaveRespositoryInfoAsJson() {

        gitRepoInfo.remoteGitInfos.gitInfos = gitRepoInfo.gitInfos;

        string t_JsonString = JsonUtility.ToJson(gitRepoInfo.remoteGitInfos, true);
        System.IO.File.WriteAllText(Application.dataPath + "/" + nameOfFile + "(Local).json", t_JsonString);

        output = JsonUtility.FromJson<RemoteGitInfos>(t_JsonString);
    }

    public void DownloadRespositoryInfoAsJson()
    {
        StartCoroutine(DownloadJsonFileFromURL());
    }


    #endregion
}
