using System;
using DG.Tweening;
using UnityEngine;

namespace AinCard
{
    public class CardView : MonoBehaviour
    {
        public SpriteRenderer Artwork;
        public Sprite CardFront { get; private set; }
        public Sprite CardBack { get; private set; }
        public Action OnClicked;
        private void Awake()
        {
            Artwork = GetComponent<SpriteRenderer>();
        }
        public void SetView(Sprite front, Sprite back)
        {
            CardFront = front;
            CardBack = back;
            Artwork.sprite = CardBack;
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
}