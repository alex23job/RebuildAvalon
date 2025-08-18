using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //[SerializeField] private float speedRot = 180f;
    [SerializeField, Range(0, 180f)] private float _rotationSmoothness;    // Коэффициент плавности поворота
    // ��������� ���������
    [SerializeField] private float moveSpeed = 5f;              // �������� ��������
    [SerializeField] private float jumpForce = 10f;             // ���� ������
    private Rigidbody rb;                       // Rigidbody ���������


    // �������� ��������������
    private bool isGrounded;

    private Vector3 movement = Vector3.zero;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // �������������
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 rot = transform.rotation.eulerAngles;
        rot.y += Time.deltaTime * speedRot;
        transform.rotation = Quaternion.Euler(rot);*/
        MovePlaer();
    }

    // ��������� ������� �����������
    public void OnMove(InputAction.CallbackContext context)
    {
        // �������� ����������� ��������
        Vector2 moveInput = context.ReadValue<Vector2>();

        // ���������� ���������
        //rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.y * moveSpeed);
        
        movement = moveInput.x * transform.right + moveInput.y * transform.forward;
        /*rb.AddForce(movement.normalized * moveSpeed);
        movement.Normalize();

        // Плавный поворот в сторону движения
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        }*/

    }

    private void MovePlaer()
    {
        //movement = moveInput.x * transform.right + moveInput.y * transform.forward;
        rb.AddForce(movement.normalized * moveSpeed);
        movement.Normalize();

        // Плавный поворот в сторону движения
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        }

    }

    // ��������� ������� ������
    public void OnJump(InputAction.CallbackContext context)
    {
        // ��������� ������, ���� �������� �� �����
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // �������� ���������� �� �����
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.1f, ~0, QueryTriggerInteraction.Ignore);
    }

}
