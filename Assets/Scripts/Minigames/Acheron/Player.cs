using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private AcheronManager acheronManager;
    [SerializeField] private LayerMask platformLayer;

    [SerializeField] private float distanceToGround;
    [SerializeField] private float fallDistance;

    [SerializeField] private float jumpHeight;
    [SerializeField] private float deathY;

    [SerializeField] private float moveAmt;

    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform[] raycastPos;

    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 camClampLeftTop;
    [SerializeField] private Vector3 camClampRightBottom;
    [SerializeField] private float distanceUntilMoveCam;

    private InputAction jump;
    private InputAction walkLeft;
    private InputAction walkRight;
    private InputAction quit;

    private Platform currentPlatform;

    private bool isFalling;
    private bool isOnGround;
    private bool isMoving;
    private bool isGoingLeft;

    private Coroutine moveCoroutine;

    private bool hasFinished;

    private void Awake()
    {
        jump = inputActionAsset.FindAction("Main/Move Up");
        walkLeft = inputActionAsset.FindAction("Main/Move Left");
        walkRight = inputActionAsset.FindAction("Main/Move Right");
        quit = inputActionAsset.FindAction("Main/Quit");

        jump.performed += OnMoveUp;
        walkLeft.started += OnMoveLeft;
        walkLeft.canceled += OnMoveLeftCancel;
        walkRight.started += OnMoveRight;
        walkRight.canceled += OnMoveRightCancel;
        quit.performed += Quit;


        jump.Enable();
        walkLeft.Enable();
        walkRight.Enable();
        quit.Enable();
    }

    private void Start()
    {
        isFalling = true;
    }

    private void FixedUpdate()
    {
        if (isFalling) Fall();
        if (isMoving) Move();
    }

    private void Fall()
    {
        if (rb2d.linearVelocityY > 0) return;

        if (isOnGround)
        {
            isFalling = false;
            return;
        }

        if (transform.position.y <= deathY) 
            acheronManager.Instakill();
    }

    private void FallStop()
    {
        isFalling = false;
        isOnGround = true;
    }

    public void OnMoveUp(InputAction.CallbackContext context)
    {
        if (!isOnGround)
        {
            if (CheckIfOnGround()) isOnGround = true;
            else return;
        }

        StartJump();
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        isGoingLeft = true;
        isMoving = true;
        spriteRenderer.flipX = true;
    }

    private void OnMoveLeftCancel(InputAction.CallbackContext context)
    {
        if (isGoingLeft)
            isMoving = false;
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        isGoingLeft = false;
        isMoving = true;
        spriteRenderer.flipX = false;
    }

    private void OnMoveRightCancel(InputAction.CallbackContext context)
    {
        if (!isGoingLeft)
            isMoving = false;
    }
    
    private void Move()
    {
        Vector2 moveDir = isGoingLeft ? Vector2.left : Vector2.right;
        moveDir *= moveAmt;

        transform.Translate(moveDir * Time.fixedDeltaTime);

        float distance = transform.position.x - cam.transform.position.x;
        if (distance > distanceUntilMoveCam * -1 && distance < distanceUntilMoveCam) return;

        if ((!isGoingLeft && transform.position.x < cam.transform.position.x) || (isGoingLeft && transform.position.x > cam.transform.position.x)) return;

        cam.transform.Translate(moveDir * Time.fixedDeltaTime);
        float x = Mathf.Clamp(cam.transform.position.x, camClampLeftTop.x, camClampRightBottom.x);
        float y = Mathf.Clamp(cam.transform.position.y, camClampLeftTop.y, camClampRightBottom.y);
        float z = cam.transform.position.z;
        Vector3 clampedPos = new(x, y, z);
        cam.transform.position = clampedPos;
    }

    private void StartJump()
    {
        rb2d.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);

        isOnGround = false;
        isFalling = true;
        isOnGround = false;
    }

    private void Quit(InputAction.CallbackContext context)
    {
        End(false, false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Spear"))
            Hit();
        else if (other.gameObject.CompareTag("Finish"))
            End(false, true);
        else if (other.gameObject.CompareTag("Platform") && isFalling)
        {
            if (!CheckIfOnGround()) return;

            FallStop();
            if (currentPlatform)
                currentPlatform.HitPlatform(false);
            currentPlatform = other.gameObject.GetComponent<Platform>();
            currentPlatform.HitPlatform(true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Platform")) return;
        isOnGround = false;
        isFalling = true;
    }

    private bool CheckIfOnGround() => raycastPos.Select(t => Physics2D.Raycast(t.position, -Vector2.up, distanceToGround, platformLayer)).Any(ray => ray.collider != null);

    public void Hit() => acheronManager.Hit();

    public void End(bool dead, bool finish)
    {
        if (hasFinished) return;
        hasFinished = true;
        jump.performed -= OnMoveUp;
        jump.Disable();
        walkLeft.started -= OnMoveLeft;
        walkLeft.canceled -= OnMoveLeftCancel;
        walkLeft.Disable();
        walkRight.started -= OnMoveRight;
        walkRight.canceled -= OnMoveRightCancel;
        walkRight.Disable();
        quit.performed -= Quit;
        quit.Disable();

        StopAllCoroutines();
        
        if (dead)
        {
            acheronManager.ShowDeathScreen();
            if (currentPlatform)
                currentPlatform.HitPlatform(false);
            gameObject.SetActive(false);
        }
        else if (finish)
            acheronManager.Finish();
        else
            acheronManager.Quit();
    }
}
