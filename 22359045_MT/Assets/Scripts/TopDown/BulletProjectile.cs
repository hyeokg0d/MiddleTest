using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    [SerializeField] float speed = 26f;
    [SerializeField] float lifetime = 2.5f;
    [SerializeField] AudioClip hitSfx;      // 충돌 사운드
    [SerializeField] float hitSfxVolume = 1f;

    float _t;

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Update()
    {
        _t += Time.deltaTime;
        if (_t >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerTopDownController>() != null)
            return;

        var enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            if (hitSfx != null)
                AudioSource.PlayClipAtPoint(hitSfx, transform.position, hitSfxVolume);

            enemy.TakeHit(1);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}