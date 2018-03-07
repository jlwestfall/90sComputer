using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Programs : MonoBehaviour
{

    public GameObject[] program;
    public GameObject[] programWindows;


    public void OpenProgram(int index)
    {
        programWindows[index].SetActive(true);
        program[index].SetActive(true);
    }

    public void CloseProgram(int index)
    {
      program[index].SetActive(false);
      programWindows[index].SetActive(false);

    }

}
