using UnityEngine;
using UnityEngine.EventSystems;

// Implementasi interface untuk mendeteksi semua input mouse yang kita butuhkan
public class FragmentPuzzle : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("ID unik untuk kepingan ini, contoh: Vas1_FragA")]
    public string fragmentID; // ID untuk dicocokkan dengan target

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private bool isLocked = false; // Status untuk mengunci kepingan

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    // 1. BEHAVIOR: ROTATE (Klik Kanan)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLocked) return; // Jangan lakukan apa-apa jika sudah terkunci

        // Sesuai brief: Rotate 45 degress
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            rectTransform.Rotate(0f, 0f, -45f); // Putar 45 derajat
            Debug.Log($"Fragment {fragmentID} diputar.");
        }
    }

    // 2. BEHAVIOR: DRAG (Klik Kiri)
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked || eventData.button != PointerEventData.InputButton.Left) return;

        Debug.Log($"Mulai drag {fragmentID}.");
        canvasGroup.alpha = 0.8f; // Buat sedikit transparan

        // transform.SetParent(canvas.transform, true); // Pindahkan ke Canvas utama agar bisa digeser bebas
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked || eventData.button != PointerEventData.InputButton.Left) return;

        canvasGroup.blocksRaycasts = false;
        // Gerakkan kepingan mengikuti mouse
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked || eventData.button != PointerEventData.InputButton.Left) return;

        Debug.Log($"Selesai drag {fragmentID}.");
        canvasGroup.alpha = 1f; // Kembalikan transparansi
        canvasGroup.blocksRaycasts = true; // Bisa diklik lagi

        // Sesuai brief: "Jika tidak snap... gapapa, ga harus balik ke posisi awal."
        // Jadi, kita tidak perlu mengembalikan posisinya. Dia akan diam di tempat terakhir di-drop.
    }

    // Fungsi untuk mengunci kepingan (dipanggil oleh FragmentTarget)
    public void LockInPlace()
    {
        Debug.Log($"Mengunci {fragmentID}.");
        isLocked = true;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}