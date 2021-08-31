using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rotationThrust = 100f;
    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem rightEngineParticles;
    [SerializeField] ParticleSystem leftEngineParticles;
    private Rigidbody rb;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessRotation()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            StartRotationLeft();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            StartRotationRight();
        }
        else
        {
            StopRotation();
        }
    }

    private void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartRotationLeft()
    {
        if (!rightEngineParticles.isPlaying)
        {
            rightEngineParticles.Play();
        }
        ApplyRotation(rotationThrust);
    }

    private void StartRotationRight()
    {
        if (!leftEngineParticles.isPlaying)
        {
            leftEngineParticles.Play();
        }
        ApplyRotation(-rotationThrust);
    }

    private void StopRotation()
    {
        leftEngineParticles.Stop();
        rightEngineParticles.Stop();
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true; // freezing rotation so we can manually rotate
        transform.Rotate(rotationThisFrame * Time.deltaTime * Vector3.forward);
        rb.freezeRotation = false; // restoring control to physics system
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(mainThrust * Time.deltaTime * Vector3.up);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSound);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void StopThrusting()
    {
        mainEngineParticles.Stop();
        audioSource.Stop();
    }
}