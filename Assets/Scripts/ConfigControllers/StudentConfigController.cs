using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StudentConfigController : MonoBehaviour
{
    public Button joinClassroom;
    public TMP_InputField classCode;
    public TMP_Text usernameText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Load()
    {
        joinClassroom = GetComponentInChildren<Button>();
        classCode = GetComponentInChildren<TMP_InputField>();
        usernameText = GetComponentInChildren<TMP_Text>();
    }
}
