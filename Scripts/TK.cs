using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TK : MonoBehaviour
{
    public enum MouseType
    {
        Idle, SmallAh, BigAh, Uh, Woo, Yee
    }

    [System.Serializable]
    public class TKDataClass
    {
        public string clipName;
        public AudioClip clip;
        public string animName;
        public float animSpeed = 1.0f;
        public float syncTime = 0;
        public float animEndTime;
        public float mirrorBallTime = 0;
        public float mirrorBallEndTime = 0;
        public float copyRightTime = 0;
    }

    [System.Serializable]
    public class MouseData
    {
        public MouseData(float _time, int _mouseType)
        {
            time = _time;
            mouseType = _mouseType;
        }
        public float time;
        public int mouseType;
    }

    public static TK instance;
    public static float animSpeed = 1.0f;

    public InputField m_SongIndex;

    [Space(20)]
    public Animator m_TKAnimator;
    public Animator m_TKChestAnimator;
    public Animator m_TKRFootAnimator;
    public Animator m_TKLFootAnimator;
    public Animator m_CameraAnimator;
    public Animator m_BrewsterAanimator;
    
    [Space(20)]
    public Renderer m_MouseMat;
    public Texture m_SprIdle;
    public Texture m_SprSmallAh;
    public Texture m_SprBigAh;
    public Texture m_SprUh;
    public Texture m_SprWoo;
    public Texture m_SprYee;

    [Space(20)]
    public AudioSource m_AudioSource;

    public TKDataClass[] m_SongList;

    public List<MouseData> m_MouseData;

    public TKDataClass cache;

    private void Awake()
    {
        instance = this;
        //cache = m_SongList[Random.Range(0, m_SongList.Length)];
        //ReadMouseData();
    }

    public void PlayTKSong()
    {
        //cache = m_SongList[Random.Range(0, m_SongList.Length)];
        int parsing = string.IsNullOrEmpty(m_SongIndex.text) ? -1 : int.Parse(m_SongIndex.text);
        int songidx = parsing < 0 ? Random.Range(0, m_SongList.Length) : parsing;

        cache = m_SongList[songidx];
        animSpeed = cache.animSpeed;
        ReadMouseData();
        m_AudioSource.clip = cache.clip;

        GameManager.instance.m_UserList.ForEach(obj => obj.m_Animator.speed = animSpeed);
    }

    public void SingASong()
    {
        m_AudioSource.Play();

        m_CameraAnimator.Play(cache.animName);
        m_TKAnimator.Play(cache.animName);
        m_TKChestAnimator.SetBool("Play", true);
        m_TKRFootAnimator.SetBool("Play", true);
        m_TKLFootAnimator.SetBool("Play", true);

        m_TKChestAnimator.speed = cache.animSpeed;
        m_TKLFootAnimator.speed = cache.animSpeed;
        m_TKRFootAnimator.speed = cache.animSpeed;
        m_BrewsterAanimator.speed = cache.animSpeed > 1 ? cache.animSpeed * 2f : cache.animSpeed;

        GameManager.instance.m_UserList.ForEach(obj => obj.m_Animator.speed = cache.animSpeed);
        //StartCoroutine(CorMouseManager(m_MouseData));
        float sync = cache.syncTime;
        StartCoroutine(DelayAct(cache.animEndTime, () => 
        {
            //m_TKAnimator.StopPlayback();
            m_TKChestAnimator.speed = 0;
            m_TKLFootAnimator.speed = 0;
            m_TKRFootAnimator.speed = 0;
            m_TKAnimator.speed = 0;
            GDebug.Log("STOP");
        }));
        for(int i = 0; i < m_MouseData.Count; i++)
        {
            StartCoroutine(DelayAction(m_MouseData[i].time + sync, m_MouseData[i].mouseType));
        }
        StartCoroutine(DelayAct(cache.mirrorBallTime, () => { GameManager.instance.ActiveMirrorBall(true); }));
        StartCoroutine(DelayAct(cache.mirrorBallEndTime, () => { GameManager.instance.ActiveMirrorBall(false); }));
        StartCoroutine(DelayAct(cache.copyRightTime, () => { GameManager.instance.ActiveCopyRight(true); }));
        StartCoroutine(DelayAct(cache.clip.length, () => {
            GameManager.instance.SetCurtain(false);
            GameManager.instance.m_TextHolder.gameObject.SetActive(false);
        }));
    }

    public IEnumerator DelayAction(float time, int idx)
    {
        yield return new WaitForSeconds(time);
        SetMouseSpr(idx);
    }

    public IEnumerator DelayAct(float _time, System.Action _act)
    {
        yield return new WaitForSeconds(_time);
        _act?.Invoke();
    }

    public void ReadMouseData()
    {
        GDebug.Log(Application.dataPath);
        StreamReader sr = new StreamReader(Application.dataPath + "/MouseData/" + cache.animName +".txt");
        bool endOfFile = false;

        m_MouseData = new List<MouseData>();

        while (!endOfFile)
        {
            string data_String = sr.ReadLine();
            if (data_String == null)
            {
                endOfFile = true;
                break;
            }
            var data_values = data_String.Split(',');
            for (int i = 0; i < data_values.Length - 1; i++)
            {
                m_MouseData.Add(new MouseData(float.Parse(data_values[0]), int.Parse(data_values[1])));
            }
        }
        sr.Close();
    }

    IEnumerator CorMouseManager(List<MouseData> _data)
    {
        int idx = 0;
        float delay = 0;
        while (idx < _data.Count)
        {
            if (idx >= _data.Count)
            {
                SetMouseSpr(MouseType.Idle);
                yield break;
            }
            if (idx == 0) delay = _data[idx].time;
            else delay = _data[idx].time - _data[idx - 1].time;
            GDebug.LogWarning("Delay : " + delay);
            yield return new WaitForSeconds((float)delay);
            GDebug.LogWarning("Action : " + _data[idx].mouseType);
            SetMouseSpr(_data[idx].mouseType);
            idx++;
        }
        GDebug.LogWarning("END");
    }

    public void SetMouseSpr(MouseType _type)
    {
        switch (_type)
        {
            case MouseType.Idle:
                m_MouseMat.material.mainTexture = m_SprIdle;
                break;
            case MouseType.SmallAh:
                m_MouseMat.material.mainTexture = m_SprSmallAh;
                break;
            case MouseType.BigAh:
                m_MouseMat.material.mainTexture = m_SprBigAh;
                break;
            case MouseType.Uh:
                m_MouseMat.material.mainTexture = m_SprUh;
                break;
            case MouseType.Woo:
                m_MouseMat.material.mainTexture = m_SprWoo;
                break;
            case MouseType.Yee:
                m_MouseMat.material.mainTexture = m_SprYee;
                break;
            default:
                m_MouseMat.material.mainTexture = m_SprIdle;
                break;
        }
    }

    public void SetMouseSpr(int _type)
    {
        MouseType type = (MouseType)_type;
        switch (type)
        {
            case MouseType.Idle:
                m_MouseMat.material.mainTexture = m_SprIdle;
                break;
            case MouseType.SmallAh:
                m_MouseMat.material.mainTexture = m_SprSmallAh;
                break;
            case MouseType.BigAh:
                m_MouseMat.material.mainTexture = m_SprBigAh;
                break;
            case MouseType.Uh:
                m_MouseMat.material.mainTexture = m_SprUh;
                break;
            case MouseType.Woo:
                m_MouseMat.material.mainTexture = m_SprWoo;
                break;
            case MouseType.Yee:
                m_MouseMat.material.mainTexture = m_SprYee;
                break;
            default:
                m_MouseMat.material.mainTexture = m_SprIdle;
                break;
        }
    }
}
