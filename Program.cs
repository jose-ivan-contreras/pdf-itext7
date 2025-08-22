string sourceFolder = @"C:\Users\JoseContreras\OneDrive - intelliflo\Desktop\6282";
string sourcePdf = @"Individual Transfer on Death Account Agreement Original.pdf";

PdfCopy.CopyAttributes(System.IO.Path.Combine(sourceFolder, sourcePdf));

void Test()
{
    //Get Mappings for new document 
    string sourceFolder = @"C:\Users\JoseContreras\OneDrive - intelliflo\Desktop\6104";
    string pdfFile = @"Traditional_IRA_Custodial_Account_Agreement Aug 2025.pdf";
    string excelFile = @"Mappings New.xlsx";

    PdfMappings pdf = new PdfMappings();

    //pdf.ExportFieldsToExcel(System.IO.Path.Combine(sourceFolder, pdfFile), System.IO.Path.Combine(sourceFolder, excelFile));

    //pdf.ChangeFieldNames(System.IO.Path.Combine(sourceFolder, pdfFile), System.IO.Path.Combine(sourceFolder, "Modified.pdf"));
    /*
    pdf.SetPdfMappings(
        System.IO.Path.Combine(sourceFolder, pdfFile),
        System.IO.Path.Combine(sourceFolder, excelFile),
        System.IO.Path.Combine(sourceFolder, "New Pdf.pdf"));

    */

    pdf.RemoveActionSemiColon(
        System.IO.Path.Combine(sourceFolder, pdfFile),
        System.IO.Path.Combine(sourceFolder, "New Pdf.pdf")
    );


    var x = pdf.GetFormats(System.IO.Path.Combine(sourceFolder, "New Pdf.pdf"));
    Console.WriteLine(x);
}