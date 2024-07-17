using Google.XR.ARCoreExtensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using UnityEngine.XR.ARSubsystems;

public class GeospatialAnchorPrecisionChecker : MonoBehaviour
{
    public ARGeospatialCreatorAnchor creatorAnchor;
    public AREarthManager earthManager;
    public float desiredAccuracyThreshold = 5f; // 5 meters
    public float maxWaitTime = 60f; // Maximum time to wait for accuracy improvement
    public int samplesForAverage = 10; // Number of samples to average

    private List<GeospatialPose> poseSamples = new List<GeospatialPose>();

    private void Start()
    {
        StartCoroutine(CheckAnchorPrecision());
    }

    private IEnumerator CheckAnchorPrecision()
    {
        float elapsedTime = 0f;

        while (elapsedTime < maxWaitTime)
        {
            if (earthManager.EarthTrackingState == TrackingState.Tracking)
            {
                yield return StartCoroutine(SamplePoses());

                GeospatialPose averagePose = CalculateAveragePose();
                float distance = CalculateDistance(averagePose, creatorAnchor.Latitude, creatorAnchor.Longitude, creatorAnchor.Altitude);

                Debug.Log($"Average pose: Lat {averagePose.Latitude}, Lon {averagePose.Longitude}, Alt {averagePose.Altitude}");
                Debug.Log($"Distance from anchor: {distance}m");

                if (distance <= desiredAccuracyThreshold)
                {
                    Debug.Log("Desired accuracy achieved!");
                    // Proceed with object placement or other operations
                    yield break;
                }
                else
                {
                    Debug.Log("Anchor position might need adjustment.");
                    // Implement logic to adjust or re-create the anchor if needed
                }
            }

            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        Debug.LogWarning("Max wait time reached without achieving desired accuracy.");
    }

    private IEnumerator SamplePoses()
    {
        poseSamples.Clear();
        for (int i = 0; i < samplesForAverage; i++)
        {
            if (earthManager.EarthTrackingState == TrackingState.Tracking)
            {
                poseSamples.Add(earthManager.CameraGeospatialPose);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private GeospatialPose CalculateAveragePose()
    {
        double sumLat = 0, sumLon = 0, sumAlt = 0;
        foreach (GeospatialPose pose in poseSamples)
        {
            sumLat += pose.Latitude;
            sumLon += pose.Longitude;
            sumAlt += pose.Altitude;
        }
        return new GeospatialPose
        {
            Latitude = sumLat / poseSamples.Count,
            Longitude = sumLon / poseSamples.Count,
            Altitude = sumAlt / poseSamples.Count
        };
    }

    private float CalculateDistance(GeospatialPose pose, double targetLat, double targetLon, double targetAlt)
    {
        // This is a simple approximation. For more accurate results, consider using a proper geodesic distance calculation.
        const double earthRadius = 6371000; // meters
        double dLat = (targetLat - pose.Latitude) * Mathf.Deg2Rad;
        double dLon = (targetLon - pose.Longitude) * Mathf.Deg2Rad;
        double a = Mathf.Sin((float)dLat / 2) * Mathf.Sin((float)dLat / 2) +
                   Mathf.Cos((float)(pose.Latitude * Mathf.Deg2Rad)) * Mathf.Cos((float)(targetLat * Mathf.Deg2Rad)) *
                   Mathf.Sin((float)dLon / 2) * Mathf.Sin((float)dLon / 2);
        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt(1 - (float)a));
        double horizontalDistance = earthRadius * c;
        double verticalDistance = targetAlt - pose.Altitude;
        return Mathf.Sqrt((float)(horizontalDistance * horizontalDistance + verticalDistance * verticalDistance));
    }
}