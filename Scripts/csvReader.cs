using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

public class csvReader : MonoBehaviour
{
    public static csvReader Instance;

    [System.Serializable]
    public class UserData
    {
        public string id = string.Empty;
        public string nickname = string.Empty;
        public int cash = 0;
        public string comment = string.Empty;
        public int dona_one = 0;
        public int dona_two = 0;

        public bool haveCreateAuth = false;
        public bool haveCreateAuthIsabelle = false;
    }
    [Space(20)]
    public InputField m_inputDate;
    public InputField m_inputIndex;
    public GameObject m_ButtonsGroup;

    [Space(20)]
    public string m_CurrTime;
    public bool m_SetToday = false;
    public bool m_SetTwo = false;
    [Space(20)]
    public string year;
    [Range(1, 12)]
    public int month;
    [Range(1, 31)]
    public int day;

    public Dictionary<string, UserData> m_Dona_One_UserLists;
    public Dictionary<string, UserData> m_Dona_Two_UserLists;
    public Dictionary<string, UserData> m_UserLists;

    public string m_AccessToken;

    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        m_Dona_One_UserLists = new Dictionary<string, UserData>();
        m_Dona_Two_UserLists = new Dictionary<string, UserData>();
        m_UserLists = new Dictionary<string, UserData>();

        BtnActiveSetUserData(null);
    }

    public void BtnActiveSetUserData(GameObject _setDateButton)
    {
        _setDateButton?.SetActive(false);

        m_Dona_One_UserLists = new Dictionary<string, UserData>();
        m_Dona_Two_UserLists = new Dictionary<string, UserData>();
        m_UserLists = new Dictionary<string, UserData>();

        Dona_One();
    }

    // Date, Account, Nickname, Cash, Msg
    public void Dona_One()
    {
        //StreamReader sr = new StreamReader(Application.dataPath + "/" + "output.csv");
        //bool endOfFile = false;
        bool first = true;
        string data_String_Prev = string.Empty;

        string[] test_data = new string[11];

        // Insert test data
        for(int i = 0; i < test_data.Length; i++)
        {
            string ins = $"{DateTime.Today},{"AccountNum " + i},{"Nickname " + i},{UnityEngine.Random.Range(1000, 5000)},{"Test MSG"}";
            if (i == 0)
            {
                ins = "Dummy,Dummy,Dummy,Dummy,Dummy";
            }

            test_data[i] = ins;
        }

        //while (!endOfFile)
        for(int i = 0;i < test_data.Length; i++)
        {
            //string data_String = sr.ReadLine();
            string data_String = test_data[i];
            //if (data_String == null)
            //{
            //    endOfFile = true;
            //    break;
            //}

            if (!first)
            {
                //var data_values = data_String.Split(',');
                char[] sep = new char[1] { ',' };
                int counter = 5;

                string ppppp = string.Empty;
                bool force_pass = true;
                var data_values = Regex.Split(data_String, SPLIT_RE);

                //if (data_values[0].Contains(m_CurrTime))
                if (force_pass)
                {
                    if (data_values.Length == 5)
                    {
                        data_values[1] = data_values[1].Replace("\"", "");
                        string Key = data_values[1];
                        if (m_Dona_One_UserLists.ContainsKey(Key))
                        {
                            int addDonation = int.Parse(data_values[3]);
                            m_Dona_One_UserLists[Key].dona_one += addDonation;
                            m_Dona_One_UserLists[Key].cash += addDonation;

                            if (!data_values[4].Contains("video://"))
                                m_Dona_One_UserLists[Key].comment = data_values[4];
                        }
                        else
                        {
                            UserData userDat = new UserData();
                            userDat.nickname = data_values[2].Replace("\"", "");
                            userDat.dona_one = userDat.cash = int.Parse(data_values[3]);

                            if (!data_values[4].Contains("video://"))
                                userDat.comment = data_values[4];
                            else
                            {
                                userDat.comment = "No comment";
                            }

                            userDat.id = Key;

                            m_Dona_One_UserLists.Add(Key, userDat);
                        }
                        data_String_Prev = data_String;
                    }
                    else // 마지막 Msg 가 너무 길어서 강제 개행이 된 경우
                    {
                        // 이전에 정상적인 입력 작업을 한 경우
                        if (!string.IsNullOrEmpty(data_String_Prev))
                        {
                            string newDate = data_values[0].Replace("\"", "");
                            newDate = data_values[0].Replace("\'", "");
                            if (DateTime.TryParse(newDate.Replace("\"", ""), out DateTime date))
                            {
                                //Debug.Log(date);
                            }
                            else
                            {
                                //Debug.Log(date);
                                m_Dona_One_UserLists[data_String_Prev.Split(',')[1].Replace("\"", "")].comment += "\n" + data_values[0].ToString();
                            }
                            data_String_Prev = string.Empty;
                        }
                        else
                        {
                            //Debug.Log("NOT : " + data_String_Prev);
                        }
                    }
                }
                //else
                //{
                //    GDebug.LogError($"{data_values[2]}  /  {data_values[0]}  /  {currTime}  / {data_values[3]}");
                //}
            }
            else first = false;
        }

        if (m_Dona_One_UserLists.Count != 0)
        {
            m_UserLists = new Dictionary<string, UserData>(m_Dona_One_UserLists);
        }

        isInitFinish = true;
        isNickChangeFinish = true;

        //Dona_Two();

        //sr.Close();
    }

    // SKIP
    // Date, Nickname, Id, cash, msg
    public void Dona_Two()
    {
        //string currTime = DateTime.Now.ToString("yyyy-MM-dd");
        GDebug.Log(m_CurrTime);

        StreamReader sr = new StreamReader(Application.dataPath + "/" + "test.csv");
        bool endOfFile = false;
        bool first = true;
        string data_String_Prev = string.Empty;


        while (!endOfFile)
        {
            string data_String = sr.ReadLine();
            if (data_String == null)
            {
                endOfFile = true;
                break;
            }

            if (!first)
            {
                var ast = Regex.Split(data_String, SPLIT_RE);
                string ppppp = string.Empty;
                foreach (var item in ast)
                {
                    ppppp += " // " + item;
                }
                GDebug.LogError("SS : " + ppppp);

                var data_values = ast;

                if (data_values[0].Contains(m_CurrTime))
                {
                    if (data_values.Length == 5)
                    {
                        if (m_Dona_Two_UserLists.ContainsKey(data_values[2]))
                        {
                            int addDonation = int.Parse(data_values[3]);
                            m_Dona_Two_UserLists[data_values[2]].cash += addDonation;
                            m_Dona_Two_UserLists[data_values[2]].dona_two += addDonation;
                        }
                        else
                        {
                            UserData userDat = new UserData();
                            userDat.nickname = data_values[1];
                            userDat.dona_two = userDat.cash = int.Parse(data_values[3]);
                            m_Dona_Two_UserLists.Add(data_values[2], userDat);
                        }
                        data_String_Prev = data_String;
                    }
                    else // 마지막 msg가 너무 길어서 강제 개행이 된 경우
                    {
                        // 이전에 정상적인 입력 작업을 한 경우
                        if (!string.IsNullOrEmpty(data_String_Prev))
                        {
                            m_Dona_Two_UserLists[data_String_Prev.Split(',')[2]].comment += "\n" + data_values[0].ToString();
                            data_String_Prev = string.Empty;
                        }
                    }
                }
                else
                {
                    GDebug.LogWarning(data_values[0] + " // " + m_CurrTime);
                }
            }
            else first = false;
        }

        if (m_Dona_Two_UserLists.Count != 0)
        {
            int i = 0;
            foreach (var p in m_Dona_Two_UserLists)
            {
                if (i == m_Dona_Two_UserLists.Count - 1)
                {
                    StartCoroutine(CorReplaceUserData(p.Key, p.Value, true));

                }
                else
                {
                    StartCoroutine(CorReplaceUserData(p.Key, p.Value));
                }
                i++;
            }

        }
        else
        {
            isInitFinish = true;
        }

        if (m_Dona_Two_UserLists.Count == 0 && m_Dona_One_UserLists.Count == 0)
        {
            isNickChangeFinish = true;
        }
        if (m_Dona_Two_UserLists.Count == 0)
        {
            isInitFinish = true;
            isNickChangeFinish = true;
        }

        sr.Close();
    }

    IEnumerator CorReplaceUserData(string _userId, UserData _userDat, bool _isEnd = false)
    {
        yield return null;
        var wait = Task.Run(() => ReplaceUserData(_userId, _userDat, _isEnd));
        yield return wait;
        while(wait.IsCompleted)
        {
            Debug.Log(wait.IsCompleted);
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        if (_isEnd)
        {
            //GDebug.Log("Active SetUsers");
            isInitFinish = _isEnd;
        }
    }

    public bool isInitFinish = false;

    // One 데이터를 먼저 계산하고 저장한 뒤
    // Two 데이터를 One 데이터에 병합
    async Task ReplaceUserData(string _userId, UserData _userDat, bool _isEnd)
    {
        GDebug.Log(_userId + " Async Start 2");
        var user2 = await TwitchClient.instance.GetApi().Helix.Users.GetUsersAsync(new List<string>() { _userId });
        foreach (var item in user2.Users)
        {
            GDebug.Log("ReplaceUserData : " + item.Id + " // " + item.DisplayName + " // " + item.Login);
        }
        //var user = await TwitchClient.instance.GetApi().V5.Users.GetUserByIDAsync(_userId);
        if (user2 != null)
        {
            var user = user2.Users[0];
            GDebug.Log("RES : " + user.Id);
            if (m_UserLists.ContainsKey(user.Id))
            {
                var data = m_UserLists[user.Id];

                //int addDonation = _userDat.twip_donation;
                data.dona_two += _userDat.dona_two;
                data.cash += data.dona_two;
                data.comment = _userDat.comment;
            }
            else // Two 캐시O, One 캐시X
            {
                _userDat.id = user.Login;
                m_UserLists.Add(user.Login, _userDat);
            }
        }
        else
        {
            //GDebug.LogError("NULL : " + _userId);
        }
    }

    public bool isNickChangeFinish = false;
    // 익명 제거기
    // 이때는 UserId 가 다 사라져있을 것
    public async Task ReplaceAnonymousToRealnick(string _userName, UserData _userDat, bool _isEnd = false)
    {
        GDebug.Log(_userName + " Async Start" + " " + _userDat.id);
        //var user = await TwitchClient.instance.GetApi().V5.Users.GetUserByNameAsync(_userName);
        var user = await TwitchClient.instance.GetApi().Helix.Users.GetUsersAsync(null, new List<string>() { _userDat.id });
        if (user != null)
        {
            GDebug.LogWarning("Realnick : " + user.Users[0].Id);
            GDebug.LogWarning("Realnick : " + user.Users[0].DisplayName);
        }
        else
        {
            GDebug.LogError(_userName + " : null");
        }
        if (_isEnd)
        {
            GDebug.Log("Active SetUsers");
        }
    }


    public Dictionary<string, UserData> GetData()
    {
        if (m_UserLists == null) m_UserLists = new Dictionary<string, UserData>();
        return m_UserLists;
    }

    public Dictionary<string, UserData> GetToonData()
    {
        return m_Dona_One_UserLists;
    }

    public Dictionary<string, UserData> GetTwipData()
    {
        return m_Dona_Two_UserLists;
    }
}