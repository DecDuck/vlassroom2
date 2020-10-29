using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeacherConfigController : MonoBehaviour
{
    public TMP_Text usernameText;
    public Button createClassroom;
    public Button joinClassroom;
    public TMP_InputField classCode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Load()
    {
        usernameText = GameObject.Find("Username").GetComponent<TMP_Text>();
        createClassroom = GameObject.Find("CreateClassroom").GetComponent<Button>();
        joinClassroom = GameObject.Find("JoinClassroom").GetComponent<Button>();
        classCode = GameObject.Find("ClassroomCode").GetComponent<TMP_InputField>();
    }
}
