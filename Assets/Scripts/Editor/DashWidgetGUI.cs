using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Movement))]
public class DashWidgetGUI : Editor
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Pickable)]
    static void DrawDashGizmo(Movement movement, GizmoType gizmoType)
    {
        Transform transform = movement.transform;
        Vector3 start = transform.position;
        Vector3 direction = GetDashDirectionByZRotation(transform.eulerAngles.z);
        Vector3 velocity = direction * (movement.DashDistance / movement.DashDuration);

        float duration = movement.DashDuration;
        float physicsStep = Time.fixedDeltaTime;

        Rigidbody rb = movement.GetComponent<Rigidbody>();
        float drag = rb != null ? rb.linearDamping : 0.5f;
        Vector3 gravity = (rb != null && rb.useGravity) ? Physics.gravity : Vector3.zero;

        List<Vector3> dashPath = new List<Vector3>();
        Vector3 position = start;

        for (float t = 0f; t <= duration; t += physicsStep)
        {
            position += velocity * physicsStep + 0.5f * gravity * physicsStep * physicsStep;
            velocity += gravity * physicsStep;
            velocity *= 1f / (1f + drag * physicsStep);
            dashPath.Add(position);
        }

        using (new Handles.DrawingScope(Color.yellow))
        {
            Handles.DrawAAPolyLine(dashPath.ToArray());
            Gizmos.DrawWireSphere(dashPath[dashPath.Count - 1], 0.125f);
            Handles.Label(dashPath[dashPath.Count - 1], "Estimated End");
        }
    }

    private static Vector3 GetDashDirectionByZRotation(float zRotation)
    {
        if (zRotation <= 15f || zRotation >= 345f)
            return Vector3.up;
        else if (zRotation > 15f && zRotation <= 180f)
            return Vector3.left;
        else
            return Vector3.right;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}

