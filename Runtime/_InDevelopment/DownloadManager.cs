namespace com.faith.packagemanager
{
    using UnityEngine.Events;
    using UnityEngine.Networking;
    using System.Threading.Tasks;


    public static class DownloadManager
    {

        #region Public Callback

        public static async void DownloadJsonFile(string t_URL, UnityAction<string> OnDownloadComplete = null, UnityAction<string> OnDownloadFailed = null)
        {

            using (var t_NewWebRequest = UnityWebRequest.Get(t_URL))
            {

                t_NewWebRequest.SendWebRequest();

                while (!t_NewWebRequest.isDone)
                    await Task.Delay(100);

                if (t_NewWebRequest.isHttpError || t_NewWebRequest.isNetworkError)
                    OnDownloadFailed?.Invoke(t_NewWebRequest.error);
                else
                    OnDownloadComplete?.Invoke(t_NewWebRequest.downloadHandler.text);

            }
        }

        #endregion
    }
}

