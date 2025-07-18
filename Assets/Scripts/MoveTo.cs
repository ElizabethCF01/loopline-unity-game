using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveTo : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 2f;

    [Header("Events")]
    [SerializeField] private UnityEvent _onStartMoving;
    [SerializeField] private UnityEvent _onStopMoving;

    private bool _isMoving = false;

    public void Move()
    {
        _isMoving = true;
    }

    private void Update()
    {
        if (!_isMoving) return;
        if (_target == null) return;

        Vector3 direction = (_target.position - transform.position).normalized;
        float step = _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _target.position) > step)
        {
            transform.position += direction * step;
            _onStartMoving?.Invoke();
        }
        else
        {
            transform.position = _target.position;
            _isMoving = false;
            _onStopMoving?.Invoke();
        }
    }
}
