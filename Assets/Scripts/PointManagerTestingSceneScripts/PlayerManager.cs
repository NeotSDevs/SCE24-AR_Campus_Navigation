using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    public GameObject messagePanel;
    public GameObject pointManager;
    public GameObject navigationController;
    public Material guideLineMaterial;

    private List<GameObject> collidedPoints = new List<GameObject>(); // List of point positions
    private GameObject actualDestinationPoint;
    private GameObject selectedDestinationPoint;
    private GameObject prevDestinationPoint;
    private bool hasFoundFirstPoint = false;
    private GameObject guideLine;
    private float distanceToDestination = 0.0f;
    private bool isLookingForElevator = false;


    // Start is called before the first frame update
    void Start()
    {
        InitializeGuideLine();
        messagePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedDestinationPoint)
        {
            if (!hasFoundFirstPoint)
            {
                this.GetComponent<SphereCollider>().radius += 0.2f;
            }
        }
        else
        {
            if (collidedPoints.Count > 0)
            {
                DrawLineFromCameraToPoint(collidedPoints[0]);
            }
        }
    }

    public GameObject GetDestinationPoint() { return selectedDestinationPoint; }
    public void SetDestinationPoint(string destinationPointName)
    {
        collidedPoints.Clear();
        PointManager script = pointManager.GetComponent<PointManager>();
        float selectedDestinationHeight = script.GetLevel(GameObject.Find(destinationPointName));
        float cameraHeight = script.GetLevel(this.gameObject);

        if(selectedDestinationHeight == cameraHeight) //on same level
        {
            selectedDestinationPoint = GameObject.Find(destinationPointName);
            distanceToDestination = Vector3.Distance(this.transform.position, selectedDestinationPoint.transform.position);
        }
        else
        {
            isLookingForElevator = true;
            float minDistance = float.PositiveInfinity;
            int minPointIndex = -1;
            for(int i = 0; i < script.GetPointTransforms().Count; i++) // look for elevator or stairs point
            {
                if(script.GetPointTypes()[i] == "Elevator Point" || script.GetPointTypes()[i] == "Stairs Point")
                {
                    float distanceToPoint = Vector3.Distance(this.transform.position, script.GetPointTransforms()[i].position);
                    if (distanceToPoint < minDistance)
                    {
                        minDistance = distanceToPoint;
                        minPointIndex = i;
                    }
                }
            }
            if(minPointIndex != -1)
            {
                selectedDestinationPoint = script.GetPointTransforms()[minPointIndex].gameObject; // set destination point to elevator point
                actualDestinationPoint = GameObject.Find(destinationPointName); // save actual destination point
            }
            else
            {
                Debug.Log("Can't find Elevator Point");
            }
        }

    }
    public void ResetDestinationPoint()
    {
        guideLine.SetActive(true);
        this.GetComponent<SphereCollider>().radius = 1.0f;
        prevDestinationPoint = selectedDestinationPoint;
        selectedDestinationPoint = null;
        hasFoundFirstPoint = false;
    }

    public void AddCollidedPoint(GameObject point) { collidedPoints.Add(point); }
    public List<GameObject> GetCollidedPoints()
    {
        return collidedPoints;
    }

    public float GetDistanceToDestination()
    {
        return distanceToDestination;
    }

    public GameObject GetGuideLine() { return guideLine; }

    // Function called when two colliders initially collide
    private void OnTriggerEnter(Collider other)
    {
        PointManager script = pointManager.GetComponent<PointManager>();

        if (other.gameObject.tag == "line")
        {
            other.gameObject.GetComponent<LineRenderer>().enabled = true;
        }
        if (other.gameObject.CompareTag("point"))
        {
            if (selectedDestinationPoint)
            {
                if (Vector3.Distance(other.gameObject.transform.position, selectedDestinationPoint.transform.position) < distanceToDestination)
                {
                    hasFoundFirstPoint = true;
                }
            }
            else
            {
                if (collidedPoints.Contains(other.gameObject))
                {
                    collidedPoints.Remove(other.gameObject);
                    if(other.gameObject == prevDestinationPoint)
                    {
                        guideLine.SetActive(false);

                    }
                }
            }
            if (isLookingForElevator)
            {
                float selectedDestinationHeight = script.GetLevel(actualDestinationPoint);
                float cameraHeight = script.GetLevel(this.gameObject);
                if(selectedDestinationHeight == cameraHeight)
                {
                    SetDestinationPoint(actualDestinationPoint.name);
                    isLookingForElevator = false;
                    actualDestinationPoint = null;
                }

            }
        }
    }

    public void InitializeGuideLine()
    {
        guideLine = new GameObject();
        guideLine.name = "line";
        LineRenderer newLineRenderer = guideLine.AddComponent<LineRenderer>();
        guideLine.tag = "line";
        newLineRenderer.startWidth = 0.3f;
        newLineRenderer.endWidth = 0.3f;
        newLineRenderer.material = guideLineMaterial;
        newLineRenderer.textureMode = LineTextureMode.Tile;
        guideLine.SetActive(false);
    }

    public void DrawLineFromCameraToPoint(GameObject point)
    {
        Vector3 cameraPosition = this.transform.position;
        cameraPosition.y -= 0.5f;
        Vector3 pointPosition = point.transform.position;
        pointPosition.y -= 0.5f;
        guideLine.GetComponent<LineRenderer>().SetPositions(new Vector3[] { cameraPosition, pointPosition });
    }
}