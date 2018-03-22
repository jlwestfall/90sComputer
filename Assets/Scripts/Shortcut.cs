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
        Instantiate(linkedProgram,Vector3.zero, linkedProgram.transform.rotation, GameManager.gM.pManager.programsParent.transform);
    }
	#endregion
}
