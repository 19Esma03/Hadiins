using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class FPSPlayerController : MonoBehaviour
{
    [Header("Hareket Hızları")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Zıplama")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -15f;

    [Header("Kamera")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float clampAngle = 80f;

    [Header("Çömelme")]
    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float crouchTransitionSpeed = 8f;

    [Header("Ayak Sesi (opsiyonel)")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepInterval = 0.4f;

    // ── Özel ──
    private CharacterController cc;
    private Animator anim;
    private AudioSource audioSrc;

    private float verticalVelocity = 0f;
    private float cameraPitch = 0f;
    private bool isCrouching = false;
    private bool isGrounded = false;

    // Havadayken yatay hızı kilitleriz
    private Vector3 lockedHorizontalVelocity = Vector3.zero;
    private bool wasGrounded = true;

    private static readonly int HashIsWalk = Animator.StringToHash("isWalk");
    private static readonly int HashIsRun = Animator.StringToHash("isRun");
    private static readonly int HashIsCouch = Animator.StringToHash("isCouch");
    private static readonly int HashIsAir = Animator.StringToHash("isAir");
    private static readonly int HashJump = Animator.StringToHash("Jump");
    private static readonly int HashIsWaC = Animator.StringToHash("isWalkAndCrouch");

    private float footstepTimer = 0f;

 
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cc.height = standHeight;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
        HandleFootsteps();
    }


    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -clampAngle, clampAngle);

        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }


    private void HandleMovement()
    {
        isGrounded = cc.isGrounded;

        // ── Girişi her frame oku ──
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // ── Basılı tutma kontrolü ──
        // Tuş bırakıldığı AN sıfır — GetAxisRaw zaten 0/1 verir, Lerp yok
        bool hasInput = (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching && v > 0.1f;

        float targetSpeed = isCrouching ? crouchSpeed
                          : isRunning ? runSpeed
                          : walkSpeed;

        Vector3 inputDir = transform.right * h + transform.forward * v;
        if (inputDir.magnitude > 1f) inputDir.Normalize();

        // ── DÜZELTME 1: Jump'ta yatay hareket kilidi ──
        // Yere değince: yatay hızı girişten hesapla
        // Havadayken: yere basmadan önceki yatay hızı kullan (sıfır ise sıfır)
        if (isGrounded)
        {
            // Yere yeni değdik mi? Kilit serbest
            lockedHorizontalVelocity = hasInput ? inputDir * targetSpeed : Vector3.zero;
        }
        // Havadayken lockedHorizontalVelocity değişmez → ileri gitmiyor

        // Gravity
        if (isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -0.5f; // daha küçük, yere yapıştırır
        }

        // Zıplama — sadece yerdeyken
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger(HashJump);
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Tek cc.Move — yatay + dikey birleşik
        Vector3 move = lockedHorizontalVelocity;
        move.y = verticalVelocity;

        cc.Move(move * Time.deltaTime);

        // ── DÜZELTME 2: Animator — tuş bırakılınca ANINDA idle ──
        // hasInput false olduğu anda isWalk ve isRun false → blend tree idle'a döner
        anim.SetBool(HashIsWalk, hasInput && !isRunning && isGrounded);
        anim.SetBool(HashIsRun, hasInput && isRunning && isGrounded);
        anim.SetBool(HashIsAir, !isGrounded);
        anim.SetBool(HashIsCouch, isCrouching);
        anim.SetBool(HashIsWaC, hasInput && isCrouching && isGrounded);

        wasGrounded = isGrounded;
    }


    private void HandleCrouch()
    {
        // ── DÜZELTME 3: GetKey — basılı tuttuğun sürece çömelir, bırakınca kalkar ──
        bool wantCrouch = Input.GetKey(KeyCode.C);

        // Kalkmak isteyince baş üstü kontrol
        if (!wantCrouch && isCrouching && HeadBlocked())
            wantCrouch = true; // zorla çömelmeye devam et

        isCrouching = wantCrouch;

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        cc.height = Mathf.Lerp(cc.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

        Vector3 center = cc.center;
        center.y = cc.height / 2f;
        cc.center = center;
    }

    private bool HeadBlocked()
    {
        Vector3 origin = transform.position + Vector3.up * crouchHeight;
        return Physics.Raycast(origin, Vector3.up, standHeight - crouchHeight + 0.1f);
    }

    // ─────────────────────────────────────────────────
    private void HandleFootsteps()
    {
        if (!isGrounded || footstepSounds == null || footstepSounds.Length == 0) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool moving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;

        if (!moving) { footstepTimer = 0f; return; }

        bool running = Input.GetKey(KeyCode.LeftShift);
        float interval = running ? footstepInterval * 0.6f : footstepInterval;

        footstepTimer += Time.deltaTime;
        if (footstepTimer >= interval)
        {
            footstepTimer = 0f;
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            audioSrc.PlayOneShot(clip, 0.6f);
        }

    }

}