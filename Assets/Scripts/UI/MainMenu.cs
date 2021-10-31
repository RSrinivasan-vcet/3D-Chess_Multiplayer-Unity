using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static Action<bool> GameLoadDataEvent = null;
    public static Action GameStartEvent = null;
    public static Action GameRestartEvent = null;
    public static Action<bool> OnJoinRoomCompletedEvent = null;

    public GameObject ChessBoard = null;
    public Animation LoadingIconAnim = null;
    public Button ConnectButton = null;
    public InputField NameText = null;
    public Text NameErrorText = null;

    public float TimeIntervel = 120.0f;
    public float fTimeIntervel = 0.0f;

    bool bGameStart = false;
    bool bWaitingStartGame = false;
    void Start()
    {
        ConnectButton.onClick.AddListener(Play);
        GameLoadDataEvent += GameLoadData;
        GameRestartEvent += GameRestart;
        GameStartEvent += GameStart;
        OnJoinRoomCompletedEvent += OnJoinRoomCompleted;
    }

    void Play()
    {
        if (!String.IsNullOrEmpty(NameText.text))
        {
            //bWaitingStartGame = true;
            fTimeIntervel = TimeIntervel;
            NetworkManager.Instance.SetPlayerName(NameText.text);
            NetworkManager.Instance.Connect();
            GameLoadData(true);
        }
        else
        {
            StartCoroutine(ErrorTextAnim());
        }
    }
    void GameLoadData(bool isGameLoad)
    {
        ConnectButton.gameObject.SetActive(!isGameLoad);
        NameErrorText.gameObject.SetActive(!isGameLoad);
        NameText.gameObject.SetActive(!isGameLoad);
        LoadingIconAnim.enabled = isGameLoad;
    }

    IEnumerator ErrorTextAnim()
    {
        Color color = NameErrorText.color;
        NameErrorText.GetComponent<Animation>().enabled = true;
        NameErrorText.color = Color.red;
        yield return new WaitForSeconds(2.50f);
        NameErrorText.GetComponent<Animation>().enabled = false;
        NameErrorText.color = color;
    }

    public void GameStart()
    {
        bGameStart = true;
        this.gameObject.SetActive(false);
        ChessBoard.SetActive(true);
    }

    void GameRestart()
    {
        bGameStart = false;
        ChessBoard.SetActive(false);
        this.gameObject.SetActive(true);
    }

    void OnJoinRoomCompleted(bool isWaiting)
    {
        bWaitingStartGame = isWaiting;        
    }
    private void Update()
    {
        if (bWaitingStartGame && !bGameStart)
        {
            if (fTimeIntervel <= 0)
            {
                print("Disconnected to server");
                PhotonNetwork.Disconnect();
                bWaitingStartGame = false;
                GameLoadData(false);
            }
            else
            {
                fTimeIntervel -= Time.deltaTime;
            }
        }
    }
}
