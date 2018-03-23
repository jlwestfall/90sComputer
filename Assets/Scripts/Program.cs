using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour 
{
    #region Public Variables
    #endregion

    #region Private Variables
    GameObject taskbarButton;
    bool handled = false;
    Vector2 mouseOffset = new Vector3(0.000f, 0.000f, 0.000f);
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Update()
    {
        if(handled)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    Input.mousePosition.x + mouseOffset.x,  //x
                Input.mousePosition.y + mouseOffset.y,      //y
                Vector3.Distance(Camera.main.transform.position, GameManager.gM.pManager.programsParent.transform.position) //z
                ));

            transform.position = newPos;
            //transform.position = 
        }
    }
    #endregion

    #region Custom Methods
    public void SetHandled(bool b)
    {
        handled = b;
        if(!handled)
        {
            mouseOffset = Vector2.zero;
        }
    }

    public void SetHandled(bool b, Vector2 offset)
    {
        handled = b;
        if(handled)
        {
            mouseOffset = offset;
        }

    }
    public void SetTaskbarButton(GameObject button)
    {
        taskbarButton = button;
    }
    public void ShutdownProgram()
    {
        GameManager.gM.pManager.taskbar.DestroyTaskbarButton(taskbarButton);
        Destroy(this.gameObject);
    }
    #endregion
}
