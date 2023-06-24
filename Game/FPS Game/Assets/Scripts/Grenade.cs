using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosionEffect;

    public float delay = 3f;
    float countdown;
    bool hasExploded = false;
    public float radius = 5f;
    public float force = 1000f;

    public AudioSource source;
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0 && !hasExploded) {
            source.Play();
            Explode();
        }
    }

    void Explode () {
        GameObject effectRB = Instantiate(explosionEffect, transform.position, transform.rotation).gameObject;

        source.PlayOneShot(clip);

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

        Destroy(gameObject);

        Destroy(effectRB, 5f);
    }

    public float GetDelay() { return delay; }
}
