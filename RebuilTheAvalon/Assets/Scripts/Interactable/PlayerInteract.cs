using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEngine.Rendering.DynamicArray<T>;

public class PlayerInteract : MonoBehaviour
{
    [Header("Player camera")]
    [SerializeField] private Camera _playerCamera;

    [Header("Layer for interactables")]
    [SerializeField] private LayerMask _interactionLayer;
    //[SerializeField] private LayerMask _highlightLayer;

    [SerializeField] private Material outlineMaterial;

    private IInteractable _currentInteractable = null;
    private Collider _lastCollider = null;
    private Ray _playerLook;
    private RaycastHit _lookHit;
    private float _interactDistance = 4f;

    private bool _isInteract = false;

    [SerializeField] private UI_Control ui_Control;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLook();
    }

    public void OnInteract(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase != InputActionPhase.Started) return;

        if (_isInteract == false)
        {
            if (_lastCollider == null) return;

            if (_lastCollider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.Interact();
                _currentInteractable = interactable;
                _isInteract = true;
            }            
        }
        else 
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.EndInteract();
                _currentInteractable = null;
            }
            _isInteract = false;
        }
    }

    private void PlayerLook()
    {
        Vector3 origin = transform.position;
        //Vector3 direction = _playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)) - origin;
        //Vector3 direction = origin - _playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
        Vector3 direction = transform.forward;
        //direction.Normalize();
        //if (_viewManager.IsFirstPerson)
        _playerLook = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //else
        //    _playerLook = _playerCamera.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(_playerLook.origin, _playerLook.direction * _interactDistance, Color.red);
        Debug.DrawRay(origin, direction * _interactDistance, Color.red);

        //if (Physics.Raycast(_playerLook, out _lookHit, _interactDistance, _interactionLayer))
        if (Physics.Raycast(origin, direction, out _lookHit, _interactDistance, _interactionLayer))
                TryHighlight(_lookHit.collider);
        else
            ClearHighlight();
    }

    private void TryHighlight(Collider interactor)
    {
        if (_lastCollider == interactor)
            return;
        //Debug.Log($"interactor => {interactor.gameObject.name}");
        ClearHighlight();
        MeshRenderer mr = interactor.gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Material[] mats = mr.materials;
            if (mats.Contains(outlineMaterial) == false)
            {
                Material[] updMats = new Material[mats.Length + 1];
                for (int i = 0; i < mats.Length; i++) updMats[i] = mats[i];
                updMats[mats.Length] = outlineMaterial;
                mr.materials = updMats;
            }
        }
        //interactor.gameObject.layer = LayerMask.NameToLayer("Outline");
        _lastCollider = interactor;
        if (_lastCollider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (ui_Control != null) ui_Control.ViewHint(interactable.GetHint());
        }
    }

    private void ClearHighlight()
    {
        if (_lastCollider)
        {
            //Debug.Log($"ClearHighlight name={_lastCollider.name}");
            MeshRenderer mr = _lastCollider.gameObject.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Material[] mats = mr.materials;
                if (mats.Length > 1)
                {
                    //Debug.Log($"ClearHighlight outlineMaterial=true ({mats[mats.Length - 1]})");
                    Material[] updMats = new Material[mats.Length - 1];
                    for (int i = 0; i < mats.Length - 1; i++)
                    {
                        //if (mats[i] != outlineMaterial)
                        updMats[i] = mats[i];
                    }
                    mr.materials = updMats;
                }
            }
            //_lastCollider.gameObject.layer = LayerMask.NameToLayer("Interaction");
            _lastCollider = null;
        }
    }
}
