using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Break))]
public class BreakScriptEditor : Editor
{
    private GameObject currentTarget;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;

    public override void OnInspectorGUI()
    {
        Break breakable = (Break)target;
        currentTarget = breakable.gameObject;

        // Get current MeshRenderer and its material
        meshRenderer = currentTarget.GetComponent<MeshRenderer>();
        if (meshRenderer != null && originalMaterial == null)
        {
            originalMaterial = meshRenderer.sharedMaterial;
        }

        EditorGUILayout.LabelField("Breakable Object Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        breakable.breakSFX = (AudioClip)EditorGUILayout.ObjectField(
            new GUIContent("Sound"),
            breakable.breakSFX,
            typeof(AudioClip),
            false);

        // VFX assignment field
        breakable.breakVFX = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("VFX Prefab"),
            breakable.breakVFX,
            typeof(GameObject),
            false);

        // Material field (auto-filled + applies to renderer)
        EditorGUI.BeginChangeCheck();
        breakable.breakableMaterial = (Material)EditorGUILayout.ObjectField(
            new GUIContent("Material"),
            breakable.breakableMaterial ?? originalMaterial,
            typeof(Material),
            false);
        if (EditorGUI.EndChangeCheck() && meshRenderer != null)
        {
            Undo.RecordObject(meshRenderer, "Assign Material");
            meshRenderer.sharedMaterial = breakable.breakableMaterial;
        }
        DrawModelSwapField(breakable);

        EditorGUILayout.HelpBox("Assign VFX, material, and model settings here.", MessageType.Info);
    }

    private void DrawModelSwapField(Break breakable)
    {
        EditorGUI.BeginChangeCheck();

        GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Model"),
            currentTarget,
            typeof(GameObject),
            false);

        if (EditorGUI.EndChangeCheck() && newPrefab != currentTarget)
        {
            if (!PrefabUtility.IsPartOfPrefabAsset(newPrefab))
            {
                Debug.LogWarning("Selected object is not a prefab asset.");
                return;
            }

            int group = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();

            // Instantiate new prefab
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab, breakable.transform.parent);
            instance.transform.position = breakable.transform.position;
            instance.transform.rotation = breakable.transform.rotation;
            instance.transform.localScale = breakable.transform.localScale;

            Undo.RegisterCreatedObjectUndo(instance, "Swap to Prefab");

            // Keep prefab settings
            Break newBreak = Undo.AddComponent<Break>(instance);
            newBreak.breakVFX = breakable.breakVFX;
            newBreak.breakableMaterial = breakable.breakableMaterial;

            // Add Rigidbody if missing
            if (!instance.TryGetComponent<Rigidbody>(out var rb))
            {
                rb = Undo.AddComponent<Rigidbody>(instance);
                rb.mass = 0.01f;
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            // Apply tag
            if (UnityEditorInternal.InternalEditorUtility.tags.Contains("Breakable"))
            {
                instance.tag = "Breakable";
            }

            // Destroy old object with Undo support
            Undo.DestroyObjectImmediate(breakable.gameObject);

            // Collapse the entire operation into one step
            Undo.CollapseUndoOperations(group);
        }
    }
}




