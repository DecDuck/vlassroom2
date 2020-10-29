using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrincipalConfigController : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_InputField newUserUsername;
    public TMP_InputField newUserPasssword;
    public TMP_Dropdown newUserPermission;
    public Button createNewUser;

    public Button createClassroom;

    public TMP_InputField classCode;
    public Button joinClassroom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Load()
    {
        usernameText = GameObject.Find("Username").GetComponent<TMP_Text>();
        newUserUsername = GameObject.Find("NewUserUsername").GetComponent<TMP_InputField>();
        newUserPasssword = GameObject.Find("NewUserPassword").GetComponent<TMP_InputField>();
        newUserPermission = GameObject.Find("NewUserPermission").GetComponent<TMP_Dropdown>();
        createNewUser = GameObject.Find("CreateNewUser").GetComponent<Button>();

        createClassroom = GameObject.Find("CreateClassroom").GetComponent<Button>();

        classCode = GameObject.Find("ClassroomCode").GetComponent<TMP_InputField>();
        joinClassroom = GameObject.Find("JoinClassroom").GetComponent<Button>();
    }
}
