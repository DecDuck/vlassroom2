using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoginCreatureComforts : MonoBehaviour
{

    public GameObject usernameObject;
    public GameObject passwordObject;

    // Start is called before the first frame updat

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            MasterGameManager.stn.SetCredential();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(EventSystem.current.currentSelectedGameObject.name == usernameObject.name)
            {
                EventSystem.current.SetSelectedGameObject(passwordObject);
            }
            else if (EventSystem.current.currentSelectedGameObject.name == passwordObject.name)
            {
                EventSystem.current.SetSelectedGameObject(usernameObject);
            }
        }
    }
}
