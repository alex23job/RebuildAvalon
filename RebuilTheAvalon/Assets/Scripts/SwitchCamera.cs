using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] private Camera cameraFPS;
    [SerializeField] private Camera cameraTPS;

    private bool isFPS = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraFPS.gameObject.SetActive(false);
        cameraTPS.gameObject.SetActive(true);
    }

    public void OnCameraSwitch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isFPS)
            {
                isFPS = false;
                cameraFPS.gameObject.SetActive(false);
                cameraTPS.gameObject.SetActive(true);
                //cameraTPS.enabled = true;
                //cameraFPS.enabled = false;
            }
            else
            {
                isFPS = true;
                cameraFPS.gameObject.SetActive(true);
                cameraTPS.gameObject.SetActive(false);
                //cameraTPS.enabled = false;
                //cameraFPS.enabled = true;
            }
            print($"isFPS={isFPS}");
        }
    }
}
