using System.Collections.Generic;
using System.IO;
using HearthstoneDisunity.Hearthstone.CardArt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HearthstoneDisunityTest
{
    /// <summary>
    /// Summary description for CardArtDbTest
    /// </summary>
    [TestClass]
    public class CardArtDbTest
    {
        private static Dictionary<string, ArtCard> _db;
        private static string _tempXml;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            CardArtDb.Read(@"data\cardartdefs.xml");
            _db = CardArtDb.All;
            _tempXml = @"data\writetest.xml";
        }

        [TestMethod]
        public void GameVersion()
        {
            Assert.AreEqual("3.2.0.10604", CardArtDb.GameVersion);
        }

        [TestMethod]
        public void ContainsTwoCards()
        {
            Assert.AreEqual(2, _db.Count);
        }

        [TestMethod]
        public void GetNullIfMaterialTypeDoesNotExist()
        {
            var card = _db["AT_001"];
            Assert.IsNull(card.GetMaterial(MaterialType.Portrait));
        }

        [TestMethod]
        public void TextureName()
        {
            var card = _db["AT_001"];
            Assert.AreEqual("HS5-033_D", card.Texture.Name);
        }

        [TestMethod]
        public void TexturePath()
        {
            var card = _db["EX1_298"];
            Assert.AreEqual("Assets/Game/Cards/01 Expert/EX1_298/w2_059_D.psd", card.Texture.Path);
        }

        [TestMethod]
        public void TextureBundle()
        {
            var card = _db["AT_001"];
            Assert.AreEqual("cardtextures0", card.Texture.Bundle);
        }

        [TestMethod]
        public void ShaderTransformExists()
        {
            var mat = _db["EX1_298"].GetMaterial(MaterialType.CardBar);
            Assert.AreEqual(0.73f, mat.GetTransform(TransformType.Shader).Scale.X);
        }

        [TestMethod]
        public void StandardTransformExists()
        {
            var mat = _db["AT_001"].GetMaterial(MaterialType.CardBar);
            Assert.AreEqual(0.25f, mat.GetTransform(TransformType.Standard).Offset.Y);
        }

        [TestMethod]
        public void WriteCorrectNumOfLines()
        {
            CardArtDb.Write(_tempXml, CreateSampleDefs());
            var lineCount = File.ReadAllLines(_tempXml).Length;
            Assert.AreEqual(31, lineCount);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            File.Delete(_tempXml);
        }

        private CardArtDefs CreateSampleDefs()
        {
            var root = new CardArtDefs();
            root.Version = "1.0.0.0";
            root.Cards = new List<ArtCard>();

            var card1 = new ArtCard();
            card1.Id = "AC_123";
            card1.Texture = new Texture() {
                Name = "WOW_1223",
                Path = "/tex/fp1_01/de2",
                Bundle = "shared0"
            };
            card1.Materials = new List<Material>();
            var mat1 = new Material();
            mat1.Type = MaterialType.Portrait;
            mat1.Transforms = new List<Transform>()
            {
                new Transform()
                {
                    Type = TransformType.Standard,
                    Offset = new CoordinateTransform(0.0f),
                    Scale = new CoordinateTransform(1.0f)
                },
                new Transform()
                {
                    Type = TransformType.Shader,
                    Offset = new CoordinateTransform(0.0f),
                    Scale = new CoordinateTransform(1.0f)
                }
            };
            card1.Materials.Add(mat1);
            var mat2 = new Material();
            mat2.Type = MaterialType.CardBar;
            mat2.Transforms = new List<Transform>()
            {
                new Transform()
                {
                    Type = TransformType.Standard,
                    Offset = new CoordinateTransform(0.0f),
                    Scale = new CoordinateTransform(1.0f)
                },
                new Transform()
                {
                    Type = TransformType.Shader,
                    Offset = new CoordinateTransform(-0.3f, 1.2f),
                    Scale = new CoordinateTransform(1.2f)
                }
            };
            card1.Materials.Add(mat2);
            root.Cards.Add(card1);

            return root;
        }
    }
}