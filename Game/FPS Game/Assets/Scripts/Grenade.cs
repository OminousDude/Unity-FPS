using UnityEngine;
using UnityEngine.Networking.Types;

public class Grenade : MonoBehaviour {
    [SerializeField] ItemInfo itemInfo;
    public GameObject explosionEffect;

    public float delay = 3f;
    float countdown;
    bool hasExploded = false;
    public float radius = 5f;
    public float force = 1000f;

    public float throwForce;

    public AudioSource[] sources;

    public AudioSource bounceSound;

    public static void Throw(GameObject grenadePrefab, Transform transform) {
        Instantiate(grenadePrefab, transform.position, transform.rotation);
    }

    // Start is called before the first frame update
    void Start() {
        countdown = Random.Range(delay - delay/3, delay + delay/3);

        var grenadeRB = GetComponent<Rigidbody>();

        var randomVal = Random.Range(throwForce - throwForce / 3, throwForce + throwForce / 3);

        grenadeRB.AddForce(transform.forward * randomVal, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate() {
        countdown -= Time.fixedDeltaTime;
        if (countdown <= 0 && !hasExploded) {
            var source = sources[Random.Range(0, sources.Length-1)];
            source.Play();
            Explode();
        }
    }

    void Explode() {
        GameObject effectRB = Instantiate(explosionEffect, transform.position, transform.rotation).gameObject;

        Collider[] collsToDamage = Physics.OverlapSphere(transform.position, radius);
        foreach (var col in collsToDamage)
        {
            PlayerController playerController = col.GetComponent<PlayerController>();
            if (playerController != null)
            {
                col.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            }
        }

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

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == 0 || collision.collider.gameObject.layer == 3) {
            bounceSound.Play();
        }
    }
}