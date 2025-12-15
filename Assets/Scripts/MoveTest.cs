using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static Climbing;
using System.Threading;

public class MoveTest : MonoBehaviour
{
    [Header("Climb Waypoint")]
    private Vector3 startPoint;
    public Transform nextWaypoint;
    public Transform currentWaypoint;

    [Header("Camera")]
    // public GameObject ClimbCam;
    public GameObject NormalCam;
    public float animY;
    [Header("Stat")]
    public float speed;
    public float progress = 0;
    public Rigidbody rb;
    public int count;

    [Header("Keys")]
    public KeyCode[] qteList = { KeyCode.R, KeyCode.E, KeyCode.F, KeyCode.T };
    public bool qteRunning = false;
    public KeyCode spamKey = KeyCode.None;
    public KeyCode oneKey = KeyCode.None;
    public KeyCode twoKey1 = KeyCode.None;
    public KeyCode twoKey2 = KeyCode.None;
    private int spamKeyAmount;

    public enum qteType { twoKeyEvent, oneKeyEvent, spamKeyEvent };
    public qteType currentQteType;

    [Header("QTE related")]
    public float currentTime;
    public float reactionTime;
    public Slider reactionSlider;
    public TextMeshProUGUI Qtetext;
    public GameObject qteBox;


    public TutioralConverstaionManager tmac;



    public void Start()
    {
        //ClimbCam.SetActive(false);
        NormalCam.SetActive(true);
        qteBox.SetActive(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && currentWaypoint == null && Input.GetKeyDown(KeyCode.E))
        {
            if (tmac != null)
            {
                if (tmac.index == 2)
                {
                    tmac.SetToLine(3);
                }
            }
            startPoint = transform.position;
            NextWaypoint waypointScript = collision.gameObject.GetComponent<NextWaypoint>();
            if (waypointScript != null)
            {
                Debug.Log("Found waypointScript");
                nextWaypoint = waypointScript.PickWaypoint();
                if (nextWaypoint != null)
                {
                    Debug.Log("Found nextWaypoint");
                    qteBox.SetActive(true);
                    currentWaypoint = nextWaypoint;
                    nextWaypoint = null;
                    progress = 0;
                }
            }
        }
    }


    private void Update()
    {

        if (nextWaypoint != null)
        {
            Debug.Log("nextWaypoint != null");
            if (spamKey == KeyCode.None && oneKey == KeyCode.None && twoKey1 == KeyCode.None && twoKey2 == KeyCode.None)
            {
                CreateQteKeys(RandomQteEvent());
            }
            else
            {
                QteEvent(currentQteType);
            }
        }

        if (currentWaypoint != null)
        {
            // ClimbCam.SetActive(true);
            NormalCam.SetActive(false);
            GetComponent<MovementSystem>().canMove = false;
            progress += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPoint, currentWaypoint.position, progress);
        }

        if (progress > 1)
        {
            CleanQTE();
            GetComponent<MovementSystem>().canMove = true;
            //   ClimbCam.SetActive(false);
            NormalCam.SetActive(true);
            currentWaypoint = null;
            progress = 0;
        }
    }

    public qteType RandomQteEvent()
    {
        Debug.Log("RandomQteEvent");
        int rand = Random.Range(0, 3);
        qteRunning = true;
        qteBox.SetActive(true);
        float createReactionTime = rand switch
        {
            0 => Random.Range(1.5f, 2.5f),
            1 => Random.Range(1.8f, 2.9f),
            _ => Random.Range(5f, 6.8f)
        };
        reactionTime = createReactionTime;
        reactionSlider.value = 1f;

        return (qteType)rand;
    }

    public void CreateQteKeys(qteType type)
    {
        //anim.SetFloat("y", 0);
        if (type == qteType.spamKeyEvent && spamKey == KeyCode.None)
        {
            spamKey = qteList[Random.Range(0, qteList.Length)];
            Qtetext.text = spamKey.ToString();
        }
        else if (type == qteType.oneKeyEvent && oneKey == KeyCode.None)
        {
            oneKey = qteList[Random.Range(0, qteList.Length)];
            Qtetext.text = oneKey.ToString();
        }
        else if (type == qteType.twoKeyEvent && twoKey1 == KeyCode.None && twoKey2 == KeyCode.None)
        {
            do
            {
                twoKey1 = qteList[Random.Range(0, qteList.Length)];
                twoKey2 = qteList[Random.Range(0, qteList.Length)];
            } while (twoKey1 == twoKey2);

            Qtetext.text = $"{twoKey1} + {twoKey2}";
        }
    }

    public void QteEvent(qteType type)
    {
        currentTime += Time.deltaTime;
        reactionSlider.value = 1 - (currentTime / reactionTime);

        if (currentTime < reactionTime)
        {
            if (type == qteType.spamKeyEvent) SpamKeyEvent();
            else if (type == qteType.oneKeyEvent) OneKeyEvent();
            else if (type == qteType.twoKeyEvent) TwoKeyEvent();
        }
        else
        {
            CleanQTE();
        }
    }

    public void CleanQTE()
    {
        reactionTime = 0;
        currentTime = 0;
        nextWaypoint = null;
        currentWaypoint = null;
        qteRunning = false;
        progress = 0;
        spamKey = KeyCode.None;
        oneKey = KeyCode.None;
        twoKey1 = KeyCode.None;
        twoKey2 = KeyCode.None;
        qteBox.SetActive(false);
    }

    public void SpamKeyEvent()
    {
        if (Input.GetKeyDown(spamKey))
        {
            spamKeyAmount++;
            if (spamKeyAmount > 17)
            {
                currentWaypoint = nextWaypoint;
                nextWaypoint = null;
                progress = 0;
                count++;
            }
        }
    }

    public void OneKeyEvent()
    {
        if (Input.GetKeyDown(oneKey))
        {
            currentWaypoint = nextWaypoint;
            nextWaypoint = null;
            progress = 0;
            count++;
        }
    }

    public void TwoKeyEvent()
    {
        if (Input.GetKey(twoKey1) && Input.GetKey(twoKey2))
        {
            currentWaypoint = nextWaypoint;
            nextWaypoint = null;
            progress = 0;
            count++;
        }
    }
}