using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DirectionalEnemySpawn))]
public class DirectionalEnemySpawnEditor : Editor
{
    private const float ArrowLength = 2f;

    private void OnSceneGUI()
    {
        var spawn = (DirectionalEnemySpawn)target;
        Vector3 origin = spawn.transform.position;

        Vector3 dir = new Vector3(spawn.direction.x, spawn.direction.y, 0f);
        if (dir.sqrMagnitude < 1e-6f) dir = Vector3.right;
        dir.Normalize();

        Vector3 tip = origin + dir * ArrowLength;

        // Draw an arrow and a guide line
        Handles.color = Color.cyan;
        Handles.DrawLine(origin, tip);
        Handles.ArrowHandleCap(0, origin, Quaternion.LookRotation(dir, Vector3.up), ArrowLength, EventType.Repaint);

        // Draggable handle constrained to XY plane (good for 2D projects)
        float handleSize = HandleUtility.GetHandleSize(tip) * 0.15f;

        EditorGUI.BeginChangeCheck();
        Vector3 newTip = Handles.Slider2D(
            tip,
            Vector3.forward,   // plane normal (XY plane)
            Vector3.right,     // x axis
            Vector3.up,        // y axis
            handleSize,
            Handles.CircleHandleCap,
            0f
        );
        if (EditorGUI.EndChangeCheck())
        {
            Vector3 newDir = (newTip - origin);
            if (newDir.sqrMagnitude > 1e-6f)
            {
                Undo.RecordObject(spawn, "Change Direction");
                spawn.direction = new Vector2(newDir.x, newDir.y).normalized;
                EditorUtility.SetDirty(spawn);
                SceneView.RepaintAll();
            }
        }
    }
}