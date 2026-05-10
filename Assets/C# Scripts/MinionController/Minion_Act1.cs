using UnityEngine;

public class Minion_Act1 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float moveRange = 200f; 
    
    private Vector3 startPos;
    private bool isIntro = true;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (isIntro)
        {
            // Gerak kanan kiri kaya jalan biasa (PingPong)
            float xOffset = Mathf.PingPong(Time.time * moveSpeed, moveRange * 2) - moveRange;
            transform.localPosition = new Vector3(startPos.x + xOffset, startPos.y, startPos.z);
        }
    }

    // Panggil ini saat masuk ke Puzzle
    public void SetToPuzzleMode()
    {
        isIntro = false;
        transform.localPosition = startPos; // Balik ke tengah/posisi awal
        Debug.Log("Minion Act 1: Mode Puzzle (Diam)");
    }

    // Panggil ini kalau mau balik ke mode Intro
    public void SetToIntroMode()
    {
        isIntro = true;
    }
}