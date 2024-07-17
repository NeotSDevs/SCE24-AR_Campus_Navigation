using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    private List<string> dropDownOptions;
    public Button userPanelNavigateButton;

    // Admin Panel and children
    public GameObject adminPanel;
    public GameObject addPointNamePanel;
    public TMP_InputField addPointNamePanelInputField;
    public TMP_Dropdown addPointPanelDropdown;
    string addPointPanelDropdownSelectedItem;

    // User type selection panel
    public GameObject userTypeSelectionPanel;

    public GameObject pointManager;
    private bool isAnchorStable = false;

    public GameObject playerManager;

    public Image leftIndicator;
    public Image rightIndicator;
    public float indicatorThreshold = 10f; // Degrees

    // Start is called before the first frame update
    void Start()
    {
        userPanelDropdownSelectedItem = string.Empty;
        dropDownOptions = new List<string>();
        addPointNamePanel.SetActive(false);
        userPanel.SetActive(false);
        adminPanel.SetActive(false);
        messagePanel.SetActive(false);
        userTypeSelectionPanel.SetActive(false);
        leftIndicator.enabled = false;
        rightIndicator.enabled = false;

        // Add listener to the user panel dropdown's onValueChanged event
        userPanelDropdown.onValueChanged.AddListener(OnUserPanelDropdownValueChanged);

        // Add listener to the admin add point panel dropdown's onValueChanged event
        addPointPanelDropdown.onValueChanged.AddListener(OnAdminAddPointPanelDropdownValueChanged);
        OnAdminAddPointPanelDropdownValueChanged(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnchorStable)
        {
            // isAnchorStable = pointManager.GetComponent<PointManager>().GetIsAnchorStable();
            // userTypeSelectionPanel.SetActive(isAnchorStable);
        }

        userPanelNavigateButton.interactable = userPanelDropdownSelectedItem.Length > 0;

        ShowIndicators();
    }

    void OnDestroy()
    {
        // Remove the listener when the script is destroyed to prevent memory leaks
        userPanelDropdown.onValueChanged.RemoveListener(OnUserPanelDropdownValueChanged);
        // Remove the listener when the script is destroyed to prevent memory leaks
        addPointPanelDropdown.onValueChanged.RemoveListener(OnAdminAddPointPanelDropdownValueChanged);
    }

    // This method will be called when the dropdown value changes
    void OnUserPanelDropdownValueChanged(int index)
    {
        userPanelDropdownSelectedItem = userPanelDropdown.options[index].text;
    }

    void OnAdminAddPointPanelDropdownValueChanged(int index)
    {
        addPointPanelDropdownSelectedItem = addPointPanelDropdown.options[index].text;
    }

    public void UpdateDropdownOptions()
    {
        dropDownOptions.Clear();
        PointManager script = pointManager.GetComponent<PointManager>();
        List<Transform> pointTransforms = script.GetPointTransforms();
        List<string> pointTypes = script.GetPointTypes();
        for (int i = 0;i< pointTransforms.Count;i++)
        {
            if(pointTypes[i]=="Destination Point")
            {
                dropDownOptions.Add(pointTransforms[i].name);
            }
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
        string newPointType = addPointPanelDropdownSelectedItem;
        PointManager script = pointManager.GetComponent<PointManager>();
        if (!script.PointExists(newPointName))
        {
            script.AddPoint(newPointName,newPointType);
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

    private void ShowIndicators()
    {
        GameObject guideLineObj = playerManager.GetComponent<PlayerManager>().GetGuideLine();

        if (guideLineObj == null) return;

        LineRenderer guideLineRenderer = guideLineObj.GetComponent<LineRenderer>();

        if (guideLineRenderer == null || guideLineRenderer.positionCount < 2) return;

        Vector3 lineEnd = guideLineRenderer.GetPosition(1);

        // Check if lineEnd is within the camera's view frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerManager.GetComponent<Camera>());
        bool isVisible = GeometryUtility.TestPlanesAABB(planes, new Bounds(lineEnd, Vector3.one));

        // Calculate the relative position of the lineEnd to the camera
        Vector3 relativePos = playerManager.GetComponent<Camera>().transform.InverseTransformPoint(lineEnd);

        // Enable indicators based on the visibility and relative position
        if (!isVisible && guideLineObj.activeSelf)
        {
            leftIndicator.enabled = relativePos.x < 0;
            rightIndicator.enabled = relativePos.x > 0;
        }
        else
        {
            leftIndicator.enabled = false;
            rightIndicator.enabled = false;
        }
    }
}