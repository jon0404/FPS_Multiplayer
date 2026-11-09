using System.Collections;
using System.Collections.Generic;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : StateNode, IInitialize
{
    // This class represents the active weapon object on the player.
    [SerializeField] private ScriptableObject properties;

    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightIkTarget, leftIkTarget;
    [SerializeField] private AudioSource audioSource;
    private Transform cam;
    public LayerMask validLayers;
    public GameObject muzzleFlare;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject movingPart;
    [SerializeField] private InputActionReference shoot;
    [SerializeField] private List<GameObject> effects = new();
    [SerializeField] private int damage = 10;
    [SerializeField] public float range;
    [SerializeField] private AnimationCurve positionCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve chargeCurve;
    [SerializeField] private float flareDisplayTime = .1f;
    [SerializeField] private float recoilDuration;
    [SerializeField] private float positionAmount;
    [SerializeField] private float rotationAmount;
    [SerializeField] private float chargeAmount;
    private Coroutine _recoilCoroutine;
    private Vector3 resetPosition;
    private Vector3 resetPosition2;
    private Quaternion resetRotation;

    protected override void OnSpawned()
    {
    }

    private void Start()
    {
        // Store the default pose so the weapon can recoil without drifting.
        if (gun != null)
            resetPosition = gun.transform.localPosition;
        if (movingPart != null)
            resetPosition2 = movingPart.transform.localPosition;
        if (gun != null)
            resetRotation = gun.transform.localRotation;
    }

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);
    }

    // Called when the gun is equipped to a player. It records the camera transform for raycasts.
    public void Initialize(Player _player)
    {
        if (_player != null && _player.cam != null)
            cam = _player.cam.transform;
    }

    private void Update()
    {
        if (!isOwner || shoot == null || cam == null)
            return;

        if (!shoot.action.WasPressedThisFrame())
            return;

        Shoot();
        PlayShotEffects();
    }

    private void Shoot()
    {
        if (audioSource != null)
            audioSource.Play();

        // Fire a ray from the camera and look for a valid surface hit.
        if (cam == null || !Physics.Raycast(cam.position, cam.forward, out var hit, range, validLayers))
            return;

        GameObject effectToSpawn = effects.Count > 0 ? effects[0] : null;

        foreach (GameObject effect in effects)
        {
            if (effect == null)
                continue;

            if (hit.transform.TryGetComponent(out Surface hitSurface) &&
                effect.TryGetComponent(out Surface effectSurface) &&
                effectSurface.Type == hitSurface.Type)
            {
                effectToSpawn = effect;
                break;
            }
        }

        if (effectToSpawn != null)
            Instantiate(effectToSpawn, hit.point, Quaternion.identity);

        if (hit.transform.TryGetComponent(out PlayerHealth playerHealth))
            playerHealth.ChangeHealth(-damage);
    }

    [ObserversRpc(runLocally: true)]
    private void PlayShotEffects()
    {
        if (muzzleFlare != null)
            StartCoroutine(MuzzleFlare());

        if (_recoilCoroutine != null)
            StopCoroutine(_recoilCoroutine);

        _recoilCoroutine = StartCoroutine(Recoil());
    }

    private void SetIkTargets()
    {
        if (rightIkTarget != null && rightHandTarget != null)
            rightIkTarget.SetPositionAndRotation(rightHandTarget.position, rightHandTarget.rotation);

        if (leftIkTarget != null && leftHandTarget != null)
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
        if (gun == null || movingPart == null)
            yield break;

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
