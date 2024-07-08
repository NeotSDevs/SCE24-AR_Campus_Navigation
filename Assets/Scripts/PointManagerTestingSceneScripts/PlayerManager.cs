using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject messagePanel;
    public GameObject pointManager;
    bool loadedPoints;

    // Start is called before the first frame update
    void Start()
    {
        loadedPoints = false;
        messagePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setLoadedPoints()
    {
        loadedPoints = true;
    }

    // Function called when two colliders initially collide
    private void OnCollisionEnter(Collision collision)
    {
        PointManager script = pointManager.GetComponent<PointManager>();

        // if (collision.gameObject.tag == "point")
        // {
        //     collision.gameObject.GetComponent<Renderer>().enabled = true;
        //     collision.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().GetComponent<Renderer>().enabled = true;
        // }
        if (collision.gameObject.tag == "line")
        {
            collision.gameObject.GetComponent<LineRenderer>().enabled = true;
        }
        if (loadedPoints)
        {
            if (collision.gameObject.transform == script.GetPointTransforms().Last())
            {
                messagePanel.SetActive(true);
            }
        }

        //Debug.Log("Collision Enter: " + collision.gameObject.name);
    }

    // Function called when the colliders stop touching
    private void OnCollisionExit(Collision collision)
    {
        // if (collision.gameObject.tag == "point")
        // {
        //     collision.gameObject.GetComponent<Renderer>().enabled = false;
        //     collision.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().GetComponent<Renderer>().enabled = false;
        // }
        if (collision.gameObject.tag == "line")
        {
            collision.gameObject.GetComponent<LineRenderer>().enabled = false;
        }

        //Debug.Log("Collision Exit: " + collision.gameObject.name);
    }
}