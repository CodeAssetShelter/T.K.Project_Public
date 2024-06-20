using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MouseData", menuName = "MouseData/MouseData", order = int.MaxValue)]
public class MouseData : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public float time;
        public Texture mouse_sprite;
    }

    public Data[] datas;
}
