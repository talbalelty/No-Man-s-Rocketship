using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float reloadDelay = 1f;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip crashSound;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] Text crashText;

    private AudioSource audioSource;
    private bool isTransitioning = false; // Performing a coroutine, stop other routines from running
    private bool collisionDisabled = false; // debug feature
    private static int numOfCrashes = 0;
    bool isDebuging = false; // unlock debugging options

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        crashText.text = " Crashes " + numOfCrashes;
    }

    void Update()
    {
        if(isDebuging)
        {
            RespondToDebugKeys(); // TODO - disable cheats when finished
        }
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
        if (isTransitioning || collisionDisabled) // Hold off other coroutines or cheat enabled
        {
            return;
        }
        switch (other.gameObject.tag)
        {
            case "Friendly":
                // Extra game feature - maybe resting area in a bigger map?
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
        isTransitioning = true; // disable other coroutines
        GetComponent<RocketMovement>().enabled = false;
        audioSource.Stop();
        audioSource.PlayOneShot(crashSound);
        crashParticles.Play();
        numOfCrashes++;
        
        yield return new WaitForSeconds(reloadDelay);
        ReloadLevel();

        GetComponent<RocketMovement>().enabled = true;
        isTransitioning = false; // enable other coroutines 
    }

    IEnumerator LoadNextLevel()
    {
        isTransitioning = true; // disable other coroutines
        GetComponent<RocketMovement>().enabled = false;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        successParticles.Play();

        yield return new WaitForSeconds(reloadDelay);
        // Calculate next scene index and load
        int NumOfLevels = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % NumOfLevels;
        SceneManager.LoadScene(nextSceneIndex);
        if(nextSceneIndex == 0)
        {
            numOfCrashes = 0;
        }

        GetComponent<RocketMovement>().enabled = true;
        isTransitioning = false; // enable other coroutines 
    }

    private void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
