using UnityEngine;

public class AngleColorChanger : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Gradient colorsByAngle;

    [SerializeField]
    private MeshRenderer meshRenderer;

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
