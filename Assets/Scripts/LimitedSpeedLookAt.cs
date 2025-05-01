using UnityEngine;

public class LimitedSpeedLookAt : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    public Transform Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField]
    private float angularSpeed = 200;
    public float AngularSpeed
    {
        get => angularSpeed;
        set => angularSpeed = value;
    }

    private void Update()
    {
        var transform = this.transform;
        var direction = target.position - transform.position;

        float angle = angularSpeed * Mathf.Deg2Rad * Time.deltaTime;
        transform.forward = Vector3.RotateTowards(transform.forward, direction, angle, 0);
    }
}
