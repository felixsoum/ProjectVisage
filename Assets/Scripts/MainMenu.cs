﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using JellySnow;

public class MainMenu : MonoBehaviour
{
	public Image profileImage;

	private void Awake()
	{
		// Initialize FB SDK              
		enabled = false;                  
		FB.Init(SetInit, OnHideUnity);  
	}

	private void SetInit()                                                                       
	{                                                                                                                                                            
		enabled = true; // "enabled" is a property inherited from MonoBehaviour
		if (FB.IsLoggedIn)
		{
			Util.Log("Already logged in");
//			OnLoggedIn();
		}                          
	}

	private void OnHideUnity(bool isGameShown)
	{              
		Util.Log("OnHideUnity");
		if (!isGameShown)                                                                        
		{                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		}                                                                                        
		else                                                                                     
		{                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}                                                                                        
	}

	public void OnClickLoginButton()
	{
		FB.LogInWithReadPermissions (
			new List<string>(){"public_profile", "email", "user_friends"},
		LoginCallback
		);
	}

	void LoginCallback(ILoginResult result)                                                        
	{                                                                                          
		Util.Log("LoginCallback");                                                          
		
		if (FB.IsLoggedIn)                                                                     
		{                                                                                      
			OnLoggedIn();                                                                      
		}                                                                                      
	}    

	void OnLoggedIn()
	{
		Util.Log("Logged in. ID: " + AccessToken.CurrentAccessToken.UserId);
		
//		// Reqest player info and profile picture                                                                           
//		FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);  
//		LoadPicture(Util.GetPictureURL("me", 128, 128),MyPictureCallback);  
		StartCoroutine("GetPic");
	}

	IEnumerator GetPic()
	{
		string url = "https" + "://graph.facebook.com/"+ AccessToken.CurrentAccessToken.UserId +"/picture";
		url += "?access_token=" + AccessToken.CurrentAccessToken.TokenString;
		WWW www = new WWW(url);
		yield return www;
		Texture2D profilePic = www.texture;
		profileImage.sprite = Sprite.Create(profilePic, new Rect(0, 0, profilePic.width, profilePic.height), Vector2.zero);
	}

}