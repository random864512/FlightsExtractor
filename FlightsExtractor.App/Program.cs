﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Text.Json;
using FlightsExtractor.Extractor;
using Microsoft.Extensions.DependencyInjection;

var rootCommand = new RootCommand(description: "FlightsExtractor - extract your flights on the fly!");
var command = new Command("extract", "extract data from pdf file");
var fileArg = new Argument<FileInfo>("file", description: "the pdf file to parse");
command.AddArgument(fileArg);
command.SetHandler((FileInfo file) =>
{
    try
    {
        using var serviceProvider = new ServiceCollection().AddFlightPlanningExtractor().BuildServiceProvider();
        var flightPlanning = serviceProvider.GetRequiredService<IFlightPlanningExtractor>();
        var extracted = flightPlanning.Extract(file.FullName);
        Console.WriteLine(JsonSerializer.Serialize(extracted, new JsonSerializerOptions { WriteIndented = true }));
    }
    catch (FlightPlanningValidationException e)
    {
        Console.WriteLine($"Invalid file structure : {e.Message}");
    }
    catch (FileDoesNotExistException)
    {
        Console.WriteLine("File does not exist!");
    }
    catch (FlightPlanningExtractionException)
    {
        Console.WriteLine("Something went wrong, file may be corrupted or in incorrect format");
    }
}, fileArg);
rootCommand.Add(command);

var builder = new CommandLineBuilder(rootCommand).UseDefaults().UseHelp();
return await builder.Build().InvokeAsync(args);