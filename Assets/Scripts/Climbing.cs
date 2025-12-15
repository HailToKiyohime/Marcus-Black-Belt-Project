using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Climbing : MonoBehaviour
{
    [Header("Waypoint")]
    public NextWaypoint[] nextWaypoints;
    [Header("Game Objects")]
    public MovementSystem fpm;
    public Camera mainCam;
    public GameObject Player;
    public Camera climbCam;
    public GameObject text;
    public TextMeshProUGUI Qtetext;
    public Slider reactionSlider;
    public GameObject qteEventUI;
    public Animator anim;
    public float animY;

    [Header("Waypoint related")]
    public Transform curentWaypoint;
    public Transform nextWaypoint;
    public float progress;
    [Header("Varribles")]
    public float reactionTime; // the amount of time allowed to do a input
    public float currentTime; //the current time
    public bool win;
    public bool fail;

    public float qtEventWaitTime; // the amount of time you need to wait before QTE happen

    [Header("Keys")]
    public KeyCode[] qteList = { KeyCode.R, KeyCode.E, KeyCode.F, KeyCode.T };

    public bool qteRuning = false;
    public KeyCode spamKey = KeyCode.None;
    public KeyCode oneKey = KeyCode.None;
    public KeyCode twoKey1 = KeyCode.None;
    public KeyCode twoKey2 = KeyCode.None;
    private int spamKeyAmount;

    public climbingState currentState;

    public enum qteType { twoKeyEvent,oneKeyEvent,spamKeyEvent};
    public enum climbingState { beforeClimb, startClimbing, midClimbing, endClimbing};

    public qteType currentQteType;

    // before not doing anything, start climbing touching wall but waiting for qte, midClimb qte and climbing, and end climbing is for QTE result
    private Quaternion intRotation;
    // Start is called before the first frame update
    void Start()
    {
        mainCam.gameObject.SetActive(true);
        //climbCam.gameObject.SetActive(false);
        //text.gameObject.SetActive(false);
        currentState = climbingState.beforeClimb;
        qteEventUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Climb();
        reactionSlider.value = 1-(currentTime / reactionTime);
        //fpm.rb.constraints = RigidbodyConstraints.FreezeRotation;

        anim.SetFloat("y", animY);

    }
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Wall") 
        {
            text.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentState = climbingState.startClimbing;
                currentTime = 0;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (currentState != climbingState.endClimbing)
            {
                currentState = climbingState.beforeClimb;
            }
        }

        if (collision.gameObject.CompareTag("Wall"))    
        {
            if (currentState != climbingState.endClimbing) {
                currentState = climbingState.beforeClimb;
                text.gameObject.SetActive(false);
            }
        }
    }

    public void Climb()
    {
        if(currentState == climbingState.beforeClimb)
        {
            Debug.Log("Before climb");
        }

        if (currentState == climbingState.startClimbing)
        {
            Debug.Log("hi jin start climb");
            climbCam.gameObject.SetActive(true);
            gameObject.GetComponent<Rigidbody>().freezeRotation = true;
            currentTime = currentTime + Time.deltaTime;
            if (currentTime > qtEventWaitTime)
            {
                currentState = climbingState.midClimbing;
                currentQteType = RandomQteEvent();
                Debug.Log("hi jin entering mid climb");
                currentTime = 0;
            }
        }

        if (currentState == climbingState.midClimbing)
        {

            qteEventUI.gameObject.SetActive(true);
            CreateQteKeys(currentQteType);
            QteEvent(currentQteType);
        }
        if (currentState == climbingState.endClimbing)
        {
            if (win == true)
            {
                movetoNextPoint(curentWaypoint, nextWaypoint, progress);
                //ResetAll();
            }
            else
            {
                ResetAll();
            }

        }
    }


    public qteType RandomQteEvent()
    {
        Debug.Log("RandomQteEvent");
        int rand = Random.Range(0, 3);
        if(rand == 0)
        {            
            if(qteRuning == false) { 
                float creatReactionTime = Random.Range(1.5f, 2.5f);
                reactionTime = creatReactionTime;
                reactionSlider.value = 1f; // Start the slider at full
                //StartCoroutine(UpdateSlider(creatReactionTime));
            }
            qteRuning = true;
            Debug.Log("hi jin one key is selected");

            return qteType.oneKeyEvent;
            
        }

        else if (rand == 1)
        {            
            if (qteRuning == false)
            {
                float creatReactionTime = Random.Range(1.8f, 2.9f);
                reactionTime = creatReactionTime;
                 reactionSlider.value = 1f; // Start the slider at full
                //StartCoroutine(UpdateSlider(creatReactionTime));
            }
            qteRuning = true;
            Debug.Log("hi jin two key is selected");

            return qteType.twoKeyEvent;
        }

        else
        {            
            if (qteRuning == false)
            {
                float creatReactionTime = Random.Range(5, 6.8f);
                reactionTime = creatReactionTime;
                reactionSlider.value = 1f; // Start the slider at full
                //StartCoroutine(UpdateSlider(creatReactionTime));

            }
            qteRuning = true;
            Debug.Log("hi jin spam key is selected");

            return qteType.spamKeyEvent;
        }

    }

    public void CreateQteKeys(qteType type)
    {
        //Debug.Log("hi jin made new qte keys");
        if (type== qteType.spamKeyEvent && spamKey == KeyCode.None)
        {
            spamKey = qteList[Random.Range(0, qteList.Length)];
            Qtetext.text = spamKey.ToString();
            Debug.Log("hi jin spam key is generated");
        }
        else if (type == qteType.oneKeyEvent && oneKey == KeyCode.None)
        {
            oneKey = qteList[Random.Range(0, qteList.Length)];
            Qtetext.text = oneKey.ToString();
            Debug.Log("hi jin one key is generated");
        }
        else if (type == qteType.twoKeyEvent && twoKey1 == KeyCode.None && twoKey2 == KeyCode.None)
        {

            twoKey1 = qteList[Random.Range(0, qteList.Length)];
            twoKey2 = qteList[Random.Range(0, qteList.Length)];
            while (twoKey1== twoKey2)
            {
                twoKey2 = qteList[Random.Range(0, qteList.Length - 1)];
            }
            Qtetext.text = $"{twoKey1} + {twoKey2}";
            Debug.Log("hi jin two key is generated");
        }
    }

    public void QteEvent(qteType type)
    {
        currentTime = currentTime + Time.deltaTime;
        if (currentTime < reactionTime)
        {
            if (type == qteType.spamKeyEvent && spamKey != KeyCode.None)
            {
                if (Input.GetKeyDown(spamKey))
                {
                    spamKeyAmount = spamKeyAmount + 1;
                    if (spamKeyAmount > 17)
                    {
                        Collider[] wall = Physics.OverlapSphere(gameObject.transform.position, 1f);
                        Debug.Log("hi"+wall);
                        for (int i = 0; i < wall.Length; i++)
                        {
                            if (wall[i].tag == "Wall")
                            {
                                nextWaypoint = wall[i].GetComponent<NextWaypoint>().PickWaypoint();

                                break;
                            }
                            if (wall[i].tag == "Goal")
                            {
                                currentState = climbingState.startClimbing;
                                ResetAll();
                                Debug.Log("end");
                                //break;
                            }
                        }
                            win = true;
                        spamKeyAmount = 0;
                        curentWaypoint = gameObject.transform;
                        currentState = climbingState.endClimbing;
                        cleanKeys();
                        Debug.Log("hi jin spam key finshed entering end climbing");
                    } 
                } 
            }
            else if (type == qteType.oneKeyEvent && oneKey != KeyCode.None)
            {
                if (Input.GetKeyDown(oneKey))
                {
                    Collider[] wall = Physics.OverlapSphere(gameObject.transform.position, 1f);
                    for (int i = 0; i < wall.Length; i++)
                    {
                        if (wall[i].tag == "Wall")
                        {
                            nextWaypoint = wall[i].GetComponent<NextWaypoint>().PickWaypoint();

                            break;
                        }
                        if (wall[i].tag == "Goal")
                        {
                            currentState = climbingState.startClimbing;
                            ResetAll();
                            Debug.Log("end");
                        }
                    }
                    win = true;
                    curentWaypoint = gameObject.transform;
                    currentState = climbingState.endClimbing;
                    cleanKeys();
                    Debug.Log("hi jin one key finshed entering end climbing");
                }
            }
            else if (type == qteType.twoKeyEvent && twoKey1 != KeyCode.None && twoKey2 != KeyCode.None)
            {
                if (Input.GetKey(twoKey1) && Input.GetKey(twoKey2))
                {
                    Collider[] wall = Physics.OverlapSphere(gameObject.transform.position, 1f);
                    for (int i = 0; i < wall.Length; i++)
                    {
                        if (wall[i].tag == "Wall")
                        {
                            nextWaypoint = wall[i].GetComponent<NextWaypoint>().PickWaypoint();
                            break;

                        }
                        if (wall[i].tag == "Goal")
                        {
                            currentState = climbingState.startClimbing;
                            ResetAll();
                            Debug.Log("end");
                        }
                    }
                    win = true;
                    curentWaypoint = gameObject.transform;
                    currentState = climbingState.endClimbing;
                    cleanKeys();
                    Debug.Log("hi jin two key finshed entering end climbing");
                }
            }
        }
        else
        {
            currentState = climbingState.endClimbing;
        }
        /*else if(win == false)
        {
            currentState = climbingState.endClimbing;
        } else if(win == true)
        {

            currentState = climbingState.midClimbing;
        }*/
    }
    void cleanKeys()
    {
        
        progress = 0;
        qteRuning = false;
        spamKey = KeyCode.None;
        oneKey = KeyCode.None;
        twoKey1 = KeyCode.None;
        twoKey2 = KeyCode.None;
        reactionSlider.value = 1f;
  
    }
    void ResetAll()
    {
        progress = 0;
        Debug.Log("ResetAll");
        fpm.rb.constraints = RigidbodyConstraints.FreezeRotation;
        transform.rotation = Quaternion.identity;
        climbCam.gameObject.SetActive(false);
        currentState = climbingState.startClimbing;
        text.gameObject.SetActive(false);
        qteEventUI.gameObject.SetActive(false);
        qteRuning = false;
        win = false;
    }

    void movetoNextPoint(Transform startPos, Transform endPos, float t)
    {
        Debug.Log("progress" + t);
        gameObject.GetComponent<Rigidbody>().useGravity=false;
        progress = progress + Time.deltaTime;
        gameObject.transform.position = Vector3.Lerp(startPos.position,endPos.position, t / 10.5f);
        if (t>1)
        {
            ResetAll();
        }
    }
}
