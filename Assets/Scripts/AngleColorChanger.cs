using UnityEngine;

public class AngleColorChanger : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Gradient colorsByAngle;

    [Space, SerializeField]
    private Transform target;

    private void Reset()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        var direction = (target.position - transform.position).normalized;
        float dot = Vector3.Dot(direction, transform.forward);

        float progress = Mathf.InverseLerp(1, -1, dot);
        Color color = colorsByAngle.Evaluate(progress);
        meshRenderer.material.color = color;
    }
}
