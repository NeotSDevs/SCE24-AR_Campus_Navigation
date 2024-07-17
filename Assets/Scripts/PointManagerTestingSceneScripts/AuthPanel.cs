using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using System.Text.RegularExpressions;



public class authPanel : MonoBehaviour
{
    public TMP_InputField UsernameField;
    public TMP_InputField PasswordField;
    public TMP_Text errorText;
    public GameObject userPanel;
    private Regex regex = new Regex(@"^[^\s@]+(@ac)?\.sce\.ac\.il$");




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
        
        var playerData = new Dictionary<string, object>{
          {"Role","client"}};
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        errorText.text ="SignUp is successful.";
        this.gameObject.SetActive(false);
        userPanel.SetActive(true);



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
        this.gameObject.SetActive(false);
        userPanel.SetActive(true);
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
        try{
            
        // Get the inputs field text
        string username = UsernameField.text.ToString();
        if(regex.IsMatch(username)){
            string password = PasswordField.text.ToString();
            await SignInWithUsernamePasswordAsync(username, password);
        }
        
        else{
            errorText.text = "Invalid email address";
        }
        }
        catch(Exception e){
            errorText.text = e.Message;
        }
    }
    public async void AnonymouslySignIn()
    {
        try{
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            errorText.text ="SignIn is successful.";
            this.gameObject.SetActive(false);
            userPanel.SetActive(true);
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
}
