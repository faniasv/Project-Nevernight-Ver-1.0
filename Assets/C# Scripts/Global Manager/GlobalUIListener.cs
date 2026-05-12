using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlobalUIListener : MonoBehaviour
{
    void Update()
    {
        // Mendeteksi setiap kali mouse diklik kiri
        if (Input.GetMouseButtonDown(0))
        {
            // Cek apakah mouse sedang berada di atas objek UI (Tombol, Gambar, dll)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PlayGlobalClick();
            }
        }
    }

    void PlayGlobalClick()
    {
        if (AudioManager.instance != null)
        {
            // Pastikan kamu sudah mendaftarkan "MouseClick" di AudioData kamu
            AudioManager.instance.PlaySFX("MouseClick");
        }
    }
}