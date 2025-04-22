using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewMonitor : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Light lightSource;

    Coroutine monitorOnRoutine;

    //panels
    [SerializeField] Image startupPanel;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject entrancePanel;
    [SerializeField] GameObject mainPanel;

    public bool isFunctional;

    public virtual void TurnMonitorOn()
    {
        monitorOnRoutine = StartCoroutine(TurnMonitorOnRoutine());
    }

    public virtual void TurnMonitorOff()
    {
        if (monitorOnRoutine != null)
        {
            StopCoroutine(monitorOnRoutine);
        }

        entrancePanel.SetActive(false);
        loadingPanel.SetActive(false);
        startupPanel.gameObject.SetActive(false);

        if(mainPanel != null)
        {
            mainPanel.SetActive(false);
        }

        canvas.enabled = false;
        isFunctional = false;
        lightSource.enabled = false;

    }

    IEnumerator TurnMonitorOnRoutine()
    {
        canvas.enabled = true;

        yield return new WaitForSeconds(2f);

        lightSource.enabled = true;

        startupPanel.fillAmount = 0.32f;
        startupPanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        startupPanel.fillAmount = 0.32f;

        yield return new WaitForSeconds(1f);
        startupPanel.fillAmount = 0.6f;

        yield return new WaitForSeconds(1f);
        startupPanel.fillAmount = 1f;

        yield return new WaitForSeconds(4f);
        startupPanel.gameObject.SetActive(false);
        loadingPanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(7f);
        loadingPanel.gameObject.SetActive(false);
        entrancePanel.SetActive(true);

        isFunctional = true;
       
    }
}
