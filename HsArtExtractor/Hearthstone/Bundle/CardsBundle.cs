﻿using System.Collections.Generic;
using HsArtExtractor.Hearthstone.CardArt;
using HsArtExtractor.Unity;
using HsArtExtractor.Unity.Objects;
using HsArtExtractor.Util;
using GameMaterial = HsArtExtractor.Unity.Objects.Material;

namespace HsArtExtractor.Hearthstone.Bundle
{
	public class CardsBundle
	{
		public List<ArtCard> Cards { get; set; }

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

		public CardsBundle(List<string> bundles) : this()
		{
			// get all card objects over all files
			foreach (var file in bundles)
			{
				var af = new AssestFile(file);
				BuildReferences(af);
			}
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
								else
								{
									Logger.Log(LogLevel.WARN, "Portriat texture not found for: {0}", go.Name);
								}
							}
							else
							{
								Logger.Log(LogLevel.WARN, "CardDef not found for {0}", pathId);
							}
						}
					}
				}
			}
		}

		private void BuildReferences(AssestFile bundle)
		{
			foreach (var obj in bundle.Objects)
			{
				var unityClass = (UnityClass)obj.Info.ClassId;
				var data = BinaryBlock.Create(obj.Buffer);
				switch (unityClass)
				{
					case UnityClass.GameObject:
						_gameObjects[obj.Id] = new GameObject(data);
						break;

					case UnityClass.MonoBehaviour:
						// In this bundle, its a HS CardDef, SoundDef, HiddenCard
						var cd = new CardDef(data);
						if (!cd.FailedToLoad) // only want carddef (Hack)
							_cardDefObjects[obj.Id] = cd;
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