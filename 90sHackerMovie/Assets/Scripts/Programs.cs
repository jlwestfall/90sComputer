using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Programs : MonoBehaviour
{
    bool active;

    public GameObject program;
	public GameObject[] programWindows;

    void Start()
    {
		program = GameManager.gM.program;
    }

    void Update()
    {
    }

    public void ProgramButtonOn()
    {
		program.SetActive(true);
    }

	public void ProgramWindowOpen()
	{
		programWindows[0].SetActive(true);
	}
   
}
