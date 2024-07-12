using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    public GameObject messagePanel;
    public GameObject pointManager;
    public GameObject navigationController;
    bool loadedPoints;
    private List<GameObject> collidedPoints = new List<GameObject>(); // List of point positions
    private GameObject nearestPoint;
    private GameObject destinationPoint;
    private GameObject currentPoint;
    private float minDistance;
    private bool hasFoundFirstPoint = false;
    private GameObject line;
    private float distanceToDestination = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        minDistance = float.PositiveInfinity;
        loadedPoints = false;
        messagePanel.SetActive(false);
        InitializeLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (destinationPoint)
        {
            if (!hasFoundFirstPoint)
            {
                this.GetComponent<SphereCollider>().radius += 0.04f;
            }
        }
    }

    public GameObject GetDestinationPoint() { return destinationPoint; }
    public void SetDestinationPoint(string destinationPointName)
    {
        destinationPoint = GameObject.Find(destinationPointName);
        distanceToDestination = Vector3.Distance(this.transform.position, destinationPoint.transform.position);
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

    public void setLoadedPoints()
    {
        loadedPoints = true;
    }

    // Function called when two colliders initially collide
    private void OnTriggerEnter(Collider other)
    {
        PointManager script = pointManager.GetComponent<PointManager>();

        // if (collision.gameObject.tag == "point")
        // {
        //     collision.gameObject.GetComponent<Renderer>().enabled = true;
        //     collision.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().GetComponent<Renderer>().enabled = true;
        // }
        if (other.gameObject.tag == "line")
        {
            other.gameObject.GetComponent<LineRenderer>().enabled = true;
        }
        if (loadedPoints)
        {
            if (other.gameObject.transform == script.GetPointTransforms().Last())
            {
                if (Vector3.Distance(this.transform.position, other.gameObject.transform.position) <= 0.5f)
                {
                    messagePanel.SetActive(true);
                }
            }
        }
        if (other.gameObject.CompareTag("point"))
        {
            if (destinationPoint)
            {
                if (Vector3.Distance(other.gameObject.transform.position, destinationPoint.transform.position) < distanceToDestination)
                {
                    hasFoundFirstPoint = true;
                    //if (!collidedPoints.Contains(other.gameObject))
                    //{
                    //    collidedPoints.Add(other.gameObject);
                    //}
                }
            }
        }

        //Debug.Log("Collision Enter: " + collision.gameObject.name);
    }

    //// Function called when the colliders stop touching
    //private void OnCollisionExit(Collision collision)
    //{
    //    // if (collision.gameObject.tag == "point")
    //    // {
    //    //     collision.gameObject.GetComponent<Renderer>().enabled = false;
    //    //     collision.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().GetComponent<Renderer>().enabled = false;
    //    // }
    //    if (collision.gameObject.tag == "line")
    //    {
    //        collision.gameObject.GetComponent<LineRenderer>().enabled = false;
    //    }
    //    if (collision.gameObject.CompareTag("point"))
    //    {
    //        //collidedPoints.Remove(collision.gameObject);
    //    }

    //    //Debug.Log("Collision Exit: " + collision.gameObject.name);
    //}

    public void FindFirstPoint()
    {
        if (!hasFoundFirstPoint)
        {
            this.GetComponent<SphereCollider>().radius += 0.1f;
        }
        else if (hasFoundFirstPoint)
        {
            foreach (GameObject point in collidedPoints)
            {
                float cameraToPointDistance = Vector3.Distance(this.transform.position, point.transform.position);
                float pointToDestinationDistance = Vector3.Distance(point.transform.position, destinationPoint.transform.position);
                // Calculate the distance between the camera and the point
                if (cameraToPointDistance + pointToDestinationDistance < minDistance)
                {
                    minDistance = cameraToPointDistance + pointToDestinationDistance;
                    nearestPoint = point;
                }
            }
            if (Vector3.Distance(this.transform.position, nearestPoint.transform.position) <= 0.5f)
            {
                hasFoundFirstPoint = false;
                distanceToDestination = Vector3.Distance(this.transform.position, destinationPoint.transform.position);
            }
            DrawLineFromCameraToPoint(nearestPoint);
        }
    }

    public void InitializeLine()
    {
        line = new GameObject();
        line.name = "line";
        LineRenderer newLineRenderer = line.AddComponent<LineRenderer>();
        line.tag = "line";
        newLineRenderer.startWidth = 0.2f;
        newLineRenderer.endWidth = 0.2f;
    }

    public void DrawLineFromCameraToPoint(GameObject point)
    {
        line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { this.transform.position, point.transform.position });
    }
}