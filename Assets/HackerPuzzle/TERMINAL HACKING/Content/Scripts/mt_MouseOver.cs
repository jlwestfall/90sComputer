using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mt_MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color originalColor;

    protected mt_Hacking m_Hacking = null;
    mt_Hacking Hacking
    {
        get
        {
            if (m_Hacking == null) m_Hacking = FindObjectOfType<mt_Hacking>();
            return m_Hacking;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        originalColor = GetComponentInChildren<Text>().color;
        GetComponentInChildren<Text>().color = Hacking.highlightColor;
        if (Hacking.hoverSound) Hacking.GetComponent<AudioSource>().PlayOneShot(Hacking.hoverSound);

        if (GetComponent<mt_Character>())
        {
            string selected = GetComponentInChildren<Text>().text.ToUpper();
            Hacking.wordText.enabled = true;
            Hacking.wordText.text = selected;
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = originalColor;
        Hacking.wordText.enabled = false;
    }
}