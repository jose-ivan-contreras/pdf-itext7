void ChangeFieldNames(string sourcePdf, string resultPdf)
{
    var pdf = new PdfMappings();
    pdf.ChangeFieldNames(sourcePdf, resultPdf);
}

void RemoveTrailingSemicolon(string sourcePdf, string resultPdf)
{
    var pdf = new PdfMappings();
    pdf.RemoveActionSemiColon(sourcePdf, resultPdf);
}

void SaveMappingsToExcel(string sourcePDF, string excelFileName)
{
    var pdf = new PdfMappings();
    pdf.ExportFieldsToExcel(sourcePDF, excelFileName);
}

void SetMappintsToNewPdf(string sourcePDF, string excelFileName, string resultPdf)
{
    var pdf = new PdfMappings();
    pdf.SetPdfMappings(sourcePDF, excelFileName, resultPdf);
}

string sourceFolder = @"C:\Users\JoseContreras\OneDrive - intelliflo\Desktop\6282\To Deploy";
string sourcePdf = @"Joint Transfer on Death Account Agreement Aug 2025.pdf";
//System.IO.Path.Combine(sourceFolder, sourcePdf)

SaveMappingsToExcel(
    System.IO.Path.Combine(sourceFolder, sourcePdf),
    System.IO.Path.Combine(sourceFolder, "mappings.xlsx")
);
