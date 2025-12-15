using UnityEngine;

public class QuitUI : MonoBehaviour
{
    public GameObject confirmationPanel;

    // Shows the confirmation popup
    public void ShowConfirmation()
    {
        confirmationPanel.SetActive(true);
    }

    // Quits the game when "Yes" is clicked
    public void QuitGame()
    {
        Debug.Log("Quitting game!");
        Application.Quit(); // works in built game
    }

    // Hides the confirmation popup when "No" is clicked
    public void CancelQuit()
    {
        confirmationPanel.SetActive(false);
    }
}
