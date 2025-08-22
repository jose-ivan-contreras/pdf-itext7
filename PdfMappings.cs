using System.Text;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using OfficeOpenXml;

public class PdfMappings
{
    public PdfMappings()
    {
        ExcelPackage.License.SetNonCommercialPersonal("Intelliflo");
    }

    public void ExportFieldsToExcel(string pdfFileName, string outputExcelFileName)
    {
        ExcelPackage excel = new ExcelPackage();
        var sheet = excel.Workbook.Worksheets.Add("Sheet 1");

        sheet.Cells[1, 1].Value = "Field Name";
        sheet.Cells[1, 2].Value = "Tipo";
        //sheet.Cells[1, 3].Value = "Export Value";
        //sheet.Cells[1, 4].Value = "CheckBox Style";
        //sheet.Cells[1, 5].Value = "Checked";
        sheet.Cells[1, 3].Value = "XPath Mapping";

        using (PdfReader reader = new PdfReader(System.IO.File.OpenRead(pdfFileName)))
        {
            PdfDocument document = new PdfDocument(reader);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(document, true);
            var dictionary = form.GetAllFormFields();
            var fieldNames = dictionary.Keys.Order().ToArray();

            int row = 2;

            foreach (var item in fieldNames)
            {
                var field = dictionary[item];
                PdfFieldInfo info = PdfFieldInfo.GetFieldInfo(field);
                sheet.Cells[row, 1].Value = info.FieldName;
                sheet.Cells[row, 2].Value = info.FieldType;
                //sheet.Cells[row, 3].Value = info.ExportValue ?? "";
                //sheet.Cells[row, 4].Value = info.CheckBoxStyle ?? "";
                //sheet.Cells[row, 5].Value = info.Checked.HasValue ? info.Checked.ToString() : "";
                sheet.Cells[row, 3].Value = info.XPathMapping ?? "";
                ++row;
            }
        }

        excel.SaveAs(outputExcelFileName);
    }

    public void SetPdfMappings(string sourcePdfFileName, string excelFile, string resultPdfFileName)
    {
        ExcelPackage excel = new ExcelPackage(excelFile);
        var sheet = excel.Workbook.Worksheets.First();

        using (PdfReader reader = new PdfReader(sourcePdfFileName))
        {
            using (PdfWriter writer = new PdfWriter(resultPdfFileName))
            {
                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
                    IDictionary<string, PdfFormField> fields = form.GetAllFormFields();

                    for (int row = 2; row <= sheet.Dimension.Rows; row++)
                    {
                        string fieldName = sheet.Cells[row, 1].Text;
                        string xpathValue = sheet.Cells[row, 3].Text;
                        if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(xpathValue))
                        {
                            continue;
                        }

                        if (fields.ContainsKey(fieldName))
                        {
                            PdfFormField field = fields[fieldName];
                            var pdfObject = field.GetPdfObject();
                            pdfObject.Put(PdfName.TM, new PdfString(xpathValue));
                        }
                    }
                }
            }
        }
    }

    public void ChangeFieldNames(string sourcePdfFileName, string resultPdfFileName)
    {
        using (PdfReader reader = new PdfReader(sourcePdfFileName))
        {
            using (PdfWriter writer = new PdfWriter(resultPdfFileName))
            {
                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
                    IDictionary<string, PdfFormField> fields = form.GetAllFormFields();

                    foreach (var item in fields)
                    {
                        if (item.Key.Contains('.'))
                        {
                            item.Value.SetFieldName(item.Key.Replace('.', '|'));
                        }
                    }
                }
            }
        }
    }

    private string GetFormTypeName(PdfFormField field)
    {
        PdfName pdfName = field.GetFormType();
        if (pdfName == null)
        {
            return "";
        }

        string name = pdfName.ToString();
        if (string.Equals(name, PdfName.Btn.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            var ff = field.GetPdfObject().GetAsNumber(PdfName.Ff);
            int flags = ff != null ? ff.IntValue() : 0;
            if ((flags & PdfButtonFormField.FF_PUSH_BUTTON) != 0)
            {
                return "Push Button";
            }
            else if ((flags & PdfButtonFormField.FF_RADIO) != 0)
            {
                return "Radio Button";
            }
            else
            {
                return "Checkbox";
            }
        }
        return name;
    }

    public string GetFormats(string pdfFileName)
    {
        StringBuilder sb = new StringBuilder();
        using (PdfReader reader = new PdfReader(System.IO.File.OpenRead(pdfFileName)))
        {
            PdfDocument document = new PdfDocument(reader);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(document, true);
            var dictionary = form.GetAllFormFields();
            var fieldNames = dictionary.Keys.Order().ToArray();

            foreach (var item in fieldNames)
            {
                var field = dictionary[item];
                if (string.Equals("PR|birthDate", item, StringComparison.OrdinalIgnoreCase))
                {
                    var dict = field.GetPdfObject();
                    var aa = dict.GetAsDictionary(PdfName.AA);
                    if (aa != null)
                    {
                        foreach (var entry in aa.EntrySet())
                        {
                            sb.AppendLine(entry.Key.ToString());
                            var actionDic = entry.Value as PdfDictionary;
                            if (actionDic != null)
                            {
                                sb.AppendLine(actionDic.ToString());
                            }
                        }
                    }
                }
            }
        }
        return sb.ToString();
    }

    public void RemoveActionSemiColon(string sourcePdfFileName, string resultPdfFileName)
    {
        using (PdfReader reader = new PdfReader(sourcePdfFileName))
        {
            using (PdfWriter writer = new PdfWriter(resultPdfFileName))
            {
                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
                    IDictionary<string, PdfFormField> fields = form.GetAllFormFields();

                    foreach (var item in fields)
                    {
                        var f = item.Value;
                        var dict = f.GetPdfObject();
                        var aa = dict.GetAsDictionary(PdfName.AA);
                        if (aa == null)
                        {
                            continue;
                        }

                        var formatAction = aa.GetAsDictionary(PdfName.F);
                        if (formatAction != null)
                        {
                            RemoveSemiColon(formatAction);
                        }

                        var keyStrokeAction = aa.GetAsDictionary(PdfName.K);
                        if (keyStrokeAction != null)
                        {
                            RemoveSemiColon(keyStrokeAction);
                        }
                    }
                }
            }
        }
    }

    private void RemoveSemiColon(PdfDictionary action)
    {
        var js = action.GetAsString(PdfName.JS);
        if (js == null)
        {
            return;
        }

        var jsCode = js.ToUnicodeString().Trim();
        if (jsCode.EndsWith(';'))
        {
            jsCode = jsCode.Substring(0, jsCode.Length - 1);
            action.Put(PdfName.JS, new PdfString(jsCode));
        }
    }
    
    private void PrintActionToConsole(PdfDictionary action)
    {
        var js = action.GetAsString(PdfName.JS);
        if (js == null)
        {
            return;
        }

        var jsCode = js.ToUnicodeString().Trim();
        Console.WriteLine(jsCode);
    }

    public void PrintActions(string sourcePdfFileName)
    {
        using var reader = new PdfReader(sourcePdfFileName);
        using var document = new PdfDocument(reader);

        PdfAcroForm form = PdfAcroForm.GetAcroForm(document, false);
        IDictionary<string, PdfFormField> fields = form.GetAllFormFields();

        foreach (var item in fields)
        {
            var f = item.Value;
            var dict = f.GetPdfObject();
            var aa = dict.GetAsDictionary(PdfName.AA);
            if (aa == null)
            {
                continue;
            }

            var formatAction = aa.GetAsDictionary(PdfName.F);
            Console.WriteLine(item.Key);
            if (formatAction != null)
            {
                PrintActionToConsole(formatAction);
            }

            var keyStrokeAction = aa.GetAsDictionary(PdfName.K);
            if (keyStrokeAction != null)
            {
                PrintActionToConsole(keyStrokeAction);
            }
        }
    }
}