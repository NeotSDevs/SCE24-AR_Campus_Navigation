using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;



public class authPanel : MonoBehaviour
{
    public TMP_InputField UsernameField;
    public TMP_InputField PasswordField;
    public TMP_Text errorText;


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
        errorText.text ="SignUp is successful.";
    }
    catch (AuthenticationException ex)
    {
        // Compare error code to AuthenticationErrorCodes
        // Notify the player with the proper error message
        errorText.text = ex.Message;
    }
    catch (RequestFailedException ex)
    {
        // Compare error code to CommonErrorCodes
        // Notify the player with the proper error message
        errorText.text = ex.Message;
    }
}

 async Task SignInWithUsernamePasswordAsync(string username, string password)
{
    try
    {
        await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
        errorText.text ="SignIn is successful.";
    }
    catch (AuthenticationException ex)
    {
        // Compare error code to AuthenticationErrorCodes
        // Notify the player with the proper error message
        errorText.text = ex.Message;
    }
    catch (RequestFailedException ex)
    {
        // Compare error code to CommonErrorCodes
        // Notify the player with the proper error message
        errorText.text = ex.Message;
    }
}

    public async void SignUp()
    {
        // Get the inputs field text
        string username = UsernameField.text.ToString();
        
        string password = PasswordField.text.ToString();
        await SignUpWithUsernamePasswordAsync(username, password);
    }
    public async void SignIn()
    {
        // Get the inputs field text
        string username = UsernameField.text.ToString();
        string password = PasswordField.text.ToString();
        await SignInWithUsernamePasswordAsync(username, password);
    }
}
