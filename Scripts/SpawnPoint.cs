using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool m_IsLookAtOn;
    public Vector3 m_LookAt;
    public Vector3 m_Default;

    [Space(20)]
    public Vector3 m_MyVector;

    public void Start()
    {
        SetLookAt();
    }

    public bool SetLookAt(Transform _text = null)
    {
        m_IsLookAtOn = Random.value > 0.5f ? true : false;
        transform.localEulerAngles = m_IsLookAtOn ? m_LookAt : m_Default;
        m_MyVector = transform.localEulerAngles;
        //GDebug.Log(name + " : " + (m_IsLookAtOn ? m_LookAt : m_Default));

        return m_IsLookAtOn;
    }

}
