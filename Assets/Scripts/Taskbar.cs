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
    int buttonCount = 0;
    List<GameObject> taskButtons;
    float taskWidth;
    float originalWidth;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Start()
    {
        taskButtons = new List<GameObject>();
        originalWidth = TaskbarButtonPrefab.GetComponent<RectTransform>().rect.width;
        taskWidth = originalWidth;
    }
    #endregion

    #region Custom Methods
    public GameObject CreateTaskbarButton(string buttonName)
    {
        GameObject button = Instantiate(TaskbarButtonPrefab, TaskbarArea.transform);
        buttonCount++;
        button.GetComponentInChildren<Text>().text = buttonName;
        RectTransform rect = button.GetComponent<RectTransform>();

        taskButtons.Add(button);
        if (buttonCount > Mathf.FloorToInt(Screen.width / taskWidth) && buttonCount != 0)
        {
            taskWidth = (TaskbarArea.GetComponent<RectTransform>().rect.width - (3 * buttonCount)) / taskButtons.Count;
            //Debug.Log(taskWidth + " | " + taskButtons.Count);
        }
        else
        {
            taskWidth = originalWidth;

        }

        ResizeTaskbar(taskWidth, rect.rect.height);

        Debug.Log("Able to fit: " + Mathf.FloorToInt(Screen.width / taskWidth)+2 + " | There are: " + buttonCount);


        return button;
    }

    private void ResizeTaskbar(float tWidth, float tHeight)
    {
        Vector2 newSizeDelta = new Vector2(tWidth, tHeight);
        for (int i = 0; i < taskButtons.Count; i++)
        {
            RectTransform rect = taskButtons[i].GetComponent<RectTransform>();
            rect.sizeDelta = newSizeDelta;
            rect.position = Vector3.zero;
            rect.localPosition = ((Vector3.right * taskWidth) * i) + (Vector3.right * 3 * i);
        }
    }

    public void DestroyTaskbarButton(GameObject button)
    {
        int i = 0;
        foreach (GameObject task in taskButtons)
        {
            if (task.GetInstanceID() == button.GetInstanceID())
            {
                taskButtons.RemoveAt(i);
                break;
            }
            i++;
        }
        Destroy(button);
        buttonCount--;

        if (buttonCount > 0)
        {
            if (buttonCount > Mathf.FloorToInt(Screen.width / taskWidth) && buttonCount != 0)
            {
                taskWidth = (TaskbarArea.GetComponent<RectTransform>().rect.width - (3 * buttonCount)) / taskButtons.Count;
                //Debug.Log(taskWidth + " | " + taskButtons.Count);
            }
            else
            {
                taskWidth = originalWidth;

            }
            ResizeTaskbar(taskWidth, taskButtons[0].GetComponent<RectTransform>().rect.height);
        }
    }
    #endregion
}
