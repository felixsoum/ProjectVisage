namespace JellySnow
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

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
		
		public static void Shuffle<T>( this IList<T> list )
		{
			int n = list.Count;
			while( n > 1 )
			{
				n--;
				int k = n + 1;
				while( k > n )
				{
					k = (int)(Random.value * (n + 1));
				}
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}
