using OfficeOpenXml;

namespace GoogleSheetParser.Parser;

public class ExcelParser
{
    public async Task<Dictionary<string, double>> GetStudentsPoints(
        string student,
        string studentsColumn,
        int headersRow,
        string pathToSheet,
        int additionalDataRow = -1)
    {
        var fileInfo = new FileInfo(pathToSheet);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var studentColIndex = ColumnLetterToColumnIndex(studentsColumn);

        await using var fileStream = fileInfo.OpenRead();
        using var package = new ExcelPackage();

        await package.LoadAsync(fileStream);

        foreach (var worksheet in package.Workbook.Worksheets)
        {
            var studentRow = FindStudentRow(worksheet, studentColIndex, student);
            if (studentRow == -1) continue;

            return ExtractStudentPoints(
                worksheet,
                studentRow,
                studentColIndex,
                headersRow,
                additionalDataRow);
        }

        return new Dictionary<string, double>();
    }

    public async Task<Dictionary<string, double>> FindDiff(
        string pathToBackUp,
        string pathToCurrentSheet,
        string student,
        string studentsColumn,
        int headersRow,
        int additionalDataRow = -1)
    {
        var backupTask = GetStudentsPoints(student, studentsColumn, headersRow, pathToBackUp, additionalDataRow);
        var currentTask = GetStudentsPoints(student, studentsColumn, headersRow, pathToCurrentSheet, additionalDataRow);

        await Task.WhenAll(backupTask, currentTask);
            
        var resultsFromBackUp = await backupTask;
        var resultsFromCurrentSheet = await currentTask;
            
        return resultsFromCurrentSheet.Except(resultsFromBackUp).ToDictionary(x => x.Key, x => x.Value);
    }

    private static int FindStudentRow(ExcelWorksheet worksheet, int studentColIndex, string student)
    {
        for (var row = 1; row <= worksheet.Dimension.End.Row; row++)
        {
            var cellValue = worksheet.Cells[row, studentColIndex].Text;
            if (!string.IsNullOrEmpty(cellValue) && IsMatchingStudent(cellValue, student))
                return row;
        }
        return -1;
    }

    private static bool IsMatchingStudent(string cellValue, string student)
    {
        var studentWords = student.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cellValueWords = cellValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (studentWords.Length < 2 || cellValueWords.Length < 2)
            return cellValue.Equals(student, StringComparison.OrdinalIgnoreCase);

        return studentWords[0].Equals(cellValueWords[0], StringComparison.OrdinalIgnoreCase)
               && studentWords[1].Equals(cellValueWords[1], StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, double> ExtractStudentPoints(
        ExcelWorksheet worksheet,
        int studentRow,
        int studentColIndex,
        int headersRow,
        int additionalDataRow)
    {
        var headerDict = GetHeaderDictionary(worksheet, headersRow, studentColIndex);
        var additionalDataDict = additionalDataRow > 0
            ? GetAdditionalDataDictionary(worksheet, additionalDataRow, studentColIndex)
            : new Dictionary<int, string>();

        return PopulatePoints(worksheet, studentRow, studentColIndex, headerDict, additionalDataDict);
    }

    private static Dictionary<int, string> GetHeaderDictionary(ExcelWorksheet worksheet, int headersRow, int studentColIndex)
    {
        var headerDict = new Dictionary<int, string>();
        PopulateMergedCells(worksheet, headersRow, headerDict);

        for (var col = studentColIndex; col <= worksheet.Dimension.End.Column; col++)
        {
            if (headerDict.ContainsKey(col)) continue;
            var headerValue = worksheet.Cells[headersRow, col].Text;
            if (!string.IsNullOrEmpty(headerValue))
                headerDict[col] = headerValue;
        }
        return headerDict;
    }

    private static Dictionary<int, string> GetAdditionalDataDictionary(ExcelWorksheet worksheet, int additionalDataRow, int studentColIndex)
    {
        var additionalDataDict = new Dictionary<int, string>();
        PopulateMergedCells(worksheet, additionalDataRow, additionalDataDict);

        for (var col = studentColIndex; col <= worksheet.Dimension.End.Column; col++)
        {
            if (additionalDataDict.ContainsKey(col)) continue;
            var additionalDataValue = worksheet.Cells[additionalDataRow, col].Text;
            if (!string.IsNullOrEmpty(additionalDataValue))
                additionalDataDict[col] = additionalDataValue;
        }
        return additionalDataDict;
    }

    private static void PopulateMergedCells(ExcelWorksheet worksheet, int targetRow, Dictionary<int, string> cellDict)
    {
        foreach (var mergedCellAddress in worksheet.MergedCells)
        {
            var mergedCell = worksheet.Cells[mergedCellAddress];
            if (mergedCell.Start.Row != targetRow) continue;

            var value = mergedCell.Text;
            for (int col = mergedCell.Start.Column; col <= mergedCell.End.Column; col++)
                cellDict[col] = value;
        }
    }

    private static Dictionary<string, double> PopulatePoints(
        ExcelWorksheet worksheet,
        int studentRow,
        int studentColIndex,
        Dictionary<int, string> headerDict,
        Dictionary<int, string> additionalDataDict)
    {
        var points = new Dictionary<string, double>();

        for (var col = studentColIndex; col <= worksheet.Dimension.End.Column; col++)
        {
            if (!headerDict.TryGetValue(col, out var header)) continue;
            if (additionalDataDict.TryGetValue(col, out var additionalData))
                header += $":{additionalData}";

            if (double.TryParse(worksheet.Cells[studentRow, col].Text, out var point))
                points[header] = point;
        }
        return points;
    }

    private static int ColumnLetterToColumnIndex(string columnLetter) => 
        columnLetter.Aggregate(0, (acc, letter) => acc * 26 + (letter - 'A' + 1));
}