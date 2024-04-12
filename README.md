# FlightsExtractor

## CLI

``dotnet test`` 

starts tests

``dotnet run --project .\FlightsExtractor.App\FlightsExtractor.App.csproj -- extract ./FlightsExtractor.Extractor.Tests/SampleFile.pdf`` 

build & run CLI app in Debug configuration (with logging level >= traces), extracts sample file places in tests folder

``dotnet run --project .\FlightsExtractor.App\FlightsExtractor.App.csproj -c Release -- extract ./FlightsExtractor.Extractor.Tests/SampleFile.pdf``

build & run CLI app in Release configuration, extracts sample file places in tests folder

## Project structure (projects)

Projects contains three projects, to use this logic in larger system only reference to FlightsExtractor.Extractor is required.

### FlightsExtractor.App
boot project, interaction by commandLine

#### libraries
* System.CommandLine
* Microsoft.Extensions.Logging
* Microsoft.Extensions.Logging.Console
* Microsoft.Extensions.DependencyInjection
### FlightsExtractor.Extractor - extraction logic
#### public interfaces
- ``FlightPlanning`` data model 
- ``AddFlightPlanningExtractor(this IServiceCollection serviceCollection)`` to cooperate with MSFT DI
- ``FlightPlanningExtractorFactory.Create()`` for other non DI scenarios
- ``IFlightPlanningExtractor`` main extractor interface

#### libraries
* Microsoft.Extensions.DependencyInjection.Abstractions
* Microsoft.Extensions.Logging.Abstractions
* Microsoft.Extensions.Localization.Abstractions
* PdfPig
* Tabula

### FlightsExtractor.Tests - extraction logic tests

#### libraries
* FluentAssertions
* NUnit
* Microsoft.Extensions.DependencyInjection

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
- [ ] Connect Crew Briefing page also by Flight date (not only by flight number)
- [ ] Better, not naive parsing and validation logic (like ATC format validation)
- [ ] Additional test cases based on another samples of documents (factories etc.)
- [ ] Extract logging configuration level to appsettings.json / based on build configuration
- [ ] Test localization mechanism
- [ ] Eventual less naive way to recognize crew briefing page
- [ ] Better messages about missing fields / warnings



