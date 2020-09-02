namespace com.faith.packagemanager
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking;
    using System.Threading.Tasks;

    public static class UploadManager
    {
        #region Public Callback

        public static async void UploadJsonFile(
            string t_Data,
            string t_URL,
            string t_FileName = "uploadedFile",
            UnityAction OnUploadComplete = null,
            UnityAction<string> OnUploadFailed = null)
        {

            string t_AbsoluteURL = t_URL;
            Debug.Log("URL" + t_AbsoluteURL);

            //Method (1)
            //List<IMultipartFormSection> t_DataFormat = new List<IMultipartFormSection>();
            //t_DataFormat.Add(new MultipartFormFileSection(t_Data, t_FileName));

            //Method (2)
            WWWForm t_DataFormat = new WWWForm();
            t_DataFormat.AddField(t_FileName, t_Data);

            using (var t_NewWebRequest = UnityWebRequest.Post(t_AbsoluteURL, t_DataFormat))
            {

                t_NewWebRequest.SendWebRequest();

                while (!t_NewWebRequest.isDone)
                    await Task.Delay(100);

                if (t_NewWebRequest.isHttpError || t_NewWebRequest.isNetworkError)
                {
                    Debug.Log("UploadFailed : " + t_NewWebRequest.error);
                    OnUploadFailed?.Invoke(t_NewWebRequest.error);
                }
                else
                    OnUploadComplete?.Invoke();

            }
        }

        #endregion
    }
}

