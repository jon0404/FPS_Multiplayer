using System.Collections.Generic;
using PurrNet;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{   
    [SerializeField] private LayerMask layers;
	[SerializeField] private CharacterController charCon;
    [SerializeField] private float moveSpeed;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    private Vector2 rotStore;
    [SerializeField] private float lookSpeed;
    private Vector3 currentMovement;

    [SerializeField] private CinemachineCamera theCam;

    [SerializeField] private float minViewAngle, maxViewAngle;

    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private float interactRange = 3;
    [SerializeField] private int interactCooldown = 20;
    private int interactElapsed = 0;

    [SerializeField] private float jumpPower;
    [SerializeField] private float gravityModifier = 4f;
    [SerializeField] private float runSpeed;
    [SerializeField] private NetworkAnimator animator;
    [SerializeField] private List<Renderer> renderers = new();
    [SerializeField] private AudioListener audioListener;
    public float camZoomNormal, camZoomOut, camZoomSpeed;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
        if (!isOwner)
            return;

        foreach (var rend in renderers)
        {
            if (rend != null)
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        
        if (theCam != null)
            theCam.gameObject.SetActive(true);

        if (audioListener != null)
            audioListener.enabled = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (charCon == null || theCam == null || moveAction == null || lookAction == null || jumpAction == null || sprintAction == null || interactAction == null)
            return;

        float yStore = currentMovement.y;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 moveForward = transform.forward * moveInput.y;
        Vector3 moveSideways = transform.right * moveInput.x;

        if (sprintAction.action.IsPressed())
        {
            currentMovement = (moveForward + moveSideways) * runSpeed;
            if (currentMovement != Vector3.zero)
                theCam.Lens.FieldOfView = Mathf.Lerp(theCam.Lens.FieldOfView, camZoomOut, camZoomSpeed * Time.deltaTime);
        }
        else
        {
            currentMovement = (moveForward + moveSideways) * moveSpeed;
            theCam.Lens.FieldOfView = Mathf.Lerp(theCam.Lens.FieldOfView, camZoomNormal, camZoomSpeed * Time.deltaTime);
        }

        if (charCon.isGrounded)
            yStore = 0f;

        currentMovement.y = yStore + (Physics.gravity.y * Time.deltaTime * gravityModifier);

        if (charCon.isGrounded && jumpAction.action.IsPressed())
            currentMovement.y = jumpPower;

        charCon.Move(currentMovement * Time.deltaTime);

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        lookInput.y = -lookInput.y;

        rotStore += lookInput * lookSpeed * Time.deltaTime;
        rotStore.y = Mathf.Clamp(rotStore.y, minViewAngle, maxViewAngle);

        transform.rotation = Quaternion.Euler(0f, rotStore.x, 0f);
        theCam.transform.localRotation = Quaternion.Euler(rotStore.y, 0f, 0f);

        if (animator != null)
        {
            animator.SetFloat("Forward", moveInput.y);
            animator.SetFloat("Sideways", moveInput.x);
        }

        if (interactAction.action.WasPressedThisFrame())
            InteractInvoke();

        if (interactAction.action.IsPressed())
        {
            interactElapsed++;
            if (interactElapsed > interactCooldown)
                InteractInvoke();
        }
    }

    public void InteractInvoke()
    {
        if (theCam == null || charCon == null)
            return;

        if (!Physics.Raycast(theCam.transform.position, theCam.transform.forward, out var hit, interactRange, layers))
            return;

        if (!hit.transform.TryGetComponent<IInteractable>(out var interactable))
            return;

        interactable.Interact(charCon.gameObject);
        interactElapsed = 0;
    }
}