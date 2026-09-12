using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PurrNet;
using PurrNet.StateMachine;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal.Internal;

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
    [SerializeField] List<Renderer> renderers = new();
    [SerializeField] AudioListener audioListener;
    //public Usable.itemType
    public float camZoomNormal, camZoomOut, camZoomSpeed;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
        if(!isOwner)
            return;
        foreach (var rend in renderers)
        {
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        
        theCam.gameObject.SetActive(true);
        audioListener.enabled = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float yStore = currentMovement.y;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 moveForward = transform.forward * moveInput.y;
        Vector3 moveSideways = transform.right * moveInput.x;

        //handle sprinting
        if(sprintAction.action.IsPressed())
        {
            currentMovement = (moveForward + moveSideways) * runSpeed;
            if(currentMovement != Vector3.zero)
            theCam.Lens.FieldOfView = Mathf.Lerp(theCam.Lens.FieldOfView, camZoomOut, camZoomSpeed * Time.deltaTime);
        }
        else
        {
            currentMovement = (moveForward + moveSideways) * moveSpeed;

            theCam.Lens.FieldOfView = Mathf.Lerp(theCam.Lens.FieldOfView, camZoomNormal, camZoomSpeed * Time.deltaTime);
        }

        if(charCon.isGrounded)
        {
            yStore = 0;
        }

        currentMovement.y = yStore + (Physics.gravity.y * Time.deltaTime * gravityModifier);

        //handle jumping
        if(charCon.isGrounded && jumpAction.action.IsPressed())
        {
            currentMovement.y = jumpPower;
        }

        charCon.Move(currentMovement * Time.deltaTime);


        //handle looking
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        lookInput.y = -lookInput.y;

        rotStore = rotStore + ( lookInput * lookSpeed * Time.deltaTime);

        rotStore.y = Mathf.Clamp(rotStore.y, minViewAngle, maxViewAngle);

        transform.rotation = Quaternion.Euler(0f, rotStore.x, 0f);
        theCam.transform.localRotation = Quaternion.Euler(rotStore.y, 0f, 0f);

        //handle animation
        animator.SetFloat("Forward", moveInput.y);
        animator.SetFloat("Sideways", moveInput.x);

        if(interactAction.action.WasPressedThisFrame())
            InteractInvoke();

        if(interactAction.action.IsPressed())
        {
            interactElapsed++;
            if(interactElapsed > interactCooldown)
                InteractInvoke();
        }
    }
    public void InteractInvoke()
    {
        if (!Physics.Raycast(theCam.transform.position, theCam.transform.forward, out var hit, interactRange, layers))
            return;
        hit.transform.TryGetComponent<IInteractable>(out var iinteractable);
        iinteractable.Interact(charCon.gameObject);
        interactElapsed = 0;
    }
}