using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mt_Hacking : MonoBehaviour
{
    public Button UIButtonPrefab;
    public Canvas HackingUI;

    public string terminalTag = "Terminal";
    public GameObject accessPanel;
    public GameObject hackingPanel;
    public Button exitButton;
    public Text wordText;
    public Text triesText;
    public Text consoleText;
    public Text headerText;
    public Text footerText;
    public List<GameObject> rowPrefab;
    public TextAsset WordList;
    public AudioClip enterSound;
    public AudioClip hoverSound;
    public AudioClip selectSound;
    public AudioClip errorSound;
    public Color highlightColor = Color.white;

    public int triesAllowed = 4;
    int rowsAmount = 18;

    // [HideInInspector]
    public string selectedWord;
    [HideInInspector]
    public string scoreWord = "";
    [HideInInspector]
    public List<string> WORD_LIST;
    Vector2 rowPosition;
    int rowLeftIndex = 0;
    int rowRightIndex = 0;
    bool setup = false;
    Button[] left;
    Button[] right;
    List<Button> buts = new List<Button>();
    Button but;
    string[] tmpTxt;
    [HideInInspector]
    public mt_Terminal term;

    [HideInInspector]
    public int tries;
    [HideInInspector]
    public bool canHack = false;
    [HideInInspector]
    public bool isHacking = false;

    protected GameObject m_Player = null;
    GameObject Player
    {
        get
        {
            if (m_Player == null) m_Player = GameObject.FindWithTag("Player");
            return m_Player;
        }
    }

    protected mt_Success m_Success = null;
    public mt_Success Success
    {
        get
        {
            if (m_Success == null) m_Success = GetComponent<mt_Success>();
            return m_Success;
        }
    }

    protected bool m_cursor = false;
    public bool HideCursor
    {
        get
        {
            return m_cursor;
        }
        set
        {
            m_cursor = value;
            if (canHack) Cursor.lockState = m_cursor ? CursorLockMode.Confined : CursorLockMode.None;
            else Cursor.lockState = m_cursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_cursor;
        }
    }

    protected bool m_Pause;
    public bool Pause
    {
        get
        {
            return m_Pause;
        }
        set
        {
            m_Pause = value;
            Time.timeScale = m_Pause ? 0 : 1;
#if !UNITY_ANDROID && !UNITY_IPHONE
            HideCursor = m_Pause ? false : true;
#endif
        }
    }

    public void Setup()
    {
        WORD_LIST.Clear();
        setup = false;
        GameObject[] row = GameObject.FindGameObjectsWithTag("Row");
        foreach (GameObject go in row) Destroy(go);

        left = null;
        right = null;
        tmpTxt = null;
        rowLeftIndex = 0;
        rowRightIndex = 0;

        but = null;
        buts.Clear();
        buts.TrimExcess();

        tries = triesAllowed;
        footerText.text = "";

        hackingPanel.gameObject.SetActive(false);
        accessPanel.gameObject.SetActive(false);

        if (term.availability == mt_Terminal.AVAILABILITY.LOCKED) LockedStart();
        else if (term.availability == mt_Terminal.AVAILABILITY.NORMAL) NormalStart();
        else if (term.availability == mt_Terminal.AVAILABILITY.UNLOCKED) UnlockedStart();
    }

    protected void Update()
    {
        FindClosestTerminal();
        term = this.GetComponent<mt_Terminal>();
        //UpdateCursorLock();

        if (!canHack) return;

        if (term.availability != mt_Terminal.AVAILABILITY.NORMAL) return;

        if (setup) SetupRows();
        

        if (selectedWord == scoreWord)
        {
            triesText.color = Color.blue;
            triesText.text = "Match Found";
            return;
        }

        if (tries > 1)
        {
            triesText.color = Color.white;
            triesText.text = "Attempts Remaining; " + tries.ToString();
        }

        else if (tries <= 1)
        {
            triesText.text = "LOCKOUT IMMINENT";
            triesText.color = Color.red;
        }
    }

    void UpdateCursorLock()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        return;
#else
        if (Pause || canHack) return;

        if (Input.GetKeyDown(KeyCode.Mouse0)) HideCursor = true;
        if (Input.GetKeyDown(KeyCode.Escape)) HideCursor = false;
#endif
    }

    void NormalStart()
    {

        headerText.text = "Microsoft(R) Windows 95 \n   (C)Copyright Microsoft Corp 1981-1996. \n C:'\'>debug";
        hackingPanel.SetActive(true);
        accessPanel.SetActive(false);

        consoleText.text = "";
        LoadGameWords();
        setup = true;
    }

    void LockedStart()
    {
        triesText.text = "TERMINAL LOCKED";
        triesText.color = Color.red;

        footerText.color = Color.red;
        footerText.text = "SYSTEM HAS BEEN LOCKED DUE TO MALICOUS ACTIVITY, PLEASE CONTACT A SYS-ADMIN TO RESET LOGIN";
    }

    void UnlockedStart()
    {
        headerText.text = "Password Accepted";
        hackingPanel.SetActive(false);
        accessPanel.SetActive(true);

        footerText.text = "LOGGED IN TO ROOT";

        //do stuff when the password is successfully guessed. add your code here
        Success.Unlocked();
    }

    protected void SetupRows()
    {
        if (!setup) return;

        GameObject go = Instantiate(rowPrefab[Random.Range(0, rowPrefab.Count - 1)]);

        if (rowLeftIndex < rowsAmount)
        {
            go.transform.SetParent(GameObject.Find("PanelLeft/Rows").transform, false);
            rowPosition.x = -30;
            rowPosition.y = 100 - (15 * rowLeftIndex);
            rowLeftIndex++;
        }
        else if (rowRightIndex < rowsAmount && rowLeftIndex >= rowsAmount)
        {
            go.transform.SetParent(GameObject.Find("PanelRight/Rows").transform, false);
            rowPosition.x = -30;
            rowPosition.y = 100 - (15 * rowRightIndex);
            rowRightIndex++;
        }

        go.GetComponent<RectTransform>().localPosition = Vector3.zero;
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(rowPosition.x, rowPosition.y);
        GetComponent<AudioSource>().PlayOneShot(hoverSound);
        if (setup && rowLeftIndex >= rowsAmount && rowRightIndex >= rowsAmount)
        {
            SetupScoreWord();
            setup = false;
        }
    }

    protected void SetupScoreWord()
    {
        left = GameObject.Find("PanelLeft/Rows").GetComponentsInChildren<Button>();
        right = GameObject.Find("PanelRight/Rows").GetComponentsInChildren<Button>();
        foreach (Button l in left) buts.Add(l);
        foreach (Button r in right) buts.Add(r);
        but = buts[Random.Range(0, buts.Count - 1)];
        but.GetComponentInChildren<Text>().text = scoreWord;
    }

    protected virtual void LoadGameWords()
    {
        tmpTxt = WordList.text.Split("\n"[0]);
        foreach (string s in tmpTxt)
            WORD_LIST.Add(s);

        scoreWord = WORD_LIST[Random.Range(0, WORD_LIST.Count - 1)];
        Debug.Log("Password for " + term.name + " is " + scoreWord);
    }

    [HideInInspector]
    public Transform closestTerminal;
    public Transform FindClosestTerminal()
    {
        if (Player == null) return null;
        GameObject[] allTerminals = GameObject.FindGameObjectsWithTag(terminalTag);

        float distance = Mathf.Infinity;
        foreach (GameObject go in allTerminals)
        {
            float curDistance = Vector3.Distance(go.transform.position, Player.transform.position);
            if (curDistance < distance)
            {
                distance = curDistance;
                closestTerminal = go.transform;
            }
        }
        return closestTerminal;
    }

    public void ToggleHacking(bool enabled)
    {
        if (HackingUI == null) return;
        if (enterSound) GetComponent<AudioSource>().PlayOneShot(enterSound);
        HackingUI.enabled = enabled;
        canHack = enabled;
        isHacking = enabled;
        if (enabled) Setup();
        Pause = enabled;
    }

}