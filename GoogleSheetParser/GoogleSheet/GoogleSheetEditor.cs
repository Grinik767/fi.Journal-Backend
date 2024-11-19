using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace GoogleSheetParser.GoogleSheet;

public class GoogleSheetEditor
{
    private SheetsService SheetsService { get; }

    public GoogleSheetEditor(SheetsService sheetsService) =>
        SheetsService = sheetsService;

    public void DeleteSingleValue(string googleSpreadSheetId, string cell, string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetId);
        CheckForNull(cell);
        var cellIndex = sheetName is null ? cell : $"{sheetName}!{cell}";

        var request =
            SheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), googleSpreadSheetId, cellIndex);
        request.Execute();
    }

    public void DeleteColumn(string googleSpreadSheetId, string cellIndex, string? sheetName = null)
    {
        CheckForNull(cellIndex);
        var intIndex = ConvertColumnIndexToInt(cellIndex);
        MakeRequestToDeleteColumn(googleSpreadSheetId, intIndex, sheetName);
    }

    public void DeleteRow(string googleSpreadSheetId, int cellIndex, string? sheetName = null)
    {
        if (cellIndex < 0)
            throw new ArgumentException("Индекс строки не может быть меньше 0");
        MakeRequestToDeleteRow(googleSpreadSheetId, cellIndex - 1, sheetName);
    }

    private void MakeRequestToDeleteColumn(string googleSpreadSheetId, int columnIndex, string? sheetName = null)
    {
        var sheetId = GetSheetId(googleSpreadSheetId, sheetName);

        var deleteRequest = new Request
        {
            DeleteDimension = new DeleteDimensionRequest
            {
                Range = new DimensionRange
                {
                    SheetId = sheetId,
                    Dimension = "COLUMNS",
                    StartIndex = columnIndex,
                    EndIndex = columnIndex + 1
                }
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { deleteRequest } };

        var batchUpdate = SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSpreadSheetId);
        batchUpdate.Execute();
    }

    private void MakeRequestToDeleteRow(string googleSpreadSheetId, int columnIndex, string? sheetName = null)
    {
        var sheetId = GetSheetId(googleSpreadSheetId, sheetName);

        var deleteRequest = new Request
        {
            DeleteDimension = new DeleteDimensionRequest
            {
                Range = new DimensionRange
                {
                    SheetId = sheetId,
                    Dimension = "ROWS",
                    StartIndex = columnIndex,
                    EndIndex = columnIndex + 1
                }
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { deleteRequest } };

        var batchUpdate = SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSpreadSheetId);
        batchUpdate.Execute();
    }

    public GoogleSheetEditor WriteSingleValue(string googleSpreadSheetId, string cell, int value,
        string? sheetName = null) =>
        UpdateSingleValue(googleSpreadSheetId, cell, value, sheetName);

    public GoogleSheetEditor WriteSingleValue(string googleSpreadSheetId, string cell, double value,
        string? sheetName = null) =>
        UpdateSingleValue(googleSpreadSheetId, cell, value, sheetName);

    public GoogleSheetEditor WriteSingleValue(string googleSpreadSheetId, string cell, DateTime value,
        string? sheetName = null) =>
        UpdateSingleValue(googleSpreadSheetId, cell, value, sheetName);

    public GoogleSheetEditor WriteSingleValue(string googleSpreadSheetId, string cell, string value,
        string? sheetName = null)
    {
        CheckForNull(value);
        UpdateSingleValue(googleSpreadSheetId, cell, value, sheetName);
        return this;
    }

    public GoogleSheetEditor WriteColumn(string googleSpreadSheetId, string startCell, List<int> values,
        string? sheetName = null) =>
        UpdateColumn(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor WriteColumn(string googleSpreadSheetId, string startCell, List<double> values,
        string? sheetName = null) =>
        UpdateColumn(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor WriteColumn(string googleSpreadSheetId, string startCell, List<DateTime> values,
        string? sheetName = null) =>
        UpdateColumn(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor WriteColumn(string googleSpreadSheetId, string startCell, List<string> values,
        string? sheetName = null) =>
        UpdateColumn(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor WriteRow(string googleSpreadSheetId, string startCell, List<int> values,
        string? sheetName = null) =>
        UpdateRow(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor WriteRow(string googleSpreadSheetId, string startCell, List<double> values,
        string? sheetName = null) =>
        UpdateRow(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor WriteRow(string googleSpreadSheetId, string startCell, List<DateTime> values,
        string? sheetName = null) =>
        UpdateRow(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor WriteRow(string googleSpreadSheetId, string startCell, List<string> values,
        string? sheetName = null) =>
        UpdateRow(googleSpreadSheetId, startCell, values, sheetName);

    public GoogleSheetEditor ColorCellsRange(string googleSpreadSheetId, string range,
        (double red, double green, double blue) color,
        string? sheetName = null)
    {
        var sheetId = GetSheetId(googleSpreadSheetId, sheetName);

        var (startRow, endRow, startCol, endCol) = ParseRange(range);
        var colorRequest = new Request
        {
            RepeatCell = new RepeatCellRequest
            {
                Range = new GridRange
                {
                    SheetId = sheetId,
                    StartRowIndex = startRow,
                    EndRowIndex = endRow,
                    StartColumnIndex = startCol,
                    EndColumnIndex = endCol
                },
                Cell = new CellData
                {
                    UserEnteredFormat = new CellFormat
                    {
                        BackgroundColor = new Color
                        {
                            Red = (float)(color.red / 255.0),
                            Green = (float)(color.green / 255.0),
                            Blue = (float)(color.blue / 255.0)
                        }
                    }
                },
                Fields = "userEnteredFormat.backgroundColor"
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { colorRequest } };
        SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSpreadSheetId).Execute();
        return this;
    }


    public GoogleSheetEditor AddFormulaToCells(string googleSpreadSheetId, string cell, string formula,
        string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetId);
        CheckForNull(cell);
        CheckForNull(formula);

        var cellIndex = sheetName is null ? cell : $"{sheetName}!{cell}";
        var requestBody = new ValueRange
        {
            Values = new List<IList<object>> { new List<object> { $"={formula}" } }
        };

        var request = SheetsService.Spreadsheets.Values.Update(requestBody, googleSpreadSheetId, cellIndex);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        request.Execute();
        return this;
    }

    public GoogleSheetEditor AddFormulaToRangeCells(string googleSpreadSheetId, string range, string formula, string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetId);
        CheckForNull(range);
        CheckForNull(formula);

        var (startRow, endRow, startCol, endCol) = ParseRange(range);
        var sheetId = GetSheetId(googleSpreadSheetId, sheetName);
        
        var startCell = $"{sheetName}!{range.Split(':')[0]}";
        var requestBody = new ValueRange
        {
            Values = new List<IList<object>> { new List<object> { $"={formula}" } }
        };

        var request = SheetsService.Spreadsheets.Values.Update(requestBody, googleSpreadSheetId, startCell);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        request.Execute();
        
        var autoFillRequest = new Request
        {
            AutoFill = new AutoFillRequest
            {
                SourceAndDestination = new SourceAndDestination
                {
                    Source = new GridRange
                    {
                        SheetId = sheetId,
                        StartRowIndex = startRow,
                        EndRowIndex = startRow + 1,
                        StartColumnIndex = startCol,
                        EndColumnIndex = endCol
                    },
                    Dimension = "ROWS",
                    FillLength = endRow - startRow - 1
                },
                UseAlternateSeries = false
            }
        };

        var batchRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { autoFillRequest } };

        SheetsService.Spreadsheets.BatchUpdate(batchRequest, googleSpreadSheetId).Execute();
        return this;
    }
    
    public GoogleSheetEditor MakeHeaderColumn(string googleSpreadSheetId, string rowIndex, string? sheetName = null)
    {
        var sheetId = GetSheetId(googleSpreadSheetId, sheetName);
        var convertColumnIndexToInt = ConvertColumnIndexToInt(rowIndex);
        var headerRequest = new Request
        {
            RepeatCell = new RepeatCellRequest
            {
                Range = new GridRange
                {
                    SheetId = sheetId,
                    StartRowIndex = convertColumnIndexToInt,
                    EndRowIndex = convertColumnIndexToInt + 1
                },
                Cell = new CellData
                {
                    UserEnteredFormat = new CellFormat
                    {
                        TextFormat = new TextFormat
                        {
                            Bold = true,
                            FontSize = 14
                        }
                    }
                },
                Fields = "userEnteredFormat.textFormat.bold"
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { headerRequest } };
        SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSpreadSheetId).Execute();
        return this;
    }

    public GoogleSheetEditor MergeCells(string googleSpreadSheetId, string range, string? sheetName = null)
    {
        var sheetId = GetSheetId(googleSpreadSheetId, sheetName);
        var (startRow, endRow, startCol, endCol) = ParseRange(range);
        var mergeRequest = new Request
        {
            MergeCells = new MergeCellsRequest
            {
                Range = new GridRange
                {
                    SheetId = sheetId,
                    StartRowIndex = startRow,
                    EndRowIndex = endRow,
                    StartColumnIndex = startCol,
                    EndColumnIndex = endCol
                },
                MergeType = "MERGE_ALL"
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { mergeRequest } };
        SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSpreadSheetId).Execute();
        return this;
    }

    public GoogleSheetEditor AutoResizeColumns(string googleSpreadSheetId, string range, string? sheetName = null)
    {
        var sheetId = GetSheetId(googleSpreadSheetId, sheetName);

        CheckForNull(googleSpreadSheetId);
        var rangeSplit = range.Split(":");
        if (rangeSplit.Length != 2)
            throw new ArgumentException($"некоректный тип переданных столбцов для выравнивания: {range}");

        var startColumnIndex = ConvertColumnIndexToInt(rangeSplit[0]);
        var endColumnIndex = ConvertColumnIndexToInt(rangeSplit[1]);

        var autoResizeRequest = new Request
        {
            AutoResizeDimensions = new AutoResizeDimensionsRequest
            {
                Dimensions = new DimensionRange
                {
                    SheetId = sheetId,
                    Dimension = "COLUMNS",
                    StartIndex = startColumnIndex,
                    EndIndex = endColumnIndex
                }
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
            { Requests = new List<Request> { autoResizeRequest } };
        var request = SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSpreadSheetId);
        request.Execute();
        return this;
    }
    
    public GoogleSheetEditor CreateNewSheet(string googleSheetId, string sheetName)
    {
        CheckForNull(googleSheetId);
        CheckForNull(sheetName);

        var addSheetRequest = new Request
        {
            AddSheet = new AddSheetRequest
            {
                Properties = new SheetProperties { Title = sheetName }
            }
        };
        
        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request> { addSheetRequest }
        };
        var batchRequest = SheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSheetId);
        batchRequest.ExecuteAsync();

        return this;
    }

    private GoogleSheetEditor UpdateSingleValue<T>(string googleSpreadSheetId, string cell, T value,
        string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetId);
        CheckForNull(cell);
        var requestBody = new ValueRange
        {
            Values = new List<IList<object>> { new List<object> { value! } }
        };

        var cellIndex = sheetName is null ? cell : $"{sheetName}!{cell}";

        var request = SheetsService.Spreadsheets.Values.Update(requestBody, googleSpreadSheetId, cellIndex);

        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        request.Execute();
        return this;
    }

    private GoogleSheetEditor UpdateColumn<T>(string googleSpreadSheetId, string startCell, List<T> values,
        string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetId);
        CheckForNull(startCell);
        var valueRange = new ValueRange();
        var rows = values.Select(value => new List<object> { value! }).Cast<IList<object>>().ToList();
        valueRange.Values = rows;
        var columnIndex = startCell.All(char.IsLetter) ? $"{startCell}1" : startCell;
        var columnIndexWithSheetName = sheetName is null ? columnIndex : $"{sheetName}!{columnIndex}";

        var request =
            SheetsService.Spreadsheets.Values.Update(valueRange, googleSpreadSheetId, columnIndexWithSheetName);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        request.Execute();
        return this;
    }


    private GoogleSheetEditor UpdateRow<T>(string googleSpreadSheetId, string startCell, List<T> values,
        string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetId);
        CheckForNull(startCell);

        var objectValues = values.ConvertAll(v => (object)v!);

        var requestBody = new ValueRange
        {
            Values = new List<IList<object>> { objectValues }
        };

        var cellIndex = sheetName is null ? startCell : $"{sheetName}!{startCell}";

        var request = SheetsService.Spreadsheets.Values.Update(requestBody, googleSpreadSheetId, cellIndex);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

        request.Execute();
        return this;
    }

    private static (int startRow, int endRow, int startCol, int endCol) ParseRange(string range)
    {
        var parts = range.Split(':');
        int endRow, endCol;

        var (startRow, startCol) = ParseCell(parts[0]);

        if (parts.Length == 1)
        {
            endRow = startRow + 1;
            endCol = startCol + 1;
        }
        else
        {
            (endRow, endCol) = ParseCell(parts[1]);
            endCol += 1;
            endRow += 1;
        }

        return (startRow, endRow, startCol, endCol);
    }

    private static (int row, int col) ParseCell(string cell)
    {
        var i = 0;
        while (i < cell.Length && char.IsLetter(cell[i]))
        {
            i++;
        }

        var colLetters = cell[..i];
        var rowNumbers = cell[i..];

        var col = ConvertColumnIndexToInt(colLetters);
        var row = int.Parse(rowNumbers) - 1;

        return (row, col);
    }

    private int GetSheetId(string googleSpreadSheetId, string? sheetName = null)
    {
        var spreadsheet = SheetsService.Spreadsheets.Get(googleSpreadSheetId).Execute();
        var sheetId = 0;
        if (sheetName is null)
            sheetId = (int)spreadsheet.Sheets.First().Properties.SheetId!;
        else
        {
            foreach (var sheet in spreadsheet.Sheets)
            {
                if (!Equals(sheet.Properties.Title, sheetName)) continue;
                sheetId = (int)sheet.Properties.SheetId!;
                break;
            }
        }

        return sheetId;
    }

    private static int ConvertColumnIndexToInt(string columnIndex)
    {
        var sum = 0;
        foreach (var c in columnIndex)
        {
            sum *= 26;
            sum += c - 'A' + 1;
        }

        return sum - 1;
    }

    private static void CheckForNull(string str)
    {
        if (string.IsNullOrEmpty(str))
            throw new ArgumentNullException("Нет значения");
    }
}