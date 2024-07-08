using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Unity.VisualScripting;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Point
{
    public string point_name;
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
    public GameObject playerManager;
    public GameObject userInterfaceManager;
    public GameObject pointManager; // Reference to Point Manager (Will appear as target in the Inspector)
    public GameObject templatePoint; // Reference to a template point that will be used to spawn points (Will appear as target in the Inspector)

    List<Transform> pointTransforms = new List<Transform>(); // List of point transforms
    List<TextMeshPro> textMeshPros = new List<TextMeshPro>(); // List of TextMeshPro components

    // Start is called before the first frame update
    void Start()
    {
        templatePoint.SetActive(false);
        // Add current children points transforms and their textMeshPros to the lists 
        foreach (Transform point in pointManager.transform)
        {
            pointTransforms.Add(point);
            textMeshPros.Add(point.transform.GetChild(0).GetComponent<TextMeshPro>());
        }

        DrawLines();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTextMeshPros();
    }

    public void AddPoint(string name)
    {
        // Get the current camera position
        Vector3 cameraPosition = Camera.main.transform.position;

        // Instantiate a new point at the camera position and the identity quaternion rotation
        GameObject newPoint = Instantiate(templatePoint, cameraPosition, Quaternion.identity, pointManager.transform);
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

        // Add the TextMeshPro component of the new point to the textMeshPros list
        textMeshPros.Add(newPoint.transform.GetChild(0).GetComponent<TextMeshPro>());

        newPoint.SetActive(true);
    }

    public List<Transform> GetPointTransforms()
    {
        return pointTransforms;
    }

    public void SavePoints()
    {
        string pointsListPath = Application.persistentDataPath + "/PointsList.json";
        StreamWriter writer = new StreamWriter(pointsListPath, false);
        writer.WriteLine("");
        writer.Close();
        writer = new StreamWriter(pointsListPath, true);
        writer.WriteLine("{\"points\":[");
        int i = 0;
        foreach (Transform point in pointTransforms)
        {
            string pointEntry =
                      "{" +
                      "\"point_name\":" + "\"" + point.name + "\"" + "," +
                      "\"pos_x\":" + point.transform.localPosition.x + "," +
                      "\"pos_y\":" + point.transform.localPosition.y + "," +
                      "\"pos_z\":" + point.transform.localPosition.z + "," +
                      "\"rot_x\":" + point.transform.localRotation.x + "," +
                     "\"rot_y\":" + point.transform.localRotation.y + "," +
                     "\"rot_z\":" + point.transform.localRotation.z + "," +
                     "\"rot_w\":" + point.transform.localRotation.w +
                     "}";
            writer.WriteLine(pointEntry + (i < pointTransforms.Count - 1 ? "," : ""));
            i++;
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
        for (int i = 0; i < pointTransforms.Count - 1; i++)
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
            // Get point name
            string name = pointTransforms[i].name;

            // Update the text with the positions
            textMeshPros[i].text = $"{name}\nL_X: {xLocalPosition:F2}\nL_Z: {zLocalPosition:F2}";

            // Make the text face the camera (billboard effect)
            Vector3 lookAtPos = new Vector3(Camera.main.transform.position.x, textMeshPros[i].transform.position.y, Camera.main.transform.position.z);
            textMeshPros[i].transform.LookAt(2 * textMeshPros[i].transform.position - lookAtPos);
        }
    }

    public void LoadPoints()
    {
        JSONFile jsonfile = ParseJsonFile();

        foreach (var point in jsonfile.points)
        {
            if (!string.Equals(point.point_name, "OriginPoint"))  // Don't load the origin point again
            {
                // Point position relative to the pointManager/Anchor position
                float newPointPositionX = point.pos_x + pointManager.transform.localPosition.x;
                float newPointPositionY = point.pos_y + pointManager.transform.localPosition.y;
                float newPointPositionZ = point.pos_z + pointManager.transform.localPosition.z;

                Vector3 newPointPosition = new Vector3(newPointPositionX, newPointPositionY, newPointPositionZ);
                Quaternion newPointRotation = new Quaternion(point.rot_x, point.rot_y, point.rot_z, point.rot_w);
                GameObject newPoint = Instantiate(templatePoint, newPointPosition, newPointRotation, pointManager.transform);
                newPoint.name = point.point_name;

                // Add the new point's transform to the pointTransforms list
                pointTransforms.Add(newPoint.transform);

                // Add the TextMeshPro component of the new point to the textMeshPros list
                textMeshPros.Add(newPoint.transform.GetChild(0).GetComponent<TextMeshPro>());

                // Show point
                newPoint.SetActive(true);

                // Hide points to show them later when collision occurs
                // newPoint.GetComponent<Renderer>().enabled = false;
                // newPoint.transform.GetChild(0).GetComponent<TextMeshPro>().GetComponent<Renderer>().enabled = false;
            }
        }

        UserInterfaceManager uiscript = userInterfaceManager.GetComponent<UserInterfaceManager>();
        uiscript.UpdateDropdownOptions();
        PlayerManager playerscript = playerManager.GetComponent<PlayerManager>();
        playerscript.setLoadedPoints();
    }

    // TODO: Figure out how to make one function work in both editor and in game
    // #if UNITY_EDITOR
    //     [MenuItem("Test Menu/Load Points")]
    //     static void LoadPointsMenuItem()
    //     {
    //         LoadPoints();
    //     }
    // #endif
}
