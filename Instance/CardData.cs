using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public SerializableGuid ID;
    public CardRank Rank;
    public CardSuit Suit;
    public Sprite Art;
    public string CardName => ToString();
    public int RankValue => (int)Rank;
    public int SuitValue => (int)Suit;
    public int chipAmount => RankValue == 14 ? 11 : RankValue > 10 ? 10 : RankValue;

    public EffectType EffectType => EffectType.AddChip;
    public override string ToString() => $"{Rank} of {Suit}";
    
}
public enum CardRank
{
    Ace = 14,
    King = 13,
    Queen = 12,
    Jack =11,
    Ten = 10,
    Nine = 9,
    Eight=8,
    Seven=7,
    Six=6,
    Five=5,
    Four=4,
    Three=3,
    Two=2,
}
public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades,
}
