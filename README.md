# HsArtExtractor
A tool for windows to extract hearthstone card art from the unity game files. The project includes a partial port of [Disunity](https://github.com/ata4/disunity), it also utilizes the [DDSReader](https://github.com/andburn/dds-reader) project.

## Features
- Extract all card art named by card id.
- Extract available card bars (deck list images) by card id.
- Extracts the card xml text files.
- Extracted cards can be filtered by set and card type.

## Build
- Clone the repository.
- Install the dependencies. On the command line (in the repository directory) run:
```
.paket\paket.bootstrapper.exe
.paket\paket.exe install
```
- Open the solution in Visual Studio and build as normal.

## Usage
You need to ensure Hearthstone is not running for the program to work correctly.

#### Programming interface
```csharp
using HsArtExtractor;
using HsArtExtractor.Hearthstone;

class Program
{
  private static void Main(string[] args)
  {
    Extract.CardArt(new CardArtExtractorOptions() {
      HearthstoneDir = @"C:\Program Files\Hearthstone",
      OutputDir = @"C:\CardArt",
      MapFile = "CardArtDefs.xml",
      PreserveAlphaChannel = false,
      FlipY = true,
      BarHeight = 35
    });
  }
}
```

#### Command line interface
```
extractor.exe cardart [options] <hearthstone directory>

```
Example:
```
extractor.exe cardart -o "C:\CardArt" -m "CardArtDefs.xml" 
```

Options:
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

--help                       Display this help screen.

--version                    Display version information.

```
