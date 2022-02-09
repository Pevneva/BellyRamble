using UnityEngine;

public class BotMover : MonoBehaviour
{
    private readonly float _speed = 0.85f;
    
    public void Move(Transform target, float speed, out Vector3 movingDirection)
    {
        movingDirection = (target.position - transform.position).normalized;
        transform.Translate(Time.deltaTime * speed * movingDirection, Space.World);
    }
    
    public void Rotate(Transform target)
    {
        var lookRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, _speed);
    }
}
