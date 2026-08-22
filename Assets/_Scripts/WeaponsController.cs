using PurrNet;
using UnityEngine;

public class WeaponsController : NetworkBehaviour
{
    public float range;

    public Transform cam;
    public GameObject impactEffect, damageEffect;
    public LayerMask validLayers;
    public GameObject muzzleFlare;
    [SerializeField] private int damage = 10;

    public float flareDisplayTime = .1f;
    private float flareCounter;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        if(flareCounter > 0)
        {
            flareCounter -= Time.deltaTime;

            if(flareCounter <= 0)
            {
                muzzleFlare.SetActive(false);
            }
        }
    }

    public void Shoot()
    {
        muzzleFlare.SetActive(true);
        flareCounter = flareDisplayTime;
        if(Physics.Raycast(cam.position, cam.forward, out var hit, range, validLayers))
        {
            Debug.Log(hit.transform.name);
            if(hit.transform.TryGetComponent(out PlayerHealth playerHealth))
            {
                Instantiate(damageEffect, hit.point, Quaternion.identity);
                playerHealth.ChangeHealth(-damage);
            } 
            else
            {            
            Instantiate(impactEffect, hit.point, Quaternion.identity);
            }
        }
    }
}
