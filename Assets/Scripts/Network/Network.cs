﻿using UnityEngine;
using SocketIO;

public class Network : MonoBehaviour
{
    private SocketIOComponent socket;
    private TimeBomb timeBomb;
    private UIManager uiManager;
    
    private string pseudoForServer;
    private string idForServer;
    private string serverUrl;
    public static string ClientID { get; private set; }
    private bool isConnected;

    void Start()
    {
        socket = GetComponent<SocketIOComponent>();

        // This line will set up the listener function
        socket.On("connectionEstabilished", onConnectionEstabilished);
        socket.On("foreignMessage", onForeignMessage);
        socket.On("playerConnection", onPlayerConnection);
        socket.On("playerDisconnection", onPlayerDisconnection);
        socket.On("sendCard", onNewHandDistributed);
        socket.On("otherCard", onOtherCardDistributed);
        socket.On("roleAssigment", onRoleAssignement);
        socket.On("gameStarted", onStartGame);
        socket.On("cardHover", onCardHover);
        socket.On("revealCard", onCardReveal);
        socket.On("token", onTokenDistributed);
        socket.On("newTurnAvailable", onNewTurnAvailable);
        socket.On("newTurn", onNewTurn);
        socket.On("endTurn", onEndTurn);
        socket.On("defausse", onDefausseSent);
        socket.On("handFlip", onPlayerHandFlip);
        socket.On("allHandFlipped", onAllHandFlipped);
        socket.On("GoodGuysWin", onGoodGuysWin);
        socket.On("BadGuysWin", onBadGuysWin);

        timeBomb = GameObject.Find("GameManager").GetComponent<TimeBomb>();
        uiManager = GameObject.Find("GameManager").GetComponent<UIManager>();
    }

    // This is the listener function definition
    void onConnectionEstabilished(SocketIOEvent evt)
    {
        isConnected = true;
        Debug.Log("You are connected: " + evt.data.GetField("id"));
        idForServer = evt.data.GetField("id").str;
        uiManager.connectionCheckIcon.gameObject.SetActive(true);
    }

    void onPlayerConnection(SocketIOEvent evt)
    {
        string id = evt.data["id"].str;
        string pseudo = evt.data["username"].str;
        Debug.Log("Player is connected: " + id);

        timeBomb.AddPlayer(id, pseudo);
    }

    void onPlayerDisconnection(SocketIOEvent evt)
    {
        string id = evt.data["id"].str;
        Debug.Log("Player is disconnected: " + id);
        
        timeBomb.RemovePlayer(id);
    }

    void onForeignMessage(SocketIOEvent evt)
    {
        Debug.Log(evt.data.GetField("message"));
    }

    void onNewHandDistributed(SocketIOEvent evt)
    {
        string jsonString = evt.data.ToString();
        PlayerHand playerHand = JsonUtility.FromJson<PlayerHand>(jsonString);

        timeBomb.GeneratePlayerHand(playerHand);
    }

    void onOtherCardDistributed(SocketIOEvent evt)
    {
        string jsonString = evt.data.ToString();
        OtherPlayerHands otherPlayerHands = JsonUtility.FromJson<OtherPlayerHands>(jsonString);
        timeBomb.GenerateOtherPlayersHand(otherPlayerHands);
    }

    void onStartGame(SocketIOEvent evt)
    {
        string maxSecureWire = evt.data.GetField("maxSecureWire").ToString();
        string maxDefusingWire = evt.data.GetField("maxDefusingWire").ToString();
        uiManager.maxSecureWire = maxSecureWire;
        uiManager.maxDefusingWire = maxDefusingWire;
        uiManager.diffusingWireText.gameObject.SetActive(true);
        uiManager.secureWireText.gameObject.SetActive(true);
        uiManager.hideStartGameButton();
        uiManager.hideRestartGameButton();
        uiManager.hideGoodWin();
        uiManager.hideBadWin();
        uiManager.updateWireCounter("0", "0");
        timeBomb.startGame();
    }

    void onRoleAssignement(SocketIOEvent evt)
    {
        string role = evt.data.GetField("role").ToString();
        Debug.Log("Role assigned: " + role);

        timeBomb.showRole(role);
    }

    public void onCardHover(SocketIOEvent evt) {
        string cardId = evt.data.GetField("hover").str;
        timeBomb.HoverCard(cardId);
    }

    public void onCardReveal(SocketIOEvent evt) {
        string jsonString = evt.data.ToString();
		PlayerCard playerCard = JsonUtility.FromJson<PlayerCard>(jsonString);
        timeBomb.RevealCard(playerCard);
    }

    public void onTokenDistributed(SocketIOEvent evt) {
        string playerId = evt.data.GetField("token").str;
        bool isSelf = playerId.CompareTo(idForServer) == 0;
        timeBomb.showToken(isSelf, playerId);
    }

    public void onNewTurn(SocketIOEvent evt) {
        string turnNumber = evt.data.GetField("turn").ToString();
        Debug.Log("Turn "+ turnNumber);
        uiManager.hideNextTurnButton();
        uiManager.hideEndTurnBanner();
        uiManager.showNewTurnBanner(turnNumber);

        timeBomb.initNewTurn();
    }

    public void onEndTurn(SocketIOEvent evt) {
        uiManager.showEndTurnBanner();
    }

    public void onNewTurnAvailable(SocketIOEvent evt) {
        uiManager.showNextTurnButton();
    }

    public void onDefausseSent(SocketIOEvent evt) {
        string secureWire = evt.data.GetField("secureWire").ToString();
        string defusingWire = evt.data.GetField("defusingWire").ToString();
        uiManager.updateWireCounter(secureWire, defusingWire);
    }

    void onPlayerHandFlip(SocketIOEvent evt)
    {
        string id = evt.data["id"].str;       
        timeBomb.flipHandForPlayer(id);
    }

    void onAllHandFlipped(SocketIOEvent evt) {
        timeBomb.startTurn();
    }

    void onGoodGuysWin(SocketIOEvent evt) {
        uiManager.showGoodWin();
    }

    void onBadGuysWin(SocketIOEvent evt) {
        uiManager.showBadWin();
    }

    public void setPseudoForServer(string pseudo) {
        pseudoForServer = pseudo;
    }

    public void setUrlForServer(string url) {
        isConnected = false;
        serverUrl = url;
    }

    public void updateConnection() {
        isConnected = false;
        uiManager.connectionCheckIcon.gameObject.SetActive(false);
        if (serverUrl == null) {
            serverUrl = socket.url;
        }
        socket.changeUrl(serverUrl);
    }

    public void joinGame() {
        if (serverUrl != null) {
            socket.changeUrl(serverUrl);
        }

        if (pseudoForServer == null) {
            pseudoForServer = "noname";
        }

        if (isConnected == true) {
            socket.Emit("register", JSONObject.CreateStringObject(pseudoForServer));
            uiManager.startGame();
        }
    }

    public void triggerNextTurn() {
        socket.Emit("newTurn");
    }

    public void triggerStartGame(){
    	socket.Emit("startGame");
    }

    public void triggerFlipHand() {
        socket.Emit("flipHand");
    }

}