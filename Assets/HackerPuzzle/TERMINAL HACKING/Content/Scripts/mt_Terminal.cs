using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mt_Terminal : MonoBehaviour
{
    public KeyCode useKey = KeyCode.E;
    public KeyCode exitKey = KeyCode.Tab;
    public enum AVAILABILITY { NORMAL, LOCKED, UNLOCKED }
    public AVAILABILITY availability = AVAILABILITY.NORMAL;
    public bool DATA, NUKE, LOCK;
    public LayerMask terminalLayer;

    public float rewardDelay = 3;
    public float detectionRange = 3;

    [HideInInspector]
    public string password;
    [HideInInspector]
    public bool inRange = false;
    public bool hackerManSwitch;

    string gText;

    protected mt_Hacking m_Hacking = null;
    mt_Hacking Hacking
    {
        get
        {
            if (m_Hacking == null) m_Hacking = FindObjectOfType<mt_Hacking>();
            return m_Hacking;
        }
    }

    void Awake()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        useKey = KeyCode.Mouse0;
        gText = "TAP TO HACK ";

#else
        gText = "PRESS " + useKey + " TO HACK ";
#endif

        Hacking.exitButton.onClick.AddListener(() =>
        {
            Hacking.ToggleHacking(false);
        });
    }

    void Update()
    {
        GetInput();
    }

    public void GetInput()
    {
        if (hackerManSwitch)
        {
            if (Hacking.isHacking) return;
            Hacking.ToggleHacking(true);
        }

        if (Input.GetKeyDown(exitKey))
        {
            if (!Hacking.isHacking) return;
            Hacking.ToggleHacking(false);
        }
    }

    /*void OnGUI()
    {
        if (Hacking.isHacking) return;

        GUIStyle myStyle = new GUIStyle();
        myStyle.alignment = TextAnchor.MiddleCenter;
        myStyle.fontSize = 24;
        GUI.Label(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 25, 200, 50), gText + hit.transform.name, myStyle);
    }
    */

    RaycastHit hit;


    public void HackerManButton()
    {
        hackerManSwitch = true;
    }

    public virtual IEnumerator DoSuccess()
    {
        Hacking.footerText.text = "LOGGING IN TO ROOT";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);
        Hacking.footerText.text = "LOGGING IN TO ROOT";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(rewardDelay / 8);

        availability = AVAILABILITY.UNLOCKED;

        GameObject[] go = GameObject.FindGameObjectsWithTag("Row");
        foreach (GameObject g in go) Destroy(g);
        Hacking.Setup();
    }

    public IEnumerator DoFail()
    {
        availability = AVAILABILITY.LOCKED;
        Hacking.footerText.text = "SYSTEM HAS BEEN LOCKED DUE TO MALICOUS ACTIVITY, PLEASE CONTACT A SYSTEM ADMINISTRATOR TO RESET LOGIN";

        yield return new WaitForSecondsRealtime(1.5f);

        Hacking.ToggleHacking(false);
    }

}