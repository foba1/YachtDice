using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks, IChatClientListener
{

    private readonly string gameVersion = "5.0";

    public GameObject Login;
    public Text Winrate;
    public GameObject Lobby;
    public GameObject Loginbutton;
    public GameObject Logoutbutton;
    public GameObject Namechange;
    public Text username;
    public Text connection;
    public Button login;
    public Text Countofplayer;
    public Button[] Room;
    public Button Right;
    public Button Left;
    public Text Chat;
    public InputField Chatting;
    public GameObject Createroom;
    public InputField Roomname;
    public Toggle Secret;
    public InputField Password;
    public Text time;
    public GameObject times;
    public GameObject PasswordInspect;
    public InputField PasswordInput;

    private ChatClient chatClient;
    private string currentChannelName;
    protected internal ChatAppSettings chatAppSettings;

    List<RoomInfo> roomlist = new List<RoomInfo>();
    int c_page = 1, m_page, multiple, passwordindex, gameexit = 0;

    bool canchat = false;

    System.Random rand;

    public void AddLeaderboard_2() => Social.ReportScore(int.Parse(PlayerPrefs.GetInt("m_score").ToString()), GPGSIds.leaderboard_2, (bool success) => { });

    public void UpdateTimeText()
    {
        time.text = "시간(" + (30 + (int)(30 * times.GetComponent<Slider>().value)) + "초)";
    }

    public void Single()
    {
        PhotonNetwork.CreateRoom(roomName: "Room " + rand.Next(1, 1000), new RoomOptions { MaxPlayers = 1, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "p", "NotLocked" } }, CustomRoomPropertiesForLobby = new string[] { "p" } });
        PlayerPrefs.SetInt("time", (30 + (int)(30 * times.GetComponent<Slider>().value)));
        PlayerPrefs.Save();
    }

    public void AddLine(string lineString)
    {
        Chat.text += lineString + "\r\n";
    }

    public void Send()
    {
        if (Chatting.text != "")
        {
            chatClient.PublishMessage(currentChannelName, Chatting.text);
        }
        Chatting.text = "";
    }
    

    public void PasswordInspection()
    {
        if ((string)roomlist[multiple + passwordindex].CustomProperties["p"] == PasswordInput.text)
        {
            PhotonNetwork.JoinRoom(roomlist[multiple + passwordindex].Name);
        }
        else
        {
            PasswordInput.text = "";
        }
    }

    public void RoomlistClick(int num)
    {
        if (num == -2)
        {
            c_page--;
        }
        else if (num == -1)
        {
            c_page++;
        }
        else
        {
            if ((string)roomlist[multiple + num].CustomProperties["p"] != "NotLocked")
            {
                PasswordInspect.SetActive(true);
                passwordindex = num;
            }
            else
            {
                PhotonNetwork.JoinRoom(roomlist[multiple + num].Name);
                return;
            }
        }
        RoomlistUpdate();
    }

    void RoomlistUpdate()
    {
        m_page = (roomlist.Count % Room.Length == 0) ? roomlist.Count / Room.Length : roomlist.Count / Room.Length + 1;

        Left.interactable = (c_page <= 1) ? false : true;
        Left.transform.GetChild(0).GetComponent<Text>().color = (c_page <= 1) ? new Color(149f / 255f, 149f / 255f, 149f / 255f, 160f / 255f) : new Color(149f / 255f, 149f / 255f, 149f / 255f, 255f / 255f);
        Right.interactable = (c_page >= m_page) ? false : true;
        Right.transform.GetChild(0).GetComponent<Text>().color = (c_page >= m_page) ? new Color(149f / 255f, 149f / 255f, 149f / 255f, 160f / 255f) : new Color(149f / 255f, 149f / 255f, 149f / 255f, 255f / 255f);

        multiple = (c_page - 1) * Room.Length;
        for (int i = 0; i < Room.Length; i++)
        {
            Room[i].interactable = (multiple + i < roomlist.Count) ? true : false;
            Room[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < roomlist.Count) ? roomlist[multiple + i].Name : "";
            Room[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < roomlist.Count) ? roomlist[multiple + i].PlayerCount + "/" + roomlist[multiple + i].MaxPlayers : "";
            Room[i].transform.GetChild(1).GetComponent<Text>().color = (multiple + i < roomlist.Count && roomlist[multiple + i].PlayerCount < roomlist[multiple + i].MaxPlayers) ? new Color(151f / 255f, 250f / 255f, 138f / 255f, 255f / 255f) : new Color(255f / 255f, 131f / 255f, 129f / 255f, 255f / 255f);
            Room[i].transform.GetChild(2).GetComponent<Button>().interactable = (multiple + i < roomlist.Count && roomlist[multiple + i].PlayerCount < roomlist[multiple + i].MaxPlayers) ? true : false;
            Room[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().color = (multiple + i < roomlist.Count && roomlist[multiple + i].PlayerCount < roomlist[multiple + i].MaxPlayers) ? new Color(204f / 255f, 204f / 255f, 204f / 255f, 255f / 255f) : new Color(204f / 255f, 204f / 255f, 204f / 255f, 160f / 255f);
        }
    }

    public void CreateRoom()
    {
        PlayerPrefs.SetInt("time", (30 + (int)(30 * times.GetComponent<Slider>().value)));
        PlayerPrefs.Save();
        if (Roomname.text == "")
        {
            if (Secret.isOn && Password.text != "")
            {
                PhotonNetwork.CreateRoom(roomName: "Room " + rand.Next(1, 1000), new RoomOptions { MaxPlayers = 2, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "p", Password.text } }, CustomRoomPropertiesForLobby = new string[] { "p" } });
            }
            else
            {
                PhotonNetwork.CreateRoom(roomName: "Room " + rand.Next(1, 1000), new RoomOptions { MaxPlayers = 2, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "p", "NotLocked" } }, CustomRoomPropertiesForLobby = new string[] { "p" } });
            }
        }
        else
        {
            if (Secret.isOn && Password.text != "")
            {
                PhotonNetwork.CreateRoom(roomName: Roomname.text, new RoomOptions { MaxPlayers = 2, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "p", Password.text } }, CustomRoomPropertiesForLobby = new string[] { "p" } });
            }
            else
            {
                PhotonNetwork.CreateRoom(roomName: Roomname.text, new RoomOptions { MaxPlayers = 2, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "p", "NotLocked" } }, CustomRoomPropertiesForLobby = new string[] { "p" } });
            }
        }
    }

    public void Backspace(int num)
    {
        if (num == 0)
        {
            if (Createroom.activeInHierarchy)
            {
                Createroom.SetActive(false);
                return;
            }
            if (PasswordInspect.activeInHierarchy)
            {
                PasswordInspect.SetActive(false);
                return;
            }
            PhotonNetwork.Disconnect();
        }
        else if (num == 1)
        {
            Createroom.SetActive(false);
        }
        else if (num == 2)
        {
            Createroom.SetActive(true);
        }
        else
        {
            PasswordInspect.SetActive(false);
        }
    }

    public void GoogleLogin()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Logoutbutton.SetActive(true);
                Loginbutton.SetActive(false);
                login.gameObject.SetActive(true);
                Login.transform.GetChild(10).GetChild(7).gameObject.SetActive(true);
                Login.transform.GetChild(10).GetChild(8).gameObject.SetActive(true);
                Login.transform.GetChild(10).GetChild(9).gameObject.SetActive(true);
                username.text = Social.localUser.userName;
                if (PlayerPrefs.HasKey("Username"))
                {
                    username.text = PlayerPrefs.GetString("Username");
                }
                float win = PlayerPrefs.GetInt("win");
                float draw = PlayerPrefs.GetInt("draw");
                float lose = PlayerPrefs.GetInt("lose");
                int m_score = PlayerPrefs.GetInt("m_score");
                float winrate;
                if (win + draw + lose != 0)
                {
                    winrate = win / (win + draw + lose) * 100;
                }
                else
                {
                    winrate = 0;
                }
                Winrate.text = (int)win + "승 " + (int)draw + "무 " + (int)lose + "패\r\n(" + (int)winrate + "%)\r\n\r\n최고점수 : " + m_score + "점";
            }
            else
            {
                Logoutbutton.SetActive(false);
                Loginbutton.SetActive(true);
                login.gameObject.SetActive(false);
                Login.transform.GetChild(10).GetChild(7).gameObject.SetActive(false);
                Login.transform.GetChild(10).GetChild(8).gameObject.SetActive(false);
                Login.transform.GetChild(10).GetChild(9).gameObject.SetActive(false);
                username.text = "정보 없음";
                Winrate.text = "0승 0무 0패\r\n(0%)\r\n\r\n최고점수 : 0점";
            }
        });
    }

    public void GoogleLogout()
    {
        //((PlayGamesPlatform)Social.Active).SignOut();
        Logoutbutton.SetActive(false);
        Loginbutton.SetActive(true);
        login.gameObject.SetActive(false);
        Login.transform.GetChild(10).GetChild(7).gameObject.SetActive(false);
        Login.transform.GetChild(10).GetChild(8).gameObject.SetActive(false);
        Login.transform.GetChild(10).GetChild(9).gameObject.SetActive(false);
        username.text = "정보 없음";
        Winrate.text = "0승 0무 0패\r\n(0%)\r\n\r\n최고점수 : 0점";
    }

    public void ShowAchievementUI() => Social.ShowAchievementsUI();

    public void ShowLeaderboardUI() => Social.ShowLeaderboardUI();

    public void NameChangeOn()
    {
        Namechange.SetActive(true);
        Namechange.transform.GetChild(2).GetComponent<InputField>().text = username.text;
    }

    public void ChangeName()
    {
        if (Namechange.transform.GetChild(2).GetComponent<InputField>().text == "정보 없음")
        {
            return;
        }
        PlayerPrefs.SetString("Username", Namechange.transform.GetChild(2).GetComponent<InputField>().text);
        PlayerPrefs.Save();
        username.text = Namechange.transform.GetChild(2).GetComponent<InputField>().text;
        Namechange.SetActive(false);
    }

    void Awake()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        GoogleLogin();
        if (PlayerPrefs.HasKey("name"))
        {
            PlayerPrefs.DeleteKey("name");
        }
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        Chat.text = "";
        currentChannelName = "Lobby";
        chatClient = new ChatClient(this);
        chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.AuthValues = new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chatClient.ConnectUsingSettings(chatAppSettings);

        rand = new System.Random();

        AddLeaderboard_2();

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            login.interactable = false;
            connection.text = "Connecting to server...";
        }
        else
        {
            Login.SetActive(false);
            Lobby.SetActive(true);
            RoomlistUpdate();
            gameexit = 1;
            PhotonNetwork.Disconnect();
            chatClient.Disconnect();
        }

        if (!PlayerPrefs.HasKey("dice"))
        {
            PlayerPrefs.SetInt("dice", 0);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("ad_count"))
        {
            PlayerPrefs.SetInt("ad_count", 0);
        }

        if (PlayerPrefs.HasKey("win") && !Social.Active.localUser.authenticated)
        {
            float win = PlayerPrefs.GetInt("win");
            float draw = PlayerPrefs.GetInt("draw");
            float lose = PlayerPrefs.GetInt("lose");
            int m_score = PlayerPrefs.GetInt("m_score");
            float winrate;
            if (win + draw + lose != 0)
            {
                winrate = win / (win + draw + lose) * 100;
            }
            else
            {
                winrate = 0;
            }
            Winrate.text = (int)win + "승 " + (int)draw + "무 " + (int)lose + "패\r\n(" + (int)winrate + "%)\r\n\r\n최고점수 : " + m_score + "점";
        }
        else if (!PlayerPrefs.HasKey("win"))
        {
            Winrate.text = "0승 0무 0패\r\n(0%)\r\n\r\n최고점수 : 0점";
            PlayerPrefs.SetInt("win", 0);
            PlayerPrefs.SetInt("draw", 0);
            PlayerPrefs.SetInt("lose", 0);
            PlayerPrefs.SetInt("m_score", 0);
            PlayerPrefs.Save();
        }
        else
        {
            Winrate.text = "0승 0무 0패\r\n(0%)\r\n\r\n최고점수 : 0점";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.InLobby)
        {
            Backspace(0);
        }

        if (Input.GetKeyDown(KeyCode.Return) && PhotonNetwork.InLobby && Chatting.text != "")
        {
            Send();
        }

        if (PhotonNetwork.InLobby)
        {
            Countofplayer.text = "접속 : " + PhotonNetwork.CountOfPlayers + "명";
            chatClient.Service();
        }

        if (chatClient.CanChat &&  !canchat)
        {
            chatClient.Subscribe(new string[] { currentChannelName });
            Chat.text = "";
            canchat = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        if (gameexit == 0)
        {
            login.interactable = true;
            connection.text = "Online";
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (gameexit == 0)
        {
            chatClient.Unsubscribe(new string[] { currentChannelName });
            canchat = false;
            Chat.text = "";
            Login.SetActive(true);
            Lobby.SetActive(false);
            login.interactable = false;
            connection.text = "Offline - Try reconnecting...";

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            chatClient.Connect("87dfe789-558e-4ad7-a014-7b29de7f8a28", gameVersion, new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            chatClient.Connect("87dfe789-558e-4ad7-a014-7b29de7f8a28", gameVersion, new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
        }
    }

    public void Connect()
    {
        login.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            if (Social.Active.localUser.authenticated)
            {
                PhotonNetwork.LocalPlayer.NickName = username.text;
            }
            else
            {
                return;
            }
            connection.text = "Connecting to Lobby...";
            PhotonNetwork.JoinLobby();
            
            chatClient.Connect("87dfe789-558e-4ad7-a014-7b29de7f8a28", gameVersion, new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
        }
        else
        {
            connection.text = "Offline - Try reconnecting...";

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    
    public override void OnJoinedLobby()
    {
        if (Login.activeInHierarchy)
        {
            Login.SetActive(false);
        }
        if (!Lobby.activeInHierarchy)
        {
            Lobby.SetActive(true);
        }
        roomlist.Clear();
        if (gameexit == 1) gameexit = 0;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!roomlist.Contains(roomList[i])) roomlist.Add(roomList[i]);
                else roomlist[roomlist.IndexOf(roomList[i])] = roomList[i];
            }
            else if (roomlist.IndexOf(roomList[i]) != -1) roomlist.RemoveAt(roomlist.IndexOf(roomList[i]));
        }
        RoomlistUpdate();
    }

    public void JoinFast()
    {
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable { { "p", "NotLocked" } }, expectedMaxPlayers: 2);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(roomName: "Room " + rand.Next(1, 1000), new RoomOptions { MaxPlayers = 2, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "p", "NotLocked" } }, CustomRoomPropertiesForLobby = new string[] { "p" } });
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(roomName: "Room " + rand.Next(1, 1000), new RoomOptions { MaxPlayers = 2, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "p", "NotLocked" } }, CustomRoomPropertiesForLobby = new string[] { "p" } });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers == 2)
        {
            PhotonNetwork.LoadLevel("Main");
        }
        else
        {
            PhotonNetwork.LoadLevel("Single");
        }
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        return;
    }

    public void OnDisconnected()
    {
        return;
    }

    public void OnChatStateChange(ChatState state)
    {
        return;
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        return;
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        return;
    }

    public void OnUnsubscribed(string[] channels)
    {
        return;
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        return;
    }

    public void OnUserSubscribed(string channel, string user)
    {
        return;
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        return;
    }

    [PunRPC]
    void RPCUpdateTime(string time)
    {
        return;
    }
}
