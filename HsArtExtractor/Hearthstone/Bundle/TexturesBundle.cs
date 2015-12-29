using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using AndBurn.DDSReader;
using HsArtExtractor.Hearthstone.CardArt;
using HsArtExtractor.Image;
using HsArtExtractor.Unity;
using HsArtExtractor.Unity.Objects;
using HsArtExtractor.Util;

namespace HsArtExtractor.Hearthstone.Bundle
{
    public class TexturesBundle
    {
        private string _dir;
        private List<string> _files;
        private string _dirFull;
        private string _dirBars;

        public TexturesBundle(List<string> files, string outDir)
        {
            _files = files;
            _dir = outDir;
            _dirFull = Path.Combine(outDir, "Full");
            Directory.CreateDirectory(_dirFull);
            _dirBars = Path.Combine(outDir, "Bars");
            Directory.CreateDirectory(_dirBars);
        }

        public void Extract(Dictionary<long, string> refs, List<ArtCard> cards)
        {
            var countFound = 0;
            foreach (var tf in _files)
            {
                AssestFile af = new AssestFile(tf);
                foreach (var obj in af.Objects)
                {
                    // check to see if obj id is found in texrefs
                    var unityClass = (UnityClass)obj.Info.ClassId;
                    if (unityClass == UnityClass.Texture2D && refs.ContainsKey(obj.Id))
                    {
                        countFound++;
                        var refPath = refs[obj.Id];
                        var refName = StringUtils.GetFilenameNoExt(refPath).ToUpper();
                        var refCardId = StringUtils.GetFilePathParentDir(refPath).ToUpper();

                        refPath = refPath.Replace("final/", "").ToLower();
                        var cardMatches = cards.Where(x => x.Texture.Path.ToLower() == refPath).ToList();
                        Texture2D tex = new Texture2D(BinaryBlock.Create(obj.Buffer));

                        if (tex.Name.ToUpper() == refName)
                        {
                            foreach (var match in cardMatches)
                            {
                                SaveImages(tex, match);
                            }
                        }
                    }
                }
            }
            Logger.Log(LogLevel.DEBUG, "TextureBundle Refs matched: " + countFound);
        }

        private void SaveImages(Texture2D tex, ArtCard match)
        {
            Bitmap original = null;
            if (tex.IsDDS)
            {
                original = DDS.LoadImage(tex.Image, false);
            }
            else
            {
                // Assumimg it is TGA
                var tga = new TargaImage(new MemoryStream(tex.Image));
                original = tga.Image;
                // Flip original on y, so like dds
                original.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            // save full size image to disk
            var full = new Bitmap(original);
            // flip right way up
            full.RotateFlip(RotateFlipType.RotateNoneFlipY);
            full.Save(Path.Combine(_dirFull, match.Id + ".png"));
            // save card bar image to disk
            Export.CardBar(match, original, _dirBars);
        }
    }
}