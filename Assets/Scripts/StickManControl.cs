using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickManControl : MonoBehaviour
{
    [Range(.05f, 3f)]
    public float stepHeight = .25f; 
    [Range(.05f, 3f)]
    public float stepFrequency = .25f; 
    [Range(.05f, 1f)]
    public float fallSpeed = 1f; 
    // Start is called before the first frame update
    [SerializeField]
    private Transform prototypeStep;
    [SerializeField]
    private Transform prototypeStair;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private Animator animator;

    private bool fallTimerElapsed = true;
    private Coroutine fallTimerRoutine;
    private Coroutine makeStairsRoutine;


    private Stack<Transform> currentStepsCarried = new Stack<Transform>();


    // Update is called once per frame
    void Update()
    {
        transform.localPosition += transform.forward * speed * Time.deltaTime;

        if (transform.localPosition.y > 0f && fallTimerElapsed)
        {
            Debug.Log("Falling");
            animator.SetTrigger("fallTrig");
            transform.localPosition -= transform.up * fallSpeed * Time.deltaTime;
        }
        else if (transform.localPosition.y < 0f)
        {
            animator.SetTrigger("runTrig");
        }

        // Listen for spacebar. While it's down, make stairs.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            makeStairsRoutine = StartCoroutine(MakeStairs());
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(makeStairsRoutine);
        }
    }

    public void AddStair()
    {
        // Only add a new stair if there are steps in the pack.
        if (currentStepsCarried.Count > 0)
        {
            // start a countdown to avoid immediate falling.
            if (fallTimerRoutine != null)
            {
                StopCoroutine(fallTimerRoutine);
            }
            fallTimerRoutine = StartCoroutine(WaitForFall());
            
            // Remove a step from the pack
            RemoveStepsFromPack(1);

            // Move the player up one "stair" unit
            transform.localPosition += transform.up * stepHeight;
            // Create a new stair by cloning the disabled prototype in the scene.
            Transform newStair = Instantiate(prototypeStair, prototypeStair.parent);
            // Set its position/scale locally, so it matches the prototype.
            newStair.localPosition = prototypeStair.localPosition;
            // Re-parent to root (so it doesn't follow position of the player object, and is left behind)
            newStair.parent = null;
            // Enable the new stair object.
            newStair.gameObject.SetActive(true);
        }

    }

    public void AddStepsToPack(int count)
    {
        // for each step added...
        for (int i = 0; i < count; i++)
        {
            // Create a new step object, based on the disabled prototype in the scene.
            Transform newStep = Instantiate(prototypeStep, prototypeStep.parent).transform;
            // Add the new step to the stack.
            currentStepsCarried.Push(newStep);
            // Figure out what the local y position of the top pack position is
            // and move the new step to that y position
            float yLocalOffset = currentStepsCarried.Count * .8f;
            newStep.localPosition = new Vector3(prototypeStep.localPosition.x, prototypeStep.localPosition.y + yLocalOffset, prototypeStep.localPosition.z);
            // Enable the new step in the scene.
            newStep.gameObject.SetActive(true);
        }
    }

    public void RemoveStepsFromPack(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // If there's a step to remove...
            if (currentStepsCarried.Count > 0)
            {
                // Pop the top object off the stack
                Transform used = currentStepsCarried.Pop();
                // Destroy the game object (so it disappears)
                Destroy(used.gameObject);
            }
        }
    }

    private IEnumerator WaitForFall()
    {
        fallTimerElapsed = false;
        // Wait until we're sure more steps aren't coming.
        yield return new WaitForSeconds(stepFrequency * 2f);
        fallTimerElapsed = true;
    }

    private IEnumerator MakeStairs()
    {
        // Add stairs over and over again. 
        // This is started when spacebar goes down
        // Stopped when spacebar comes up.
        while (true)
        {
            AddStair();
            yield return new WaitForSeconds(stepFrequency);            
        }
    }
}
