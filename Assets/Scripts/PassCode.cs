using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassCode : MonoBehaviour
{
	public Text textInput;
	string virusCode;

	public GameObject virusPanel;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		virusCode = textInput.text;

		if (virusCode == "0000")
		{	
			virusPanel.SetActive(true);
		}
    }
}
