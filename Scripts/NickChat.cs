using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NickChat : MonoBehaviour
{
    public Text m_UserName;
    public GameObject m_ChatBox;
    public Text m_Message;
    public CanvasGroup m_CanvasGroup;

    public float m_DelayTime = 6.0f;
    Coroutine m_Coroutine;

    public csvReader.UserData m_UserData;

    public void ActiveChat(string _chat)
    {
        if (m_CanvasGroup.alpha == 0) return;
        if (m_Coroutine != null) return;

        m_ChatBox.SetActive(false);
        m_ChatBox.SetActive(true);

        m_Message.text = _chat;

        m_Coroutine = StartCoroutine(CorDelayChat());
    }

    IEnumerator CorDelayChat()
    {
        float timer = 0;
        while(timer < m_DelayTime)
        {
            timer += Time.deltaTime;
            yield return Time.deltaTime;
        }

        m_Coroutine = null;
    }

}
