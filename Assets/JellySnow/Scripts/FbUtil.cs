namespace JellySnow
{
	using UnityEngine;
	using System.Collections;
	using Facebook.Unity;
	
	public static class FbUtil
	{
		public static string GetPictureURL(string facebookID, int width, int height)
		{
			string url = string.Format("https://graph.facebook.com/{0}/picture?width={1}&height={2}", facebookID, width, height);
			url += "&access_token=" + AccessToken.CurrentAccessToken.TokenString;
			return url;
		}
	}
}
