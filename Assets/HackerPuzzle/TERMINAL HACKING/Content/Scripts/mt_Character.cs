using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class mt_Character : MonoBehaviour
{
    public enum TYPE { LETTER, WORD }
    public TYPE type = TYPE.WORD;

    string[] alphabet;
    Button button;

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
        GetCharacter();
    }

    void GetCharacter()
    {
        if (type == TYPE.WORD)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                ConfirmEntry();
            });
            GetComponentInChildren<Text>().text = Hacking.WORD_LIST[Random.Range(0, Hacking.WORD_LIST.Count - 1)];
        }

        else if (type == TYPE.LETTER)
        {
            alphabet = new string[54] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+", "-", "=", "{", "}", "[", "]", "|", ":", ";", "'", "<", ">", ",", ".", "/", "?" };
            GetComponentInChildren<Text>().text = alphabet[Random.Range(0, alphabet.Length)];
        }
    }

    void ConfirmEntry()
    {
        Hacking.tries--;
        Hacking.selectedWord = Hacking.wordText.text;
        if (Hacking.selectedWord == Hacking.scoreWord)
        {
            Hacking.consoleText.text += "\nMatch Found: \n" + Hacking.selectedWord;
            Hacking.GetComponent<AudioSource>().PlayOneShot(Hacking.selectSound);
            //StartCoroutine(Hacking.closestTerminal.GetComponent<mt_Terminal>().DoSuccess());
        }

        else if (Hacking.tries > 1)
        {
            Hacking.consoleText.text += "\nInvalid Response: \n" + Hacking.selectedWord;
            Hacking.GetComponent<AudioSource>().PlayOneShot(Hacking.errorSound);
            GetComponent<Button>().interactable = false;
            ReportMatch();
        }

        else if (Hacking.tries <= 1)
        {
            //StartCoroutine(Hacking.closestTerminal.GetComponent<mt_Terminal>().DoFail());
        }

        
    }

    void ReportMatch()
    {
        string txt0 = Hacking.selectedWord;
        string txt1 = Hacking.scoreWord;
        int cor = 0;

        if (txt0[0] == txt1[0]) cor++;
        if (txt0[1] == txt1[1]) cor++;
        if (txt0[2] == txt1[2]) cor++;
        if (txt0[3] == txt1[3]) cor++;
        if (txt0[4] == txt1[4]) cor++;
        if (txt0[5] == txt1[5]) cor++;
        if (txt0[6] == txt1[6]) cor++;

        Hacking.consoleText.text += "\n" + cor + "/" + txt0.Length.ToString() + " correct";
    }

}