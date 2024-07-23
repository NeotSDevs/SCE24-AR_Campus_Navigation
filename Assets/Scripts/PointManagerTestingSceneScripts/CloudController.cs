using System;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using System.Collections.Generic;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using System.IO;
using Newtonsoft.Json;

public class CloudController : MonoBehaviour
{
    public PointManager pointManager;
    private const string customItemId = "factory_simulation_points";
    /*
   * The response from the script, used for deserialization.
   * In this example, the script would return a JSON in the format
   * {"welcomeMessage": "Hello, arguments['name']. Welcome to Cloud Code!"}
   */
    private class CloudCodeResponse
    {
        public string responseMessage;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    private async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async void SavePoint(Point pointObj)
    {
        var arguments = new Dictionary<string, object> { { "point", pointObj }, { "data_id", customItemId } };
        var response = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResponse>("SavePointScript", arguments);
        Debug.Log(response.responseMessage);
    }

    public async void LoadPoints()
    {
        
        var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAllAsync(customItemId);
        string pointsListPath = Application.persistentDataPath + "/PointsList.json";
        StreamWriter writer = new StreamWriter(pointsListPath, false);
        writer.WriteLine("");
        writer.Close();
        writer = new StreamWriter(pointsListPath, true);
        writer.WriteLine("{\"points\":[");
        int i = 0;
        foreach (var customItem in customItemData)
        {
            writer.WriteLine(JsonConvert.SerializeObject(customItem.Value.Value) + (i < customItemData.Count - 1 ? "," : ""));
            i++;
        }
        writer.WriteLine("]}");
        writer.Close();
        pointManager.LoadPoints();
    }

    public async void RemovePoint(string pointName)
    {
        var arguments = new Dictionary<string, object> { { "point_name", pointName },{ "data_id", customItemId } };
        var response = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResponse>("DeletePointScript", arguments);
        Debug.Log(response.responseMessage);
    }
}

