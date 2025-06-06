using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2;
    [SerializeField] AudioClip successSFX;
    [SerializeField] AudioClip crashSFX;
    [SerializeField] AudioClip powerUpSFX;
    [SerializeField] AudioClip BreakSFX;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] GameObject destroyedVFX;

    AudioSource audioSource;
    FuelSystem fuelSystem;
    Movement movement;


    bool isControllabe = true;
    bool isCollidable = true;

    // Start is called once before the first exection of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fuelSystem = GetComponent<FuelSystem>();
        movement = GetComponent<Movement>();

    }

    // Update is called once per frame
    void Update()
    {
        RespondToDebugKeys();
    }

    void RespondToDebugKeys()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            LoadNextLevel();
        }
        else if(Keyboard.current.cKey.wasPressedThisFrame) 
        {
            isCollidable = !isCollidable;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isControllabe || !isCollidable)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Fuel":
                audioSource.Stop();
                audioSource.PlayOneShot(powerUpSFX);
                fuelSystem.RefillFuel(30);
                Destroy(collision.gameObject);
                break;
            case "Breakable":
                // Needs to be dashing to break the rock!
                if (movement.isDashing)
                {
                    
                    AudioSource.PlayClipAtPoint(BreakSFX, transform.position);
                    GenerateParticleSFX(collision);
                    Destroy(collision.gameObject);
                }
                else
                {
                    StartCrashSequence();
                };
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    private void GenerateParticleSFX(Collision collision)
    {
        Vector3 vfxPosition = collision.transform.position + new Vector3(0, -2.5f, 0); // slight downward offset to make explosion centered
        Instantiate(destroyedVFX, vfxPosition, Quaternion.identity);
        Destroy(collision.gameObject);
    }

    void StartSuccessSequence()
    {
        isControllabe = false;
        audioSource.Stop();
        audioSource.PlayOneShot(successSFX);
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartCrashSequence()
    {
        isControllabe = false;
        audioSource.Stop();
        audioSource.PlayOneShot(crashSFX);
        crashParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", levelLoadDelay);
    }

    void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;

        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }
        SceneManager.LoadScene(nextScene);
    }
    
   
}
