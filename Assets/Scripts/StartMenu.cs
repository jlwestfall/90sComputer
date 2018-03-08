using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
	bool active;

	GameObject startMenu;

	void Start()
	{
		startMenu = GameManager.gM.startPanel;
	}

	void Update()
	{
		if(active)
			startMenu.SetActive(true);
		else
			startMenu.SetActive(false);
	}

	public void StartButtonOn()
	{
		if(active)
			active = true;
		else
			active = false;
	}

	public void StartButtonOff()
	{
		if(active)
			active = false;
		else
			active = true;
	}
}
