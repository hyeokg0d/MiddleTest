using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHp = 1;
    [SerializeField] GameObject explosionPrefab; // �ν����� - ���� ������ 

    int _hp;

    void OnEnable()
    {
        _hp = maxHp;
    }

    public void TakeHit(int amount)
    {
        _hp -= amount;
        if (_hp <= 0)
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            GameScore.Add(1);
            Destroy(gameObject);
        }
    }
}