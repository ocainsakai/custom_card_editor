using UniRx;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class CardManager : MonoBehaviour
{
    [SerializeField] FactoryManager factoryManager;
    [SerializeField] GameContext gameContext;
    [SerializeField] GridObjectLayout2D handGrid;
    [SerializeField] GridObjectLayout2D deckGrid;
    [SerializeField] GridObjectLayout2D discardPile;
    private List<Card> allCards = new List<Card>();
    public List<Card> GetCards(IEnumerable<SerializableGuid> cardIDs)
    {
        return allCards.Where(x => cardIDs.Contains(x.ID)).ToList();
    }
    public Sprite back;
    private void Awake()
    {
        CardSelectHandle();
    }
    #region Card_Select
    private void CardSelectHandle()
    {
        Card.OnCardSelected.Subscribe(_ =>
        {
            OnCardSelctHandle(_);
        });
        gameContext.selectedCards.ObserveCountChanged().Subscribe(_ =>
        {
            Card.CanSelect = _ < 5;
        });
    }
    private void OnCardSelctHandle(Card _)
    {
        bool isSelect = gameContext.selectedCards.Contains(_.ID);
        if (Card.CanSelect && !isSelect)
        {
            gameContext.selectedCards.Add(_.ID);
            _.MoveUp();
        }
        else if (isSelect)
        {
            gameContext.selectedCards.Remove(_.ID);
            _.MoveDown();
        }
    }
    #endregion
    public void Initialize(IEnumerable<CardData> cards)
    {
        foreach (CardData cardData in cards)
        {
            var card = factoryManager.CreateCard(cardData, back, deckGrid.transform);
            allCards.Add(card);
        }
        gameContext.deck.AddRange(allCards.Select(x => x.ID));
        gameContext.deck.Shuffle();
    }
    public async void DrawCard()
    {
        if (!gameContext.CanDarw)
        {
            gameContext.Discard2Deck();
            await deckGrid.ApplyWithoutLayout(GetCards(gameContext.deck));
        }

        gameContext.Deck2Hand();
        var hand = GetCards(gameContext.hand);
        hand = hand.OrderBy(x => x.Data.Rank).ThenBy(x => x.Data.Suit).ToList();
        hand.ForEach(x => x.SetFace(true));
        await handGrid.ApplyLayout(hand);
    }
    public async void DiscardHand()
    {
        if (gameContext.selectedCards.Count == 0) return;
        await discardPile.ApplyWithoutLayout(GetCards(gameContext.selectedCards));
        gameContext.discardPile.AddRange(gameContext.selectedCards);
        DrawCard();
    }
}
