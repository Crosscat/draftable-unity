using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class CardQuery : MonoBehaviour
{
    public string inFilePath;
    public string outFilePath;

    public void Start()
    {
        //ConvertTextToJSON();
    }

    public void ConvertTextToJSON()
    {
        string data = ReadTextFile(inFilePath);

        FileUtil.DeleteFileOrDirectory(outFilePath);
        string[] names = data.Split('\n');
        foreach (string name in names)
        {
            StartCoroutine(ScryfallQuery(name));
        }
    }

    private IEnumerator ScryfallQuery(string cardName)
    {
        WWW request = new WWW("https://api.scryfall.com/cards/named?exact=" + cardName);
        yield return request;

        if (request.error != null)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.text);
            Card card = JsonUtility.FromJson<Card>(request.text);
            WriteJSONToFile(outFilePath, JsonUtility.ToJson(card) + ",");
        }
    }

    public string ReadTextFile(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);
        string data = reader.ReadToEnd();
        reader.Close();
        return data;
    }

    public void WriteJSONToFile(string filePath, string data)
    {
        StreamWriter writer = new StreamWriter(filePath, true);
        writer.WriteLine(data);
        writer.Close();
    }
}
