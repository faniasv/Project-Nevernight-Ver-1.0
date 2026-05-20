using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private PlayerData _sessionData;
    public PlayerData SessionData => _sessionData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateNewSession();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateNewSession()
    {
        // Jangkauan 4 digit diperluas untuk menghindari duplikasi ID di Google Sheets
        int randomId = UnityEngine.Random.Range(1000, 9999);

        _sessionData = new PlayerData
        {
            playerID = "User_" + randomId,
            username = "Playtesting_Guest",
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            currentAct = 1
        };
    }

    public void SetUsername(string inputName)
    {
        if (_sessionData != null && !string.IsNullOrEmpty(inputName))
        {
            _sessionData.username = inputName;
        }
    }
}