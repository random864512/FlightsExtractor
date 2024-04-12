using Microsoft.Extensions.Localization;

namespace FlightsExtractor.Extractor;

public class Messages(IStringLocalizer<Messages>? stringLocalizer = default)
{
    private string Localize(string text, params object[] parameters) => stringLocalizer != default ? (string)stringLocalizer[text, parameters] : string.Format(text, parameters);
    public string FlightDateIsNotAvailable => Localize("Flight date is not available");
    public string FlightNumberIsNotAvailable => Localize("Flight number is not available");
    public string IncorrectDocumentStructure() => Localize("Incorrect document structure");
    public string BriefingPageIsMissing => Localize("Briefing page is missing");
    public string ICAOAAirportCodeIsInIncorrectFormat(string currentValue) => Localize("ICAOA airport code is in incorrect format", currentValue);
    public string AircraftRegistrationInIncorrectFormat(string currentValue) => Localize("Aircraft registration is in incorrect format, found {0}", currentValue);
    public string IncorrectFormat(string field, string currentValue) => Localize($"{field} incorrect format, found {1}", field, currentValue);
    public string FlightNumberIsInIncorrectFormat(string currentValue, string expected) => Localize("Flight number is in incorrect format. found {0}, expected {1}", currentValue, expected);
    public string AircraftRegistrationIsNotAvailable => Localize("Aircraft registration is not available");
    public string FromIsNotAvailable => Localize("From is not available");
    public string ToIsNotAvailable => Localize("To is not available");
    public string ATCCallSignIsNotAvailable => Localize("ATC call sign is not available");
    public string TimeToDestinationIsNoTAvailable => Localize("Time to destination is not available");
    public string FuelToDestinationIsNotAvailable => Localize("Fuel to destination is not available");
    public string TimeToAlternativeIsNotAvailable => Localize("Time to alternative is not available");
    public string FuelToAlternativeIsNotAvailable => Localize("Fuel to alternative is not available");
    public string FuelMinIsNotAvailable => Localize("Fuel min is not available");
    public string CrewMemberFunctionIsNotAvailable => Localize("Crew member function is not available");
    public string CrewMembersNameIsNotAvailable => Localize("Crew members name is not available");
    public string Airdrom1IsNotAvailable => Localize("Airdrom 2 is not available");
    public string Airdrom2IsNotAvailable => Localize("Airdrom 2 is not available");
    public string CrewMembersListIsNotAvailable => Localize("Crew members list is not available");
}
