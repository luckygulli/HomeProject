﻿using UnityEngine;
using SocketIO;

public class PlayerController : MonoBehaviour 
{
	SocketIOComponent socket;
	JSONObject message;

	void Start()
	{
		socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent> ();
		message = new JSONObject();
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space)){
			socket.Emit("register", JSONObject.CreateStringObject("debug"));
		}

		// declenche manuellement le début d'une partie 
		if (Input.GetKeyDown(KeyCode.Z)) 
		{
			socket.Emit("startGame");
		}

		if (Input.GetKeyDown(KeyCode.R)) 
		{
//			string jsonString = "{\"otherPlayerHand\":[{\"playerId\":\"F2cz5kl13BksPJ40AAAA\",\"cardId\":[11,13,0,12,3]},{\"playerId\":\"yEChzy01pZO48QkdAAAB\",\"cardId\":[14,6,9,4,5]}]}";
//			OtherPlayerHands otherPlayerHands = JsonUtility.FromJson<OtherPlayerHands>(jsonString);
//			string jsonString = "{\"id\":\"6\",\"value\":1,\"player\":\"lO9P41X728aly81YAAAC\"}";
//			PlayerCard playerCard = JsonUtility.FromJson<PlayerCard>(jsonString);
//			Debug.Log(playerCard);
		}
	}
}