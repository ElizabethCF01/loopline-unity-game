using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    [Header("Parts to Move")]
    [SerializeField] private Transform buttonTop;

    [Header("Press Settings")]
    [SerializeField] private float pressDepth = 0.2f;
    [SerializeField] private float pressSpeed = 2f;
    [SerializeField] private float pressDelay = 0.1f;
    [SerializeField] private bool onlyPressOnce = false;

    [Header("Player Sink")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool sinkPlayer = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onPress;
    [SerializeField] private UnityEvent onRelease;
    [SerializeField] private float pressEventDelay = 0f;

    private Vector3 startLocalPos;
    private Vector3 pressedLocalPos;
    private bool isPressed;
    private bool hasPressed;
    private NavMeshAgent playerAgent;
    private Coroutine pressRoutine;
    private Coroutine releaseRoutine;

    private void Awake()
    {
        if (buttonTop == null && transform.childCount > 0)
            buttonTop = transform.GetChild(0);
    }

    private void Start()
    {
        startLocalPos = buttonTop.localPosition;
        pressedLocalPos = startLocalPos + Vector3.down * pressDepth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPressed || (onlyPressOnce && hasPressed)) return;
        if (!other.CompareTag(playerTag)) return;

        if (sinkPlayer)
        {
            playerAgent = other.GetComponentInParent<NavMeshAgent>();
        }
        
        if (pressRoutine != null)
            StopCoroutine(pressRoutine);
        pressRoutine = StartCoroutine(PressDown());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPressed) return;
        if (!other.CompareTag(playerTag)) return;

        if (releaseRoutine != null)
            StopCoroutine(releaseRoutine);
        releaseRoutine = StartCoroutine(ReleaseUp());
    }

    private IEnumerator PressDown()
    {
        yield return new WaitForSeconds(pressDelay);
        yield return StartCoroutine(InvokeEventAfterDelay(onPress, pressEventDelay));

        isPressed = true;
        hasPressed = true;
        float elapsed = 0f;
        float originalOffset = playerAgent != null ? playerAgent.baseOffset : 0f;
        float targetOffset = originalOffset - pressDepth;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * pressSpeed;
            float t = Mathf.Clamp01(elapsed);
            buttonTop.localPosition = Vector3.Lerp(startLocalPos, pressedLocalPos, t);
            if (playerAgent != null)
                playerAgent.baseOffset = Mathf.Lerp(originalOffset, targetOffset, t);
            yield return null;
        }
        pressRoutine = null;
    }

    private IEnumerator ReleaseUp()
    {
        onRelease?.Invoke();
        float elapsed = 0f;
        float originalOffset = playerAgent != null ? playerAgent.baseOffset : 0f;
        float targetOffset = originalOffset + pressDepth;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * pressSpeed;
            float t = Mathf.Clamp01(elapsed);
            if (!onlyPressOnce)
                buttonTop.localPosition = Vector3.Lerp(pressedLocalPos, startLocalPos, t);
            if (playerAgent != null)
                playerAgent.baseOffset = Mathf.Lerp(originalOffset, targetOffset, t);
            yield return null;
        }
        isPressed = false;
        playerAgent = null;
        releaseRoutine = null;
    }

    private IEnumerator InvokeEventAfterDelay(UnityEvent unityEvent, float delay)
    {
        yield return new WaitForSeconds(delay);
        unityEvent?.Invoke();
    }
}
