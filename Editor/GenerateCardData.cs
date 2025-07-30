using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace AinCard
{
    public class GenerateCardData : EditorWindow
    {
        [MenuItem("Tools/Generate Deck")]
        public static void ShowWindow()
        {
            GetWindow<GenerateCardData>("Card Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Card Generator", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Deck"))
            {
                GenerateDeck();
            }
            if (GUILayout.Button("Regenerate GUID"))
            {
                RegenerateGUID();
            }
            if (GUILayout.Button("Regenerate Art"))
            {
                RegenerateArt();
            }
        }
#if UNITY_EDITOR
        public static string SpriteFolderPath = "Assets/Resources/Art/PNG";
        public static string CardDataFolderPath = "Assets/Resources/GameData/Deck";

        /// <summary>
        /// Generate and save a full standard deck (52 cards).
        /// </summary>
        #region REGENERATE
        public static void Regenerate(Action<CardData> func)
        {
            string[] guids = AssetDatabase.FindAssets("t:CardData", new[] { CardDataFolderPath });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CardData card = AssetDatabase.LoadAssetAtPath<CardData>(path);
                func(card);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static void RegenerateArt()
        {
            Action<CardData> func = (card) =>
            {
                if (card != null)
                {
                    int rank = (int)card.Rank;
                    rank = rank == 14 ? 1 : rank;

                    string spritePath = Path.Combine(SpriteFolderPath, $"card-{card.Suit}-{rank}.png");
                    card.Art = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                }
            };
            Regenerate(func);
        }
        public static void RegenerateGUID()
        {
            Action<CardData> func = (card) =>
            {
                if (card != null)
                {
                    card.ID = SerializableGuid.NewGuid();
                }
            };
            Regenerate(func);
        }
        #endregion
        public static void GenerateDeck()
        {
            if (!Directory.Exists(CardDataFolderPath))
            {
                Directory.CreateDirectory(CardDataFolderPath);
            }


            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    // Create new CardData ScriptableObject
                    CardData newCard = CreateInstance<CardData>();

                    // Setup internal name
                    string rankName = rank.ToString();
                    string suitName = suit.ToString();
                    string assetName = $"{rankName}_{suitName}";

                    // Set data fields
                    newCard.ID = SerializableGuid.NewGuid();
                    newCard.name = assetName;
                    newCard.Rank = rank;
                    newCard.Suit = suit;
                    // set art
                    int _rank = (int)rank == 14 ? 1 : (int)rank;
                    string spritePath = Path.Combine(SpriteFolderPath, $"card-{suit}-{_rank}.png");
                    Sprite spriteAsset = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                    newCard.Art = spriteAsset;
                    // Save as .asset file (ensure no name conflict)
                    string assetPath = Path.Combine(CardDataFolderPath, assetName + ".asset");
                    assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                    AssetDatabase.CreateAsset(newCard, assetPath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}