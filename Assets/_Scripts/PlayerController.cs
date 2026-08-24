using System.Collections.Generic;
using System.ComponentModel.Design;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerController : NetworkBehaviour
{
	public CharacterController charCon;
    public float moveSpeed;

    public InputActionReference moveAction;
    public InputActionReference lookAction;
    private Vector2 rotStore;
    public float lookSpeed;
    private Vector3 currentMovement;

    public Camera theCam;

    public float minViewAngle, maxViewAngle;

    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    public float jumpPower;
    public float gravityModifier = 4f;
    public float runSpeed;
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] List<StateNode> weaponStates = new();
    [SerializeField] private NetworkAnimator animator;


    public float camZoomNormal, camZoomOut, camZoomSpeed;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
        if(!isOwner)
            return;
        theCam.gameObject.SetActive(true);
        Debug.Log("StateMachine activeSelf: " + stateMachine.gameObject.activeSelf);
        Debug.Log("StateMachine activeInHierarchy: " + stateMachine.gameObject.activeInHierarchy);
        stateMachine.SetState(weaponStates[0]);
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
        Debug.Log($"{moveInput}");
        Vector3 moveForward = transform.forward * moveInput.y;
        Vector3 moveSideways = transform.right * moveInput.x;

        //handle sprinting
        if(sprintAction.action.IsPressed())
        {
            currentMovement = (moveForward + moveSideways) * runSpeed;

            if(currentMovement != Vector3.zero)
            theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, camZoomOut, camZoomSpeed * Time.deltaTime);
        }
        else
        {
            currentMovement = (moveForward + moveSideways) * moveSpeed;

            theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, camZoomNormal, camZoomSpeed * Time.deltaTime);
        }

        //handle gravity
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
    }
}
// 