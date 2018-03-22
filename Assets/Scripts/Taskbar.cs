using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Taskbar : MonoBehaviour 
{
    #region Public Variables
    public GameObject TaskbarButtonPrefab;
    public GameObject TaskbarArea;
    #endregion

    #region Private Variables
    Vector3 buttonSpawnLocation;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Start()
    {
        buttonSpawnLocation = TaskbarArea.transform.position;
    }
    #endregion

    #region Custom Methods
    public GameObject CreateTaskbarButton(string buttonName)
    {
        GameObject button = Instantiate(TaskbarButtonPrefab, buttonSpawnLocation, Quaternion.identity, this.gameObject.transform);
        button.GetComponentInChildren<Text>().text = buttonName;
        return button;
    }
	#endregion
}
