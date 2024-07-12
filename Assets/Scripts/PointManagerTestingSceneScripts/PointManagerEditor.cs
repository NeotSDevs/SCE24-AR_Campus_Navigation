using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointManager))]
public class YourScriptNameEditor : Editor
{
    [MenuItem("Tools/Load Points")]
    public static void LoadPointsMenuItem()
    {
        // Find the GameObject with your script
        PointManager script = FindObjectOfType<PointManager>();

        if (script != null)
        {
            script.LoadPoints();
            Debug.Log("Points loaded successfully!");
        }
        else
        {
            Debug.LogError("Could not find the script in the scene!");
        }
    }
}