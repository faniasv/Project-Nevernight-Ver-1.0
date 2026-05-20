using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerData data = PlayerDataManager.Instance.SessionData;
            Debug.Log($"[GameManager] Engine Ready. Tracking Player ID: {data.playerID}");
        }
    }
}