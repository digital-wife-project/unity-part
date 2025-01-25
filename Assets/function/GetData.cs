using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class GetData : MonoBehaviour
{
    public GameObject Agent_Panel;
    public GameObject agent详细信息panel;
    public static string user_chioce;

    public class AgentData
    {
        public string id;
        public string name;
    }

    public GameObject buttonPrefab; // TMP按钮预制体，需要在Inspector中赋值
    public Transform panel; // 面板Transform，需要在Inspector中赋值
    public float spacing = 10f; // 按钮之间的间隔

    string Read_json2string()
    {
        GetData dataManager = FindObjectOfType<GetData>();
        if (dataManager == null)
        {
            Debug.LogError("GetData component not found in the scene.");
            return null;
        }
        return dataManager.LoadJsonFromFile("agent.json");
    }

    int CountUniqueIds(string jsonString)
    {
        AgentData[] agents = JsonConvert.DeserializeObject<AgentData[]>(jsonString);
        HashSet<string> uniqueIds = new(agents.Select(a => a.id));
        return uniqueIds.Count;
    }

    // 获取唯一的Agent名称列表
    HashSet<string> CountAgentNames(string jsonString)
    {
        AgentData[] agents = JsonConvert.DeserializeObject<AgentData[]>(jsonString);
        HashSet<string> uniqueNames = new(agents.Select(a => a.name));
        return uniqueNames;
    }

    // 创建按钮的方法
    void CreateButtons(List<string> sortedNames)
    {
        // 获取按钮的宽度
        float buttonWidth = buttonPrefab.GetComponent<RectTransform>().sizeDelta.x;
        // 获取按钮的高度
        float buttonHeight = buttonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        // 计算按钮的总宽度
        float totalWidth = buttonWidth + spacing;
        // 每行按钮数量
        int buttonsPerRow = 4;
        // 计算最大列数
        int maxColumns = Mathf.CeilToInt((float)sortedNames.Count / buttonsPerRow);

        // 获取面板的RectTransform组件
        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();

        // 遍历排序后的名字列表
        for (int i = 0; i < sortedNames.Count; i++)
        {
            // 获取当前名字
            string name = sortedNames[i];

            // 实例化按钮
            GameObject newButton = Instantiate(buttonPrefab, panel);
            // 设置按钮的名称
            newButton.name = "AgentButton" + name;

            // 获取按钮上的文本组件
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            // 如果文本组件存在，设置文本内容
            if (buttonText != null)
            {
                buttonText.text = "Agent " + name;
            }
            // 如果文本组件不存在，输出错误信息
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on button: " + newButton.name);
            }

            // 计算按钮所在的行和列
            int row = i / buttonsPerRow;
            int column = i % buttonsPerRow;
            // 获取按钮的RectTransform组件
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            // 计算按钮的位置，从左上角开始排列
            Vector3 position = new(column * totalWidth,
                                   -row * (buttonHeight + spacing),
                                   0);
            // 设置按钮的位置
            rectTransform.anchoredPosition = position;

            // 尝试获取按钮的Button组件
            if (newButton.TryGetComponent<Button>(out var buttonComponent))
            {
                // 记录按钮的索引
                string buttonIndex = name;
                // 添加按钮点击事件
                buttonComponent.onClick.AddListener(() => ButtonClicked(buttonIndex));
            }
            // 如果Button组件不存在，输出错误信息
            else
            {
                Debug.LogError("Button component not found on button: " + newButton.name);
            }
        }
    }

    void ButtonClicked(string buttonIndex)
    {
        user_chioce = buttonIndex;
        Debug.Log("Button for Agent " + buttonIndex + " clicked!");
        Agent_Panel.SetActive(false);
        agent详细信息panel.SetActive(true);
    }




    //#####################################################################################
    public string url = "http://localhost:8283/v1/agents/";
    public string bearerToken = "YOUR_BEARER_TOKEN"; // 替换为您的Bearer令牌
    public string fileName = "agent.json"; // JSON文件的名称

    void Start()
    {
        StartCoroutine(GetAgentData());
        string jsonData = Read_json2string();
        Debug.Log("获取json");
        if (jsonData != null)
        {
            // 获取唯一ID的数量
            _ = CountUniqueIds(jsonData);
            // 获取唯一的Agent名称列表
            HashSet<string> uniqueNames = CountAgentNames(jsonData);
            // 将HashSet转换为List并排序
            List<string> sortedNames = uniqueNames.ToList();
            sortedNames.Sort();
            // 创建按钮
            Debug.Log("创建按钮");
            CreateButtons(sortedNames);
        }
        else
        {
            Debug.LogError("Failed to read JSON data.");
        }

    }

    public void Getdatabyclick()
    {
        StartCoroutine(GetAgentData());
    }

    public IEnumerator GetAgentData()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = webRequest.downloadHandler.text;
            SaveJsonToFile(jsonResult, fileName);
            Debug.Log(jsonResult);
            Debug.Log("发送get请求");
        }
        else
        {
            Debug.LogError("Error: " + webRequest.error);
        }
    }

    public void SaveJsonToFile(string jsonData, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        using (StreamWriter writer = new(filePath, false))
        {
            writer.WriteLine(jsonData);
        }

        Debug.Log("Data saved to " + filePath);
    }

    // 使用这个方法来读取保存的JSON文件
    public string LoadJsonFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            using StreamReader reader = new(filePath);
            return reader.ReadToEnd();
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }
}
