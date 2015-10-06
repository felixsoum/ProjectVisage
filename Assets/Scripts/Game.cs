using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using JellySnow;

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
	}

	void Update()
	{
	}

	private void ShuffleCards()
	{
		foreach( var card in cardsInPlay )
		{
			Destroy( card.Display.GameObject );
		}
		cardsInPlay.Clear();
		cardsTarget.Clear();
		cardsAll.Shuffle();
		for( int i = 0; i < numberOfCards; ++i )
		{
			cardsInPlay.Add( new CardInfo() { Data = cardsAll[i] } );
			cardsTarget.Add( cardsAll[i] );
		}
		//cardsTarget.Shuffle();
	}

	private void DisplayCards()
	{
		foreach( var card in cardsInPlay )
		{
			card.Display = new CardDisplay() { GameObject = Instantiate( cardTarget ) };
			card.Display.GameObject.transform.SetParent( cardGrid.transform, false );
			card.Display.GameObject.GetComponent<Button>().interactable = true;
			card.Display.GameObject.GetComponentInChildren<Text>().text = card.Data.Id.ToString();

			var copy = card;
			card.Display.GameObject.GetComponent<Button>().onClick.AddListener( () => OnClick_Card( copy ) );
		}
	}

	private void ShowTargetCard()
	{
		cardTarget.GetComponentInChildren<Text>().text = cardsTarget[0].Id.ToString();
		cardTarget.GetComponent<Animator>().SetTrigger( "Show" );
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
}
