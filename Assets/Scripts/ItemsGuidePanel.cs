using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ItemsGuidePanel : MonoBehaviour
{
    public List<string> itemTitles;
    public List<string> itemDescriptions;

    [SerializeField] TextMeshProUGUI titleTf;
    [SerializeField] TextMeshProUGUI descriptionTf;
    [SerializeField] TextMeshProUGUI itemCountTf;

    [SerializeField] GameObject descriptionHolder;
    [SerializeField] GameObject menuHolder;

    [SerializeField] DragableObject dragScript;

    public int currentIndex = 0;

    private void OnLocaleInitialized(Locale locale)
    {
        titleTf.text = LocalizationSettings.StringDatabase.GetLocalizedString("Menu items description", itemTitles[0]);
        descriptionTf.text = LocalizationSettings.StringDatabase.GetLocalizedString("Menu items description", itemDescriptions[0]);
    }

    private void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleInitialized;
        MenuManager.OnLocaleInitialized.AddListener(OnLocaleInitialized);
        //LocalizationSettings.SelectedLocaleChanged += OnLocaleInitialized;
        //StartCoroutine(LocalizationRoutine());
        itemCountTf.text = $"[1/{itemTitles.Count}]";
    }

    public void ToAllItems()
    {
        if (MessagePrint.Instance._isCoroutineOngoing)
            MessagePrint.Instance.StopPrinting();

        descriptionHolder.SetActive(false);
        menuHolder.SetActive(true);
    }
    public void NextItem()
    {
        if (MessagePrint.Instance._isCoroutineOngoing)
            MessagePrint.Instance.StopPrinting();

        int _currentIndex = currentIndex == itemTitles.Count - 1 ? 0 : currentIndex+1;
        currentIndex = _currentIndex;
        Display(itemTitles[currentIndex], itemDescriptions[currentIndex], currentIndex);
        
    }
    public void PreviosuItem()
    {
        if (MessagePrint.Instance._isCoroutineOngoing)
            MessagePrint.Instance.StopPrinting();

        int _currentIndex = currentIndex == 0 ? itemTitles.Count - 1 : currentIndex-1;
        currentIndex = _currentIndex;
        Display(itemTitles[_currentIndex], itemDescriptions[_currentIndex], currentIndex);
    }

    public void Display(string title, string description, int index)
    {
        currentIndex = index;
        string _title = LocalizationSettings.StringDatabase.GetLocalizedString("Menu items description", title);
        string _description = LocalizationSettings.StringDatabase.GetLocalizedString("Menu items description", description);
        dragScript.DisplayItem(index);
        titleTf.text = _title;
        MessagePrint.Instance.Message(_description, descriptionTf);
        itemCountTf.text = $"[{index+1}/{itemTitles.Count}]";
    }

    public void OnItemClick(int index)
    {
        menuHolder.SetActive(false);
        descriptionHolder.SetActive(true);
        Display(itemTitles[index], itemDescriptions[index], index);
    }

    IEnumerator LocalizationRoutine()
    {
       
        while (LocalizationSettings.AvailableLocales.Locales.Count == 0)
            yield return null;

        
    }

}
