using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWorld1 : MonoBehaviour
{
    public GameObject World1Button;
    public GameObject locks;

    private void Start()
    {
        bool unlocked = GameData.worldCount >= 1;

        World1Button.SetActive(unlocked);
        locks.SetActive(!unlocked);
    }

    public void World1Load()
    {
        if (GameData.worldCount >= 1)
        {
            SceneManager.LoadScene("World 1");
        }
    }
}
