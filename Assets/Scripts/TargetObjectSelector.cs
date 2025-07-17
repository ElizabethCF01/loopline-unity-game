using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TargetObjectSelector : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private NavMeshAgent _playerAgent;

    [SerializeField]
    private Material _targetMaterial;

    private GameObject _selectedObject;
    private Renderer _selectedRenderer;

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var newTarget = hit.collider.gameObject;
                UpdateSelectedObject(newTarget);
                _playerAgent.SetDestination(newTarget.transform.position);
            }
        }
    }

    private void UpdateSelectedObject(GameObject newTarget)
    {
        if (_selectedObject != null)
        {
            var m = new List<Material>();
            _selectedRenderer.GetSharedMaterials(m);
            m.Remove(_targetMaterial);
            _selectedRenderer.SetSharedMaterials(m);
        }

        _selectedRenderer = newTarget.GetComponentInChildren<Renderer>();
        if (_selectedRenderer == null)
        {
            Debug.LogWarning("Not able to find a Renderer component in the children of the selected object");
            return;
        }

        var materials = new List<Material>();
        _selectedRenderer.GetSharedMaterials(materials);
        materials.Add(_targetMaterial);
        _selectedRenderer.SetSharedMaterials(materials);

        _selectedObject = newTarget;
    }
}