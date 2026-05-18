using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // 1. WAJIB TAMBAHKAN INI UNTUK UI CLICK

// 2. Tambahkan ", IPointerClickHandler" di belakang nama class
public class ThoughtBubble : MonoBehaviour, IPointerClickHandler 
{
    private Image bubbleImage;
    private TextMeshProUGUI bubbleText;
    private bool isGoodBubble;
    private float moveSpeed;
    private float targetEndX;

    void Awake()
    {
        bubbleImage = GetComponent<Image>();
        bubbleText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(Sprite sprite, string text, bool isGood, float speed, float endX)
    {
        if(bubbleImage != null) bubbleImage.sprite = sprite;
        if(bubbleText != null) bubbleText.text = text;
        
        isGoodBubble = isGood;
        moveSpeed = speed;
        targetEndX = endX;
    }

    void Update()
    {
        // Cek dulu apakah game lagi freeze via manager
        Puzzle3_Manager manager = FindObjectOfType<Puzzle3_Manager>();
        if (manager != null && manager.IsSystemFrozen) return; // Berhenti bergerak jika freeze

        // Pergerakan UI ke arah kiri
        transform.localPosition += Vector3.left * moveSpeed * Time.deltaTime;

        // Jika sudah melewati End Point X, hancurkan diri
        if (transform.localPosition.x < targetEndX)
        {
            Destroy(gameObject);
        }
    }

    // 3. FUNGSI KLIK UNTUK UI CANVAS
    public void OnPointerClick(PointerEventData eventData)
    {
        Puzzle3_Manager manager = FindObjectOfType<Puzzle3_Manager>();
        if (manager != null)
        {
            // Laporkan ke manager apakah gelembung ini benar (Good) atau salah (Bad)
            manager.OnBubbleClicked(isGoodBubble); 
        }
        
        Destroy(gameObject); // Hancurkan balon setelah diklik
    }
}