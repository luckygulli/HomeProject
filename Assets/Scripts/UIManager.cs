﻿using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject loginUi;
    public Transform uiRoot;
    private Network network;
    private GameObject currentGameObject;
    private TextMeshProUGUI turnCounterTextMesh;
    public RectTransform newTurnPopUpPrefab;
    public TextMeshProUGUI diffusingWireText;
    public TextMeshProUGUI secureWireText;
    public string maxDefusingWire;
    public string maxSecureWire;
    public GameObject goodInstructionPopUpPrefab;
    public GameObject badInstructionPopUpPrefab;
    public GameObject connectionCheckIcon;
    public GameObject endTurnPopUpPrefab;
    public GameObject nextTurnButtonPrefab;
    public GameObject startGameButtonPrefab;
    public Button flipCardButton;
    public GameObject badWinPopUpPrefab;
    public GameObject goodWinPopUpPrefab;
    private GameObject badWinPopUp;
    private GameObject goodWinPopUp;
    public GameObject restartGameButtonPrefab;
    private GameObject endTurnPopUp;
    private RectTransform newTurnPopUp;
    private GameObject nextTurnButton;
    private GameObject startGameButton;
    private GameObject restartGameButton;

    void Start()
    {
        network = GameObject.Find("SocketIO").GetComponent<Network>();
    }

    public void startGame() {
        loginUi.SetActive(false);
        showStartGameButton();
    }

    public void showNextTurnButton()
    {
        nextTurnButton = Instantiate(nextTurnButtonPrefab, new Vector3(Screen.width * 0.5f, 210, 0), Quaternion.identity);
        nextTurnButton.transform.localScale = Vector3.zero;
        nextTurnButton.transform.SetParent(uiRoot);
        nextTurnButton.gameObject.GetComponent<Button>().onClick.AddListener(network.triggerNextTurn);
        LeanTween.scale(nextTurnButton, new Vector3(2, 2, 0), 1f);
    }

    public void hideNextTurnButton()
    {
        if (nextTurnButton != null)
        {
            scaleDownAndDestroy(nextTurnButton);
        }
    }

    public void showEndTurnBanner()
    {
        endTurnPopUp = Instantiate(endTurnPopUpPrefab, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), Quaternion.identity);
        endTurnPopUp.GetComponent<RectTransform>().localScale = Vector3.zero;
        endTurnPopUp.transform.SetParent(uiRoot);
        LeanTween.scale(endTurnPopUp, Vector3.one * 2, 1f);
    }

    public void hideEndTurnBanner()
    {
        if (endTurnPopUp != null)
        {
            scaleDownAndDestroy(endTurnPopUp);
        }
    }

    public void showGoodInstruction()
    {
        GameObject popUp = Instantiate(goodInstructionPopUpPrefab, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), Quaternion.identity);
        popUp.transform.SetParent(uiRoot);
    }

    public void showBadInstruction()
    {
        GameObject popUp = Instantiate(badInstructionPopUpPrefab, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), Quaternion.identity);
        popUp.transform.SetParent(uiRoot);
    }

    public void showStartGameButton() {
        startGameButton = Instantiate(startGameButtonPrefab, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f,0), Quaternion.identity);
        currentGameObject = startGameButton;
        startGameButton.transform.SetParent(uiRoot);
        startGameButton.gameObject.GetComponent<Button>().onClick.AddListener(network.triggerStartGame);
        startGameButton.gameObject.GetComponent<Button>().onClick.AddListener(DestroyGameObject);
    }

    public void hideStartGameButton() {
        if (startGameButton != null) {
            scaleDownAndDestroy(startGameButton);
        }
    }

    private void showRestartButton() {
        restartGameButton = Instantiate(restartGameButtonPrefab, new Vector3(Screen.width * 0.5f,Screen.height * 0.2f,0), Quaternion.identity);
        currentGameObject = restartGameButton;
        restartGameButton.transform.SetParent(uiRoot);
        restartGameButton.gameObject.SetActive(true);
        restartGameButton.gameObject.GetComponent<Button>().onClick.AddListener(network.triggerStartGame);
        restartGameButton.gameObject.GetComponent<Button>().onClick.AddListener(DestroyGameObject);
        LeanTween.scale(restartGameButton, Vector3.one, 1f);
    }

    public void hideRestartGameButton() {
        if (restartGameButton != null) {
            scaleDownAndDestroy(restartGameButton);
        }
    }

    public void showNewTurnBanner(string turnNumber)
    {
        newTurnPopUp = Instantiate(newTurnPopUpPrefab, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), Quaternion.identity);
        newTurnPopUp.SetParent(uiRoot);
        newTurnPopUp.localScale = Vector3.zero;
        turnCounterTextMesh = newTurnPopUp.transform.GetComponentInChildren<TextMeshProUGUI>();//GetChild(0).gameObject.transform.GetChild(0).GetComponent<;
        turnCounterTextMesh.SetText(turnNumber);
        TimeBomb.IsInputEnabled = false;
        LeanTween.scale(newTurnPopUp, Vector3.one * 2, 1f);
        LeanTween.delayedCall(2, PrepareNewTurn);
    }

    public void showFlipCardButton(){
        flipCardButton.gameObject.SetActive(true);
    }

    public void hideFlipButton() {
        flipCardButton.gameObject.SetActive(false);
    }

    public void showBadWin() {
        TimeBomb.IsInputEnabled = false;
        badWinPopUp = Instantiate(badWinPopUpPrefab, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), Quaternion.identity);
        badWinPopUp.GetComponent<RectTransform>().localScale = Vector3.zero;
        badWinPopUp.transform.SetParent(uiRoot);
        LeanTween.scale(badWinPopUp, Vector3.one, 1f);

        showRestartButton();
    }

    public void showGoodWin() {
        TimeBomb.IsInputEnabled = false;
        goodWinPopUp = Instantiate(goodWinPopUpPrefab, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), Quaternion.identity);
        goodWinPopUp.GetComponent<RectTransform>().localScale = Vector3.zero;
        goodWinPopUp.transform.SetParent(uiRoot);
        LeanTween.scale(goodWinPopUp, Vector3.one, 1f);

        showRestartButton();
    }

    public void hideBadWin() {
        if (badWinPopUp != null) {
            scaleDownAndDestroy(badWinPopUp);
        }
    }

    public void hideGoodWin() {
        if (goodWinPopUp != null) {
            scaleDownAndDestroy(goodWinPopUp);
        }
    }

    public void PrepareNewTurn()
    {
        scaleDownAndDestroy(newTurnPopUp.gameObject);
        TimeBomb.IsInputEnabled = true;
    }

    public void updateWireCounter(string secureCounter, string difusingCounter)
    {
        secureWireText.SetText("Secure wire: " + secureCounter + "/" + maxSecureWire);
        diffusingWireText.SetText("Defusing wire: " + difusingCounter + "/" + maxDefusingWire);
    }

    public void scaleDownAndDestroy(GameObject gameObject) {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 1f).setDestroyOnComplete(true);
    }

    private void DisableGameObject()
    {
        currentGameObject.gameObject.SetActive(false);
    }

    private void DestroyGameObject()
    {
        Destroy(currentGameObject);
    }

    public void flipCard() {

    }

}
