using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public bool IsKeyPickedUp { get; set; } = false;

    [SerializeField] private UnityEvent onWinGame;

    public void WinGame()
    {
        if (!IsKeyPickedUp)
        {
            Debug.LogWarning("Cannot win the game without picking up the key first.");
            return;
        }

        Debug.Log("You win!");
        onWinGame?.Invoke();
    }
}
