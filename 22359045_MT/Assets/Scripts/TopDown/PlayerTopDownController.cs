using UnityEngine;

//
// XZ 평면 이동, 마우스로 조준, 마우수 왼쪽으로 발사.
//
[RequireComponent(typeof(Rigidbody))]
public class PlayerTopDownController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 9f;
    [SerializeField] float fireCooldown = 0.18f;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Camera playerCamera;
    [SerializeField] float aimPlaneY;
    [SerializeField] GameOverUIController gameOverUI;

    Rigidbody _rb;
    float _nextFire;
    bool _isGameOver;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _rb.useGravity = true;
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (gameOverUI == null)
            gameOverUI = FindObjectOfType<GameOverUIController>();
        aimPlaneY = transform.position.y;
    }

    void Update()
    {
        if (playerCamera == null)
            return;
        if (_isGameOver)
            return;

        Vector3 mouseWorld = GetMouseOnPlane(aimPlaneY);
        Vector3 look = mouseWorld - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(look.normalized, Vector3.up);

        if (bulletPrefab != null && firePoint != null && Input.GetButton("Fire1") && Time.time >= _nextFire)
        {
            _nextFire = Time.time + fireCooldown;
            var dir = (mouseWorld - firePoint.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f)
                dir = transform.forward;
            dir.Normalize();
            var rot = Quaternion.LookRotation(dir, Vector3.up);
            Instantiate(bulletPrefab, firePoint.position, rot);
        }
    }

    void FixedUpdate()
    {
        if (_isGameOver)
        {
            var stop = _rb.velocity;
            stop.x = 0f;
            stop.z = 0f;
            _rb.velocity = stop;
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        var move = new Vector3(h, 0f, v);
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        var vel = _rb.velocity;
        vel.x = move.x * moveSpeed;
        vel.z = move.z * moveSpeed;
        _rb.velocity = vel;
    }

    Vector3 GetMouseOnPlane(float planeY)
    {
        var ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));
        if (plane.Raycast(ray, out float dist))
            return ray.GetPoint(dist);
        return transform.position + transform.forward;
    }

    public void Configure(Transform fire, GameObject bullet, Camera cam, float planeY)
    {
        firePoint = fire;
        bulletPrefab = bullet;
        playerCamera = cam;
        aimPlaneY = planeY;
    }

    public void ConfigureGameOver(GameOverUIController ui)
    {
        gameOverUI = ui;
    }

    void OnCollisionEnter(Collision collision)
    {
        TryGameOver(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        TryGameOver(other);
    }

    void TryGameOver(Collider other)
    {
        if (_isGameOver || other == null)
            return;
        if (other.GetComponentInParent<EnemyHealth>() == null)
            return;

        _isGameOver = true;
        if (gameOverUI == null)
            gameOverUI = FindObjectOfType<GameOverUIController>();
        if (gameOverUI != null)
            gameOverUI.ShowGameOver();
    }
}
