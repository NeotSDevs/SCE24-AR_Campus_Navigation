using System;
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
    public TMP_Text cameraHeightText;
    // Auth Panel
    public GameObject authPanel;

    // Message Panel and children
    public GameObject messagePanel;

    // Navigation Panel and children
    public GameObject navigationPanel;
    public TMP_Dropdown navigationPanelDropdown;
    string navigationPanelDropdownSelectedItem;
    private List<string> navigationDropDownOptions;
    private List<string> dropDownOptions;
    public Button navigationPanelNavigateButton;

    // Admin Panel and children
    public GameObject adminPanel;

    public GameObject addPointPanel;
    public TMP_InputField addPointPanelInputField;
    public TMP_Dropdown addPointPanelDropdown;
    string addPointPanelDropdownSelectedItem;

    public GameObject removePointPanel;
    public TMP_Dropdown removePointPanelDropdown;
    string removePointPanelDropdownSelectedItem;
    public Button removePointPanelRemoveButton;

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
        authPanel.SetActive(true);
        navigationPanelDropdownSelectedItem = string.Empty;
        addPointPanelDropdownSelectedItem = string.Empty;
        removePointPanelDropdownSelectedItem = string.Empty;
        navigationDropDownOptions = new List<string>();
        dropDownOptions = new List<string>();
        addPointPanel.SetActive(false);
        removePointPanel.SetActive(false);
        navigationPanel.SetActive(false);
        adminPanel.SetActive(false);
        messagePanel.SetActive(false);
        //userTypeSelectionPanel.SetActive(false);
        leftIndicator.gameObject.SetActive(true);
        rightIndicator.gameObject.SetActive(true);
        leftIndicator.enabled = false;
        rightIndicator.enabled = false;

        // Add listener to the navigation panel dropdown's onValueChanged event
        navigationPanelDropdown.onValueChanged.AddListener(OnNavigationPanelDropdownValueChanged);

        // Add listener to the admin add point panel dropdown's onValueChanged event
        addPointPanelDropdown.onValueChanged.AddListener(OnAdminAddPointPanelDropdownValueChanged);

        // Add listener to the admin add point panel dropdown's onValueChanged event
        removePointPanelDropdown.onValueChanged.AddListener(OnAdminRemovePointPanelDropdownValueChanged);

        // Set admin panel dropdown selected item value to Bridge Point
        OnAdminAddPointPanelDropdownValueChanged(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnchorStable)
        {
            isAnchorStable = pointManager.GetComponent<PointManager>().GetIsAnchorStable();
        }

        ShowIndicators();
        float height = (playerManager.transform.position - pointManager.transform.position).y;
        cameraHeightText.text = "height(relative):" + height +
            "\ncurrent level:" + pointManager.GetComponent<PointManager>().GetLevel(height)
            + $"\nplayer position: ({playerManager.transform.position.x:F2},{playerManager.transform.position.y:F2},{playerManager.transform.position.z:F2})"
            + $"\nanchor position:({pointManager.transform.position.x:F2},{pointManager.transform.position.y:F2},{pointManager.transform.position.z:F2})";
    }

    void OnDestroy()
    {
        // Remove the listeners when the script is destroyed to prevent memory leaks
        navigationPanelDropdown.onValueChanged.RemoveListener(OnNavigationPanelDropdownValueChanged);
        addPointPanelDropdown.onValueChanged.RemoveListener(OnAdminAddPointPanelDropdownValueChanged);
        removePointPanelDropdown.onValueChanged.RemoveListener(OnAdminRemovePointPanelDropdownValueChanged);
    }

    // This method will be called when the dropdown value changes
    void OnNavigationPanelDropdownValueChanged(int index)
    {
        navigationPanelDropdownSelectedItem = navigationPanelDropdown.options[index].text;
    }

    void OnAdminAddPointPanelDropdownValueChanged(int index)
    {
        addPointPanelDropdownSelectedItem = addPointPanelDropdown.options[index].text;
    }
    void OnAdminRemovePointPanelDropdownValueChanged(int index)
    {
        removePointPanelDropdownSelectedItem = removePointPanelDropdown.options[index].text;
    }

    public void UpdateDropdownOptions()
    {
        navigationDropDownOptions.Clear();
        dropDownOptions.Clear();
        navigationPanelDropdown.ClearOptions();
        removePointPanelDropdown.ClearOptions();
        PointManager script = pointManager.GetComponent<PointManager>();
        List<Transform> pointTransforms = script.GetPointTransforms();
        List<string> pointTypes = script.GetPointTypes();
        for (int i = 0; i < pointTransforms.Count; i++)
        {
            dropDownOptions.Add(pointTransforms[i].name);
            if (pointTypes[i] == "Destination Point")
            {
                navigationDropDownOptions.Add(pointTransforms[i].name);
            }
        }
        navigationPanelDropdown.AddOptions(navigationDropDownOptions);
        removePointPanelDropdown.AddOptions(dropDownOptions);

        if (navigationPanelDropdown.options.Count > 0 && navigationPanelDropdown.options.Count <= 1)
        {
            navigationPanelDropdownSelectedItem = navigationPanelDropdown.options[0].text;
        }

        if (navigationPanelDropdown.options.Count > 0 && removePointPanelDropdown.options.Count <= 1)
        {
            removePointPanelDropdownSelectedItem = removePointPanelDropdown.options[0].text;
        }

        navigationPanelNavigateButton.interactable = navigationPanelDropdownSelectedItem != null;

        removePointPanelRemoveButton.interactable = removePointPanelDropdownSelectedItem != null;
    }

    public void ShowAdminPanel()
    {
        if (adminPanel.activeInHierarchy)
        {
            adminPanel.SetActive(false);
        }
        else
        {
            if (navigationPanel.activeInHierarchy)
            {
                navigationPanel.SetActive(false);
            }
            adminPanel.SetActive(true);
        }
    }

    public void ShowNavigationPanel()
    {
        if (navigationPanel.activeInHierarchy)
        {
            navigationPanel.SetActive(false);
        }
        else
        {
            navigationPanel.SetActive(true);
        }
    }

    public void ShowAddPointPanel()
    {
        addPointPanel.SetActive(true);
    }

    public void HideAddPointPanel()
    {
        addPointPanel.SetActive(false);
    }

    public void ShowRemovePointPanel()
    {
        removePointPanel.SetActive(true);
    }

    public void HideRemovePointPanel()
    {
        removePointPanel.SetActive(false);
    }

    public void ConfirmAddPointPanel()
    {
        string newPointName = addPointPanelInputField.text.ToString();
        string newPointType = addPointPanelDropdownSelectedItem;
        PointManager script = pointManager.GetComponent<PointManager>();
        if (!script.PointExists(newPointName))
        {
            script.AddPoint(newPointName, newPointType);
            addPointPanelInputField.text = "";
            addPointPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Point named " + newPointName + " already exists.");
        }
    }

    public void OnNavigatePressed()
    {
        PlayerManager script = playerManager.GetComponent<PlayerManager>();
        script.SetDestinationPoint(navigationPanelDropdownSelectedItem);
        navigationPanel.SetActive(false);
    }

    public void OnRemovePointPressed()
    {
        PointManager script = pointManager.GetComponent<PointManager>();
        script.RemovePoint(removePointPanelDropdownSelectedItem);
        removePointPanel.SetActive(false);
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

    public void ToggleMessagePanel()
    {
        if (messagePanel.activeInHierarchy)
        {
            messagePanel.SetActive(false);
        }
        else
        {
            messagePanel.SetActive(true);
        }
    }
}