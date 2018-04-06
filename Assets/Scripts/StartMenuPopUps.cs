using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuPopUps : MonoBehaviour 
{
	#region Public Variables
	public GameObject programsMenu;

	#endregion

	#region Private Variables
	private bool menuOpen;
	#endregion

	#region Enumerations
	#endregion

	#region Unity Methods
	void Start()
	{
		
	}
	#endregion

	#region Custom Methods
	public void OnMouseOver()
	{
		programsMenu.SetActive(true);
	}

	public void OnMouseExit()
	{
		if(!menuOpen)
			programsMenu.SetActive(false);
	}

	public void OnMouseOverMenu()
	{
		menuOpen = true;
	}

	public void OnMouseExitMenu()
	{
		menuOpen = false;
	}
	#endregion
}
