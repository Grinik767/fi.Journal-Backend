namespace GoogleSheetParser.GoogleSheet;

public class GoogleSheetConfiguration
{
    public GoogleSheetConfiguration(string credentialsPath)
    {
        CredentialsPath = credentialsPath;
    }

    public string CredentialsPath { get; }
}