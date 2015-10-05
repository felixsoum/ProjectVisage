namespace JellySnow
{
	using UnityEngine;
	using System.Collections;
	using Facebook.Unity;
	
	public static class FbUtil
	{
		public static string GetPictureURL(string facebookID, string type = "large")
		{
			string url = string.Format("https://graph.facebook.com/{0}/picture?type={1}", facebookID, type);
			url += "&access_token=" + AccessToken.CurrentAccessToken.TokenString;
			return url;
		}
	}
}
