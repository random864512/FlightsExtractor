# FlightsExtractor

## Interface

``dotnet test`` 

starts test 

``dotnet run --project FlightsExtractor.App/FlightsExtractor.App.csproj`` 

starts cli interface

## Project structure (projects)
### FlightsExtractor.App - boot project, interaction by commandLine
### FlightsExtractor.Extractor - extraction logic
### FlightsExtractor.Tests - extraction logic tests

## Used libraries
* PdfPig
* FluentAssertions
* NUnit


## ToDo

### Operational Flight Plan
- [x] Date
- [x] Aircraft registration
- [x] Route
- [x] Alternate airdrome 1
- [x] Alternate airdrome 2
- [x] Flight number
- [x] ATC call sign
- [ ] Departure time
- [ ] Arrival time
- [ ] Zero fuel mass (ZFM)
- [x] Time to destination
- [x] Fuel to destination
- [x] Time to alternate
- [x] Fuel to alternate
- [x] Minimum fuel required
- [ ] Route first and last navigation point: VEBIT – RIXUV
- [ ] Gain/loss: +0 (positive number for gain, negative number for loss)

### Crew briefing
- [ ] Number of passengers in business (C) class
- [ ] Number of passengers in economy (Y) class
- [ ] Dry operating weight (DOW)
- [ ] Dry operating index
- [x] Crew and functions (list, number may vary)

### Other
- [ ] fFix naive Airport registration regex
- [ ] ATC format validation



