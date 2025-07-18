using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerReaction : MonoBehaviour
{
    [Header("Reaction Settings")]
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private UnityEvent _onExit;

    [Header("Filter Settings")]
    [SerializeField] private bool _filterByTag = false;
    [SerializeField] private string _filterTag = "Player";

    void Awake()
    {
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("TriggerReaction requires a Collider component.");
            enabled = false;
            return;
        }

        // Ensure the collider is set as a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_filterByTag && !other.CompareTag(_filterTag))
        {
            return;
        }

        _onEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_filterByTag && !other.CompareTag(_filterTag))
        {
            return;
        }
        
        _onExit?.Invoke();
    }
}
