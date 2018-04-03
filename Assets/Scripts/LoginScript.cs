using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*===================================
Project:	#PROJECTNAME#	
Developer:	#DEVNAME#
Company:	#COMPANY#
Date:		#CREATIONDATE#
-------------------------------------
Description:

===================================*/

public class LoginScript : MonoBehaviour
{
    #region Public Variables
    public InputField input;
    #endregion

    #region Private Variables
    [SerializeField]
    private string correctPassword = "password";
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods

    #endregion

    #region Custom Methods
    public void TryLogin()
    {

        if(input.text == correctPassword)
        {
            LoginSucceed();
        }
        else
        {
            LoginFail();
        }

    }

    private void LoginSucceed()
    {
        SceneManager.LoadScene("Desktop");
    }

    private void LoginFail()
    {
        input.text = "";
    }
    #endregion
}
