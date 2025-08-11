using UnityEngine;
using UnityEngine.UI;

public class ScrollingRawImage : MonoBehaviour
{
    public RawImage rawImage;
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0f;

    private Rect uvRect;

    void Start()
    {
        uvRect = rawImage.uvRect;
    }

    void Update()
    {
        uvRect.x += scrollSpeedX * Time.deltaTime;
        uvRect.y += scrollSpeedY * Time.deltaTime;

        rawImage.uvRect = uvRect;
    }
}