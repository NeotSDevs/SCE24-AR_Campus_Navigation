using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    // Message Panel and children
    public GameObject messagePanel;

    // User Panel and children
    public GameObject userPanel;
    public TMP_Dropdown userPanelDropdown;
    string userPanelDropdownSelectedItem;
    public Button userPanelNavigateButton;
    private List<string> dropDownOptions;

    // Admin Panel and children
    public GameObject adminPanel;
    public GameObject addPointNamePanel;
    public TMP_InputField addPointNamePanelInputField;

    // User type selection panel
    public GameObject userTypeSelectionPanel;

    public GameObject pointManager;
    private bool isAnchorStable = false;

    public GameObject playerManager;

    // Start is called before the first frame update
    void Start()
    {
        dropDownOptions = new List<string>();
        addPointNamePanel.SetActive(false);
        userPanel.SetActive(false);
        adminPanel.SetActive(false);
        messagePanel.SetActive(false);
        userTypeSelectionPanel.SetActive(false);

        // Add listener to the dropdown's onValueChanged event
        userPanelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnchorStable)
        {
            isAnchorStable = pointManager.GetComponent<PointManager>().GetIsAnchorStable();
            userTypeSelectionPanel.SetActive(isAnchorStable);
        }
    }

    void OnDestroy()
    {
        // Remove the listener when the script is destroyed to prevent memory leaks
        userPanelDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    // This method will be called when the dropdown value changes
    void OnDropdownValueChanged(int index)
    {
        userPanelDropdownSelectedItem = userPanelDropdown.options[index].text;
        userPanelNavigateButton.interactable = true;
    }

    public void UpdateDropdownOptions()
    {
        dropDownOptions.Clear();
        PointManager script = pointManager.GetComponent<PointManager>();
        foreach (Transform point in script.GetPointTransforms())
        {
            dropDownOptions.Add(point.name);
        }
        userPanelDropdown.ClearOptions();
        userPanelDropdown.AddOptions(dropDownOptions);
    }

    public void ShowAdminPanel()
    {
        if (adminPanel.activeInHierarchy)
        {
            adminPanel.SetActive(false);
        }
        else
        {
            if (userPanel.activeInHierarchy)
            {
                userPanel.SetActive(false);
            }
            adminPanel.SetActive(true);
        }
    }

    public void ShowUserPanel()
    {
        if (userPanel.activeInHierarchy)
        {
            userPanel.SetActive(false);
        }
        else
        {
            if (adminPanel.activeInHierarchy)
            {
                adminPanel.SetActive(false);
            }
            userPanel.SetActive(true);
        }
    }

    public void ShowAddPointNamePanel()
    {
        addPointNamePanel.SetActive(true);
    }

    public void HideAddPointNamePanel()
    {
        addPointNamePanel.SetActive(false);
    }

    public void ConfirmAddPointNamePanel()
    {
        string newPointName = addPointNamePanelInputField.text.ToString();
        PointManager script = pointManager.GetComponent<PointManager>();
        if (!script.PointExists(newPointName))
        {
            script.AddPoint(newPointName);
            addPointNamePanelInputField.text = "";
            addPointNamePanel.SetActive(false);
        }
        else
        {
            Debug.Log("Point named " + newPointName + " already exists.");
        }
    }

    public void OnNavigatePressed()
    {
        PlayerManager script = playerManager.GetComponent<PlayerManager>();
        script.SetDestinationPoint(userPanelDropdownSelectedItem);
    }
}