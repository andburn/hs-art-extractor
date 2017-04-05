# HsArtExtractor
A tool for windows to extract hearthstone card art from the unity game files. The project includes a partial port of [Disunity](https://github.com/ata4/disunity), it also utilizes the [DDSReader](https://github.com/andburn/dds-reader) project.

## Obsolete
As of version *0.3.3* (Hearthstone patch 8.0.0.18336) the card art can still be extracted as before, but support for card tiles and card xml has been removed.

This project has become increasingly difficult to maintain with each new Hearthstone patch. Future updates to this project are unlikely. The [HearthSim](https://hearthsim.info/) tool chain is the recommended replacement:
- [HearthstoneJSON](https://github.com/HearthSim/HearthstoneJSON)
- [HsData](https://github.com/HearthSim/hsdata)
- [UnityPack](https://github.com/HearthSim/UnityPack)

---

## Features
- Extract all card art.
- ~~Extract available card bars (deck list images).~~
- ~~Extracts the card xml text files.~~
- Extracted cards can be filtered by set and card type, they can also be named by card name rather than by id.

## Usage
When Hearthstone is running the necessary game files cannot be accessed. Therefore, you need to quit out of Hearthstone for the program to work correctly.

#### Command line interface
```
extractor.exe cardart [options] <hearthstone directory>

```
**Examples**

Extract the card art including card bars to *C:\CardArt* directory, this excludes enchantments and miscellaneous sets.
```
extractor.exe cardart -o "C:\CardArt" "C:\Hearthstone"
```

Extract the full card art images only (no bars) and disable all default filtering on sets and card type.
```
extractor.exe cardart --full-only --no-filters -o "C:\CardArt" "C:\Hearthstone"
```

Extract the card bars with "hidden" areas cropped off, and set the output height to be 34 pixels. Also, use the supplied [CardArtDefs.xml](CardArtDefs.xml) to create bars for tokens.
```
extractor.exe cardart -o "C:\CardArt" --crop-hidden --bar-height 34 --bar-only -m CardArtDefs.xml C:\Hearthstone
```

Other available options:
```
--full-only                  Extract only the full size card art.

--height                     Specify the height of full size card art (height
                             == width).

--image-type                 Specify the image type; png or jpg (or dds not
                             compatible with height or flip).

--without-bar-coords-only    Only output images without any card bar
                             coordinates.

--bar-only                   Extract only the card bar images.

--bar-height                 The desired height of the card bar images.

-a, --keep-alpha             Keep the alpha channel in the exported images
                             (removed by default).

-f, --no-flip                Do not flip the full images over the y-axis
                             (does not effect card bar images).

-g, --group-by-set           Group the images into directories by card set.

-n, --name                   Add the card name to the exported file names.

-s, --include-sets           Include cards from the specified sets only.

-t, --include-types          Include cards of the specified types only.

--no-filters                 Exports all cards, ignores default filtering.

--save-map-file              Save the generated card to texture mapping file
                             (ignored if using a map file as input).

-m, --map-file               Use a previously generated card to texture
                             mapping file.

-o, --output-dir             The directory where the extracted files will be
                             saved to.

--crop-hidden                Remove parts of the card bar that are hidden by
                             the bar frame in the game.

--help                       Display this help screen.

--version                    Display version information.

```

#### Programming interface
- Include a reference to `HsArtExtractor.dll` in your .NET project.

```csharp
using HsArtExtractor;
using HsArtExtractor.Hearthstone;
using HsArtExtractor.Util;

class Program
{
  private static void Main(string[] args)
  {
    // set the logging level and location
    Logger.SetLogLevel(LogLevel.WARN);
    Logger.SetLogLocation(@"C:\CardArt");

    // do the extraction
    Extract.CardArt(new CardArtExtractorOptions() {
      HearthstoneDir = @"C:\Program Files\Hearthstone",
      OutputDir = @"C:\CardArt",
      MapFile = "CardArtDefs.xml",
      Height = 256,
      BarHeight = 35
    });
  }
}
```

## Build
- Clone the repository.
- Install the dependencies. On the command line (in the repository directory) run:
```
.paket\paket.bootstrapper.exe
.paket\paket.exe install
```
- Open the solution in Visual Studio and build as normal.
