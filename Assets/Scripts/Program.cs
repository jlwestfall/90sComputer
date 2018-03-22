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
    Vector2 mouseOffset = Vector2.zero;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Update()
    {
        if(handled)
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + mouseOffset.x, Input.mousePosition.y + mouseOffset.y, Vector3.Distance(Camera.main.transform.position, GameManager.gM.pManager.programsParent.transform.position)));
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
    #endregion
}
