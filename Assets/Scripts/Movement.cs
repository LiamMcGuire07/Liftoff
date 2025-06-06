using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Movement : MonoBehaviour
{
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;
    [SerializeField] InputAction dash; 
    [SerializeField] float thrustStrength = 1000;
    [SerializeField] float rotationStrength = 100;
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] AudioClip DashSFX;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] float dashDistance = 10; 
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 5;
    FuelSystem fuelSystem;

    Rigidbody rb;
    AudioSource audioSource;
    bool canDash = true;
    public bool isDashing = false;

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
        dash.Enable(); 
        dash.performed += ctx => Dash(); 
    }

    private void OnDisable()
    {
        thrust.Disable();
        rotation.Disable();
        dash.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        fuelSystem = GetComponent<FuelSystem>();
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessThrust()
    {
        if (fuelSystem.GetCurrentFuel() <= 0)
        {
            mainEngineParticles.Stop();  
            audioSource.Stop();
            return; //No more thrusting when all out of fuel!

        }
        if (thrust.IsPressed() && !isDashing)
        {
            StartThrusting();

            fuelSystem.ConsumeFuel(10 * Time.fixedDeltaTime);
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }

        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }

        if (fuelSystem.fuel <= 50)
        {
            thrustStrength = 700;
            rotationStrength = 50;
        }
        else if (fuelSystem.fuel <= 30)
        {
            thrustStrength = 600;
            rotationStrength = 20;
        }
        else
        {
            thrustStrength = 1000;
            rotationStrength = 100;
        }
        
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();
        if (rotationInput < 0)
        {
            RotateRight();
        }
        else if (rotationInput > 0)
        {
            RotateLeft();
        }
        else
        {
            StopRotating();
        }
    }

    private void StopRotating()
    {
        rightThrustParticles.Stop();
        leftThrustParticles.Stop();
    }

    private void RotateLeft()
    {
        ApplyRotation(-rotationStrength);
        if (!leftThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
            leftThrustParticles.Play();
        }
    }

    private void RotateRight()
    {
        ApplyRotation(rotationStrength);
        if (!rightThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Play();
        }
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }

    public void Dash()
    {

        if (canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        if (fuelSystem.fuel < 20)
        {
            yield break; 
        }

        fuelSystem.ConsumeFuel(30);
        fuelSystem.UseDash(100);

        AudioSource.PlayClipAtPoint(DashSFX, transform.position);
        
        canDash = false;
        isDashing = true;

        Vector3 dashForce = transform.right * (dashDistance / dashDuration);
        rb.AddForce(dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // Cooldown before another dash
        fuelSystem.DashCooldown(5);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    
}

