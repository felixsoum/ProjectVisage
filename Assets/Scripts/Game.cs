using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using JellySnow;
using Facebook.Unity;

public class Game : MonoBehaviour
{
	// Amount of cards in play.
	public int numberOfCards = 54;

	// Game objects to show the cards on
	public GameObject cardTarget = null;
	public GameObject cardGrid = null;

	// Very simple implementation for testing
	class CardData
	{
		public int Id { get; set; }
		public Texture2D Picture { get; set; }
	}
	class CardDisplay
	{
		public GameObject GameObject { get; set; }
	}
	class CardInfo
	{
		public CardData Data { get; set; }
		public CardDisplay Display { get; set; }
	}

	private List<CardData> cardsAll = new List<CardData>();
	private List<CardData> cardsTarget = new List<CardData>();

	private List<CardInfo> cardsInPlay = new List<CardInfo>();

	void Awake()
	{
		// Put some random numbers to represent cards for now
		for( int i = 0; i < 10000; ++i )
		{
			var card = new CardData();
			card.Id = i;
			cardsAll.Add( card );
		}
	}

	void Start()
	{
		ShuffleCards();
		DisplayCards();
		ShowTargetCard();
		
		// Test getting photos
		//FB.API( "me/photos?fields=picture", HttpMethod.GET, OnFB_GetSelfPictures );
		//FB.API( "search?q=a&type=user&fields=picture", HttpMethod.GET, OnFB_GetSearchPictures );
		//FB.API( "search?q=b&type=user&fields=picture", HttpMethod.GET, OnFB_GetSearchPictures );
		//FB.API( "search?q=c&type=user&fields=picture", HttpMethod.GET, OnFB_GetSearchPictures );
		FB.API( "me/invitable_friends?limit=" + numberOfCards.ToString(), HttpMethod.GET, OnFB_GetFriendPictures );
	}

	void Update()
	{
	}

	private void ShuffleCards()
	{
		foreach( var card in cardsInPlay )
		{
			Destroy( card.Display.GameObject );
			card.Display.GameObject = null;
		}
		cardsInPlay.Clear();
		cardsTarget.Clear();
		cardsAll.Shuffle();
		for( int i = 0; i < numberOfCards; ++i )
		{
			cardsInPlay.Add( new CardInfo() { Data = cardsAll[i] } );
			cardsTarget.Add( cardsAll[i] );
		}
		cardsTarget.Shuffle();
	}

	private void DisplayCards()
	{
		foreach( var card in cardsInPlay )
		{
			card.Display = new CardDisplay() { GameObject = Instantiate( cardTarget ) };
			card.Display.GameObject.transform.SetParent( cardGrid.transform, false );
			card.Display.GameObject.GetComponent<Button>().interactable = true;
			SetCardObjectDisplay( card.Display.GameObject, card.Data );

			var copy = card;
			card.Display.GameObject.GetComponent<Button>().onClick.AddListener( () => OnClick_Card( copy ) );
		}
	}

	private void ShowTargetCard()
	{
		SetCardObjectDisplay( cardTarget, cardsTarget[0] );
		cardTarget.GetComponent<Animator>().SetTrigger( "Show" );
	}

	private void RefreshCardPicture( CardInfo card, Texture2D picture )
	{
		card.Data.Picture = picture;
		SetCardObjectDisplay( card.Display.GameObject, card.Data );
		if( cardsTarget.Count > 0 && cardsTarget[0] == card.Data )
		{
			SetCardObjectDisplay( cardTarget, cardsTarget[0] );
		}
	}

	private void SetCardObjectDisplay( GameObject cardObject, CardData cardData )
	{
		if( cardObject != null )
		{
			if( cardData.Picture != null  )
			{
				cardObject.GetComponent<Image>().sprite = Util.GetSprite( cardData.Picture );
			}
			else
			{
				cardObject.GetComponentInChildren<Text>().text = cardData.Id.ToString();
			}
			cardObject.GetComponentInChildren<Text>().enabled = cardData.Picture == null;
		}
	}

	private void OnClick_Card( CardInfo card )
	{
		if( card.Data == cardsTarget[0] )
		{
			card.Display.GameObject.GetComponent<Image>().color = new Color( 0.0f, 0.0f, 0.0f, 0.0f );
			card.Display.GameObject.GetComponent<Button>().interactable = false;
			card.Display.GameObject.GetComponentInChildren<Text>().text = "";

			cardsTarget.RemoveAt( 0 );
			if( cardsTarget.Count == 0 )
			{
				ShuffleCards(); // Just restart for now
				DisplayCards();
			}
			ShowTargetCard();
		}
		else
		{
			Debug.Log( "Wrong card pressed " + card.Data.Id );
		}
	}

	private void OnFB_GetSelfPictures( IGraphResult result ) // me/photos?fields=picture
	{
		Debug.Log( result );
		var data = result.ResultDictionary.GetValueOrDefault<List<object>>( "data" );
		if( data != null )
		{
			var pictureUrls = new List<String>();
			foreach( var entry in data )
			{
				var entryData = entry as Dictionary<string, object>;
				if( entryData != null )
				{
					string pictureUrl = (string)entryData["picture"];
					pictureUrls.Add( pictureUrl );
				}
			}
			StartCoroutine( OnWWW_GetPictures( pictureUrls ) );
		}
	}

	private void OnFB_GetFriendPictures( IGraphResult result ) // me/photos?fields=picture
	{
		OnFB_GetSearchPictures( result ); //it's the same structure
	}

	private void OnFB_GetSearchPictures( IGraphResult result ) // search?q={0}&type=user&fields=picture, where {0} is searched string
	{
		Debug.Log( result );
		var data = result.ResultDictionary.GetValueOrDefault<List<object>>( "data" );
		if( data != null )
		{
			var pictureUrls = new List<String>();
			foreach( var entry in data )
			{
				var entryData = entry as Dictionary<string, object>;
				if( entryData != null )
				{
					var pictureData = entryData["picture"] as Dictionary<string, object>;
					if( pictureData != null )
					{
						var urlData = pictureData["data"] as Dictionary<string, object>;
						if( urlData != null )
						{
							string pictureUrl = (string)urlData["url"];
							pictureUrls.Add( pictureUrl );
						}
					}
				}
			}
			StartCoroutine( OnWWW_GetPictures( pictureUrls ) );
		}
	}

	private IEnumerator OnWWW_GetPictures( List<String> urls )
	{
		for( int i = 0; i < urls.Count; ++i )
		{
			WWW www = new WWW( urls[i] );
			Debug.Log( urls[i] );
			yield return www;
			foreach( var card in cardsInPlay ) // should track properly instead of searching everytime
			{
				if( card.Data.Picture == null )
				{
					RefreshCardPicture( card, www.texture );
					break;
				}
			}
		}
	}
}
