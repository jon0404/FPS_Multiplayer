using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class Gun : StateNode
{   
    public float range;

    public Transform cam;
    public LayerMask validLayers;
    public GameObject muzzleFlare;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject movingPart;
    [SerializeField] private InputActionReference shoot;
    [SerializeField] private int damage = 10;
    [SerializeField] private float flareDisplayTime = .1f;    
    [SerializeField] private List<GameObject> effects = new();
    [SerializeField] private float recoilDuration;
    [SerializeField] private float positionAmount;
    [SerializeField] private float rotationAmount;
    [SerializeField] private float chargeAmount;
    [SerializeField] private Vector3 resetPosition;
    [SerializeField] private Vector3 resetPosition2;
    [SerializeField] private Quaternion resetRotation;
    [SerializeField] private AnimationCurve positionCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve chargeCurve;
    [SerializeField] private Transform rightHandTarget, leftHandTarget;
    [SerializeField] private Transform rightIkTarget, leftIkTarget;
    private Coroutine _recoilCoroutine;
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
    public override void StateUpdate(bool asServer)
    {
        base.StateUpdate(asServer);

        SetIkTargets();

        if(asServer)
            return;
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
