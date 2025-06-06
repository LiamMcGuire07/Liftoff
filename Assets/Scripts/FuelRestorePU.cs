using UnityEngine;

public class FuelRestorePU : MonoBehaviour
{
    FuelSystem fuelSystem;
    [SerializeField] float fuelRestoreAmount = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fuelSystem = GetComponent<FuelSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fuelSystem != null)
            {
                fuelSystem.RefillFuel(fuelRestoreAmount);
            }

            // Optional: play sound, animation, particles here
            Destroy(gameObject); // remove power-up after use
        }
    }


}
