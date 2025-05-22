using UnityEngine;
using UnityEngine.InputSystem;

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

    Rigidbody rb;
    AudioSource audioSource;
    bool canDash = true;
    bool isDashing = false;

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
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessThrust()
    {
        if (thrust.IsPressed() && !isDashing)
        {
            StartThrusting();
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

    private System.Collections.IEnumerator PerformDash()
    {
       
        
        AudioSource.PlayClipAtPoint(DashSFX, transform.position);
        

        canDash = false;
        isDashing = true;

        Vector3 dashForce = transform.right * (dashDistance / dashDuration);
        rb.AddForce(dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // Cooldown before another dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    
}

