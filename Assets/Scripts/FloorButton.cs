using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    [Header("Parts to Move")]
    [Tooltip("The visible button top that moves down.")]
    public Transform buttonTop;

    [Header("Press Settings")]
    public float pressDepth = 0.2f;
    public float pressSpeed = 2f;
    public float pressDelay = 0.1f;

    [Header("Player Sink")]
    [Tooltip("Tag your player GameObject accordingly.")]
    public string playerTag = "Player";

    [Header("Events")]
    public UnityEvent onPress;
    public UnityEvent onRelease;

    private Vector3 _startPos;
    private Vector3 _pressedPos;
    private bool _isPressed;
    private NavMeshAgent _playerAgent;

    void Start()
    {
        _startPos = buttonTop.localPosition;
        _pressedPos = _startPos + Vector3.down * pressDepth;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_isPressed) return;

        if (other.CompareTag(playerTag))
        {
            _playerAgent = other.GetComponentInParent<NavMeshAgent>();
            StartCoroutine(PressDown());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!_isPressed) return;

        if (other.CompareTag(playerTag))
        {
            StartCoroutine(ReleaseUp());
        }
    }

    IEnumerator PressDown()
    {
        yield return new WaitForSeconds(pressDelay);
        onPress?.Invoke();
        
        float startOffset = _playerAgent.baseOffset;
        float endOffset   = startOffset - pressDepth;
        _isPressed = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            buttonTop.localPosition = Vector3.Lerp(_startPos, _pressedPos, t);

            if (_playerAgent != null)
            {
                _playerAgent.baseOffset = Mathf.Lerp(startOffset, endOffset, t);
            }

            yield return null;
        }
    }

    IEnumerator ReleaseUp()
    {
        onRelease?.Invoke();

        float startOffset = _playerAgent.baseOffset;
        float endOffset   = startOffset + pressDepth;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            // buttonTop.localPosition = Vector3.Lerp(_pressedPos, _startPos, t);
            if (_playerAgent != null)
            {
                _playerAgent.baseOffset = Mathf.Lerp(startOffset, endOffset, t);
            }
            yield return null;
        }
        _isPressed = false;
        _playerAgent = null;
    }
}
