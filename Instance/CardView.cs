using System;
using DG.Tweening;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public CardAnim baseAnimation;
    public SpriteRenderer Artwork;
    public Sprite CardFront { get; private set; }
    public Sprite CardBack { get; private set; }
    public Action OnClicked;
    private void Awake()
    {
        Artwork = GetComponent<SpriteRenderer>();
        baseAnimation = GetComponent<CardAnim>();
    }
    public void SetView(Sprite front, Sprite back)
    {
        CardFront = front;
        CardBack = back;
        Artwork.sprite = CardBack;
    }
    public async void Flip(bool IsFlip, float duration = 0.5f)
    {
        float halfDuration = duration / 2f;

        // Step 1: Rotate to 90 degrees Y (hide)
        await transform.DORotate(new Vector3(0, 90, 0), halfDuration)
                       .SetEase(Ease.InSine)
                       .AsyncWaitForCompletion();

        // Step 2: Change sprite

        Artwork.sprite = IsFlip ? CardFront : CardBack;

        // Step 3: Rotate back to 0 or 180 degrees Y
        float targetY = IsFlip ? 0f : 180f;
        await transform.DORotate(new Vector3(0, targetY, 0), halfDuration)
                       .SetEase(Ease.InOutSine)
                       .AsyncWaitForCompletion();
    }
    public void LocolmotionY(float y = 0)
    {
        transform.DOLocalMoveY(y, 0.2f);
    }
    public void OnMouseDown()
    {
        OnClicked?.Invoke();
    }
}