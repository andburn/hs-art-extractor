using System.Collections.Generic;
using HearthstoneDisunity.Hearthstone.CardArt;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;
using GameMaterial = HearthstoneDisunity.Unity.Objects.Material;

namespace HearthstoneDisunity.Hearthstone.Bundle
{
    public class CardsBundle
    {
        public List<ArtCard> Cards { get; set; }

        private AssestFile _bundle;
        private List<ObjectData> _bundleObjects;
        private Dictionary<long, GameObject> _gameObjects;
        private Dictionary<long, GameMaterial> _materialObjects;
        private Dictionary<long, CardDef> _cardDefObjects;

        public CardsBundle()
        {
            Cards = new List<ArtCard>();
            _gameObjects = new Dictionary<long, GameObject>();
            _materialObjects = new Dictionary<long, GameMaterial>();
            _cardDefObjects = new Dictionary<long, CardDef>();
        }

        public CardsBundle(AssestFile bundle) : this()
        {
            _bundle = bundle; // TODO: not used?
            _bundleObjects = bundle.Objects;
            BuildReferences();
            ProcessObjects();
        }

        private void ProcessObjects()
        {
            foreach (var entry in _gameObjects)
            {
                var go = entry.Value;

                if (string.IsNullOrWhiteSpace(go.Name))
                {
                    Logger.Log(LogLevel.WARN, "GameObject id={0} has no Name", entry.Key);
                }
                else
                {
                    // go through all the GameObject file references
                    foreach (var fp in go.Components)
                    {
                        // only interested in CardDef (MonoBehaviour)
                        if ((UnityClass)fp.ClassID == UnityClass.MonoBehaviour)
                        {
                            var pathId = fp.PathID;
                            // find the CarDef object from the file reference
                            if (_cardDefObjects.ContainsKey(pathId))
                            {
                                CardDef def = _cardDefObjects[pathId];
                                // if no texture path defined, skip it
                                if (!string.IsNullOrWhiteSpace(def.PortratitTexturePath))
                                {
                                    GameMaterial portMat = FindMaterial(def.EnchantmentPortrait);
                                    GameMaterial barMat = FindMaterial(def.DeckCardBarPortrait);
                                    ArtCard card = new ArtCard(go.Name, def, portMat, barMat);
                                    Cards.Add(card);
                                }
                            }
                            else
                            {
                                // TODO: CardDef not found
                            }
                        }
                    }
                }
            }
        }

        private void BuildReferences()
        {
            foreach (var obj in _bundleObjects)
            {
                var unityClass = (UnityClass)obj.Info.ClassId;
                var data = BinaryBlock.Create(obj.Buffer);
                switch (unityClass)
                {
                    case UnityClass.GameObject:
                        _gameObjects[obj.Id] = new GameObject(data);
                        break;

                    case UnityClass.MonoBehaviour:
                        // In this bundle, its a HS CardDef
                        _cardDefObjects[obj.Id] = new CardDef(data);
                        break;

                    case UnityClass.Material:
                        _materialObjects[obj.Id] = new GameMaterial(data);
                        break;

                    default:
                        break;
                }
            }
        }

        private GameMaterial FindMaterial(FilePointer fp)
        {
            if (_materialObjects.ContainsKey(fp.PathID))
            {
                return _materialObjects[fp.PathID];
            }
            else
            {
                return null;
            }
        }
    }
}