using UnityEngine;

//
// 플레이어를 향해 XZ 평면에서 이동.
//
public class EnemyChasePlayer : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.2f;
    [SerializeField] Transform target;

    void Update()
    {
        if (target == null)
        {
            var p = FindObjectOfType<PlayerTopDownController>();
            if (p != null)
                target = p.transform;
            else
                return;
        }

        Vector3 d = target.position - transform.position;
        d.y = 0f;
        if (d.sqrMagnitude < 0.0001f)
            return;
        transform.position += d.normalized * (moveSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform t) => target = t;
}
