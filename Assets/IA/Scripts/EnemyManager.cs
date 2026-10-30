using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Action<Transform> HunterDetected;
    public static EnemyManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}