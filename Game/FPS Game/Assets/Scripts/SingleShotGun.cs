using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;

	PhotonView PV;

	public float gunIndex = 0f;

    public AudioSource source;

	public float throwForce = 50f;

	public GameObject grenadePrefab;
    public GameObject satchelPrefab;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	public override void Use()
	{
		Shoot();
	}

	void Shoot()
	{
        if (gunIndex == 4) {
            source.Play();

            var grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);

            //explodeSource.PlayDelayed(grenade.GetComponent<Grenade>().delay);

            var grenadeRB = grenade.GetComponent<Rigidbody>();

            grenadeRB.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        } else if(gunIndex == 5) {
            source.Play();
            
            var grenade = Instantiate(satchelPrefab, transform.position, transform.rotation);
            
            //explodeSource.PlayDelayed(grenade.GetComponent<Grenade>().delay);

            var grenadeRB = grenade.GetComponent<Rigidbody>();

            grenadeRB.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        } else {
            source.Play();

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider.gameObject.name == "Head") {
                    hit.collider.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage * 2);
                } else {
                    hit.collider.gameObject.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                }

                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }
        }
    }

	[PunRPC]
	void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
	{
		Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
		if(colliders.Length != 0)
		{
			GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
			Destroy(bulletImpactObj, 10f);
			bulletImpactObj.transform.SetParent(colliders[0].transform);
		}
	}
}
