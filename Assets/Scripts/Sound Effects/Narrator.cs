using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script responsible to control the Narrator's voice
public class Narrator : MonoBehaviour
{
    public static Narrator instance = null;
    public List<AudioClip> combos;
    public GameObject ComboUI;
    public AudioSource narrator;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        instance = GetComponent<Narrator>();
        narrator = this.GetComponent<AudioSource>();
    }

    public void PlayCombo(int combo)
    {
        if (combo == 0)
            CreateComboUI("Good");
        else if (combo == 1)
            CreateComboUI("Grape");
        else if (combo == 2)
            CreateComboUI("Eggcellent");
        else if (combo == 3)
            CreateComboUI("Foodelicious");

        PlayNarrator(combos[combo], false);
    }

    public void PlayNarrator(AudioClip clip, bool wait)
    {
        narrator.Stop();

        if(wait)
            StartCoroutine(Wait(clip));
        else
            narrator.PlayOneShot(clip);
    }

    public IEnumerator Wait(AudioClip clip)
    {
        yield return new WaitForSeconds(1);
        narrator.PlayOneShot(clip);
    }

    public void CreateComboUI(string combo_line)
    {
        if(this.gameObject.transform.childCount > 0)
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
            
        // Create new Points UI
        GameObject newCombo = Instantiate(ComboUI, new Vector3(0, 0, 0), ComboUI.transform.rotation);
        newCombo.transform.SetParent(this.gameObject.transform);
        RectTransform combo_r = newCombo.GetComponent<RectTransform>();

        // Place it in the middle of the match
        combo_r.anchoredPosition = Board_side.instance.getPosition(4,-2);
        combo_r.localScale = new Vector3(1, 1, 1);
        combo_r.sizeDelta = new Vector2(Board_side.instance.size / 2, Board_side.instance.size / 12);

        newCombo.GetComponent<Text>().text = combo_line;
        StartCoroutine(DestroyComboUI());
    }

    // Function that after 1 seconds will destroy the primogenitor
    public IEnumerator DestroyComboUI()
    {
        yield return new WaitForSeconds(2);
        if(this.gameObject.transform.childCount > 0)
            Destroy(this.gameObject.transform.GetChild(0).gameObject);
    }
}
