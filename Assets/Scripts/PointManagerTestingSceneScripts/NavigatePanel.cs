using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigatePanel : MonoBehaviour
{
    public GameObject userPanel;
    public Button cancelButton;

    // Start is called before the first frame update
    void Start()
    {
        cancelButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressCancelButton()
    {
        this.gameObject.SetActive(false);
        userPanel.SetActive(true);
    }

    public void EnableCancelButton()
    {
        cancelButton.gameObject.SetActive(true);
    }
}
