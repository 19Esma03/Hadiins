using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f; // Zýplama gücü eklendi

    [Header("Çömelme Ayarlarý")]
    public float normalHeight = 1.58f;
    public float crouchHeight = 1f;

    [Header("Referanslar")]
    public Animator anim;

    private CharacterController controller;
    private float currentSpeed;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Yere Deðme ve Yerçekimi
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Klavye Girdileri (WASD)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Vektörü normalize ediyoruz ki çapraz giderken karakter ekstra hýzlanmasýn
        Vector3 move = (transform.right * x + transform.forward * z).normalized;

        // 3. Durum Kontrolleri (Kesin Mantýk)
        // Eðer x veya z 0'dan farklýysa kesinlikle bir tuþa basýlýyordur
        bool isMoving = (x != 0 || z != 0);
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // 4. Zýplama (Space)
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Hýz ve Boyut Ayarlamalarý
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
            controller.height = crouchHeight;
        }
        else if (isSprinting && isMoving)
        {
            currentSpeed = sprintSpeed;
            controller.height = normalHeight;
        }
        else
        {
            currentSpeed = walkSpeed;
            controller.height = normalHeight;
        }

        // 6. Fiziksel Hareket Uygulamasý
        controller.Move(move * currentSpeed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        // 7. GÜNCELLENMÝÞ ANÝMASYON BAÐLANTILARI
        if (anim != null)
        {
            // Parametre isimleri ekran görüntündeki gibi tam eþleþtirildi
            anim.SetBool("isWalk", isMoving && !isSprinting && !isCrouching);
            anim.SetBool("isRun", isMoving && isSprinting && !isCrouching);
            anim.SetBool("isCouch", isCrouching);
            anim.SetBool("isJump", !controller.isGrounded);
        }
    }
}