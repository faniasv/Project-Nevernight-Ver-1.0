using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Settings")]
    public int priorityLevel; // 1, 2, 3
    public bool isScoringZone = true; // CENTANG ini untuk Slot Papan, JANGAN CENTANG untuk Meja Bawah

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop Terpanggil di: " + gameObject.name);

        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        DraggableItem draggableItem = droppedObj.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            // 1. Set rumah baru item ini ke object ini
            draggableItem.parentAfterDrag = transform;

            // 2. Cek apakah ini zona yang menghitung skor/load?
            if (isScoringZone)
            {
                Puzzle1_Manager manager = FindObjectOfType<Puzzle1_Manager>();
                if (manager != null)
                {
                    manager.OnTaskDroppedOnSlot(priorityLevel, draggableItem.TaskID);
                }
            }
        }
    }
}