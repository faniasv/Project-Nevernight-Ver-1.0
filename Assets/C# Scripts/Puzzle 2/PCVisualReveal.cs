using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PCVisualReveal : MonoBehaviour, IDragHandler
{
    [Header("Referensi Objek")]
    public CanvasGroup vividLayer;
    public GameObject closeButton;

    [Header("Settings Gosok")]
    public float targetDistance = 5000f; // Total jarak geser mouse biar foto jadi 100% jelas
    private float currentDistance = 0f;
    private bool isFinished = false;

    public Action OnRevealComplete; 

    public void ResetReveal() {
        isFinished = false;
        currentDistance = 0;
        if (vividLayer != null) vividLayer.alpha = 0;
        if (closeButton != null) closeButton.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData) {
        if (isFinished) return;

        // Akumulasi jarak mouse (delta.magnitude)
        currentDistance += eventData.delta.magnitude;

        float progres = currentDistance / targetDistance; 
        if (vividLayer != null) vividLayer.alpha = Mathf.Clamp01(progres);

        if (progres >= 1f && !isFinished) {
            isFinished = true;
            // TERIAK KE MANAGER: "Gue udah beres digosok!"
            OnRevealComplete?.Invoke(); 
        }
    }

    public void ShowCloseButton() {
        if (closeButton != null) closeButton.SetActive(true);
    }
}