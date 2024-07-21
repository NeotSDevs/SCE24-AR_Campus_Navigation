using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour
{
    public GameObject navigatePanel;

    public TMP_Text userGreetingText;

    private string username;

    public void SetUsername(string value)
    {
        username = value;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (userGreetingText != null)
        {
            userGreetingText.text = $"Hello, {username}";
        }
    }

    public void OnClickUserPanelNavigateButton()
    {
        this.gameObject.SetActive(false);
        navigatePanel.SetActive(true);
    }
}
