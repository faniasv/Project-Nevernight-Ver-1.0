using UnityEngine;

public class Minion_Act2 : MonoBehaviour
{
    public enum MinionState { Intro, Selection, Puzzle }
    public MinionState currentState = MinionState.Intro;

    [Header("Settings")]
    public float bobSpeed = 3f;
    public float bobAmount = 20f;
    public float shakeSpeed = 5f;
    public float shakeAmount = 2f;

    [Header("Positions")]
    public Vector3 centerPos; 
    public Vector3 cornerPos; 

    void Start()
    {
        centerPos = transform.localPosition;
    }

    void Update()
    {
        switch (currentState)
        {
            case MinionState.Intro:
                // Gerak naik turun (Bobbing)
                float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
                transform.localPosition = new Vector3(centerPos.x, centerPos.y + yOffset, centerPos.z);
                transform.localRotation = Quaternion.Euler(0, 0, 0); // Reset rotasi
                break;

            case MinionState.Selection:
                // Diam di tengah
                transform.localPosition = centerPos;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;

            case MinionState.Puzzle:
                // Pindah ke pojok kanan bawah + Shaking (Rotate)
                transform.localPosition = cornerPos;
                float zRotation = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
                transform.localRotation = Quaternion.Euler(0, 0, zRotation);
                break;
        }
    }

    public void ChangeState(MinionState newState)
    {
        currentState = newState;
        Debug.Log("Minion Act 2 State: " + newState);
    }
}