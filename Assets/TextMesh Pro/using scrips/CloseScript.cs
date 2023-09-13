using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseScript : MonoBehaviour
{
    private Button btn;
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    private void TaskOnClick()
    {
        Application.Quit();
        Debug.Log("close");
    }
}
