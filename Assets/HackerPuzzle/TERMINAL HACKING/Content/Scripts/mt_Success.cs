
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class mt_Success : MonoBehaviour
{
    /*
        Will change all of this to add custom code for when the game is beaten.
     */
    public Canvas PDAUI;
    public GameObject JournalPanel;
    public Button journalButton;
    public Button pdaExitButton;
    public KeyCode pdaKey = KeyCode.P;

    public Button dataButton;
    public Button nukeButton;
    public Button lockButton;

    //nuke
    public string[] targetTags;
    public Transform nukePanel;
    public Transform nukeContent;

    //lock/unlock
    public Transform doorTransform;
    public bool locked;

    //data journal
    public Text journalText;
    public List<string> journalEntries;

    protected mt_Hacking m_Hacking = null;
    mt_Hacking Hacking
    {
        get
        {
            if (m_Hacking == null) m_Hacking = FindObjectOfType<mt_Hacking>();
            return m_Hacking;
        }
    }

    protected mt_MissileLauncher m_Nuke = null;
    mt_MissileLauncher Nuke
    {
        get
        {
            if (m_Nuke == null) m_Nuke = FindObjectOfType<mt_MissileLauncher>();
            return m_Nuke;
        }
    }

    void Awake()
    {
        dataButton.onClick.AddListener(() =>
               {
                   DoData();
               });


        nukeButton.onClick.AddListener(() =>
        {
            DoNuke();
        });


        lockButton.onClick.AddListener(() =>
        {
            DoLock();
        });

        pdaExitButton.onClick.AddListener(() =>
        {
            TogglePDA();
        });

        journalButton.onClick.AddListener(() =>
        {
            ToggleJournal();
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(pdaKey)) TogglePDA();
    }

    public void Unlocked()
    {
        nukePanel.gameObject.SetActive(false);
        dataButton.gameObject.SetActive(false);
        nukeButton.gameObject.SetActive(false);
        lockButton.gameObject.SetActive(false);

        if (Hacking.term.DATA) dataButton.gameObject.SetActive(true);
        if (Hacking.term.NUKE) nukeButton.gameObject.SetActive(true);
        if (Hacking.term.LOCK) lockButton.gameObject.SetActive(true);

        dataButton.interactable = true;
        nukeButton.interactable = true;
        lockButton.interactable = true;

        lockButton.GetComponentInChildren<Text>().text = locked ? "UNLOCK " + doorTransform.name : "LOCK " + doorTransform.name;
    }

    void DoData()
    {
        if (Hacking.term.password == "" || Hacking.term.password == null)
        {
            Hacking.term.password = Hacking.scoreWord;
            WriteJournal("PWD for " + Hacking.term.name + " is " + Hacking.term.password);
        }
        Hacking.footerText.text = "PWD RESET:\n NEW PWD is " + Hacking.term.password;
        dataButton.interactable = false;
    }

    void DoNuke()
    {
        if (Nuke.missile != null)
        {
            StartCoroutine(DoError("LAUNCH IN PROGRESS!", 5));
            return;
        }
        foreach (Transform b in nukeContent) Destroy(b.gameObject);

        nukePanel.gameObject.SetActive(true);
        Hacking.accessPanel.SetActive(false);

        for (int i = 0; i < targetTags.Length; i++)
        {
            Button b = (Button)Instantiate(Hacking.UIButtonPrefab);
            if (GameObject.FindGameObjectsWithTag(targetTags[i]).Length < 1) b.interactable = false;
            else b.interactable = true;

            b.onClick.AddListener(() =>
            {
                NukeTarget(b.GetComponentInChildren<Text>().text);
            });

            b.transform.SetParent(nukeContent, false);
            b.GetComponentInChildren<Text>().text = targetTags[i];
            Hacking.footerText.text = "SELECT TARGET";
            nukeButton.interactable = false;
        }
    }

    void NukeTarget(string target)
    {
        Hacking.ToggleHacking(false);
        Nuke.targetTag = target;
        Nuke.Fire();
    }

    void DoLock()
    {
        locked = !locked;
        lockButton.interactable = false;
        StartCoroutine(Lock());
    }

    IEnumerator Lock()
    {
        Hacking.footerText.text = "ACCESSING MAGLOCKS";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text = "ACCESSING MAGLOCKS";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text += ".";
        yield return new WaitForSecondsRealtime(1);
        Hacking.footerText.text = locked ? doorTransform.name + " LOCKED" : doorTransform.name + " UNLOCKED";

        //unlock/lock door/safe 
        doorTransform.localRotation = locked ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, -140, 0);
        if (doorTransform.GetComponent<AudioSource>()) doorTransform.GetComponent<AudioSource>().Play();

        yield return new WaitForSecondsRealtime(1);
        Hacking.ToggleHacking(false);
    }

    IEnumerator DoError(string msg, float time)
    {
        string oldtext = Hacking.footerText.text;
        Color oldColor = Hacking.footerText.color;

        Hacking.footerText.color = Color.red;
        Hacking.footerText.text = msg;

        yield return new WaitForSecondsRealtime(time);

        Hacking.footerText.color = oldColor;
        Hacking.footerText.text = oldtext;
    }

    public void WriteJournal(string txt)
    {
        journalEntries.Add(txt);
    }

    public void ReadJournal()
    {
        journalText.text = "JOURNAL ENTRIES:\n";
        foreach (string s in journalEntries) journalText.text += "\n" + s;
    }

    public void TogglePDA()
    {
        if (PDAUI == null) return;
        PDAUI.enabled = !PDAUI.enabled;
        Hacking.Pause = PDAUI.enabled;
    }

    public void ToggleJournal()
    {
        if (JournalPanel == null) return;
        JournalPanel.SetActive(!JournalPanel.activeSelf);
        ReadJournal();
    }

}