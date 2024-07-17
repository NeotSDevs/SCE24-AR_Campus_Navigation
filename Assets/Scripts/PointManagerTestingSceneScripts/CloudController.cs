using System;
using Unity.Services.Core;
using UnityEngine;


public class CloudController : MonoBehaviour
{
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

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
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
async Task SignInWithUsernamePasswordAsync(string username, string password)
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
}

