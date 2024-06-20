using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;

public class NetClient : MonoBehaviour
{
	public static NetClient Instance;

	string clientName;

	bool socketReady;
	TcpClient socket;
	NetworkStream stream;
	StreamWriter writer;
	StreamReader reader;


    private void Awake()
    {
		if (Instance == null) Instance = this;
    }

    private void Start()
    {
		//ConnectToServer();
		StartCoroutine(CorConnectToServer());
    }


	IEnumerator CorConnectToServer()
    {
		// 기본 호스트/ 포트번호
		string ip = "127.0.0.1";
		int port = 7777;

		yield return new WaitForSeconds(0.5f);

        while (!socketReady)
        {
			// 소켓 생성
			try
			{
				socket = new TcpClient(ip, port);
				stream = socket.GetStream();
				writer = new StreamWriter(stream);
				reader = new StreamReader(stream);
				socketReady = true;
			}
			catch (Exception e)
			{
				Debug.Log($"소켓에러 : {e.Message}");
			}
			
			if (socketReady)
            {
				break;
            }
            else
            {
				yield break;
			}
		}

		Debug.Log("소켓 연결 성공함");

		//Send("테스트 데이터");
    }

    public void ConnectToServer()
	{
		// 이미 연결되었다면 함수 무시
		if (socketReady) return;

		// 기본 호스트/ 포트번호
		string ip = "127.0.0.1";
		int port = 7777;

		// 소켓 생성
		try
		{
			socket = new TcpClient(ip, port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
			socketReady = true;
		}
		catch (Exception e)
		{
			Debug.Log($"소켓에러 : {e.Message}");
		}
	}

	void Update()
	{
		if (socketReady && stream.DataAvailable)
		{
			string data = reader.ReadLine();
			if (data != null)
				OnIncomingData(data);
		}
	}

	void OnIncomingData(string data)
	{
		if (data == "%NAME")
		{
			clientName = "Guest" + UnityEngine.Random.Range(1000, 10000);
			Send($"&NAME|{clientName}");
			return;
		}

		Debug.Log(data);
	}

	public void Send(string data)
	{
		if (!socketReady) return;

		writer.WriteLine(data);
		writer.Flush();
	}

	public void OnSendButton(InputField SendInput)
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (!Input.GetButtonDown("Submit")) return;
		SendInput.ActivateInputField();
#endif
		if (SendInput.text.Trim() == "") return;

		string message = SendInput.text;
		SendInput.text = "";
		Send(message);
	}


	void OnApplicationQuit()
	{
		CloseSocket();
	}

	void CloseSocket()
	{
		if (!socketReady) return;

		writer.Close();
		reader.Close();
		socket.Close();
		socketReady = false;
	}
}