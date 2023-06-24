using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
    public GameObject explosionEffect;

    public float delay = 3f;
    float countdown;
    bool hasExploded = false;
    public float radius = 5f;
    public float force = 1000f;

    public float throwForce;

    private AudioSource source;

    public static void Throw(GameObject grenadePrefab, Transform transform) {
        Instantiate(grenadePrefab, transform.position, transform.rotation);
    }

    // Start is called before the first frame update
    void Start() {
        source = GetComponent<AudioSource>();

        countdown = delay;

        var grenadeRB = GetComponent<Rigidbody>();

        grenadeRB.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update() {
        countdown -= Time.deltaTime;
        if (countdown <= 0 && !hasExploded) {
            source.Play();
            Explode();
        }
    }

    void Explode() {
        GameObject effectRB = Instantiate(explosionEffect, transform.position, transform.rotation).gameObject;

        Collider[] collsToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach (var col in collsToDestroy) {
            Destructible destructible = col.GetComponent<Destructible>();

            if (destructible != null) {
                destructible.Destroy();
            }
        }

        Collider[] collsToMove = Physics.OverlapSphere(transform.position, radius);

        foreach (var col in collsToMove) {
            var rb = col.GetComponent<Rigidbody>();

            if (rb != null) {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        hasExploded = true;

        GetComponent<Renderer>().enabled = false;

        Destroy(gameObject, 5f);

        Destroy(effectRB, 5f);
    }
}
