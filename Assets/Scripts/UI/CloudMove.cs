using UnityEngine;

public class CloudMove : MonoBehaviour
{
    [Header("移动设置")]
    public Vector2 direction = Vector2.right; // 移动方向
    public float speed = 1f;

    [Header("边界设置")]
    public float resetPosition = -10f;  // 重置到的位置
    public float limitPosition = 10f;   // 到达这个位置后重置

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);

        // 横向移动检测
        if (direction.x != 0)
        {
            if ((direction.x > 0 && transform.position.x > limitPosition) ||
                (direction.x < 0 && transform.position.x < limitPosition))
            {
                transform.position = new Vector3(resetPosition, transform.position.y, transform.position.z);
            }
        }

        // 纵向移动检测
        if (direction.y != 0)
        {
            if ((direction.y > 0 && transform.position.y > limitPosition) ||
                (direction.y < 0 && transform.position.y < limitPosition))
            {
                transform.position = new Vector3(transform.position.x, resetPosition, transform.position.z);
            }
        }
    }
}