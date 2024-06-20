using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngle : MonoBehaviour
{
    public Transform body;
    public Transform head;

    public float angle;
    // Update is called once per frame
    void Update()
    {
        //angle = Vector3.SignedAngle(transform.up, TK.instance.transform.position - body.transform.position, transform.forward);
        //head.transform.localEulerAngles = new Vector3(angle, 0, 0);

        angle = Vector3.SignedAngle(transform.up, TK.instance.transform.position - body.transform.position, transform.forward);
        head.transform.localEulerAngles = new Vector3(angle, 0, 0);
    }
    private void LateUpdate()
    {
        //var rot = head.transform.localEulerAngles;
        //rot.y = rot.z = 0;
        //head.transform.localEulerAngles = rot;
    }
}
