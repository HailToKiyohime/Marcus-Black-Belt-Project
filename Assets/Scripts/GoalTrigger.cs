using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    public TutioralConverstaionManager tmac;
    public GameObject particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {

            GameData.worldCount++;

            if (tmac != null && tmac.index == 4)
            {
                tmac.SetToLine(5);
            }
        }

        if (other.CompareTag("Truck"))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
