using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{

    public Taskbar taskbar;
    public GameObject programsParent;
    public Program[] programWindows;

    Program currentWindow = null;
    
    private void Update()
    {
        DragWindows();
    }

    private void DragWindows()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero, 100.0f);

            if (hit && hit.collider.gameObject.GetComponent<Program>())
            {
                //Debug.Log("hit something");
                Program temp = hit.collider.gameObject.GetComponent<Program>();
                if (temp && currentWindow == null)
                {
                    currentWindow = temp;
                    Vector2 mOffset = new Vector2((temp.transform.position.x - mousePos.x), (temp.transform.position.y - mousePos.y));
                    //Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), hit.point, Color.red, 30.0f);
                    //Debug.DrawRay(temp.transform.position, -mOffset, Color.green, 30.0f);
                    currentWindow.SetHandled(true, mOffset * 50);
                    Debug.Log(mOffset);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentWindow)
            {
                GameManager.gM.lastProgram = currentWindow;
                currentWindow.SetHandled(false);
                currentWindow = null;
            }
        }
    }
    /*
    public void OpenProgram(int index)
    {
        programWindows[index].gameObject.SetActive(true);
        taskbar.CreateTaskbarButton("TESTING");
    }

    public void CloseProgram(int index)
    {
        programWindows[index].gameObject.SetActive(false);
        //taskbarButtons[index].SetActive(false);

    }
    */
}
