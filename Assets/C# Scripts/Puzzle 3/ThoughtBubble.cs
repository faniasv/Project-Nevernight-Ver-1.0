using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ThoughtBubble : MonoBehaviour, IPointerClickHandler 
{
    private Image bubbleImage;
    private TextMeshProUGUI bubbleText;
    private bool isGoodBubble;
    private float moveSpeed;
    private float targetEndX;
    
    private Puzzle3_Manager manager;

    void Awake()
    {
        bubbleImage = GetComponent<Image>();
        bubbleText = GetComponentInChildren<TextMeshProUGUI>();
        manager = FindObjectOfType<Puzzle3_Manager>();
    }

    public void Setup(Sprite sprite, string text, bool isGood, float speed, float endX)
    {
        if(bubbleImage != null && sprite != null) bubbleImage.sprite = sprite;
        if(bubbleText != null) bubbleText.text = text;
        
        isGoodBubble = isGood;
        moveSpeed = speed;
        targetEndX = endX;

        // CCTV: Bukti kalau balon ini beneran berhasil diciptakan!
        Debug.Log("🟢 BALON LAHIR! Teks: [" + text + "] | Posisi X: " + transform.localPosition.x);
    }

    void Update()
    {
        if (manager != null && manager.IsSystemFrozen) return; 

        // 1. UBAH ARAH: Vector3.left diganti jadi Vector3.right biar jalannya ke KANAN
        transform.localPosition += Vector3.right * moveSpeed * Time.deltaTime;

        // 2. UBAH CEK BATAS: Tanda '<' diganti jadi '>' karena sekarang garis finisnya ada di nilai X positif
        if (transform.localPosition.x > targetEndX)
        {
            Debug.Log("🔴 Balon Hancur di X: " + transform.localPosition.x);
            Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager != null)
        {
            manager.OnBubbleClicked(isGoodBubble); 
        }
        Destroy(gameObject); 
    }
}