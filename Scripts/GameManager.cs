using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool m_Debug;
    public bool atLeastOneDonater = false;
    public GameObject m_Enter;
    public InputField m_inputIndex;

    [Space(20)]
    public AudioSource m_Audiosource;
    public Animator m_Curtain;
    public Animator m_HallLight;
    public GameObject m_MirrorBall;
    public GameObject m_CopyRight;
    public Transform m_StaffRoll;
    public float m_StaffRollSpeed = 1.0f;
    public int m_DonationMin = 1000;
    public int m_DonateChat = 1000;
    public int m_DonateIsabella = 2000;

    [Space(20)]
    public UserController[] m_Prefab;
    public UserController m_IsabellePrefab;
    
    public Text m_TextPrefab;
    public Transform m_TextHolder;
    public bool m_IsLookAt;
    public SpawnPoint[] spawnPoints;
    public Transform spawnIsabelle;

    [Space(20)]
    public float m_NeckLerpTime = 2.0f;
    public float m_DelayTime = 5.0f;

    public List<Transform> m_UserTexts;
    public List<UserController> m_UserList;
    public List<NickChat> m_NickChatList;

    public Dictionary<string, csvReader.UserData> Data;
    public List<csvReader.UserData> ViewData;

    private void Awake()
    {
        instance = this;
        //StartCoroutine(CorTest());
        ViewData = new List<csvReader.UserData>();
    }

    IEnumerator CorTest()
    {
        yield return null;
        //Task.Run(() => Tester());
    }

    // Start is called before the first frame update
    void Start()
    {
        //for(int i = 0; i < spawnPoints.Length; i++)
        //{
        //    GameObject cache = Instantiate(m_Prefab[0].gameObject, spawnPoints[i].transform.GetChild(0));
        //    spawnPoints[i].m_IsLookAtOn = Random.value > 0.5f ? true : false;
        //    cache.transform.localPosition = Vector3.zero;
        //    cache.transform.localEulerAngles = Vector3.zero;
        //    spawnPoints[i].SetLookAt();

        //    cache.SetActive(true);
        //}
        m_UserList = new List<UserController>();
        m_NickChatList = new List<NickChat>();
        StartCoroutine(CorDelayStart());
    }

    IEnumerator CorDelayStart()
    {
        while (true)
        {
            if (csvReader.Instance.isInitFinish)
            {
                Data = csvReader.Instance.GetData();
                if (Data == null || Data.Count == 0)
                {
                    StartCoroutine(CorWaitEnter());
                    yield break;
                }
                //SetUsers();
                int idx = 0;
                foreach (var p in Data)
                {
                    //GDebug.LogWarning(p.Value.nickname);
                    StartCoroutine(CorChangeAnony(p.Key, p.Value, idx == Data.Count - 1));
                    idx++;
                }
                //StartCoroutine(CorWaitForRealnickEnd());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator CorChangeAnony(string _userId, csvReader.UserData _userDat, bool _isEnd = false)
    {
        yield return null;
        var wait = Task.Run(() => csvReader.Instance.ReplaceAnonymousToRealnick(_userId, _userDat, _isEnd));
        yield return wait;

        while (wait.IsCompleted)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        if (_isEnd)
        {
            csvReader.Instance.isNickChangeFinish = true;
            SetUsers();
        }
    }


    IEnumerator CorWaitForRealnickEnd()
    {
        while (true)
        {
            if (csvReader.Instance.isNickChangeFinish)
            {
                SetUsers();
                yield break;
            }
            yield return null;
        }
    }

    public void SetUsers()
    {
        if (m_Debug)
        {
            for (int k = 0; k < this.spawnPoints.Length - 6; k++)
            {
                var newData = new csvReader.UserData();
                newData.comment = "Test Message";
                newData.cash = 2000;
                newData.nickname = "Super Tester" + (2+k).ToString();
                newData.dona_one = 1444;
                newData.dona_two = 1555;
                newData.id = "gros" + (2+k).ToString();
                Data.Add(newData.id, newData);
                //ToonData.Add("Tester", newData);
                //TwipData.Add("Tester", newData);
            }
        }

        foreach (var userDat in Data)
        {
            GDebug.LogFormat("{0} : {1}, {2}, {3}, {4}, {5}",
                            userDat.Key, userDat.Value.nickname.Replace("\"", ""), userDat.Value.cash, userDat.Value.comment,
                            userDat.Value.dona_one, userDat.Value.dona_two);
        }
        //Debug.Log("Count : " + Data.Count);
        List<SpawnPoint> spawnPoints = new List<SpawnPoint>(this.spawnPoints);

        //for (int i = 0; i < Data.Count; i++)
        int i = 0;

        bool setIsabelle = false;
        // 미리 생성 대상을 세팅

        var FilterData = new Dictionary<string, csvReader.UserData>();
        var filterDataNew = new Dictionary<string, csvReader.UserData>();

        // 캐시 미충족 필터링
        // 이자벨라 조건 필터링
        bool isIsabellable = false;
        foreach(var userDat in Data)
        {
            if (userDat.Value.cash >= m_DonationMin)
            {
                Debug.Log("ADD " + userDat.Key);
                FilterData.Add(userDat.Key, userDat.Value);
            }
            if (userDat.Value.cash >= m_DonateIsabella)
                isIsabellable = true;
        }

        int book = 0;
        int pointCount = isIsabellable ? spawnPoints.Count : spawnPoints.Count;

        // 좌석 할당
        while(book < pointCount)
        {
            if (book >= pointCount || book >= FilterData.Count) break;
            foreach (var userDat in FilterData)
            {
                if (book >= pointCount || book >= FilterData.Count) break;

                if (Random.Range(0, 2) == 0 && !userDat.Value.haveCreateAuth)
                {
                    userDat.Value.haveCreateAuth = true;
                    book++;
                    ViewData.Add(userDat.Value);
                    filterDataNew.Add(userDat.Key, userDat.Value);
                }
            }
        }

        if (isIsabellable)
        {
            //while (!setIsabelle)
            //{
                foreach (var item in filterDataNew)
                {
                    if (item.Value.cash >= m_DonateIsabella && Random.Range(0,100) < 25)
                    {
                        item.Value.haveCreateAuthIsabelle = true;
                        setIsabelle = true;
                        break;
                    }
                }
            //}
        }

        // 캐셔 리스트 조건에 맞는 사람만큼 생성
        foreach (var UserDat in filterDataNew)
        {
            //GDebug.Log(UserDat.Key);
            if (UserDat.Value.cash >= m_DonationMin)
            {
                // 0-3 2000P 미만
                // 0-6 2000P 이상 3000P 미만
                // 4-6 3000P 이상 4000P 미만
                // 7 5000P 이상
                // 2000P 이상일 때 이자벨라를 1명 생성가능
                // 이미 이자벨라가 생성 중이라면 생성하지 않음
                // 생성확률 25%

                //int userIdx = Random.Range(0, m_Prefab.Length);
                int userIdx = 0;

                int userDonate = UserDat.Value.cash;
                if (userDonate < 2000) userIdx = Random.Range(0, 3);
                else if (userDonate < 3000) userIdx = Random.Range(0, 7);
                else if (userDonate < 4000) userIdx = Random.Range(4, 7);
                else userIdx = 7;

                int idx = 0;

                GameObject cache;
                UserController cacheUser;

                // 이자벨라 생성
                if (UserDat.Value.haveCreateAuthIsabelle)
                {
                    cache = Instantiate(m_IsabellePrefab.gameObject, spawnIsabelle);
                }
                else
                {
                    idx = Random.Range(0, spawnPoints.Count);
                    cache = Instantiate(m_Prefab[userIdx].gameObject, spawnPoints[idx].transform.GetChild(0));
                }
                cacheUser = cache.GetComponent<UserController>();

                GameObject cacheText = Instantiate(m_TextPrefab.gameObject, m_TextHolder);
                var cacheTextScript = cacheText.GetComponent<Text>();
                cacheText.transform.position = Vector3.zero;
                //if (cacheText == null) GDebug.LogError("NULLLLLL");

                cacheTextScript.text = UserDat.Value.nickname;

                if (!UserDat.Value.haveCreateAuthIsabelle)
                    spawnPoints[idx].m_IsLookAtOn = Random.value > 0.5f ? true : false;
                cache.transform.localPosition = Vector3.zero;
                cache.transform.localEulerAngles = Vector3.zero;
                m_IsLookAt = spawnPoints[idx].SetLookAt();

                cacheUser.SetUser(cacheText.transform, spawnPoints[idx]);

                if (!UserDat.Value.haveCreateAuthIsabelle)
                {
                    spawnPoints.Remove(spawnPoints[idx]);
                }

                m_UserList.Add(cacheUser);
                m_UserTexts.Add(cacheText.transform);

                var nickChatScript = cacheTextScript.GetComponent<NickChat>();
                nickChatScript.m_UserData = UserDat.Value;

                m_NickChatList.Add(nickChatScript);
                i++;

                atLeastOneDonater = true;
            }
        }

        // 모든 데이터 생성이 끝남
        // 입력 대기중
        StartCoroutine(CorWaitEnter());
    }

    IEnumerator CorWaitEnter()
    {
        m_Enter.SetActive(true);
        while (true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (!string.IsNullOrEmpty(m_inputIndex.text) && csvReader.Instance.m_UserLists.Count > 0)
                {
                    if (int.Parse(m_inputIndex.text) == 9999 || Random.value > 0.9f)
                    {
                        yield return new WaitForSeconds(2.5f);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("8BitStyle");
                        yield break;
                    }
                    else
                    {
                        m_Enter.SetActive(false);
                        ActiveUpdater();
                        yield break;
                    }
                }
                else
                {
                    m_Enter.SetActive(false);
                    ActiveUpdater();
                    yield break;
                }
            }
            yield return null;
        }
    }

    public void ActiveUpdater()
    {
        if (atLeastOneDonater)
        {
            SetCurtain(true);
            StartCoroutine(TK.instance.DelayAct(1.5f, () => 
            {
                StartCoroutine(CorActiveUpdaters());
                m_HallLight.Play("HallLight");
            }));

        }
        else
        {
            SetCurtain(true);
            StartCoroutine(TK.instance.DelayAct(3.0f, () =>
            {
            m_Audiosource.Play();
            SetCurtain(false);

                StartCoroutine(TK.instance.DelayAct(2.0f,() => 
                {
                    csvReader.Instance.m_ButtonsGroup.SetActive(true);
                }));
            }));
            //GDebug.LogWarning("T_T No Villager");
        }
    }

    public void SetCurtain(bool active)
    {
        //m_Curtain.gameObject.SetActive(active);
        if (active)
        {
            m_Curtain.Play("Open");
        }
        else
        {
            m_Curtain.Play("Close");
        }
    }

    public void ActiveMirrorBall(bool _isActive)
    {
        m_MirrorBall.SetActive(_isActive);
    }
    public void ActiveCopyRight(bool _isActive)
    {
        m_CopyRight.SetActive(_isActive);
    }

    IEnumerator CorActiveUpdaters()
    {
        yield return new WaitForEndOfFrame();
        //for (int i = 0; i < spawnPoints.Length; i++) spawnPoints[i].SetLookAt();
        //Debug.LogWarning("삽입시작");
        for (int i = 0; i < m_UserList.Count; i++)
        {
            m_UserList[i].SetUser(m_UserTexts[i]);
            m_UserList[i].StartUpdater(Random.Range(1.0f, m_NeckLerpTime), Random.Range(1.0f, m_DelayTime));
        }

        StartCoroutine(CorActiveStaffRoll(TK.instance.m_AudioSource.clip.length));
        TK.instance.PlayTKSong();
        TK.instance.SingASong();
    }

    public void ActiveStaffRoll()
    {
        StartCoroutine(CorActiveStaffRoll(TK.instance.m_AudioSource.clip.length));
    }

    IEnumerator CorActiveStaffRoll(float _time)
    {
        var wait = new WaitForFixedUpdate();
        float timer = 0;
        while (timer < _time)
        {
            m_StaffRoll.Translate(0, Time.deltaTime * m_StaffRollSpeed, 0, Space.Self);
            yield return Time.deltaTime;   
        }
    }

    public void ActiveEndCard()
    {
        m_StaffRoll.GetComponent<Animator>().Play("StaffRoll");
    }

    public void ActiveNickChat(string _id, string _message)
    {
        for(int i = 0; i < m_NickChatList.Count; i++)
        {
            var cache = m_NickChatList[i].m_UserData;
            if (cache.nickname.Equals(_id))
            //if (m_NickChatList[i].m_UserName.text.Equals(_nickName))
            {
                // 캐시가 천P보다 적으면 채팅잠금
                if (cache.cash < m_DonateChat) return;
                m_NickChatList[i].ActiveChat(_message);
                return;
            }
        }
        //GDebug.Log("There is no Users");
    }
}
