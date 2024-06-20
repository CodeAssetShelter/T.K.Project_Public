using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Pixel : MonoBehaviour
{
    public enum MOVE_STATE
    {
        IDLE,
        MOVE,
        STANDSTILL,
        ROTATE,
        STATE_COUNT
    }

    [System.Serializable]
    public class SpriteGroup
    {
        public Sprite idle;
        public Sprite move_left;
        public Sprite move_right;
    }

    public static bool g_PlayBlock = true;

    public bool m_IsBot = false;


    [Header("- Common Settings")]
    public csvReader.UserData m_userdat;
    public bool left = true;
    public MOVE_STATE m_MoveState = MOVE_STATE.IDLE;
    public GameObject m_NickBox;

    public Transform m_Image;
    public SpriteRenderer m_SprRenderer;
    public SpriteRenderer m_SprImojiRenderer;
    public GameObject m_Chat;
    public Text m_ChatText;
    public TileBooker m_TB;

    [Header("[Sprite Settings]")]
    public Sprite m_Imoji_QuestionMark;
    public Sprite m_Imoji_ExclamationMark;
    public Sprite m_Imoji_Smile;
    public Sprite m_Imoji_kk;
    public Sprite m_Imoji_Sad;
    public Sprite m_Imoji_Bye;
    public Sprite m_Imoji_Cute;

    [Space(20)]
    public SpriteGroup m_Spr_up;
    public SpriteGroup m_Spr_down;
    public SpriteGroup m_Spr_left;
    public SpriteGroup m_Spr_right;



    int m_LayerMask = -1;

    private void Awake()
    {
        m_LayerMask = 
            (1 << LayerMask.NameToLayer("Character")) |
            (1 << LayerMask.NameToLayer("Map")) |
            (1 << LayerMask.NameToLayer("Tile Booker"));

        m_TB = Instantiate(m_TB, transform.position, Quaternion.identity);
        m_TB.m_MadeBy = transform;

        Sprite spr = m_Spr_down.idle;
        switch (Random.Range(0, 3))
        {
            case 0:
                spr = m_Spr_down.idle;
                break;
            case 1:
                spr = m_Spr_left.idle;
                break;
            case 2:
                spr = m_Spr_right.idle;
                break;
        }

        m_SprRenderer.sprite = spr;
    }

    public string debug_string;

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (m_IsBot)
        {
            StartCoroutine(CorBot());
        }
        Chat(debug_string);
#endif
        StartCoroutine(CorNickbox());

    }


    IEnumerator CorNickbox()
    {
        var temp = Instantiate(m_NickBox, transform.position, Quaternion.identity, transform);
        var newPos = Vector3.zero;
        newPos.y = -0.7f;
        temp.transform.localPosition = newPos;

        yield return new WaitUntil(() => m_userdat != null);
        yield return new WaitUntil(() => g_PlayBlock == false);

        var canvasGroup = temp.transform.GetChild(0).GetComponent<CanvasGroup>();
        temp.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = m_userdat.nickname;

        yield return new WaitForSeconds(3.5f);

        float time = 0;
        while(time < 25f)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / 25f);
            yield return null;
        }
    }


#if UNITY_EDITOR
    IEnumerator CorBot()
    {
        while (true)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    CheckMovableTile(KeyCode.UpArrow, m_Spr_up);
                    break;
                case 1:
                    CheckMovableTile(KeyCode.DownArrow, m_Spr_down);
                    break;
                case 2:
                    CheckMovableTile(KeyCode.LeftArrow, m_Spr_left);
                    break;
                case 3:
                    CheckMovableTile(KeyCode.RightArrow, m_Spr_right);
                    break;
                case 4:
                    switch (Random.Range(0, 8))
                    {
                        case 0:
                            Chat(DefineData.IMOJI_QUESTION_MARK);
                            break;
                        case 1:
                            Chat(DefineData.IMOJI_EXCLAMATION_MARK);
                            break;
                        case 2:
                            Chat(DefineData.IMOJI_BYE);
                            break;
                        case 3:
                            Chat(DefineData.IMOJI_SAD);
                            break;
                        case 4:
                            Chat(DefineData.IMOJI_KK);
                            break;
                        case 5:
                            Chat(DefineData.IMOJI_CUTE);
                            break;
                        case 6:
                            Chat(DefineData.IMOJI_SMILE);
                            break;
                        case 7:
                            Chat("하이바이");
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(Random.Range(2.5f, 6.0f));
        }
    }
#endif

#if UNITY_EDITOR
    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_MoveState == MOVE_STATE.IDLE && !m_IsBot)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CheckMovableTile(KeyCode.UpArrow, m_Spr_up);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CheckMovableTile(KeyCode.DownArrow, m_Spr_down);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CheckMovableTile(KeyCode.LeftArrow, m_Spr_left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CheckMovableTile(KeyCode.RightArrow, m_Spr_right);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                switch (Random.Range(0, 8))
                {
                    case 0:
                        Chat(DefineData.IMOJI_QUESTION_MARK);
                        break;
                    case 1:
                        Chat(DefineData.IMOJI_EXCLAMATION_MARK);
                        break;
                    case 2:
                        Chat(DefineData.IMOJI_BYE);
                        break;
                    case 3:
                        Chat(DefineData.IMOJI_SAD);
                        break;
                    case 4:
                        Chat(DefineData.IMOJI_KK);
                        break;
                    case 5:
                        Chat(DefineData.IMOJI_CUTE);
                        break;
                    case 6:
                        Chat(DefineData.IMOJI_SMILE);
                        break;
                    case 7:
                        Chat("하이바이");
                        break;
                    default:
                        break;
                }
            }
        }
    }
#endif

    IEnumerator CorMoveCommand(KeyCode _key, SpriteGroup _sprGroup, int _time, bool _look = false)
    {
        if (_time == 0) yield break;

        m_MoveState = MOVE_STATE.MOVE;
        m_TB.gameObject.SetActive(false);
        m_TB.transform.position = transform.position;

        if (!_look)
        {
            for (int i = 0; i < _time; i++)
            {
                yield return StartCoroutine(CheckMovableTile(_key, _sprGroup));
            }
        }
        else
        {
            yield return StartCoroutine(WaitSeconds(0.5f, _sprGroup, true));
        }
        m_MoveState = MOVE_STATE.IDLE;
    }



    IEnumerator CheckMovableTile(KeyCode _key, SpriteGroup _sprGroup)
    {

        Vector3 rayStart = transform.position;
        rayStart.z = -1;
        rayStart.y += _key == KeyCode.UpArrow ? 1 : _key == KeyCode.DownArrow ? -1 : 0;
        rayStart.x += _key == KeyCode.RightArrow ? 1 : _key == KeyCode.LeftArrow ? -1 : 0;

        m_TB.gameObject.SetActive(true);
        m_TB.transform.position = rayStart + Vector3.forward;
       
        Ray2D ray = new Ray2D(rayStart, Vector3.forward);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 10f);

        var hit = Physics2D.Raycast(ray.origin, ray.direction, 100f, m_LayerMask);
        if (hit.transform != null)
        {
            return WaitSeconds(0.5f, _sprGroup);
        }
        else
        {
            return Move(_sprGroup);
        }
    }

    IEnumerator WaitSeconds(float _time, SpriteGroup _sprGroup, bool _look = false)
    {
        m_SprRenderer.sprite = _sprGroup.idle;
        yield return new WaitForSeconds(_time * 0.3f);
        if (!_look)
        {
            m_SprRenderer.sprite = left ? _sprGroup.move_left : _sprGroup.move_right;
            left = !left;
        }
        yield return new WaitForSeconds(_time);
        if (!_look)
            m_SprRenderer.sprite = _sprGroup.idle;

        m_TB.gameObject.SetActive(false);
        m_TB.transform.position = transform.position;

        RevisePosition();
    }

    IEnumerator Move(SpriteGroup _sprGroup)
    {
        float time = 0;

        Vector3 start = transform.position;
        Vector3 end = m_TB.transform.position;

        float animTime = 0.5f;
        float moveAnimTime = animTime * 0.3f;

        bool animBlock = false;

        m_SprRenderer.sprite = _sprGroup.idle;

        var wait = new WaitForFixedUpdate();

        while (true)
        {
            time += Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(start, end, time / animTime);

            if (time > moveAnimTime && !animBlock)
            {
                m_SprRenderer.sprite = left ? _sprGroup.move_left : _sprGroup.move_right;
                left = !left;
                animBlock = true;
            }
            if (time > animTime)
            {
                break;
            }
            yield return wait;
        }

        m_SprRenderer.sprite = _sprGroup.idle;
        m_TB.gameObject.SetActive(false);

        RevisePosition();

        m_MoveState = MOVE_STATE.IDLE;
    }

    public void Chat(string _msg)
    {
        if (g_PlayBlock)
            return;

        Debug.Log(_msg);
        int ea = 1;

        bool isMove = false;
        if (!string.IsNullOrEmpty(_msg))
        {
            int.TryParse(_msg[_msg.Length - 1].ToString(), out ea);
            ea = ea == 0 ? 1 : ea;

            if (m_MoveState == MOVE_STATE.IDLE)
            {
                if (_msg.Equals(DefineData.P_MOVE_DOWN) || _msg.Equals(DefineData.P_MOVE_DOWN2)
                    || _msg.Equals(DefineData.P_MOVE_DOWN3) || _msg.Equals(DefineData.P_MOVE_DOWN4))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.DownArrow, m_Spr_down, ea));
                }
                else if (_msg.Equals(DefineData.P_MOVE_UP) || _msg.Equals(DefineData.P_MOVE_UP2)
                    || _msg.Equals(DefineData.P_MOVE_UP3) || _msg.Equals(DefineData.P_MOVE_UP4))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.UpArrow, m_Spr_up, ea));
                }
                else if (_msg.Equals(DefineData.P_MOVE_LEFT) || _msg.Equals(DefineData.P_MOVE_LEFT2)
                    || _msg.Equals(DefineData.P_MOVE_LEFT3) || _msg.Equals(DefineData.P_MOVE_LEFT4))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.LeftArrow, m_Spr_left, ea));
                }
                else if (_msg.Equals(DefineData.P_MOVE_RIGHT) || _msg.Equals(DefineData.P_MOVE_RIGHT2)
                    || _msg.Equals(DefineData.P_MOVE_RIGHT3) || _msg.Equals(DefineData.P_MOVE_RIGHT4))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.RightArrow, m_Spr_right, ea));
                }
                else if (_msg.Equals(DefineData.P_MOVE_DOWN_VIEW))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.DownArrow, m_Spr_down, 0, true));
                }
                else if (_msg.Equals(DefineData.P_MOVE_UP_VIEW))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.UpArrow, m_Spr_up, 0, true));
                }
                else if (_msg.Equals(DefineData.P_MOVE_LEFT_VIEW))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.LeftArrow, m_Spr_left, 0, true));
                }
                else if (_msg.Equals(DefineData.P_MOVE_RIGHT_VIEW))
                {
                    isMove = true;
                    StartCoroutine(CorMoveCommand(KeyCode.RightArrow, m_Spr_right, 0, true));
                }
            }

            if (_msg.Equals(DefineData.IMOJI_QUESTION_MARK))
            {
                ActiveImoji(m_Imoji_QuestionMark);
            }
            else if (_msg.Equals(DefineData.IMOJI_EXCLAMATION_MARK))
            {
                ActiveImoji(m_Imoji_ExclamationMark);
            }
            else if (_msg.Equals(DefineData.IMOJI_SMILE))
            {
                ActiveImoji(m_Imoji_Smile);
            }
            else if (_msg.Equals(DefineData.IMOJI_KK))
            {
                ActiveImoji(m_Imoji_kk);
            }
            else if (_msg.Equals(DefineData.IMOJI_SAD))
            {
                ActiveImoji(m_Imoji_Sad);
            }
            else if (_msg.Equals(DefineData.IMOJI_BYE))
            {
                ActiveImoji(m_Imoji_Bye);
            }
            else if (_msg.Equals(DefineData.IMOJI_CUTE))
            {
                ActiveImoji(m_Imoji_Cute);
            }
            else
            {
                if (!isMove)
                    ActiveChat(_msg);
            }
        }
    }

    public void ActiveImoji(Sprite _sprite)
    {
        if (m_CoChatBalloonDelayManage != null)
        {
            StopCoroutine(m_CoChatBalloonDelayManage);
        }
        m_Chat.gameObject.SetActive(false);
        m_SprImojiRenderer.sprite = _sprite;
        m_SprImojiRenderer.gameObject.SetActive(true);
        m_CoChatBalloonDelayManage = StartCoroutine(CorChatBalloonDelayManage());
    }
    public void ActiveChat(string _msg)
    {
        if (m_CoChatBalloonDelayManage != null)
        {
            StopCoroutine(m_CoChatBalloonDelayManage);
        }
        m_SprImojiRenderer.gameObject.SetActive(false);
        m_ChatText.text = _msg;
        m_Chat.gameObject.SetActive(true);
        m_CoChatBalloonDelayManage = StartCoroutine(CorChatBalloonDelayManage());
    }

    Coroutine m_CoChatBalloonDelayManage = null;
    IEnumerator CorChatBalloonDelayManage()
    {
        yield return new WaitForSeconds(3.0f);
        m_Chat.gameObject.SetActive(false);
        m_SprImojiRenderer.gameObject.SetActive(false);
        m_CoChatBalloonDelayManage = null;
    }

    void RevisePosition()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        int z = (int)transform.position.z;
        transform.position = new Vector3(x, y, z);
    }
}
