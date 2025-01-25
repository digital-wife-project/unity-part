using UnityEngine;

public class Swich_panel : MonoBehaviour
{
    private void Start()
    {
        Agent_Panel.SetActive(false); 
        Creat_agent_Panel.SetActive(false);
        agent详细信息panel.SetActive(false);
    }
    public GameObject Agent_Panel;
    public GameObject Creat_agent_Panel;
    public GameObject agent详细信息panel;
    public void Agent_buttom()
    {
        Agent_Panel.SetActive(!Agent_Panel.activeSelf);
    }
    public void Creat_agent_buttom()
    {
        Creat_agent_Panel.SetActive(!Creat_agent_Panel.activeSelf);
    }
}
