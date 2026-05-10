using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // ==== Global State Variables === 

    // Dipanggil saat ada aksi yang menambah beban mental Ava.
    // Int= Jumlah beban yang ditambahkan (misal: 10)
    public static Action<int> OnAvaLoadAdded;

    // Dipanggil GameManager saat total beban mental berubah.
    // Int= Total beban saat ini (untuk update UI Slider)
    public static Action<int> OnAvaLoadChanged;

    // Dipanggil saat pindah Act.
    // Int= Nomor Act tujuan (misal: 2)
    public static Action<int> OnActChanged;
    
    // ==== ACT 1: Task Puzzle Events === 

    // Dipanggil saat pemain gagal menaruh tugas di papan.
    public static Action OnTaskFailed;

    // Dipanggil saat pemain kehabisan jatah gagal (misal: sudah 3x salah).
    public static Action OnPlayerStuck;

    // ==== ACT 2: Vase Puzzle Events === 

    // Dipanggil saat seluruh kepingan vas sudah terpasang dengan benar.
    public static Action OnVaseCompleted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
