using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgramManager : MonoBehaviour
{

    public Taskbar taskbar;
    public GameObject programsParent;
    public GraphicRaycaster GR;
    public List<Program> programWindows;

    Program currentHeldWindow = null;
    Program currentFocusWindow = null;
    GameObject lastWindow = null;

    private void Start()
    {
        programWindows = new List<Program>();
    }
    private void Update()
    {
        DragWindows();
        FocusWindows();
    }

    private void FocusWindows()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Create the PointerEventData with null for the EventSystem
            PointerEventData ped = new PointerEventData(null)
            {
                //Set required parameters, in this case, mouse position
                position = Input.mousePosition
            };
            //Create list to receive all results
            List<RaycastResult> results = new List<RaycastResult>();
            //Raycast it
            GR.Raycast(ped, results);

            if (results[0].gameObject.GetComponent<Program>())
            {
                currentFocusWindow = results[0].gameObject.GetComponent<Program>();
                if (lastWindow != null && lastWindow != currentFocusWindow.gameObject)
                {
                    currentFocusWindow.transform.SetParent(lastWindow.transform);
                    currentFocusWindow.transform.SetParent(programsParent.transform);
                }
                lastWindow = currentFocusWindow.gameObject;
            }
        }
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
                if (temp && currentHeldWindow == null)
                {
                    currentHeldWindow = temp;
                    Vector2 mOffset = new Vector2((temp.transform.position.x - mousePos.x), (temp.transform.position.y - mousePos.y));
                    //Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), hit.point, Color.red, 30.0f);
                    //Debug.DrawRay(temp.transform.position, -mOffset, Color.green, 30.0f);
                    currentHeldWindow.SetHandled(true, mOffset * 50);
                    Debug.Log(mOffset);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentHeldWindow)
            {
                GameManager.gM.lastProgram = currentHeldWindow;
                currentHeldWindow.SetHandled(false);
                currentHeldWindow = null;
            }
        }
    }

    public void AddToProgramList(Program prog)
    {
        bool exists = false;
        foreach (Program p in programWindows)
        {
            if (p.gameObject.name == prog.gameObject.name)
            {
                exists = true;
                prog.ShutdownProgram();
            }

        }

        if (!exists)
        {
            programWindows.Add(prog);
            lastWindow = prog.gameObject;
        }
    }
    public void RemoveFromProgramList(Program prog)
    {
        programWindows.Remove(prog);
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
