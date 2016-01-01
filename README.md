# HsArtExtractor
A tool for windows to extract hearthstone card art from the unity game files. The project includes a partial port of [Disunity](https://github.com/ata4/disunity), it also utilizes the [DDSReader](https://github.com/andburn/dds-reader) project.

## Features
- Extract all card art named by card id.
- Extract available card bars (deck list images) by card id.
 - *Generally only collectible cards have card bar information associated with them.*
- Extracts the card xml text files.

Incomplete:
- [ ] Allow filtering of extracted cards by set and card type.
- [ ] Extract all card bars, using some default coordinates.

## Usage
Command line interface
```
extractor.exe cardart <hearthstone_directory> <output_directory>
```

Programming interface
```
using HsArtExtractor;

Extract.CardArt(outDir, hsDir);
```
