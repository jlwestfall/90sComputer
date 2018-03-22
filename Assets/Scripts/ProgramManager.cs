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
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 10.0f);

            if (hit && hit.collider.gameObject.GetComponent<Program>())
            {
                Debug.Log("hit something");
                Program temp = hit.collider.gameObject.GetComponent<Program>();
                if (temp && currentWindow == null)
                {
                    currentWindow = temp;
                    Vector2 mOffset = new Vector2((temp.transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (temp.transform.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                    Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), hit.point, Color.red, 30.0f);
                    Debug.DrawRay(temp.transform.position, -mOffset, Color.green, 30.0f);
                    currentWindow.SetHandled(true, mOffset*50);
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

}
