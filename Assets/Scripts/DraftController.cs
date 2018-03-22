using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class DraftController : MonoBehaviour
{
    public string serverUrl = "localhost:3000/";
    public string cubeJsonFileLocation;
    public int drafterId;
    public int draftId;
    public string draftType;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateDraft("winston", 45);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            JoinDraft(draftId);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartDraft();
        }
    }

    public void CreateDraft(string draftType, int cardsPerPlayer)
    {
        StartCoroutine(SendCreateRequest(draftType, cardsPerPlayer));
    }

    private IEnumerator SendCreateRequest(string draftType, int cardsPerPlayer)
    {
        Draft draft = new Draft();
        draft.cardsPerPlayer = cardsPerPlayer;
        draft.draftType = draftType;
        draft.cube = "[" + GetComponent<CardQuery>().ReadTextFile(cubeJsonFileLocation) + "]";
        string json = JsonUtility.ToJson(draft).Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]").Replace("\\r\\n", "").Replace(",]", "]");

        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl + "draft/create", json))
        {
            UploadHandlerRaw uH = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json)); //I don't know why this is required but it is
            uH.contentType = "application/json";
            request.uploadHandler = uH;

            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.Log("Received: " + request.downloadHandler.text);
                Debug.Log("Error While Sending: " + request.error);
            }
            else
            {
                Debug.Log("Received: " + request.downloadHandler.text);
                draftId = JsonUtility.FromJson<Draft>(request.downloadHandler.text).draftId;
            }
        }
    }

    public void JoinDraft(int draftId)
    {
        this.draftId = draftId;
        StartCoroutine(SendJoinRequest());
    }

    private IEnumerator SendJoinRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl + "draft/" + draftId + "/join", ""))
        {
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.Log("Received: " + request.downloadHandler.text);
                Debug.Log("Error While Sending: " + request.error);
            }
            else
            {
                Debug.Log("Received: " + request.downloadHandler.text);
                Drafter draftInfo = JsonUtility.FromJson<Drafter>(request.downloadHandler.text);
                drafterId = draftInfo.drafterId;
                draftType = draftInfo.draftType;
            }
        }
    }

    public void StartDraft()
    {
        StartCoroutine(SendStartRequest());
    }

    private IEnumerator SendStartRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl + "draft/" + draftId + "/start", ""))
        {
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.Log("Received: " + request.downloadHandler.text);
                Debug.Log("Error While Sending: " + request.error);
            }
            else
            {
                Debug.Log("Received: " + request.downloadHandler.text);
            }
        }
    }
}
