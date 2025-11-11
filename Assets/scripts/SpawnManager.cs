using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    // --- Singleton ---
    public static SpawnManager Instance { get; private set; }

    [Header("Master Prefab List")]
    [Tooltip("The one, true list of ALL unique scene prefabs.")]
    [SerializeField] private GameObject[] scenes;
    
    [Tooltip("The single, specific prefab to spawn when all 'scenes' are exhausted and Endless Mode is OFF.")]
    [SerializeField] private GameObject endOfCyclePrefab;

    [Header("Debugging")]
    [Tooltip("If checked, the spawner will ONLY spawn the 'debugPrefab' and ignore all other logic.")]
    [SerializeField] private bool forceSpecificPrefab = false;
    [SerializeField] private GameObject debugPrefab;
    [Tooltip("If checked, the spawner will loop back to the beginning after exhausting the 'scenes' list.")]
    [SerializeField] private bool endlessMode = false;

    private List<int> remainingPrefabIndices;
    private static int spawnCount = 0; 
    private bool isSetupValid = true; 
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return; 
        }
        else
            Instance = this;
            // DontDestroyOnLoad(this.gameObject); THIS BREAKS IT. DON'T ENABLE. BAD.

        isSetupValid = true; // Assume true until a check fails

        if (scenes == null || scenes.Length == 0)
        {
            Debug.LogError("[SpawnManager] FATAL ERROR: 'Scenes' array is null or empty! " +
                           "Please assign scene prefabs in the Inspector.", this);
            isSetupValid = false;
        }

        // this might be FUKCED
        if (forceSpecificPrefab && debugPrefab == null)
        {
            Debug.LogError("[SpawnManager] FATAL ERROR: 'Force Specific Prefab' is ON, " +
                           "but no 'Debug Prefab' is assigned! Disabling script.", this);
            isSetupValid = false;
        }
        
        // is endless cooked?
        if (!endlessMode && endOfCyclePrefab == null)
        {
            // would still owrk, but not extremely important
            Debug.LogWarning("[SpawnManager] WARNING: 'Endless Mode' is OFF, " +
                             "but no 'End Of Cycle Prefab' is assigned. Nothing will spawn after the cycle ends.", this);
        }

        if (!isSetupValid)
        {
            this.enabled = false; // die.
            return;
        }

        InitializePool();
    }

    private void InitializePool()
    {
        remainingPrefabIndices = new List<int>(scenes.Length);
        for (int i = 0; i < scenes.Length; i++)
            remainingPrefabIndices.Add(i);
        Debug.Log($"[SpawnManager] Initialized shared pool with {scenes.Length} prefab indices.");
    }

    public GameObject GetNextPrefabToSpawn()
    {
        if (forceSpecificPrefab)
        {
            Debug.Log($"[SpawnManager] DEBUG MODE: Forcing return of '{debugPrefab.name}'");
            return debugPrefab;
        }

        if (remainingPrefabIndices.Count == 0)
        {
            if (endlessMode)
            {
                Debug.Log("[SpawnManager] Pool is empty. Refilling pool for a new cycle...");
                InitializePool();
            }
            else
            {
                Debug.Log("[SpawnManager] Cycle complete. Returning 'endOfCyclePrefab'.");
                return endOfCyclePrefab;
            }
        }
        
        int randomPoolIndex = Random.Range(0, remainingPrefabIndices.Count);
        int prefabArrayIndex = remainingPrefabIndices[randomPoolIndex];
        remainingPrefabIndices.RemoveAt(randomPoolIndex);

        GameObject selectedPrefab = scenes[prefabArrayIndex];
        
        spawnCount++; 
        
        Debug.Log($"[SpawnManager] Returning {selectedPrefab.name}. {remainingPrefabIndices.Count} prefabs left. Total Spawns: {spawnCount}");
        return selectedPrefab;
    }
}