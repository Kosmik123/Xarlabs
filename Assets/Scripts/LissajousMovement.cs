using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public class LissajousMovement : MonoBehaviour
{
    [SerializeField]
    private Vector3 amplitude;
    public float3 Amplitude { get => amplitude; set => amplitude = value; }

    [SerializeField]
    private Vector3 offset;
	public float3 Offset { get => offset; set => offset = value; }

    [SerializeField]
    private Vector3 frequency;
	public float3 Frequency
    {
        get => frequency;
        set => frequency = value;
    }

    [SerializeField]
    private Vector3 phase;
	public float3 Phase { get => phase; set => phase = value; }

    private float time;

    private void Update()
    {
        time += Time.deltaTime;
        transform.localPosition = CalculatePosition(time);
    }

	public float3 CalculatePosition(float time) => CalculatePosition(Amplitude, Offset, Frequency, Phase, time);

    public static float3 CalculatePosition(float3 amplitude, float3 offset, float3 frequency, float3 phase, float time)
    {
        var position = amplitude * sin(2 * PI * time * frequency + phase) + offset;
        return position;
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (UnityEditorInternal.InternalEditorUtility.GetIsInspectorExpanded(this) == false)
            return;

        var matrix = Gizmos.matrix;
        if (transform.parent)
            Gizmos.matrix = transform.parent.localToWorldMatrix;

        Gizmos.color = Color.yellow;
        float time = Application.isPlaying ? this.time : (float)UnityEditor.EditorApplication.timeSinceStartup;
        Gizmos.DrawSphere(CalculatePosition(time), 0.1f);
        Gizmos.matrix = matrix;
#endif
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(LissajousMovement))]
public class LissajousMovementEditor : UnityEditor.Editor
{
    private const float preferedTimeDelta = 0.05f;

    private Vector3 previousFrequencies;
    private float frequency;

    private void OnEnable()
    {
        var frequencies = serializedObject.FindProperty("frequency").vector3Value;
        frequency = GreatestCommonDivisor3(frequencies.x, frequencies.y, frequencies.z);
    }

    public void OnSceneGUI()
    {
        if (UnityEditorInternal.InternalEditorUtility.GetIsInspectorExpanded(target) == false)
            return;

        var transform = ((Component)target).transform;
        var frequencies = serializedObject.FindProperty("frequency").vector3Value;

        if (previousFrequencies != frequencies)
        {
            previousFrequencies = frequencies;
            frequency = GreatestCommonDivisor3(frequencies.x, frequencies.y, frequencies.z);
        }
        float period = 2f / frequency;
        int resolution = Mathf.FloorToInt(period / preferedTimeDelta);
        float dt = period / resolution;

        var amplitude = serializedObject.FindProperty("amplitude").vector3Value;
        var offset = serializedObject.FindProperty("offset").vector3Value;
        var phase = serializedObject.FindProperty("phase").vector3Value;

        UnityEditor.Handles.color = Color.yellow;
        var matrix = UnityEditor.Handles.matrix;
        if (transform.parent)
            UnityEditor.Handles.matrix = transform.parent.localToWorldMatrix;
        var previousPosition = LissajousMovement.CalculatePosition(amplitude, offset, frequencies, phase, 0);
        for (int i = 1; i <= resolution; i++)
        {
            var localPosition = LissajousMovement.CalculatePosition(amplitude, offset, frequencies, phase, i * dt);
            UnityEditor.Handles.DrawLine(previousPosition, localPosition);
            previousPosition = localPosition;
        }

        UnityEditor.Handles.matrix = matrix;
    }

    private static float GreatestCommonDivisor(float a, float b, float maxError)
    {
        if (a < b)
            (a, b) = (b, a);

        if (Mathf.Abs(b) < maxError)
            return a;

        return GreatestCommonDivisor(b, a - Mathf.Floor(a / b) * b, maxError);
    }

    private static float GreatestCommonDivisor3(float a, float b, float c, float maxError = 0.001f)
    {
        return GreatestCommonDivisor(c, GreatestCommonDivisor(b, a, maxError), maxError);
    }
}

#endif
