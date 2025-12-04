using UnityEngine;
using TMPro;
using System.Collections;

public class DialogHandler : MonoBehaviour
{
    private static DialogHandler _i;
    public static DialogHandler i { get { return _i; } }

    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogWindow;
    [SerializeField] private DialogData dialogToDisplay;
    [SerializeField] private GameObject fastForwardButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private float dialogSpeed;
    
    void Awake()
    {
        _i = this;
    }

    public void FinishDialog()
    {
        if (dialogPanel.activeInHierarchy == false)
        {
            fastForwardButton.SetActive(false);
            return;
        }

        StopAllCoroutines();
        dialogWindow.text = dialogToDisplay.text;
        startButton.SetActive(true);
    }

    public void StartDialog()
    {
        fastForwardButton.SetActive(true);
        startButton.SetActive(false);

        dialogWindow.text = string.Empty;
        StartCoroutine(ReadText());
    }

    IEnumerator ReadText()
    {
        foreach (char c in dialogToDisplay.text.ToCharArray())
        {
            dialogWindow.text += c;
            SoundManager.PlaySound(SoundManager.Sound.textSound);
            yield return new WaitForSeconds(dialogSpeed);
            
        }
        fastForwardButton.SetActive(false);
        startButton.SetActive(true);

    }   
}
