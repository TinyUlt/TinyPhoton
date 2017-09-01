using UnityEngine;

public class PanelManager : MonoBehaviour {

    public GameObject StartPanel;
    public GameObject[] PanelList;
    
    //Dictionary<string, GameObject> PanelList;

    private GameObject CurrentActivPanel = null;
    private void Awake()
    {
        foreach (GameObject child in PanelList)
        {
            var Ipm = child.GetComponent<IPanelManager>();
            if (Ipm != null)
            {
                Ipm.SetPanelManager(this);
            }
        }
    }
    // Use this for initialization
    void Start () {

        foreach (GameObject child in PanelList)
        {
            child.SetActive(false);
        }
        OpenPanel(StartPanel);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OpenPanel(string name, object message = null)
    {
        if (CurrentActivPanel != null)
        {
            CurrentActivPanel.SetActive(false);
        }

        foreach(GameObject child in PanelList)
        {
            if(child.name == name)
            {
                OpenPanel(child, message);
                return;
            }
        }

        Debug.LogError("can not find " + name);
    }
    public void OpenPanel(GameObject obj, object message = null)
    {
        if(obj == null)
        {
            return;
        }
        CurrentActivPanel = obj;
        CurrentActivPanel.SetActive(true);
        var Ipm = obj.GetComponent<IPanelManager>();
        Ipm.OnPanelOpenMessage(message);
    }
}
