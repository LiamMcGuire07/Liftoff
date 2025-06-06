using UnityEngine;
using UnityEditor;

public class BreakableToolWindow : EditorWindow
{
    private Material breakableMaterial;

    [MenuItem("Tools/Breakable Object Converter")]
    public static void ShowWindow()
    {
        GetWindow<BreakableToolWindow>("Breakable Tool"); // Show the window when clicked on
    }

    private void OnGUI()
    {
        GUILayout.Label("Convert Selected Objects to Breakables", EditorStyles.boldLabel); // Text label on top
        EditorGUILayout.Space(5); // Add some space 

        breakableMaterial = (Material)EditorGUILayout.ObjectField("Breakable Material", breakableMaterial, typeof(Material), false); // Select the material that will be assigned to the object

        EditorGUILayout.Space(10); // More spacing

        if (GUILayout.Button("Convert Selection")) // Convert when hit
        {
            ConvertSelectionToBreakables();
        }
    }

    private void ConvertSelectionToBreakables()
    {
        if (breakableMaterial == null) // Material must be assigned before converting. The breakable object needs to visually stand out from the original!
        {
            Debug.LogWarning("Please assign a breakable material before converting.");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            
            Undo.RegisterCompleteObjectUndo(obj, "Convert to Breakable Object"); // Allows operation on GameObject to be undone of needed


            obj.tag = "Breakable"; // Add tag

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) // Add rigid body if not present on object
            {
                rb = Undo.AddComponent<Rigidbody>(obj);
            }    

            // Keep mass low to prevent object from affecting the object's position and rotation on hit
            rb.mass = 0.01f;

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>(); 

            if (renderer != null) // Assign material if mesh renderer exists
            {
                Undo.RecordObject(renderer, "Assign Breakable Material"); // Allows operation to undo material change if needed
                renderer.sharedMaterial = breakableMaterial;
            }
            else
            {
                Debug.LogWarning($"GameObject '{obj.name}' has no MeshRenderer component — cannot assign breakable material.");
            }
                
                EditorUtility.SetDirty(obj); // Object has changed and needs to be saved
        }
    }
}
