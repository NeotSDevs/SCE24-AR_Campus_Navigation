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
    private GameObject destinationPoint;
    private bool hasFoundFirstPoint = false;
    private GameObject guideLine;
    private float distanceToDestination = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        messagePanel.SetActive(false);
        InitializeGuideLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (destinationPoint)
        {
            if (!hasFoundFirstPoint)
            {
                this.GetComponent<SphereCollider>().radius += 1.0f;
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

    public GameObject GetDestinationPoint() { return destinationPoint; }
    public void SetDestinationPoint(string destinationPointName)
    {
        destinationPoint = GameObject.Find(destinationPointName);
        distanceToDestination = Vector3.Distance(this.transform.position, destinationPoint.transform.position);
    }
    public void ResetDestinationPoint()
    {
        destinationPoint = null;
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
            if (destinationPoint)
            {
                if (Vector3.Distance(other.gameObject.transform.position, destinationPoint.transform.position) < distanceToDestination)
                {
                    hasFoundFirstPoint = true;
                }
            }
            else
            {
                if (collidedPoints.Contains(other.gameObject))
                {
                    collidedPoints.Remove(other.gameObject);
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
    }

    public void DrawLineFromCameraToPoint(GameObject point)
    {
        Vector3 cameraPosition = this.transform.position;
        cameraPosition.y -= 0.5f;
        guideLine.GetComponent<LineRenderer>().SetPositions(new Vector3[] { cameraPosition, point.transform.position });
    }
}