using UnityEngine;

//
// 플레이어 위에서 내려다보는 탑다운 카메라.
//
public class TopDownCameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 worldOffset = new Vector3(0f, 22f, 0f);

    void LateUpdate()
    {
        if (target == null)
            return;
        transform.position = target.position + worldOffset;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void SetTarget(Transform t) => target = t;
}
