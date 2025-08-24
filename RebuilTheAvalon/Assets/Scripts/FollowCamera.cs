using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviour
{
    // Целевой объект (персонаж)
    [SerializeField] private Transform target;

    // Параметры камеры
    public float distance = 5f;                  // Расстояние от персонажа
    public float height = 2f;                    // Высота камеры над персонажем
    public float rotationSmoothTime = 0.12f;     // Скорость сглаживания вращения
    public float zoomSmoothTime = 0.12f;         // Скорость сглаживания зума
    public float minZoom = 3f;                   // Минимальное расстояние зума
    public float maxZoom = 10f;                  // Максимальное расстояние зума

    // Переменные для расчета
    private Vector3 velocity = Vector3.zero;
    private float currentZoom = 5f;
    private float currentHeight;

    private void Start()
    {
        currentHeight = height;
    }

    // Обработка события зума
    public void OnZoom(InputAction.CallbackContext context)
    {
        // Обновляем расстояние зума
        //float scrollDelta = context.ReadValue<float>();
        float scrollDelta = context.ReadValue<Vector2>().y;
        //print($"OnZoom scroll={scrollDelta}");
        currentZoom = Mathf.Clamp(currentZoom - scrollDelta, minZoom, maxZoom);
        distance = currentZoom;
        currentHeight = height + 0.2f * (currentZoom - minZoom); 
    }

    // Обновление камеры
    void LateUpdate()
    {
        // Рассчитываем позицию камеры
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * currentHeight;

        // Обновляем положение камеры
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, rotationSmoothTime);

        // Обновляем направление камеры
        transform.LookAt(target);
    }
}
