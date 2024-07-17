using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Unity.VisualScripting;
using System;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using Google.XR.ARCoreExtensions;



#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Point
{
    public string point_name;
    public string point_type;
    public float pos_x;
    public float pos_y;
    public float pos_z;
    public float rot_x;
    public float rot_y;
    public float rot_z;
    public float rot_w;
}

[Serializable]
public class JSONFile
{
    public Point[] points;
}

public class PointManager : MonoBehaviour
{
    //public GameObject debugtext;
    //private TextMeshPro debugtextTMP;

    public GameObject userInterfaceManager;
    public GameObject pointManager; // Reference to Point Manager (Will appear as target in the Inspector)
    public GameObject templatePoint; // Reference to a template point that will be used to spawn points (Will appear as target in the Inspector)

    List<Transform> pointTransforms = new List<Transform>(); // List of point transforms
    List<string> pointTypes = new List<string>();
    List<TextMeshPro> textMeshPros = new List<TextMeshPro>(); // List of TextMeshPro components

    public float movementThreshold = 0.01f; // Adjust this value to set sensitivity
    public float timeThreshold = 5.0f; // Time the movement should be below threshold
    private float minimalMovementTime = 0f;
    private bool isAnchorStable = false;
    private Vector3 lastPosition;

    private GameObject newPointsOrigin;

    // Start is called before the first frame update
    void Start()
    {
        //debugtextTMP = debugtext.GetComponent<TextMeshPro>();
        lastPosition = this.transform.position;
        templatePoint.SetActive(false);


        //DrawLines();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointTransforms.Count > 0)
        {
            UpdateTextMeshPros();
        }

        if (!isAnchorStable)
        {
            CheckAnchorStability();
        }

        //UpdateDebugText();

    }

    public bool GetIsAnchorStable() { return isAnchorStable; }

    public void AddPoint(string name, string type)
    {
        
        // Get the current camera position
        Vector3 cameraPosition = Camera.main.transform.position;

        if (!GameObject.Find("pointsorigin"))
        {
            newPointsOrigin = new GameObject("pointsorigin");
            newPointsOrigin.transform.position = this.transform.position;
            newPointsOrigin.transform.rotation = this.transform.rotation;
        }

        // Instantiate a new point at the camera position
        GameObject newPoint = Instantiate(templatePoint, cameraPosition, Quaternion.identity, newPointsOrigin.transform);

        if (string.IsNullOrEmpty(name))
        {
            newPoint.name = "point" + UnityEngine.Random.ColorHSV().ToHexString();
            while (PointExists(newPoint.name))
            {
                newPoint.name = "point" + UnityEngine.Random.ColorHSV().ToHexString();
            }
        }
        else
        {
            newPoint.name = name;
        }

        // Add the new point's transform to the pointTransforms list
        pointTransforms.Add(newPoint.transform);

        //Add the new point's type to the pointTypes list
        pointTypes.Add(type);

        // Add the TextMeshPro component of the new point to the textMeshPros list
        textMeshPros.Add(newPoint.transform.GetChild(0).GetComponent<TextMeshPro>());

        newPoint.SetActive(true);

        UserInterfaceManager uiscript = userInterfaceManager.GetComponent<UserInterfaceManager>();
        uiscript.UpdateDropdownOptions();
    }

    public List<Transform> GetPointTransforms()
    {
        return pointTransforms;
    }

    public List<string> GetPointTypes()
    {
        return pointTypes;
    }

    public void SavePoints()
    {
        string pointsListPath = Application.persistentDataPath + "/PointsList.json";
        StreamWriter writer = new StreamWriter(pointsListPath, false);
        writer.WriteLine("");
        writer.Close();
        writer = new StreamWriter(pointsListPath, true);
        writer.WriteLine("{\"points\":[");
        for(int i = 0; i < pointTransforms.Count; i++)
        {
            string pointEntry =
                      "{" +
                      "\"point_name\":" + "\"" + pointTransforms[i].name + "\"" + "," +
                      "\"point_type\":" + "\"" + pointTypes[i] + "\"" + "," +
                      "\"pos_x\":" + pointTransforms[i].transform.localPosition.x + "," +
                      "\"pos_y\":" + pointTransforms[i].transform.localPosition.y + "," +
                      "\"pos_z\":" + pointTransforms[i].transform.localPosition.z + "," +
                      "\"rot_x\":" + 0 + "," +
                     "\"rot_y\":" + 0 + "," +
                     "\"rot_z\":" + 0 + "," +
                     "\"rot_w\":" + 1 +
                     "}";
            writer.WriteLine(pointEntry + (i < pointTransforms.Count - 1 ? "," : ""));
        }
        writer.WriteLine("]}");
        writer.Close();
    }

    public bool PointExists(string name)
    {
        //// Checking point exists in the file
        // string pointsListPath = Application.persistentDataPath + "/PointsList.json";
        // if (File.Exists(pointsListPath))
        // {
        //     JSONFile jsonfile = ParseJsonFile();
        //     foreach (var point in jsonfile.points)
        //     {
        //         if (point.point_name == name)
        //         {
        //             return true;
        //         }
        //     }
        //     return false;
        // }
        // else
        // {
        //     return false;
        // }
        foreach (Transform point in pointTransforms)
        {
            if (string.Equals(name, point.name)) return true;
        }
        return false;
    }

    public static JSONFile ParseJsonFile()
    {
        string pointsListPath = Application.persistentDataPath + "/PointsList.json";
        StreamReader reader = new StreamReader(pointsListPath);
        string data = reader.ReadToEnd();
        reader.Close();
        return JsonUtility.FromJson<JSONFile>(data);
    }

    public void DrawLines()
    {
        for (int i = 1; i < pointTransforms.Count - 1; i++)
        {
            GameObject newObj = new GameObject();
            LineRenderer newLineRenderer = newObj.AddComponent<LineRenderer>();
            newObj.tag = "line";
            newObj.name = "lineFrom|" + pointTransforms[i].name + "|To|" + pointTransforms[i + 1].name;
            newLineRenderer.startWidth = 0.2f;
            newLineRenderer.endWidth = 0.2f;
            newLineRenderer.SetPositions(new Vector3[] { pointTransforms[i].position, pointTransforms[i + 1].position });
            BoxCollider newBoxCollider = newObj.AddComponent<BoxCollider>();
            newBoxCollider.includeLayers |= 1 << LayerMask.NameToLayer("Default");
            // Hide lines to show them later when collision occurs
            newLineRenderer.enabled = false;
        }
    }

    public void UpdateTextMeshPros()
    {
        // Update the textMeshPro of each point with its current position
        for (int i = 0; i < pointTransforms.Count; i++)
        {
            // Get local X and Z positions of a point (relative to origin point/anchor)
            float xLocalPosition = pointTransforms[i].localPosition.x;
            float zLocalPosition = pointTransforms[i].localPosition.z;
            float xLocalRotation = pointTransforms[i].localRotation.x;
            float yLocalPosition = pointTransforms[i].localRotation.y;
            float zLocalRotation = pointTransforms[i].localRotation.z;
            float wLocalPosition = pointTransforms[i].localRotation.w;

            // Get point name
            string name = pointTransforms[i].name;

            // Update the text with the positions
            textMeshPros[i].text = $"{name}" +
                $"\nL_X: {xLocalPosition:F2}" +
                $"\n L_Z: {zLocalPosition:F2}";

            // Make the text face the camera (billboard effect)
            Vector3 lookAtPos = new Vector3(Camera.main.transform.position.x, textMeshPros[i].transform.position.y, Camera.main.transform.position.z);
            textMeshPros[i].transform.LookAt(2 * textMeshPros[i].transform.position - lookAtPos);
        }
    }

    public void LoadPoints()
    {
        JSONFile jsonfile = ParseJsonFile();

        GameObject oldPointsOrigin = GameObject.Find("pointsorigin");
        if (oldPointsOrigin != null)
        {
            Destroy(oldPointsOrigin);
            pointTransforms.Clear();
            pointTypes.Clear();
            textMeshPros.Clear();
        }

        newPointsOrigin = new GameObject("pointsorigin");
        newPointsOrigin.transform.position = this.transform.position;
        newPointsOrigin.transform.rotation = this.transform.rotation;

        foreach (var point in jsonfile.points)
        {
            // Create new point object
            GameObject newPoint = Instantiate(templatePoint, newPointsOrigin.transform, false);

            // Set new point's position relative to pointmanager/anchor (from saved position)
            newPoint.transform.localPosition = new Vector3(point.pos_x, point.pos_y, point.pos_z);

            // Set new point's name (from save file)
            newPoint.name = point.point_name;

            // Add the new point's transform to the pointTransforms list
            pointTransforms.Add(newPoint.transform);

            //Add the new point's type to the pointTypes list
            pointTypes.Add(point.point_type);

            // Add the TextMeshPro component of the new point to the textMeshPros list
            textMeshPros.Add(newPoint.transform.GetChild(0).GetComponent<TextMeshPro>());

            // Show point
            newPoint.SetActive(true);

            // Hide points to show them later when collision occurs
            // newPoint.GetComponent<Renderer>().enabled = false;
            // newPoint.transform.GetChild(0).GetComponent<TextMeshPro>().GetComponent<Renderer>().enabled = false;
        }

        UserInterfaceManager uiscript = userInterfaceManager.GetComponent<UserInterfaceManager>();
        uiscript.UpdateDropdownOptions();
    }

    public void CheckAnchorStability()
    {
        Vector3 currentPosition = this.transform.position;
        float movement = Vector3.Distance(currentPosition, lastPosition);

        if (movement < movementThreshold)
        {
            minimalMovementTime += Time.deltaTime;
            if (minimalMovementTime >= timeThreshold && !isAnchorStable)
            {
                isAnchorStable = true;
            }
        }
        else
        {
            minimalMovementTime = 0f;
        }

        lastPosition = currentPosition;
    }

    //private void UpdateDebugText()
    //{
    //    Vector3 pointManagerPosition = this.transform.position;
    //    Quaternion pointManagerRotation = this.transform.rotation;

    //    string debugText = $"Anchor Position: \n" +
    //                       $"x: {pointManagerPosition.x}, y: {pointManagerPosition.y}, z: {pointManagerPosition.z}\n" +
    //                       $"Anchor Rotation: \n" +
    //                       $"x: {pointManagerRotation.x}, y: {pointManagerRotation.y}, z: {pointManagerRotation.z}, w: {pointManagerRotation.w}\n";

    //    GameObject pointsOriginObject = GameObject.Find("pointsorigin");
    //    if (pointsOriginObject)
    //    {
    //        Vector3 pointsOriginPosition = pointsOriginObject.transform.position;
    //        Quaternion pointsOriginRotation = pointsOriginObject.transform.rotation;

    //        debugText += $"Points Origin Position: \n" +
    //                     $"x: {pointsOriginPosition.x}, y: {pointsOriginPosition.y}, z: {pointsOriginPosition.z}\n" +
    //                     $"Points Origin Rotation: \n" +
    //                     $"x: {pointsOriginRotation.x}, y: {pointsOriginRotation.y}, z: {pointsOriginRotation.z}, w: {pointsOriginRotation.w}";
    //    }

    //    debugtextTMP.text = debugText;
    //}
}
