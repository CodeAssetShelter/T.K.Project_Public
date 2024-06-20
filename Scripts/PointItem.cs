using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointItem : MonoBehaviour
{
    public enum PointType
    {
        C, B, A, S, STATE_COUNT
    }

    public PointType m_PointType;
    public Text m_Text;
    public GameObject m_BG;
    public float m_DisableTime = 40f;
    public float m_FlickerTime = 15f;
    public float m_FlickerDelay = 0.25f;

    const string cmp_tag = "Character";

    private void Start()
    {
        m_PointType = (PointType)Random.Range(0, (int)PointType.STATE_COUNT);
        m_Text.text = m_PointType.ToString();

        StartCoroutine(CorFade());
    }

    IEnumerator CorFade()
    {
        float time = 0;
        while(time < m_DisableTime - m_FlickerTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        float flicker_time = 0;
        while (time < m_FlickerTime)
        {
            time += Time.deltaTime;
            flicker_time += Time.deltaTime;
            if (flicker_time > m_FlickerDelay)
            {
                m_BG.SetActive(!m_BG.activeInHierarchy);
                flicker_time = 0;
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(11))
        {
            string send_data = string.Empty;
            switch (m_PointType)
            {
                case PointType.C:
                    send_data = "1000";
                    break;
                case PointType.B:
                    send_data = "2000";
                    break;
                case PointType.A:
                    send_data = "4000";
                    break;
                case PointType.S:
                    send_data = "8000";
                    break;
                default:
                    break;
            }

            NetClient.Instance.Send(send_data);
            Destroy(gameObject);
        }
    }
}
