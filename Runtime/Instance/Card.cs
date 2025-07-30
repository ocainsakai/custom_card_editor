using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
namespace AinCard
{



    [RequireComponent(typeof(CardView))]
    public class Card : MonoBehaviour
    {
        // static state
        public static bool CanSelect = true;
        public static Subject<Card> OnCardSelected = new Subject<Card>();
        private float originY;
        // property
        public SerializableGuid ID;
        public SerializableGuid DataID;
        public CardData Data { get; private set; }
        private CardView cardView;

        public bool IsFront { get; private set; }

        private void Awake()
        {
            cardView = GetComponent<CardView>();
            cardView.OnClicked += () => OnCardSelected.OnNext(this);
        }

        public void Initialize(CardData data, Sprite cardBack)
        {
            this.Data = data;
            DataID = data.ID;
            ID = SerializableGuid.NewGuid();
            cardView.SetView(Data.Art, cardBack);
        }
        public void SetFace(bool isFront)
        {
            IsFront = isFront;
            Flip(false);
        }
        public void MoveUp()
        {
            originY = transform.localPosition.y;
            cardView.LocolmotionY(originY + 0.5f);
        }
        public void MoveDown() => cardView.LocolmotionY(originY);
        public void Flip(bool toggle = true)
        {
            if (toggle)
            {
                IsFront = !IsFront;
            }
            //cardView.Flip(IsFront);
        }
    }
    public enum CardState
    {
        None,
        Hold,
        Select,
        Play,
        Score
    }
}