using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Break : MonoBehaviour
{
    public GameObject breakVFX;
    public Material breakableMaterial;
    public AudioClip breakSFX;

    AudioSource audioSource;

    [HideInInspector] public Rigidbody rb;

    public void TriggerBreak()
    {
        if (breakSFX)
        {
            AudioSource.PlayClipAtPoint(breakSFX, transform.position);
        }

        if (breakVFX)
        {
            Instantiate(breakVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = 0.01f;
        rb.useGravity = false;
        rb.isKinematic = false;

    #if UNITY_EDITOR
        if (UnityEditorInternal.InternalEditorUtility.tags.Contains("Breakable"))
        {
            gameObject.tag = "Breakable";
        }
        else
        {
            Debug.LogWarning("Tag 'Breakable' does not exist. Please add it manually in the Tag Manager.");
        }
    #endif
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }
}

