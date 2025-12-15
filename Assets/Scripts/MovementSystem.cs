using UnityEngine;
using UnityEngine.SceneManagement;

public class MovementSystem : MonoBehaviour
{
    [Header("Outside of script")]
    public Camera playerCamera;
    public Transform VCamera;
    public Rigidbody rb;
    public GameObject Flag;
    public TutioralConverstaionManager tmac;
    public MovementSystem ms;

    [Header("Adjustables")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public Vector3 moveDirection = Vector3.zero;
    public bool canMove = true;

    [Header("Bools and such")]
    public bool isGrounded = false;
    private float rotationX = 0;
    private float rotationY = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Automatic cursor handling based on scene
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Tut" || sceneName == "World 1") // gameplay scenes
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else // menus / UI scenes
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.2f))
        {
            isGrounded = true;
            Debug.DrawRay(transform.position, -Vector3.up * 3, Color.yellow);
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(transform.position, -Vector3.up * 3, Color.red);
        }

        float xInput = Input.GetAxis("Vertical");
        float yInput = Input.GetAxis("Horizontal");

        moveDirection = (transform.forward * xInput) + (transform.right * yInput);

        if (canMove)
        {
            rb.AddForce(moveDirection * walkSpeed);
        }

        if (Input.GetButton("Jump") && isGrounded && canMove)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
        }
    }

    void Update()
    {
        // Look rotation
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        VCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    // Expose private fields so other scripts can read them
    public bool IsGrounded => isGrounded;
    public Vector3 MoveDirection => moveDirection;
    public Vector3 Velocity => rb.velocity;
}
