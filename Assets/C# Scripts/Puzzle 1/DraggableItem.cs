using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Visual References")]
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [HideInInspector] public Transform parentAfterDrag; // Rumah item (bisa berubah)
    private Canvas mainCanvas;

    [Header("Task Data")]
    public string TaskID;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Mencari Canvas paling atas agar sorting layernya benar saat ditarik
        mainCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Simpan parent asal
        parentAfterDrag = transform.parent;

        // DEBUG BARU: Cek siapa parent saat ini?
        Debug.Log("Item ditarik dari parent: " + parentAfterDrag.name);

        // 2. Cari komponen DropZone di parent (atau kakek/buyutnya)
        // Kita ganti 'GetComponent' jadi 'GetComponentInParent' biar lebih aman
        DropZone oldSlot = GetComponentInParent<DropZone>();

        if (oldSlot != null)
        {
            Debug.Log("Parent dikenali sebagai DropZone! Priority: " + oldSlot.priorityLevel);

            // Panggil Manager
            Puzzle1_Manager manager = FindObjectOfType<Puzzle1_Manager>();
            if (manager != null)
            {
                manager.OnTaskRemovedFromSlot(oldSlot.priorityLevel);
            }
            else
            {
                Debug.LogError("Puzzle1_Manager tidak ditemukan di Scene!");
            }
        }
        else
        {
            Debug.Log("Parent BUKAN DropZone (Mungkin ini meja awal/Inventory). Tidak ada yang perlu dikurangi.");
        }

        // 3. Pindah visual ke root canvas
        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling();

        // 4. Transparan & Tembus Raycast
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Gerakkan item mengikuti mouse (dikali scaleFactor biar akurat di beda resolusi)
        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // INI BAGIAN PENTINGNYA:
        // Pindahkan item ke parent baru (Slot) yang sudah ditentukan DropZone tadi
        transform.SetParent(parentAfterDrag);

        // Reset posisi biar pas di tengah slot
        // rectTransform.anchoredPosition = Vector2.zero; // (Opsional, aktifkan kalau mau rapi otomatis)
    }
}