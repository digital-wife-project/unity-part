using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class AgentController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        agentButton.onClick.AddListener(SendGetRequest);
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Serializable]
    public class Agent
    {
        public string id;
        public string name;
        // 其他你需要的字段...
    }

    [Serializable]
    public class MessageRequest
    {
        public List<Message> messages;
    }

    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class MessageResponse
    {
        public string id;
        public string date;
        public string message_type;
        public string content;
    }

    [Serializable]
    public class PostResponse
    {
        public MessageResponse[] messages;
    }

    public Button agentButton;
    public TMP_Dropdown agentDropdown;
    public TMP_InputField messageInput;
    Dictionary<string, string> agentDictionary = new Dictionary<string, string>();


    void SendGetRequest()
    {
        string url = "http://localhost:8283/v1/agents/";
        UnityWebRequest www = UnityWebRequest.Get(url);
        StartCoroutine(WaitForGetRequest(www));
    }

    IEnumerator WaitForGetRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string response = www.downloadHandler.text;
            ParseResponse(response);
            SendPostRequest();
        }
    }

    // 解析响应
    void ParseResponse(string response)
    {
        Agent[] agents = JsonConvert.DeserializeObject<Agent[]>(response);
        List<string> options = new List<string>();
        foreach (var agent in agents)
        {
            agentDictionary.Add(agent.name, agent.id);
            options.Add(agent.name);
        }
        agentDropdown.ClearOptions();
        agentDropdown.AddOptions(options);
    }
    void SendPostRequest()
    {
        if (agentDropdown.options.Count > 0)
        {
            string selectedName = agentDropdown.options[agentDropdown.value].text;
            string id = agentDictionary[selectedName];
            string url = $"http://localhost:8283/v1/agents/{id}/messages";
            string message = messageInput.text;

            Message msg = new Message();
            msg.role = "user";
            msg.content = message;

            MessageRequest request = new MessageRequest();
            request.messages = new List<Message>();
            request.messages.Add(msg);

            string json = JsonUtility.ToJson(request);
            UnityWebRequest www = UnityWebRequest.PostWwwForm(url, json);
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            StartCoroutine(WaitForPostRequest(www));
        }
        else
        {
            Debug.LogError("No agents available");
        }
    }

    IEnumerator WaitForPostRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string response = www.downloadHandler.text;
            ParsePostResponse(response);
        }
    }

    void ParsePostResponse(string response)
    {
        try
        {
            PostResponse postResponse = JsonUtility.FromJson<PostResponse>(response);
            foreach (var message in postResponse.messages)
            {
                Debug.Log(message.content);
            }
        }
        catch (JsonException e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
        }
    }
}
