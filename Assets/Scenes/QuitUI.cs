using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // This method is called when the Yes button is clicked
    public void Quit()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // Only works in a built game
    }
}
