using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mono.Cecil.Cil;
using NUnit.Framework.Internal;
using PurrNet;
using PurrNet.StateMachine;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class Gun : StateNode, IInitialize
{

    [SerializeField] private ScriptableObject properties;

[ColorSpacer(0, 0.5f, 2, 1000, 0.57f, 0.57f, 0.57f)]
[ColoredHeader("References", .77f, .77f, .77f, 15, -1)]

    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightIkTarget, leftIkTarget;
    [SerializeField] private AudioSource audioSource;
    Transform cam;
    public LayerMask validLayers;
    public GameObject muzzleFlare;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject movingPart;
    [SerializeField] private InputActionReference shoot;
[ColoredHeader("Bullet hit FX", .77f, .77f, .77f, 15, -1)] 
    [SerializeField] List<GameObject> effects = new();
[ColorSpacer(25, 5, 3, 120, 0.77f, 0.77f, 0.77f)]
[Space (10)]
[ColoredHeader("Gun Settings", .77f, .77f, .77f, 15, -1)]
    [SerializeField] private int damage = 10;
    [SerializeField] public float range;

[ColorSpacer(25, 5, 3, 120, 0.77f, 0.77f, 0.77f)]
[Space (10)]    
[ColoredHeader("Animation Settings", .77f, .77f, .77f, 15, -1)]
    [SerializeField] private AnimationCurve positionCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve chargeCurve;
    [SerializeField] private float flareDisplayTime = .1f;    
    [SerializeField] private float recoilDuration;
    [SerializeField] private float positionAmount;
    [SerializeField] private float rotationAmount;
    [SerializeField] private float chargeAmount;
[Space (7)]      
[ColorSpacer(0, 0.5f, 2, 1000, 0.57f, 0.57f, 0.57f)]
[Indent(1000)]  

    [SerializeField] public bool The_end_of_the_script_bracket_needed_for_last_header_end_bracket = true;
    private Coroutine _recoilCoroutine;
    private Vector3 resetPosition;
    private Vector3 resetPosition2;
    private Quaternion resetRotation;

    protected override void OnSpawned()
    {

    }

    private void Start()
    {
        resetPosition = gun.transform.localPosition;
        resetRotation = gun.transform.localRotation;
        resetPosition2 = movingPart.transform.localPosition;
    }
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    public void Initialize(Player _player)
    {
        cam = _player.cam.transform;

    }
    void Update()
    {

        //SetIkTargets();

        if (!isOwner)
            return;
        if(!shoot.action.WasPressedThisFrame())
            return;   
        Shoot();
        PlayShotEffects();
    }

    private void Shoot()
    {   
        Debug.Log("shot");
        audioSource.Play();
        if(!Physics.Raycast(cam.position, cam.forward, out var hit, range, validLayers))
            return;
        Debug.Log("hit something");
        GameObject _effect = effects[0];
        foreach(GameObject effect in effects)
        {
            //Surface effectSurface = effect.GetComponent<Surface>();
            if (!hit.transform.TryGetComponent(out Surface surface))
                return;
            if(effect.GetComponent<Surface>().Type == hit.transform.GetComponent<Surface>().Type)
                _effect = effect;
                Debug.Log(hit.transform.GetComponent<Surface>().Type);
        }

        Instantiate(_effect, hit.point, Quaternion.identity);

        if(hit.transform.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.ChangeHealth(-damage);
            } 
    }
    [ObserversRpc(runLocally:true)]
    private void PlayShotEffects()
    {
        StartCoroutine(MuzzleFlare());
        if(_recoilCoroutine != null)
            StopCoroutine(_recoilCoroutine);

        _recoilCoroutine = StartCoroutine(Recoil()); 
    }

    private void SetIkTargets()
    {
        rightIkTarget.SetPositionAndRotation(rightHandTarget.position, rightHandTarget.rotation);
        leftIkTarget.SetPositionAndRotation(leftHandTarget.position, leftHandTarget.rotation);
    }

    private IEnumerator MuzzleFlare()
    {
        muzzleFlare.SetActive(true);
        yield return new WaitForSeconds(flareDisplayTime);
            muzzleFlare.SetActive(false);
    }
    
    private IEnumerator Recoil()
    {
        float elapsed = 0;
        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;
            float curveTime = elapsed / recoilDuration;

            float positionValue = positionCurve.Evaluate(curveTime);
            Vector3 positionOffset = Vector3.back * (positionValue * positionAmount);
            transform.localPosition = resetPosition + positionOffset;

            float chargeValue = chargeCurve.Evaluate(curveTime);
            Vector3 chargeOffset = Vector3.back * (chargeValue * chargeAmount);
            movingPart.transform.localPosition = resetPosition2 + chargeOffset;

            float rotationValue = rotationCurve.Evaluate(curveTime);
            Vector3 rotationOffset = new Vector3(rotationValue * rotationAmount, 0f, 0f);
            transform.localRotation = resetRotation * Quaternion.Euler(rotationOffset);
            yield return null;
        }
        
    
        gun.transform.localPosition = resetPosition;
        movingPart.transform.localPosition = resetPosition2;
        gun.transform.localRotation = resetRotation;
    
    }
}
