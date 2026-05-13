using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GlobalUIListener : MonoBehaviour
{
    void Update()
    {
        // Mendeteksi setiap kali mouse diklik kiri
        if (Input.GetMouseButtonDown(0))
        {
            // Cek apakah mouse sedang ada di dialog
            if (DialogueManager.instance != null && DialogueManager.instance.isDialogueActive) 
            {
                return; 
            }
            
            // Cek apakah mouse sedang berada di atas objek UI (Tombol, Gambar, dll)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PlayGlobalClick();
            }
        }
    }

    bool IsClickValid()
    {
        // Mendapatkan objek yang tepat berada di bawah kursor saat ini
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            GameObject clickedObj = results[0].gameObject;

            // 3. CEK DRAG: Jika yang diklik adalah item yang bisa di-drag, 
            // biarkan script DraggableItem yang mengurus suaranya sendiri.
            if (clickedObj.GetComponent<DraggableItem>() != null || 
                clickedObj.GetComponentInParent<DraggableItem>() != null)
            {
                return false; // Batalkan suara klik global
            }

            // [Polish] Cek apakah tombol ini punya suara khusus (seperti tombol 'Done')
            // Jika kamu memberi tag "SpecialButton" pada tombol penting, kita bisa skip di sini
            if (clickedObj.CompareTag("SpecialButton")) return false;
        }

        return true;
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