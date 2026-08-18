using PurrNet;
using UnityEngine;

public class WeaponsController : NetworkBehaviour
{
    public float range;

    public Transform cam;
    public GameObject impactEffect, damageEffect;
    public LayerMask validLayers;
    public GameObject muzzleFlare;

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
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, range, validLayers))
        {
            Debug.Log(hit.transform.name);
            if(hit.transform.tag == "Enemy")
            {
                Instantiate(damageEffect, hit.point, Quaternion.identity);
            } 
            else
            {            
            Instantiate(impactEffect, hit.point, Quaternion.identity);
            }
        }
        muzzleFlare.SetActive(true);
        flareCounter = flareDisplayTime;
    }
}
