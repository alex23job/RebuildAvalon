using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviour
{
    // ������� ������ (��������)
    [SerializeField] private Transform target;

    // ��������� ������
    public float distance = 5f;                  // ���������� �� ���������
    public float height = 2f;                    // ������ ������ ��� ����������
    public float rotationSmoothTime = 0.12f;     // �������� ����������� ��������
    public float zoomSmoothTime = 0.12f;         // �������� ����������� ����
    public float minZoom = 3f;                   // ����������� ���������� ����
    public float maxZoom = 10f;                  // ������������ ���������� ����

    // ���������� ��� �������
    private Vector3 velocity = Vector3.zero;
    private float currentZoom = 5f;
    private float currentHeight;

    private void Start()
    {
        currentHeight = height;
    }

    // ��������� ������� ����
    public void OnZoom(InputAction.CallbackContext context)
    {
        // ��������� ���������� ����
        //float scrollDelta = context.ReadValue<float>();
        float scrollDelta = context.ReadValue<Vector2>().y;
        //print($"OnZoom scroll={scrollDelta}");
        currentZoom = Mathf.Clamp(currentZoom - scrollDelta, minZoom, maxZoom);
        distance = currentZoom;
        currentHeight = height + 0.2f * (currentZoom - minZoom); 
    }

    // ���������� ������
    void LateUpdate()
    {
        // ������������ ������� ������
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * currentHeight;

        // ��������� ��������� ������
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, rotationSmoothTime);

        // ��������� ����������� ������
        transform.LookAt(target);
    }
}
