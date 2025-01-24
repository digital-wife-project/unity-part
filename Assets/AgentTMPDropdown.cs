using UnityEngine;
using UnityEngine.UI;
using TMPro; // 确保包含了TextMeshPro的命名空间
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AgentTMPDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public string authToken = "YOUR_AUTH_TOKEN";

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate {
            OnDropdownValueChanged(dropdown);
        });
        StartCoroutine(GetAgents());
    }

    IEnumerator GetAgents()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:8283/v1/agents/");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            // Assuming the API returns a list of agents in JSON format
            // You will need to adjust the parsing logic based on the actual response format
            var agents = JsonConvert.DeserializeObject<List<Agent>>(json);
            UpdateDropdownOptions(agents);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    void UpdateDropdownOptions(List<Agent> agents)
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (var agent in agents)
        {
            options.Add(new TMP_Dropdown.OptionData(agent.name));
        }
        dropdown.options = options;
    }

    void OnDropdownValueChanged(TMP_Dropdown change)
    {
        // Handle dropdown value changed if needed
    }
}

public class Agent
{
    public string id;
    public string name;
    // Add other properties as needed
}
