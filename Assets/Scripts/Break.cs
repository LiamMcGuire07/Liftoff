using UnityEngine;


public class Break : MonoBehaviour
{
    public GameObject fractured;
    public float breakForce;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void BreakObject()
    {
        GameObject frac = Instantiate(fractured, transform.position, transform.rotation);

        foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position).normalized * breakForce;
            rb.AddForce(force);
        }

    }
}
