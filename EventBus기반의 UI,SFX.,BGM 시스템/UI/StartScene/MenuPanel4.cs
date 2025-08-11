using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel4 : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        EventBus.PlayBGM(BGMEventType.PlayStartMenu);
    }

    // Update is called once per frame
    void OnDisable()
    {

    }
}
