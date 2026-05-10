using UnityEngine;

public class StickyNoteController : MonoBehaviour
{
    // Fungsi ini yang akan dipanggil saat tombol diklik
    public void CloseNote()
    {
        OvtNotesManager manager = FindObjectOfType<OvtNotesManager>();
        
        if (manager != null)
        {
            // LAPOR: "Bos, saya (kertas ini) sudah ditutup!"
            manager.CloseThisNote(gameObject);
            Debug.Log("[StickyNote] Berhasil melapor ke Manager.");
        }
        else
        {
            // Jika manager tidak ketemu, kita hancurkan sendiri saja (tapi ini error)
            Debug.LogError("[StickyNote] Manager tidak ditemukan di Scene!");
            Destroy(gameObject);
        }
    }
}