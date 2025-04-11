using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowText : MonoBehaviour
{
    public Text messageText;         // Riferimento al componente UI Text
    public float displayTime = 7f;   // Quanto tempo il testo resta visibile

    private Coroutine currentRoutine;

    void Start()
    {
        ShowMessage(messageText.text);
    }

    // Chiamata per mostrare un messaggio
    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        messageText.gameObject.SetActive(false);
    }
}
