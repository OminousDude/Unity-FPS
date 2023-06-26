using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun {
    [SerializeField] Camera cam;

    PhotonView PV;

    public float gunIndex = 0f;

    public AudioSource source;

    public GameObject grenadePrefab;
    public GameObject satchelPrefab;

    public GameObject bulletHitEffect;

    public CapsuleCollider capsuleCollider;

    public LayerMask layerPassThrough;

    public bool isAuto;

    public float shotsFiredInRow;

    public float recoilAmountX;
    public float recoilAmountY;

    public float currentRecoilPosX;
    public float currentRecoilPosY;

    public float gunPrecision;

    [Range(0, 15f)] public float maxRecoilTime = 4;

    [Range(0, 15f)] public float finalRecoilTime = 8;

    void Awake() {
        PV = GetComponent<PhotonView>();
    }

    public override void Use() {
        Shoot();
    }

    public void RecoilMath () {
        var controller = GetComponentInParent<PlayerController>();
        
        if (shotsFiredInRow <= maxRecoilTime) {
            currentRecoilPosX = (Random.value - .5f);
            currentRecoilPosY = (Random.value - .5f);

            controller.horizontalLookRotation += currentRecoilPosX * gunPrecision;
            controller.verticalLookRotation += Mathf.Abs(currentRecoilPosY * gunPrecision);
        } else if (shotsFiredInRow <= finalRecoilTime) {
            currentRecoilPosX = (Random.value - .5f) * recoilAmountX;
            currentRecoilPosY = (Random.value - .5f) * recoilAmountY;

            controller.horizontalLookRotation += currentRecoilPosX * gunPrecision;
            controller.verticalLookRotation += Mathf.Abs(currentRecoilPosY * gunPrecision);
        } else {
            currentRecoilPosX = (Random.value - .5f) * recoilAmountX * 1.5f;
            currentRecoilPosY = (Random.value - .5f) * (shotsFiredInRow >= maxRecoilTime ? recoilAmountY / 8 : recoilAmountY);

            controller.horizontalLookRotation += currentRecoilPosX * gunPrecision;
            controller.verticalLookRotation += currentRecoilPosY * gunPrecision;
        }
    }

    void Shoot() {
        if (gunIndex == 4) {
            source.Play();

            Grenade.Throw(grenadePrefab, transform);
        } else if (gunIndex == 5) {
            source.Play();

            Grenade.Throw(satchelPrefab, transform);
        } else {
            source.Play();

            capsuleCollider.enabled = false;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;
            RecoilMath();
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~layerPassThrough)) {
                if (hit.collider != capsuleCollider) {
                    if (hit.collider.gameObject.name == "Head") {
                        hit.collider.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage * 2);
                    } else {
                        hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                    }
                }

                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }

            capsuleCollider.enabled = true;
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal) {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0) {
            Debug.Log(hitNormal);
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);

            GameObject bulletEffectObj = Instantiate(bulletHitEffect, hitPosition, Quaternion.FromToRotation(Vector3.right, hitNormal));
            
            Destroy(bulletImpactObj, 10f);

            Destroy(bulletEffectObj, 1f);
/*
            bulletImpactObj.transform.SetParent(colliders[0].transform);
            bulletEffectObj.transform.SetParent(colliders[0].transform);*/
        }
    }
}
