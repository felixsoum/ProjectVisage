namespace JellySnow
{
	using UnityEngine;
	using System.Collections;

	public static class Util
	{
		public static void Log(string msg)
		{
			Debug.Log(msg);
			if (Application.isWebPlayer)
			{
				Application.ExternalCall("console.log", msg);
			}
		}

		public static Sprite GetSprite(Texture2D tex)
		{
			return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
		}
	}
}
