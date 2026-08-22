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
    [SerializeField] private InputActionReference shoot;
    [SerializeField] private int damage = 10;
    [SerializeField] private float flareDisplayTime = .1f;    
    [SerializeField] private List<GameObject> effects = new();

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    public override void StateUpdate(bool asServer)
    {
        base.StateUpdate(asServer);

        if (!isOwner)
            return;

        if(shoot.action.IsPressed())
            Shoot();
    }

    private void Shoot()
    {
        if(!Physics.Raycast(cam.position, cam.forward, out var hit, range, validLayers))
            return;

        GameObject _effect = effects[0];

        foreach(GameObject effect in effects)
        {
            if(effect.GetComponent<Surface>() == hit.transform.GetComponent<Surface>())
                _effect = effect;
        }

        Instantiate(_effect, hit.point, Quaternion.identity);

        if(hit.transform.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.ChangeHealth(-damage);
            } 
    }

    private IEnumerator MuzzleFlare()
    {
        muzzleFlare.SetActive(true);
        yield return new WaitForSeconds(flareDisplayTime);
            muzzleFlare.SetActive(false);
    }
}
