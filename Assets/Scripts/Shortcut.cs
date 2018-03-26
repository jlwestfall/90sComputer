using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	#PROJECTNAME#	
Developer:	#DEVNAME#
Company:	#COMPANY#
Date:		#CREATIONDATE#
-------------------------------------
Description:

===================================*/

public class Shortcut : MonoBehaviour 
{
    #region Public Variables
    public GameObject linkedProgram;
    public string appName;
	#endregion
	
	#region Private Variables
	
	#endregion
	
	#region Enumerations
	
	#endregion

	#region Unity Methods
	
	#endregion
	
	#region Custom Methods
	public void Launch()
    {
        GameObject temp = Instantiate(linkedProgram, GameManager.gM.pManager.programsParent.transform);
        temp.name = appName;
        temp.GetComponent<Program>().SetTaskbarButton(GameManager.gM.pManager.taskbar.CreateTaskbarButton(appName));
        GameManager.gM.pManager.AddToProgramList(temp.GetComponent<Program>());
    }
	#endregion
}
