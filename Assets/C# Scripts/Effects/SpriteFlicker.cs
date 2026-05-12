using UnityEngine;

public class SpriteFlicker : MonoBehaviour
{
    private SpriteRenderer _sprite;
    
    [Header("Flicker Settings")]
    [Range(0, 1)] public float minAlpha = 0.2f;
    [Range(0, 1)] public float maxAlpha = 0.8f;
    public float flickerSpeed = 0.1f;

    private float _lastTime;

    void Awake() => _sprite = GetComponent<SpriteRenderer>();

    void Update()
    {
        if (Time.time - _lastTime > flickerSpeed)
        {
            // Ambil warna asli, lalu ganti Alpha-nya saja
            Color tempColor = _sprite.color;
            tempColor.a = Random.Range(minAlpha, maxAlpha);
            _sprite.color = tempColor;

            _lastTime = Time.time;
        }
    }
}