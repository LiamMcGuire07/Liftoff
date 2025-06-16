using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Break : MonoBehaviour
{
    public GameObject breakVFX;
    public Material breakableMaterial;
    public AudioClip breakSFX;

    AudioSource audioSource;

    [HideInInspector] public new Rigidbody rigidbody;

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
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.mass = 0.01f;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = false;

        if (UnityEditorInternal.InternalEditorUtility.tags.Contains("Breakable"))
        {
            tag = "Breakable";
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }
}

