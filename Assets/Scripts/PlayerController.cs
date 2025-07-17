using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Animator _animator;

    [Header("Movement Settings")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _turnSpeed = 360f;

    [SerializeField, Range(0f, 45f)]
    private float _turnThreshold = 10f;

    [Header("Selection Highlight")]
    [SerializeField] private Material _targetMaterial;

    private NavMeshAgent _playerAgent;
    private GameObject _selectedObject;
    private Renderer _selectedRenderer;

    void Awake()
    {
        _playerAgent = GetComponent<NavMeshAgent>();
        _playerAgent.updateRotation = false;
        _playerAgent.updateUpAxis = true;
    }

    void Update()
    {
        HandleClickMove();
        UpdateAnimator();
        TryConditionalRotate();
    }

    private void HandleClickMove()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out var hit, 100f, _groundMask))
        {
            UpdateSelectedObject(hit.collider.gameObject);
            var bounds = hit.collider.bounds;
            Vector3 targetPos = new(
                bounds.center.x,
                bounds.max.y,
                bounds.center.z
            );
            _playerAgent.SetDestination(targetPos);
        }
    }

    private void UpdateAnimator()
    {
        float speedPercent = _playerAgent.velocity.magnitude / _playerAgent.speed;
        _animator.SetFloat("Speed", speedPercent);
    }

    private void TryConditionalRotate()
    {
        Vector3 desired = _playerAgent.desiredVelocity;
        desired.y = 0;

        if (desired.sqrMagnitude < 0.01f)
            return;

        float angle = Vector3.Angle(transform.forward, desired);
        if (angle < _turnThreshold)
            return;

        Quaternion targetRot = Quaternion.LookRotation(desired);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            _turnSpeed * Time.deltaTime
        );
    }

    private void UpdateSelectedObject(GameObject newTarget)
    {
        if (_selectedObject != null && _selectedRenderer != null)
        {
            var mats = new List<Material>();
            _selectedRenderer.GetSharedMaterials(mats);
            mats.Remove(_targetMaterial);
            _selectedRenderer.SetSharedMaterials(mats);
        }

        // Add highlight to the new one
        _selectedRenderer = newTarget.GetComponentInChildren<Renderer>();
        if (_selectedRenderer != null)
        {
            var mats = new List<Material>();
            _selectedRenderer.GetSharedMaterials(mats);
            mats.Add(_targetMaterial);
            _selectedRenderer.SetSharedMaterials(mats);
            _selectedObject = newTarget;
        }
        else
        {
            Debug.LogWarning("No Renderer found on selected object.");
        }
    }
}
