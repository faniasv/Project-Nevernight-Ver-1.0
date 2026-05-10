using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System; // WAJIB ADA INI buat pake Action

public class PCVisualReveal : MonoBehaviour, IDragHandler
{
    [Header("Referensi Objek")]
    public CanvasGroup vividLayer;
    public RectTransform handle;
    public GameObject closeButton;

    [Header("Batas Gerak")]
    public float minX = -300f;
    public float maxX = 300f;

    // --- INI JEMBATANNYA ---
    public Action OnRevealComplete; 
    private bool isFinished = false;

    void Start() {
        if (vividLayer != null) vividLayer.alpha = 0;
        if (closeButton != null) closeButton.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData) {
        if (isFinished) return;

        float currentX = handle.localPosition.x + eventData.delta.x;
        currentX = Mathf.Clamp(currentX, minX, maxX);
        handle.localPosition = new Vector3(currentX, handle.localPosition.y, 0);

        float progres = (currentX - minX) / (maxX - minX);
        if (vividLayer != null) vividLayer.alpha = progres;

        // Cek kalau udah mentok kanan (100%)
        if (progres >= 0.98f && !isFinished) {
            isFinished = true;
            // Panggil jembatan: "Manager, gue udah beres!"
            OnRevealComplete?.Invoke(); 
        }
    }

    // Fungsi ini dipanggil sama Manager setelah dialog selesai
    public void ShowCloseButton() {
        if (closeButton != null) closeButton.SetActive(true);
    }
}