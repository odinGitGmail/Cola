using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Cola.Utils.Helper;

public class ExcelHelper
{
    /// <summary>
    /// 将Excel转换为Datatable
    /// </summary>
    /// <param name="excelFileStream"></param>
    /// <param name="sheetIndex"></param>
    /// <param name="headerRowIndex"></param>
    /// <param name="fileExt"></param>
    /// <param name="columnCount"></param>
    /// <param name="endRow"></param>
    /// <returns></returns>
    public static DataTable RenderDataTableFromExcel(
        Stream excelFileStream,
        int sheetIndex,
        int headerRowIndex,
        string fileExt,
        int columnCount = 0,
        int endRow = 0
    )
    {
        IWorkbook? workbook;
        IFormulaEvaluator iformulaEval;
        switch (fileExt)
        {
            case ".xlsx":
                workbook = new XSSFWorkbook(excelFileStream);
                iformulaEval = new XSSFFormulaEvaluator(workbook);
                break;
            case ".xls":
                workbook = new HSSFWorkbook(excelFileStream);
                iformulaEval = new HSSFFormulaEvaluator(workbook);
                break;
            default:
                throw new Exception("请使用Excel文件导入");
        }
        var sheet = workbook.GetSheetAt(sheetIndex);

        var table = new DataTable();

        var headerRow = sheet.GetRow(headerRowIndex);

        var cellCount = columnCount == 0 ? headerRow.LastCellNum : columnCount;

        for (int i = headerRow.FirstCellNum; i < cellCount; i++)
        {
            var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
            table.Columns.Add(column);
        }

        endRow = endRow == 0 ? sheet.LastRowNum : endRow;
        for (var i = headerRowIndex + 1; i <= endRow; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null) continue;
            var dataRow = table.NewRow();

            for (int j = row.FirstCellNum; j < cellCount; j++)
            {
                if (j == -1 || row.GetCell(j) == null)
                    continue;

                var cell = row.GetCell(j);

                switch (cell.CellType)
                {
                    case CellType.Unknown:
                        dataRow[j] = cell.ToString();
                        break;
                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            dataRow[j] = cell.DateCellValue.ToString();
                        }
                        else
                        {
                            dataRow[j] = cell.NumericCellValue.ToString();
                        }
                        break;
                    case CellType.String:
                        dataRow[j] = cell.StringCellValue.ToString();
                        break;
                    case CellType.Formula:
                        dataRow[j] = iformulaEval.Evaluate(cell).StringValue;
                        break;
                    case CellType.Blank:
                        dataRow[j] = null;
                        break;
                    case CellType.Boolean:
                        dataRow[j] = cell.BooleanCellValue.ToString();
                        break;
                    case CellType.Error:
                    default:
                        dataRow[j] = cell.ToString();
                        break;
                }
            }

            var isTrueRow = dataRow.ItemArray.OfType<object>().Any(t => !string.IsNullOrEmpty(t!.ToString()));

            if (isTrueRow)
            {
                table.Rows.Add(dataRow);
            }
        }

        excelFileStream.Close();
        workbook = null;
        sheet = null;
        return table;
    }
    
    /// Description:  Inserts a existing row into a new row, will automatically push down
    ///               any existing rows.  Copy is done cell by cell and supports, and the
    ///               command tries to copy all properties available (style, merged cells, values, etc...)
    ///
    public void CopyRow(IWorkbook workbook, int worksheetIndex, int sourceRowNum, int destinationRowNum)
    {
        var worksheet = workbook.GetSheetAt(worksheetIndex);
        // Get the source / new row
        IRow newRow = worksheet.GetRow(destinationRowNum);
        IRow sourceRow = worksheet.GetRow(sourceRowNum);

        // If the row exist in destination, push down all rows by 1 else create a new row
        if (newRow != null)
        {
            worksheet.ShiftRows(destinationRowNum, worksheet.LastRowNum, 1);
        }
        else
        {
            newRow = worksheet.CreateRow(destinationRowNum);
        }

        // Loop through source columns to add to new row
        if (sourceRow != null)
        {
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                ICell oldCell = sourceRow.GetCell(i);
                ICell newCell = newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }

                // Copy style from old cell and apply to new cell
                ICellStyle newCellStyle = workbook.CreateCellStyle();
                newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                ;
                newCell.CellStyle = newCellStyle;

                // If there is a cell comment, copy
                if (newCell.CellComment != null)
                    newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null)
                    newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data type
                newCell.SetCellType(oldCell.CellType);

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    case NPOI.SS.UserModel.CellType.Blank:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case NPOI.SS.UserModel.CellType.Boolean:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case NPOI.SS.UserModel.CellType.Error:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case NPOI.SS.UserModel.CellType.Formula:
                        newCell.SetCellFormula(oldCell.CellFormula);
                        break;
                    case NPOI.SS.UserModel.CellType.Numeric:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case NPOI.SS.UserModel.CellType.String:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case NPOI.SS.UserModel.CellType.Unknown:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                }
                
                newCell.CellStyle.DataFormat = oldCell.CellStyle.DataFormat;
            }
        }
    }
}