using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creddit : MonoBehaviour
{
    [SerializeField] private GameObject creddit;

    public void OnCredditClicked()
    {
        creddit.SetActive(true);
    }

    public void OnExitClicked()
    {
        creddit.SetActive(false);
    }
}
