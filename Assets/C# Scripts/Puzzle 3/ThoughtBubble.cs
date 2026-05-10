using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThoughtBubble : MonoBehaviour
{
    public Image bubbleImage;
    public TextMeshProUGUI textComp;
    
    private float speed;
    private float targetX;
    private bool isReady = false;

    // Fungsi Setup dengan 5 Parameter (Harus SAMA dengan Manager)
    public void Setup(Sprite s, string txt, bool good, float moveSpeed, float endX)
    {
        if (bubbleImage != null) bubbleImage.sprite = s;
        if (textComp != null) textComp.text = txt;
        
        speed = moveSpeed;
        targetX = endX;
        isReady = true; 
    }

    void Update()
    {
        if (!isReady) return;

        // Gerak ke kiri
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Hancurkan kalau sudah lewat targetX
        if (transform.localPosition.x < targetX)
        {
            Destroy(gameObject);
        }
    }
}