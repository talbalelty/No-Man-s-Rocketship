using UnityEngine;

// This class is used to oscilate obstacles movement 
public class Oscilator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector; // determines how many units the obstacle will move in total
    [SerializeField] float period = 2f; // length of animation - shorter means faster

    Vector3 startPosition;
    float movementFactor; // multiplied by movementVector to determine how much offset to move the obstacle

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period; // continually growing through time
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau); // [-1,1]

        movementFactor = (rawSinWave + 1f) / 2; // [0,1]

        Vector3 offset = movementVector * movementFactor;
        transform.position = startPosition + offset;
    }
}
