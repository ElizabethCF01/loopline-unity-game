using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum MoveToOrbitMode
{
    Once,
    PingPong,
    Loop,
}

public class MoveToOrbit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _objectToMove;
    [SerializeField] private Transform _center;
    [SerializeField] private Transform _target;

    [Header("Timing & Modes")]
    [Tooltip("Seconds to go from start → target (or back)")]
    [SerializeField] private float _duration = 2f;
    [SerializeField] private MoveToOrbitMode _mode = MoveToOrbitMode.Once;
    [SerializeField] private float _delayAtTarget = 0f;
    [SerializeField] private float _delayAtStart = 0f;

    [Header("Finish Events")]
    [SerializeField] private UnityEvent _onStopMoving;
    [SerializeField] private UnityEvent _onFinish;

    private Vector3 _startOffset;
    private Vector3 _startDir, _targetDir;
    private float _startRadius, _targetRadius;

    private float _timer;
    private bool _isMoving, _forward = true, _isPaused;

    private void Start()
    {
        // nothing to do until Move() is called
    }

    /// <summary>
    /// Call to kick off the automatic orbit‐and‐move.
    /// </summary>
    public void Move()
    {
        if (_center == null || _target == null || _objectToMove == null)
        {
            Debug.LogWarning("MoveToOrbit: missing references");
            return;
        }

        // Capture start‐and‐target offsets (relative to pivot)
        _startOffset = _objectToMove.position - _center.position;
        _startRadius = _startOffset.magnitude;
        _startDir = _startOffset.normalized;

        Vector3 tgtOff = _target.position - _center.position;
        _targetRadius = tgtOff.magnitude;
        _targetDir = tgtOff.normalized;

        // Reset timers & flags
        _timer = 0f;
        _isMoving = true;
        _forward = true;
        _isPaused = false;
    }

    private void Update()
    {
        if (!_isMoving || _isPaused)
            return;

        // Advance our normalized progress (0→1 over _duration seconds)
        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / _duration);

        // Determine which directions/radii to interpolate between
        Vector3 fromDir = _forward ? _startDir : _targetDir;
        Vector3 toDir = _forward ? _targetDir : _startDir;
        float fromR = _forward ? _startRadius : _targetRadius;
        float toR = _forward ? _targetRadius : _startRadius;

        // Compute the orbital position
        Vector3 curDir = Vector3.Slerp(fromDir, toDir, t);
        float curRadius = Mathf.Lerp(fromR, toR, t);
        _objectToMove.position = _center.position + curDir * curRadius;

        // Always look back at the pivot
        _objectToMove.LookAt(_center);

        // If we’ve reached the end of this leg…
        if (t >= 1f)
        {
            StartCoroutine(HandleEndpoint(_forward));
        }
    }

    private IEnumerator HandleEndpoint(bool reachedTarget)
    {
        _isPaused = true;
        _onStopMoving?.Invoke();

        // Wait at whichever end we’re at
        float delay = reachedTarget ? _delayAtTarget : _delayAtStart;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // Fire overall finish if appropriate
        if ((reachedTarget && _mode == MoveToOrbitMode.Once) ||
            (!reachedTarget && _mode == MoveToOrbitMode.PingPong))
        {
            _onFinish?.Invoke();
        }

        switch (_mode)
        {
            case MoveToOrbitMode.Once:
                _isMoving = false;
                break;

            case MoveToOrbitMode.PingPong:
                // Flip direction and restart
                _forward = !_forward;
                _timer = 0f;
                _isPaused = false;
                break;

            case MoveToOrbitMode.Loop:
                // Jump back to start and replay
                if (reachedTarget)
                {
                    // reset the timer & position
                    _timer = 0f;
                    _objectToMove.position = _center.position + _startDir * _startRadius;
                    if (_delayAtStart > 0f)
                        yield return new WaitForSeconds(_delayAtStart);
                }
                _isPaused = false;
                break;
        }
    }
}
