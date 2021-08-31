using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float reloadDelay = 1f;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip crashSound;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    private AudioSource audioSource;
    private bool isTransitioning = false;
    private bool collisionDisabled = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        RespondToDebugKeys(); // TODO - disable cheats when finished
    }

    // TODO - disable cheats when finished
    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(LoadNextLevel());
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; // Toggle collisions
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isTransitioning || collisionDisabled)
        {
            return;
        }
        switch (other.gameObject.tag)
        {
            case "Friendly":
                // Extra game feature - maybe resting area?
                break;
            case "Finish":
                StartCoroutine(LoadNextLevel());
                break;
            default:
                StartCoroutine(StartCrashSequence());
                break;
        }
    }

    IEnumerator StartCrashSequence()
    {
        isTransitioning = true;
        GetComponent<RocketMovement>().enabled = false;
        audioSource.Stop();
        audioSource.PlayOneShot(crashSound);
        crashParticles.Play();
        yield return new WaitForSeconds(reloadDelay);
        ReloadLevel();
        GetComponent<RocketMovement>().enabled = true;
        isTransitioning = false;
    }

    IEnumerator LoadNextLevel()
    {
        isTransitioning = true;
        GetComponent<RocketMovement>().enabled = false;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        successParticles.Play();
        yield return new WaitForSeconds(reloadDelay);
        int NumOfLevels = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % NumOfLevels;
        SceneManager.LoadScene(nextSceneIndex);
        GetComponent<RocketMovement>().enabled = true;
        isTransitioning = false;
    }

    private void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
