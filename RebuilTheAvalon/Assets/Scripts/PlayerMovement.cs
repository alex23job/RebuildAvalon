using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody))]
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
    private Vector3 rotation = Vector3.zero;
    private Vector3 oldPos = Vector3.zero;

    private float hor, ver;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // �������������
    void Start()
    {
        transform.gameObject.SetActive(false);
        Invoke("RestoredPosAndRot", 0.1f);
    }

    private void RestoredPosAndRot()
    {
        try
        {
            transform.position = GameManager.Instance.currentPlayer.oldPosition;
            transform.rotation = Quaternion.Euler(GameManager.Instance.currentPlayer.oldRotation);
            print($"Recover pos={transform.position} rot={transform.rotation}");
            transform.gameObject.SetActive(true);
        }
        catch
        {
            Debug.Log("No GameManager loaded !");
        }

    }

    // Update is called once per frame
    void Update()
    {
        hor = UnityEngine.Input.GetAxis("Horizontal");
        ver = UnityEngine.Input.GetAxis("Vertical");
        /*Vector3 rot = transform.rotation.eulerAngles;
        rot.y += Time.deltaTime * speedRot;
        transform.rotation = Quaternion.Euler(rot);*/
        //MovePlaer();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //rb.AddForce(movement, ForceMode.Impulse);
        Move(ver);
        Turn(hor);
    }
    private void Move(float input)
    {
        //if (input > 0.99f) runStart += Time.deltaTime;
        //else runStart = 0;
        //if (runStart < 3f) input = (input > 0.95f) ? 0.9f : input;
        float mult = 1f;
        //if (runStart >= 3f) mult = 2f;
        //transform.Translate(Vector3.forward * input * moveSpeed * mult * Time.fixedDeltaTime);//Можно добавить Time.DeltaTime
        rb.AddForce(transform.forward * input * moveSpeed * mult * Time.fixedDeltaTime);
        //anim.SetFloat("speed", Mathf.Abs(input));
    }

    private void Turn(float input)
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, input * _rotationSmoothness * Time.fixedDeltaTime, 0));
        //transform.Rotate(0, input * _rotationSmoothness * Time.deltaTime, 0);
    }

    // ��������� ������� �����������
    public void OnMove(InputAction.CallbackContext context)
    {
        // �������� ����������� ��������
        Vector2 moveInput = context.ReadValue<Vector2>();
        //print($"moveInput={moveInput}");

        //transform.Translate(moveInput.y * moveSpeed * transform.forward);
        //transform.Rotate(0, moveInput.x * _rotationSmoothness, 0);


        // ���������� ���������
        //rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.y * moveSpeed);

        movement = moveInput.x * transform.right + moveInput.y * transform.forward;
        //movement = moveInput.y * transform.forward;
        rotation = moveInput.x * transform.right;
        //rotation = moveInput.x * Vector3.right;

        //rb.AddForce(movement.normalized * moveSpeed);
        //movement.Normalize();

        //// Плавный поворот в сторону движения
        //if (movement != Vector3.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(movement);
        //    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness);
        //}

    }

    private void MovePlaer()
    {
        //movement = moveInput.x * transform.right + moveInput.y * transform.forward;
        //Vector3 move = movement; // + rotation;
        //rb.AddForce(move.normalized * moveSpeed);
        //movement.Normalize();

        //Плавный поворот в сторону движения
        //if (transform.position != oldPos)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(movement);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        //    oldPos = transform.position;
        //}

        //if (movement != Vector3.zero || rotation != Vector3.zero)
        //{
        //    //Quaternion targetRotation = Quaternion.LookRotation(rotation);
        //    //print($"rotation={rotation} targetRotation={targetRotation}");
        //    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        //    //print($"rotation={rotation}  axisY={rotation.x * _rotationSmoothness * Time.deltaTime}");
        //    transform.Rotate(0, rotation.x * _rotationSmoothness * Time.deltaTime, 0);
        //}

        // Поворачиваем персонажа
        //transform.Rotate(Vector3.up, rotation.x * _rotationSmoothness * Time.deltaTime);
        transform.Rotate(Vector3.up, rotation.x * _rotationSmoothness);

        // Перемещаем персонажа
        Vector3 moveDirection = movement;
        rb.AddForce(movement.normalized * moveSpeed);
        //rb.MovePosition(transform.position + moveDirection.normalized * moveSpeed * Time.deltaTime);

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
