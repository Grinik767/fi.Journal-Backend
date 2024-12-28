namespace GoogleSheetParser.Parser;

public interface IExcelParser
{
    public Task<Dictionary<string, double>> GetStudentsPoints(
        string student,
        string studentsColumn,
        int headersRow,
        string pathToSheet,
        int subHeader = -1,
        bool needToConsiderHeaderRow = false,
        string listForChecking = "");

    public Task<Dictionary<string, double>> FindDiff(
        string pathToBackUp,
        string pathToCurrentSheet,
        string student,
        string studentsColumn,
        int headersRow,
        int subHeader = -1,
        string listForChecking = "");

    public Task<(int studentRow, string sheetName)> GetStudentRow(string studentColumnIndex, string student,
        string pathToSheet, string listForChecking = "");
}