using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RestAPI : MonoBehaviour
{
    private string URL = "http://51.210.117.22:8080";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetMap(new Vector2(0, 10), new Vector2(0, 10)));
    }

    IEnumerator GetMap(Vector2 x, Vector2 y)
    {
        string uri = URL + $"/monde/map?x_range={x.x},{x.y}&y_range={y.x},{y.y}";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
        }
    }

}
