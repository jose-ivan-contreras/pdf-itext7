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


string sourceFolder = @"C:\Users\JoseContreras\OneDrive - intelliflo\Desktop\6282";
string sourcePdf = @"Individual Transfer on Death Account Agreement Attachment.pdf";

RemoveTrailingSemicolon(
        System.IO.Path.Combine(sourceFolder, sourcePdf),
        System.IO.Path.Combine(sourceFolder, "NewPdf.pdf")
);