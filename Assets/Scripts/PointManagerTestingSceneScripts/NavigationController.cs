using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public GameObject playerManager;
    private bool foundNextPoint = false;

    void Update()
    {
        if (foundNextPoint)
        {
            this.gameObject.GetComponent<SphereCollider>().radius += 1.0f;
        }
    }

    public void SetFoundNextPoint(bool foundNextPoint)
    {
        this.foundNextPoint = foundNextPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        // this הנפגע
        // other הפוגע
        // בודקים אם הנפגע הוא מסוג נקודה
        //Debug.Log($"OnTrigger this:{this.gameObject.name} other:{other.gameObject.name}");
        if (this.gameObject.CompareTag("point"))
        {
            //Debug.Log($"{this.gameObject.name} is point tagged");
            // להשיג את הסקריפט
            PlayerManager script = playerManager.GetComponent<PlayerManager>();
            // להשיג את הנקודת יעד
            GameObject destinationPoint = script.GetDestinationPoint();
            List<GameObject> collidedPoints = script.GetCollidedPoints();

            // בודקים האם הרשימה לא מכילה את הנפגע
            if (!collidedPoints.Contains(this.gameObject))
            {

                // האם הנפגע הוא לא נקודת היעד
                if (this.gameObject != destinationPoint)
                {
                    
                    //Debug.Log($"{this.gameObject.name} not the destination");
                    //חישוב מרחק מהנפע אל היעד
                    float distanceFromThisToDest = Vector3.Distance(this.gameObject.transform.position, destinationPoint.transform.position);
                    // חישוב מרחק מהפוגע ליעד
                    float distanceFromOtherToDest = Vector3.Distance(other.gameObject.transform.position, destinationPoint.transform.position);
                    //Debug.Log($"distance from this to dest {distanceFromThisToDest}");
                    //Debug.Log($"distance from other to dest {distanceFromOtherToDest}");
                    Debug.Log($"this {this.gameObject.name} distance to dest = {distanceFromThisToDest} | other {other.gameObject.name} distance to dest = {distanceFromOtherToDest} | this<other={distanceFromThisToDest < distanceFromOtherToDest}");
                    // האם המרחק מהנפע ליעד קטן מהפוגע ליעד
                    if (distanceFromThisToDest < distanceFromOtherToDest)
                    {
                       
                        script.AddCollidedPoint(this.gameObject);
                        // מדליקים את הגדילה של הרדיוס של הנפגע
                        this.foundNextPoint = true;
                        // מחזירים הרדיוס של הפוגע
                        other.gameObject.GetComponent<SphereCollider>().radius = 0.5f;
                        if (!other.gameObject.CompareTag("MainCamera"))
                        {
                            // מכבים את רדיוס ההתנגשות של הפוגע
                            other.GetComponent<SphereCollider>().enabled = false;
                            // מכבים את הגדילה של הרדיוס של הפוגע
                            other.gameObject.GetComponent<NavigationController>().foundNextPoint = false;
                        }
                    }
                    
                    
                }
                else
                {
                    if (!other.gameObject.CompareTag("MainCamera"))
                    {
                        // מכבים את רדיוס ההתנגשות של הפוגע
                        other.GetComponent<SphereCollider>().enabled = false;
                        // מכבים את הגדילה של הרדיוס של הפוגע
                        other.gameObject.GetComponent<NavigationController>().foundNextPoint = false;
                    }
                    Debug.Log("found destination");
                    script.GetCollidedPoints().Last().GetComponent<SphereCollider>().radius = 0.5f;
                    script.AddCollidedPoint(this.gameObject);
                    string path = "";
                    foreach (GameObject point in script.GetCollidedPoints())
                    {
                        path += point.name + "->";
                    }
                    Debug.Log(path);

                    // Draw lines
                    for (int i = 0; i < collidedPoints.Count - 1; i++)
                    {
                        GameObject newObj = new GameObject();
                        LineRenderer newLineRenderer = newObj.AddComponent<LineRenderer>();
                        newObj.tag = "line";
                        newObj.name = "lineFrom|" + collidedPoints[i].name + "|To|" + collidedPoints[i + 1].name;
                        newLineRenderer.startWidth = 0.1f;
                        newLineRenderer.endWidth = 0.1f;
                        newLineRenderer.SetPositions(new Vector3[] { collidedPoints[i].transform.position, collidedPoints[i + 1].transform.position });
                        BoxCollider newBoxCollider = newObj.AddComponent<BoxCollider>();
                    }

                }

            }
        }
    }
}
