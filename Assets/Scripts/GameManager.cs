using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager gM;

	[Header("Start Menu")]
	public GameObject startPanel;

	[Header("Program")]
	public GameObject program;

	void Awake()
	{
		if(!gM)
			gM = this;
		else
			Destroy(this.gameObject);
	}
}
