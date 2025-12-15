using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public enum QTEType { OneKey, TwoKey, SpamKey }

public class QTESystem : MonoBehaviour
{
    // References
    public MovementSystem playerMovement;
    public TutioralConverstaionManager tmac;
    public GameObject mainCam;
    public GameObject climbCam;
    public Audio Audio;
    public GameObject playerMesh;   // <-- added
    public Animator animator;

    // QTE keys
    public KeyCode[] qteList = { KeyCode.R, KeyCode.E, KeyCode.F, KeyCode.T };

    // UI
    public Slider qteTimerSlider;
    public TMP_Text qtePromptText;
    public GameObject hitBoxText;
    public ParticleSystem winParticle;

    // Movement after success/fail
    [Header("Movement")]
    public float moveDuration = 1f;

    // QTE timing
    public float qteDuration = 3f;
    public int spamRequired = 5;

    // Runtime QTE state
    private bool isPlayerInWallTrigger;
    private bool isPlayerInRestWallTrigger;
    private NextWaypoint currentNextWaypoint;
    private QTEType qteType;
    private KeyCode[] selectedKeys;
    private float qteStartTime;
    public bool isQTEActive;
    private bool[] keyPressedFlags;
    private int spamCount;

    // Prevent double-trigger while inside the same collider
    private float lastQTEExitTime = -999f;
    private float qteCooldown = 1f;
    private bool requireExitBeforeNextQTE = false;
    private Collider activeTrigger;

    // Auto-move flag
    public bool isAutoMoving = false;

    // Checkpoint fallback
    [Header("Checkpoints (optional)")]
    [SerializeField] private Transform initialWaypoint;
    private Vector3 lastSafePos;

    public void Start()
    {
        playerMesh.SetActive(false);   // <-- same as Script 1
        Audio.triggerPlay = false;

        winParticle.Stop();
        if (hitBoxText) hitBoxText.SetActive(false);
        if (qteTimerSlider) qteTimerSlider.gameObject.SetActive(false);
        if (animator != null) animator.Play("idle");

        lastSafePos = initialWaypoint ? initialWaypoint.position : playerMovement.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            isPlayerInWallTrigger = true;
            activeTrigger = other;
            currentNextWaypoint = other.GetComponent<NextWaypoint>();
        }
        else if (other.CompareTag("RestWall"))
        {
            isPlayerInRestWallTrigger = true;
            activeTrigger = other;
            currentNextWaypoint = other.GetComponent<NextWaypoint>();
            if (hitBoxText) hitBoxText.SetActive(true);
        }
        else if (other.CompareTag("Goal"))
        {
            playerMesh.SetActive(false);   // <-- same as Script 1
            winParticle.Play();
            Invoke(nameof(SwitchSceneWin), 5f);
        }
        else if (other.CompareTag("GoalWin"))
        {
            playerMesh.SetActive(false);   // <-- same as Script 1
            winParticle.Play();
            Invoke(nameof(SwitchSceneUltimateWin), 5f);
        }
        else if(other.CompareTag("Truck"))
        {
            Invoke(nameof(SwitchSceneMap), 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == activeTrigger)
        {
            requireExitBeforeNextQTE = false;
            activeTrigger = null;
            lastQTEExitTime = Time.time;
        }

        if (other.CompareTag("Wall"))
        {
            isPlayerInWallTrigger = false;
            if (currentNextWaypoint != null && other.gameObject == currentNextWaypoint.gameObject)
                currentNextWaypoint = null;
        }
        else if (other.CompareTag("RestWall"))
        {
            isPlayerInRestWallTrigger = false;
            if (hitBoxText) hitBoxText.SetActive(false);
            if (currentNextWaypoint != null && other.gameObject == currentNextWaypoint.gameObject)
                currentNextWaypoint = null;
        }
    }

    public void SwitchSceneWin()
    {
        winParticle.Stop();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Winning Scene");
    }

    public void SwitchSceneUltimateWin()
    {
        winParticle.Stop();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("WinningUltimate");
    }
    public void SwitchSceneMap()
    {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Map");
    }

    private void Update()
    {
        if (!isQTEActive)
        {
            if (isAutoMoving) return;
            if (requireExitBeforeNextQTE) return;
            if (Time.time - lastQTEExitTime < qteCooldown) return;

            if (isPlayerInRestWallTrigger && Input.GetKeyDown(KeyCode.E))
            {
                if (tmac) tmac.SetToLine(3);
                StartQTE();
            }
            else if (isPlayerInWallTrigger)
            {
                StartQTE();
            }
            else return;
        }

        if (qteTimerSlider != null)
        {
            float remaining = Mathf.Clamp(qteDuration - (Time.time - qteStartTime), 0f, qteDuration);
            qteTimerSlider.value = remaining;
        }

        if (Time.time - qteStartTime > qteDuration)
        {
            EndQTEFailure();
            return;
        }

        switch (qteType)
        {
            case QTEType.OneKey:
                if (Input.GetKeyDown(selectedKeys[0])) EndQTESuccess();
                break;

            case QTEType.TwoKey:
                for (int i = 0; i < selectedKeys.Length; i++)
                {
                    if (!keyPressedFlags[i] && Input.GetKeyDown(selectedKeys[i]))
                        keyPressedFlags[i] = true;
                }
                if (System.Array.TrueForAll(keyPressedFlags, b => b)) EndQTESuccess();
                break;

            case QTEType.SpamKey:
                if (Input.GetKeyDown(selectedKeys[0]))
                {
                    spamCount++;
                    if (spamCount >= spamRequired) EndQTESuccess();
                }
                break;
        }

        if (playerMovement != null && animator != null)
        {
            bool climbingNow = isQTEActive || isAutoMoving;
            animator.SetBool("isClimbing", climbingNow);
        }
    }

    private void StartQTE()
    {
        playerMesh.SetActive(true);   // <-- same as Script 1
        if (climbCam) climbCam.SetActive(true);
        if (mainCam) mainCam.SetActive(false);
        Audio.triggerPlay = true;

        isQTEActive = true;
        requireExitBeforeNextQTE = true;
        if (playerMovement != null) playerMovement.canMove = false;

        int typeCount = System.Enum.GetValues(typeof(QTEType)).Length;
        qteType = (QTEType)Random.Range(0, typeCount);
        selectedKeys = (qteType == QTEType.TwoKey)
            ? SelectTwoKeys()
            : new KeyCode[1] { qteList[Random.Range(0, qteList.Length)] };

        qteStartTime = Time.time;
        if (qteType == QTEType.TwoKey) keyPressedFlags = new bool[selectedKeys.Length];
        if (qteType == QTEType.SpamKey) spamCount = 0;

        if (qteTimerSlider != null)
        {
            qteTimerSlider.gameObject.SetActive(true);
            qteTimerSlider.maxValue = qteDuration;
            qteTimerSlider.value = qteDuration;
        }
        if (qtePromptText != null)
        {
            qtePromptText.text = (qteType == QTEType.TwoKey)
                ? selectedKeys[0] + " + " + selectedKeys[1]
                : selectedKeys[0].ToString();
            qtePromptText.gameObject.SetActive(true);
        }
    }

    private KeyCode[] SelectTwoKeys()
    {
        int first = Random.Range(0, qteList.Length);
        int second;
        do { second = Random.Range(0, qteList.Length); } while (second == first);
        return new KeyCode[] { qteList[first], qteList[second] };
    }

    private void EndQTESuccess()
    {
        if (tmac) tmac.SetToLine(4);
        isQTEActive = false;
        HideUI();
        if (currentNextWaypoint != null && playerMovement != null)
        {
            Transform next = currentNextWaypoint.PickWaypoint();
            StartCoroutine(MovePlayerTo(next.position, setAsCheckpoint: true));
        }
        else
        {
            RestorePlayerMovement();
        }
    }

    private void EndQTEFailure()
    {
        isQTEActive = false;
        HideUI();

        bool inRest = isPlayerInRestWallTrigger && activeTrigger != null && activeTrigger.CompareTag("RestWall");
        bool inWall = isPlayerInWallTrigger && activeTrigger != null && activeTrigger.CompareTag("Wall");

        if (inRest)
        {
            RestorePlayerMovement();
            return;
        }

        Vector3 backPos = lastSafePos;
        if (inWall && currentNextWaypoint != null && currentNextWaypoint.TryPickPast(out Transform past))
            backPos = past.position;

        requireExitBeforeNextQTE = true;
        StartCoroutine(MovePlayerTo(backPos, setAsCheckpoint: false));
    }

    private IEnumerator MovePlayerTo(Vector3 targetPosition, bool setAsCheckpoint)
    {
        isAutoMoving = true;
        float elapsed = 0f;
        Vector3 start = playerMovement.transform.position;

        if (playerMovement != null) playerMovement.canMove = false;
        if (animator != null) animator.SetBool("isClimbing", true);

        while (elapsed < moveDuration)
        {
            playerMovement.transform.position = Vector3.Lerp(start, targetPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerMovement.transform.position = targetPosition;

        if (setAsCheckpoint) lastSafePos = targetPosition;

        if (!isPlayerInWallTrigger && !isPlayerInRestWallTrigger)
        {
            requireExitBeforeNextQTE = false;
            activeTrigger = null;
            lastQTEExitTime = Time.time;
        }

        isAutoMoving = false;
        RestorePlayerMovement();
    }

    private void RestorePlayerMovement()
    {
        if (playerMovement != null) playerMovement.canMove = true;
        if (climbCam) climbCam.SetActive(true);
        if (mainCam) mainCam.SetActive(true);
        if (animator != null)
        {
            animator.SetBool("isClimbing", false);
            animator.Play("idle");
        }
        Audio.triggerPlay = false;
        playerMesh.SetActive(false);   // <-- same as Script 1
    }

    private void HideUI()
    {
        if (qteTimerSlider != null) qteTimerSlider.gameObject.SetActive(false);
        if (qtePromptText != null) qtePromptText.gameObject.SetActive(false);
        if (hitBoxText != null && !isPlayerInRestWallTrigger) hitBoxText.SetActive(false);
    }
}
