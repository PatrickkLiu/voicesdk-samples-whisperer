using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using TMPro; 

public class GoogleSheetsUpdater : MonoBehaviour
{
    public GameObject transcriptObject;

    private string transcriptText;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeNiX3bX_bRcfMGzQ3zSiCsrGYtjg5VjZifIqkJaJB3MNjl2A/formResponse";
    
    public void Send() {
        transcriptText = transcriptObject.GetComponent<TextMeshPro>().text;
        StartCoroutine(Post(transcriptText));

    }
    IEnumerator Post(string name) {
        WWWForm form = new WWWForm();
        form.AddField("entry.788289799", name);

        /*
        ** Outdated
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return www;
        */
        UnityWebRequest www = UnityWebRequest.Post(BASE_URL, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form uploaded"+name);
        }
    }
}
