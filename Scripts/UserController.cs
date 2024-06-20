using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserController : MonoBehaviour
{
    public Transform m_Neck;
    public Transform m_Murabito;
    [Space(20)]
    public int m_AnimatorPlayIdxMax = 2;
    public Animator m_Animator;

    [Space(20)]
    public Transform m_TextSocket;
    //public Transform m_MessageSocket;

    public Transform m_TextTransform;
    //public Transform m_MessageTransform;

    public bool m_IsLookAt = false;

    public float angle;
    Vector3 start = Vector3.zero;
    Vector3 end = Vector3.zero;


    private SpawnPoint m_SpawnPoint;

    IEnumerator CorUpdate()
    {
        Debug.Log("ON");
        while (true)
        {
            angle = Vector3.SignedAngle(transform.up, TK.instance.transform.position - m_Murabito.position, Vector3.up);
            m_Neck.transform.eulerAngles = new Vector3(0, angle - 180, -90f);
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetUser(Transform _text, SpawnPoint _isLookAt)
    {
        m_TextTransform = _text;
        //m_IsLookAt = _isLookAt;
        m_SpawnPoint = _isLookAt;
        gameObject.SetActive(true);
        //if (_text == null) Debug.LogError("NULL ! ! !");
        //else Debug.LogError(_text.GetComponent<Text>().text);
    }

    public void SetUser(Transform _text)
    {
        m_TextTransform = _text;
        gameObject.SetActive(true);
        //if (_text == null) Debug.LogError("NULL ! ! !");
        //else
        //    Debug.LogError(_text.GetComponent<Text>().text);

        if (m_SpawnPoint != null && !name.Contains("Isabelle"))
        {
            //Debug.LogWarning("삽입");
            m_IsLookAt = m_SpawnPoint.m_IsLookAtOn;
        }
    }

    public void StartUpdater(float _time, float _delayTime)
    {
        if (name.Contains("Isabelle"))
        {
            m_Animator.SetInteger("Play", Random.Range(1, m_AnimatorPlayIdxMax));
        }
        else
        {
            m_IsLookAt = transform.parent.parent.GetComponent<SpawnPoint>().m_IsLookAtOn;
            StartCoroutine(CorDelayHeadLookAt(_time, _delayTime));
        }
        StartCoroutine(CorTextUpdater(m_TextTransform));
        m_Animator.speed = TK.instance.cache.animSpeed;
    }

    public void StartUpdaterIsabelle()
    {
        m_Animator.SetInteger("Play", Random.Range(1, m_AnimatorPlayIdxMax));
    }

    IEnumerator CorTextUpdater(Transform _trans)
    {
        var wait = new WaitForFixedUpdate();

        m_TextTransform.gameObject.SetActive(true);
        var m_TextCanvasGroup = _trans.GetComponent<CanvasGroup>();
        //StartCoroutine(CorFixer());
        Camera cam = Camera.main;
        Vector3 screenPoint;
        while (true)
        {
            screenPoint = cam.WorldToViewportPoint(transform.position);

            if (screenPoint.z > 0 && screenPoint.x > -1.4f && screenPoint.x < 1.4f && screenPoint.y < 1.4f && screenPoint.y > -1.4f)
            {
                //_trans.gameObject.SetActive(true);
                m_TextCanvasGroup.alpha = 1;
                _trans.position = Camera.main.WorldToScreenPoint(m_TextSocket.position);
            }
            else
                m_TextCanvasGroup.alpha = 0;
            //_trans.gameObject.SetActive(false);
            yield return wait;
        }
    }

    IEnumerator CorDelayHeadLookAt(float _time, float _delayTime)
    {
        if (!m_IsLookAt)
        {
            m_Animator.SetInteger("Play", Random.Range(1, m_AnimatorPlayIdxMax));
            yield break;
        }

        //GDebug.Log("Angle LockOn");
        angle = Vector3.SignedAngle(transform.up, TK.instance.transform.position - m_Murabito.transform.position, transform.forward);
        //m_Neck.transform.localEulerAngles = new Vector3(angle, 0, 0);

        Vector3 currAngle = new Vector3((m_Neck.transform.localEulerAngles.x) % 360f, 0, 0);
        Vector3 targetAngle = new Vector3((angle) % 360f, 0, 0);
            //Debug.Log(currAngle + " " + targetAngle);
        float timer = 0;

        yield return new WaitForSeconds(_delayTime);

        m_Animator.SetInteger("Play", Random.Range(1, m_AnimatorPlayIdxMax));
        while (timer < _time)
        {
            timer += Time.deltaTime;
            m_Neck.transform.localEulerAngles = Vector3.Lerp(currAngle, targetAngle, timer / _time);

            yield return Time.deltaTime;
        }
        //Debug.Log(timer + " " + _time + " END");
    }
}
