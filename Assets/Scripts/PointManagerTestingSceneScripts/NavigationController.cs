using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public GameObject playerManager;
    public GameObject pointManager;
    private PlayerManager playerManagerScript;
    private PointManager pointManagerScript;
    public Material debugLinesMaterial;
    private bool foundNextPoint = false;

    private void Start()
    {
        playerManagerScript = playerManager.GetComponent<PlayerManager>();
        pointManagerScript = pointManager.GetComponent<PointManager>();
    }

    void Update()
    {
        if (foundNextPoint)
        {
            this.gameObject.GetComponent<SphereCollider>().radius += 0.1f;
        }
    }

    public void SetFoundNextPoint(bool foundNextPoint)
    {
        this.foundNextPoint = foundNextPoint;
    }

    // this הנפגע
    // other הפוגע
    private void OnTriggerEnter(Collider other)
    {
        // בודקים אם הנפגע הוא מסוג נקודה
        if (this.gameObject.CompareTag("point"))
        {
            if (pointManagerScript.GetLevel(this.gameObject) == pointManagerScript.GetLevel(other.gameObject))
            {
                // להשיג את הנקודת יעד
                GameObject destinationPoint = playerManagerScript.GetDestinationPoint();
                if (destinationPoint)
                {
                    // להשיג את רשימת הנקודות
                    List<GameObject> collidedPoints = playerManagerScript.GetCollidedPoints();

                    // בודקים האם הרשימה לא מכילה את הנפגע
                    if (!collidedPoints.Contains(this.gameObject))
                    {
                        // האם הנפגע הוא לא נקודת היעד
                        if (this.gameObject != destinationPoint)
                        {
                            //חישוב מרחק מהנפע אל היעד
                            float distanceFromThisToDest = Vector3.Distance(this.gameObject.transform.position, destinationPoint.transform.position);
                            // חישוב מרחק מהפוגע ליעד
                            float distanceFromOtherToDest = Vector3.Distance(other.gameObject.transform.position, destinationPoint.transform.position);

                            // האם המרחק מהנפע ליעד קטן מהפוגע ליעד
                            if (distanceFromThisToDest < distanceFromOtherToDest)
                            {
                                //מוסיפים את הנפגע לרשימה
                                playerManagerScript.AddCollidedPoint(this.gameObject);
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
                        else // found destination point
                        {
                            if (!other.gameObject.CompareTag("MainCamera"))
                            {
                                // מכבים את רדיוס ההתנגשות של הפוגע
                                other.GetComponent<SphereCollider>().enabled = false;
                                // מכבים את הגדילה של הרדיוס של הפוגע
                                other.gameObject.GetComponent<NavigationController>().foundNextPoint = false;

                                if (other.gameObject != destinationPoint)
                                {
                                    collidedPoints.Last().GetComponent<SphereCollider>().radius = 0.5f;
                                }
                            }

                            playerManagerScript.AddCollidedPoint(this.gameObject);

                            // להדליק את הרדיוס התנגשות לכל הנקודות שכיבינו במסלול
                            foreach (GameObject point in collidedPoints)
                            {
                                point.GetComponent<SphereCollider>().enabled = true;
                            }

                            // Draw debug lines
                            //for (int i = 0; i < collidedPoints.Count - 1; i++)
                            //{
                            //    GameObject newObj = new GameObject();
                            //    LineRenderer newLineRenderer = newObj.AddComponent<LineRenderer>();
                            //    newObj.tag = "line";
                            //    newObj.name = "lineFrom|" + collidedPoints[i].name + "|To|" + collidedPoints[i + 1].name;
                            //    newLineRenderer.startWidth = 0.1f;
                            //    newLineRenderer.endWidth = 0.1f;
                            //    newLineRenderer.SetPositions(new Vector3[] { collidedPoints[i].transform.position, collidedPoints[i + 1].transform.position });
                            //    newLineRenderer.material = debugLinesMaterial;
                            //    BoxCollider newBoxCollider = newObj.AddComponent<BoxCollider>();
                            //}

                            playerManagerScript.ResetDestinationPoint();

                        }

                    }
                }
            }
        }
    }
}
