using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationWindow : MonoBehaviour
{
    [Header("UI Elements")]
    public Button yesButton;
    public Button noButton;
    public TMP_Text messageText;
    private string cachedMessage;


    private void Awake()
    {
        // Make sure the window is hidden at start
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Call this to show the confirmation window with a message
    /// </summary>
   public void Open(string message)
{
    cachedMessage = message; // store it
    gameObject.SetActive(true);

    if (messageText != null)
        messageText.text = cachedMessage; // set text
}


    private void YesClicked()
    {
        Debug.Log("Yes Clicked");
        gameObject.SetActive(false);
        // Add any action you want here when Yes is clicked
    }

    public void NoClicked()
    {
        Debug.Log("No Clicked");
        gameObject.SetActive(false);
    }
}
