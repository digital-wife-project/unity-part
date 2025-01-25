using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Agent操作 : MonoBehaviour
{
    public GameObject Agent_Panel;
    public GameObject agent详细信息panel;

    public string Read_json2string()
    {
        GetData dataManager = FindObjectOfType<GetData>();
        if (dataManager == null)
        {
            Debug.LogError("GetData component not found in the scene.");
            return null;
        }
        return dataManager.LoadJsonFromFile("agent.json");
    }

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public static string GetIdByName(string jsonString, string name)
    {
        // 解析JSON字符串为对象数组
        Item[] items = JsonConvert.DeserializeObject<Item[]>(jsonString);

        // 遍历数组，查找匹配的name
        foreach (var item in items)
        {
            if (item.Name == name)
            {
                // 找到匹配的name，返回对应的id
                return item.Id;
            }
        }
        // 如果没有找到匹配的name，返回null或空字符串
        return null;
    }

    private string Get_agent_id()
    {
        string user_choose_agent_name = AgentButtonCreator.user_chioce;
        string jsonString = Read_json2string();
        string id = GetIdByName(jsonString, user_choose_agent_name);
        return id;
    }

    public void OnDeleteButtonClicked()
    {
        StartCoroutine(DeleteAgentRequest());
    }

    IEnumerator DeleteAgentRequest()
    {
        string agentId = Get_agent_id();
        if (agentId == null)
        {
            Debug.LogError("Agent ID is null. Cannot delete agent.");
            yield break; // 退出协程
        }

        string url = "http://localhost:8283/v1/agents/" + agentId;
        UnityWebRequest uwr = UnityWebRequest.Delete(url);

        // 添加必要的认证头
        uwr.SetRequestHeader("Authorization", "Bearer <your_token_here>");

        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Agent deleted successfully.");
            // 重新获取数据
            GetData dataManager = FindObjectOfType<GetData>();
            if (dataManager != null)
            {
                AgentButtonCreator agentButtonCreatorInstance = new AgentButtonCreator();
                agentButtonCreatorInstance.OnReadJsonButtonClicked();
                Debug.Log("重新生成按钮");
            }
            else
            {
                Debug.LogError("GetData component not found in the scene.");
            }
            // 设置面板状态
            agent详细信息panel.SetActive(false);
            Agent_Panel.SetActive(true);
        }
    }
}
