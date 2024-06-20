// If type or namespace TwitchLib could not be found. Make sure you add the latest TwitchLib.Unity.dll to your project folder
// Download it here: https://github.com/TwitchLib/TwitchLib.Unity/releases
// Or download the repository at https://github.com/TwitchLib/TwitchLib.Unity, build it, and copy the TwitchLib.Unity.dll from the output directory
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Api;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public static class Secrets
{
    public const string CLIENT_ID = ""; //Your application's client ID, register one at https://dev.twitch.tv/dashboard
    public const string OAUTH_TOKEN = ""; //A Twitch OAuth token which can be used to connect to the chat
    public const string USERNAME_FROM_OAUTH_TOKEN = ""; //The username which was used to generate the OAuth token
    public const string CHANNEL_ID_FROM_OAUTH_TOKEN = ""; //The channel Id from the account which was used to generate the OAuth token
}

public class TwitchClient : MonoBehaviour
{
    public static TwitchClient instance;

    [SerializeField] //[SerializeField] Allows the private field to show up in Unity's inspector. Way better than just making it public
    private string _channelToConnectTo = Secrets.USERNAME_FROM_OAUTH_TOKEN;

    private Client _client;
    private Api _api;

    public Api GetApi()
    {
        return _api;
    }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
        // Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
        // This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
        // Application.runInBackground = true;

        //Create Credentials instance
        ConnectionCredentials credentials = new ConnectionCredentials(Secrets.USERNAME_FROM_OAUTH_TOKEN, Secrets.OAUTH_TOKEN);

        // Create new instance of Chat Client
        _client = new Client();
        _api = new Api();

        _api.Settings.ClientId = Secrets.CLIENT_ID;
        _api.Settings.AccessToken = Secrets.OAUTH_TOKEN;


        // Initialize the client with the credentials instance, and setting a default channel to connect to.
        _client.Initialize(credentials, _channelToConnectTo);
        
        // Bind callbacks to events
        _client.OnConnected += OnConnected;
        _client.OnJoinedChannel += OnJoinedChannel;
        _client.OnMessageReceived += OnMessageReceived;
        _client.OnChatCommandReceived += OnChatCommandReceived;
        _client.OnUserJoined += OnUserJoined;

        // Connect
        _client.Connect();

        //StartCoroutine(CorTest());
    }

    private void _client_OnUserJoined(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
    {
        Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");

        if (!string.IsNullOrWhiteSpace(e.AutoJoinChannel))
            Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
    }

    private void OnUserJoined(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
    {
        TwitchLib.Api.V5.Models.Users.Users users;
        Debug.Log($"The bot {e.Username} succesfully connected to Twitch.");
        
        if (!string.IsNullOrWhiteSpace(e.Channel))
            Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.Channel}");
    }

    private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
    {
        Debug.Log($"The bot {e.BotUsername} just joined the channel: {e.Channel}");
        //_client.SendMessage(e.Channel, "I just joined the channel! PogChamp");
    }
    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        Debug.Log($"Message received from {e.ChatMessage.Username} : {e.ChatMessage.DisplayName} : {e.ChatMessage.Message}");
        if (GameManager.instance != null)
            GameManager.instance.ActiveNickChat(e.ChatMessage.DisplayName, e.ChatMessage.Message);
        if (P_GameManager.Instance != null)
        {
            Debug.LogError("HERE");
            P_GameManager.Instance.Chat(e.ChatMessage.Username, e.ChatMessage.Message);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("ON!!!");
            P_GameManager.Instance.Chat("ddangol", "!하");
        }
    }
#endif
    public void SendMsg(string _msg)
    {
        _client.SendMessage(_channelToConnectTo, _msg);
    }

    private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
    {
        //switch (e.Command.CommandText)
        //{
        //    case "hello":
        //        _client.SendMessage(e.Command.ChatMessage.Channel, $"Hello {e.Command.ChatMessage.DisplayName}!");
        //        break;
        //    case "about":
        //        _client.SendMessage(e.Command.ChatMessage.Channel, "I am a Twitch bot running on TwitchLib!");
        //        break;
        //    default:
        //        _client.SendMessage(e.Command.ChatMessage.Channel, $"Unknown chat command: {e.Command.CommandIdentifier}{e.Command.CommandText}");
        //        break;
        //}
    }

    private async System.Threading.Tasks.Task UpdateAsync()
    //private void Update()
    {
        // Don't call the client send message on every Update,
        // this is sample on how to call the client,
        // not an example on how to code.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _client.SendMessage(_channelToConnectTo, "I pressed the space key within Unity.");
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            TwitchLib.Api.V5.Models.Users.Users users = new TwitchLib.Api.V5.Models.Users.Users();
            foreach (var p in users.Matches)
            {
                Debug.Log(p.Name + " // " + p.DisplayName + " // " + p.Id + " // " + p.Logo);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _api.Invoke(
                _api.Undocumented.GetChattersAsync(_client.JoinedChannels[0].Channel),
                GetChatterListCallback
                );
        }
    }

    private void GetChatterListCallback(List<ChatterFormatted> obj)
    {
        foreach (var chatterObject in obj)
        {
            Debug.Log(chatterObject.Username + " // " + chatterObject.UserType.ToString());
        }
    }
}