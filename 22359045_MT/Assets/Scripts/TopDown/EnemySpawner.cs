using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform player;
    [SerializeField] float interval = 2f;
    [SerializeField] float spawnRadius = 22f;
    [SerializeField] float spawnHeight = 0.6f;

    float _next;

    void Update()
    {
        if (enemyPrefab == null)
            return;
        if (player == null)
        {
            var p = FindObjectOfType<PlayerTopDownController>();
            if (p != null)
                player = p.transform;
            else
                return;
        }

        if (Time.time < _next)
            return;
        _next = Time.time + interval;

        float ang = Random.Range(0f, Mathf.PI * 2f);
        var pos = player.position + new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * spawnRadius;
        pos.y = spawnHeight;
        var go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        var chase = go.GetComponent<EnemyChasePlayer>();
        if (chase != null)
            chase.SetTarget(player);
    }

    public void Setup(GameObject prefab, Transform target, float spawnEvery, float radius, float y)
    {
        enemyPrefab = prefab;
        player = target;
        interval = spawnEvery;
        spawnRadius = radius;
        spawnHeight = y;
    }
}
