using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class P_GameManager : MonoBehaviour
{
    public static P_GameManager Instance;

    [System.Serializable]
    public class Reward_Character
    {
        public int maxVal = 1000;
        public List<Player_Pixel> charList;

        public Player_Pixel GetOneChar()
        {
            return charList[Random.Range(0, charList.Count)];
        }
    }

    [Header("- Spawn Pos")]
    public Vector3 m_LeftDown;
    public Vector3 m_RightTop;

    [Header("- Music")]
    public AudioSource m_Audio;
    public List<AudioClip> m_AudioClips;

    [Header("- Character Data")]
    public List<Reward_Character> m_CharList;
    public List<Vector2> m_SpawnPosList;


    [Header("- List")]
    public List<Player_Pixel> newCharList = new List<Player_Pixel>();
    public Dictionary<string, csvReader.UserData> userList = new Dictionary<string, csvReader.UserData>();

    [Header("- Staff Roll")]
    public GameObject m_StaffRoll;
    public Text m_StaffRollText;
    public Animator m_EndRoll;
    [TextArea]
    public string m_StaffRollData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        Player_Pixel.g_PlayBlock = true;
        m_StaffRollList = m_StaffRollData.Split('\n');
        m_CoStaffRoll = StartCoroutine(CorStaffRoll(m_StaffRollList));
        StartCoroutine(CorMusicChecker());
        CreateCharacters();
    }
    public string[] m_StaffRollList;

    Coroutine m_CoStaffRoll = null;
    IEnumerator CorStaffRoll(string[] _msgList, System.Action _act = null)
    {

        var front_wait = new WaitForSeconds(1.9f);

        yield return new WaitUntil(() => Player_Pixel.g_PlayBlock == false);
        yield return new WaitForSeconds(1.5f);

        m_StaffRoll.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        m_StaffRollText.gameObject.SetActive(true);

        foreach (var item in _msgList)
        {
            m_StaffRollText.text = item;
            yield return front_wait;
        }

        yield return new WaitForSeconds(0.25f);
        m_StaffRoll.SetActive(false);
        m_CoStaffRoll = null;

        m_StaffRollText.text = string.Empty;

        _act?.Invoke();
    }

    IEnumerator CorMusicChecker()
    {
        yield return new WaitForSeconds(2.0f);
        yield return new WaitUntil(() => Player_Pixel.g_PlayBlock == false);

        m_Audio.clip = m_AudioClips[Random.Range(0, m_AudioClips.Count)];
        m_Audio.Play();
        TwitchClient.instance.SendMsg($"명령어 : !상(향,2~4), !하(향,2~4), !우(향,2~4), !좌(향,2~4), !물음표, !느낌표, !웃음, !ㅋ, !슬픔, !ㅂㅂ, !애교");
        yield return new WaitForSeconds(m_Audio.clip.length);

        m_Audio.clip = m_AudioClips[Random.Range(0, m_AudioClips.Count)];
        m_Audio.Play();

        yield return new WaitUntil(() => m_CoStaffRoll == null);

        StartCoroutine(CorStaffRoll(new string[6] { "Year - Year", "Year - Year", "Year - Year", "Name", "Name", "Name" }, () => 
        {
            StartCoroutine(CorEndGame());
        }));
    }

    IEnumerator CorEndGame()
    {
        m_EndRoll.gameObject.SetActive(true);

        Camera cam = Camera.main;
        // 13.09

        float orig_size = cam.orthographicSize;

        float time = 0;
        while(time < 2.5f)
        {
            time += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(orig_size, 13.09f, time / 2.5f);
            yield return null;
        }

        Player_Pixel.g_PlayBlock = true;

        m_EndRoll.Play("Off");
        m_Audio.Stop();
    }

    List<Player_Pixel> PeekCharacter(int _maxChar)
    {
        Dictionary<string, csvReader.UserData> data = new Dictionary<string, csvReader.UserData>(csvReader.Instance.GetData());

        Debug.Log(data.Count);
        int cc = 0;
        int created = 0;


        List<csvReader.UserData> pickedDat = data.Values.ToList();

        if (pickedDat.Count > m_SpawnPosList.Count)
        {
            while (pickedDat.Count > m_SpawnPosList.Count)
            {
                pickedDat.RemoveAt(Random.Range(0, pickedDat.Count));
            }
        }


        foreach (var item in pickedDat)
        {
            int prevVal = 99;
            foreach (var condition in m_CharList)
            {
                if (prevVal <= item.cash && item.cash < condition.maxVal)
                {
                    {
                        var newItem = Instantiate(condition.GetOneChar());
                        newItem.m_userdat = item;

                        if (userList.ContainsKey(item.id))
                        {
                            userList.Add(item.id + Random.Range(0,1000000), item);
                        }
                        else
                        {
                            userList.Add(item.id, item);
                        }
                        newCharList.Add(newItem);
                        created++;
                        break;
                    }
                }
                prevVal = condition.maxVal;
            }
        }

        return newCharList;
    }

    void CreateCharacters()
    {
        List<GameObject> createdChar = new List<GameObject>();
        List<Player_Pixel> charList = new List<Player_Pixel>(PeekCharacter(0));
        List<Vector2> spawnVec = new List<Vector2>(m_SpawnPosList);

        int m_LayerMask =
        (1 << LayerMask.NameToLayer("Character")) |
        (1 << LayerMask.NameToLayer("Map")) |
        (1 << LayerMask.NameToLayer("Tile Booker"));


        int cc = 0;
        while(true)
        { 
            if (charList.Count == 0 || spawnVec.Count == 0)
                break;

            var selected = charList[Random.Range(0, charList.Count)];
            var spawnPos = spawnVec[Random.Range(0, spawnVec.Count)];

            Ray2D ray = new Ray2D(spawnPos, Vector3.forward);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 10f);

            var hit = Physics2D.Raycast(ray.origin, ray.direction, 100f, m_LayerMask);
            if (hit.transform != null)
            {
            }
            else
            {
                // Create Chance
                int chance = Random.Range(0, 100);
                if (chance < 10)
                {
                    //var newChar = Instantiate(selected, spawnPos, Quaternion.identity);
                    //newCharList.Add(newChar);\
                    selected.transform.position = spawnPos;
                    charList.Remove(selected);
                    spawnVec.Remove(spawnPos);
                }
            }
        }
    }

    public void Chat(string _userName, string _msg)
    {
        var temp = newCharList.Find(obj => obj.m_userdat.id.Equals(_userName));
        if (temp != null)
        {
            Debug.Log("FIND");
            temp.Chat(_msg);
        }
    }
}
