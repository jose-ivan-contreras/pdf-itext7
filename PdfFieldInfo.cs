using System.Reflection;
using iText.Forms.Fields;
using iText.Kernel.Pdf;

public class PdfFieldInfo
{
    public string FieldName { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string? ExportValue { get; set; }
    public bool? Checked { get; set; }
    public string? CheckBoxStyle { get; set; }
    public string? XPathMapping { get; set; }

    public static PdfFieldInfo GetFieldInfo(PdfFormField field)
    {
        PdfFieldInfo fi = new PdfFieldInfo();
        fi.FieldName = field.GetFieldName().ToString();
        fi.FieldType = GetFielType(field);

        var xPathObject = field.GetPdfObject().Get(PdfName.TM);
        if (xPathObject != null)
        {
            fi.XPathMapping = xPathObject.ToString();
        }

        /* Not working
        var x = GetRadioCheckBoxValues(field);
        fi.CheckBoxStyle = x.CheckBoxStyle;
        fi.Checked = x.Checked;
        fi.ExportValue = x.ExportValue;
        */
        return fi;
    }

    private static string GetFielType(PdfFormField field)
    {
        var type = field.GetFormType();
        if (type.Equals(PdfName.Tx))
        {
            return "Text";
        }
        else if (type.Equals(PdfName.Ch))
        {
            return "Choice";
        }
        else if (type.Equals(PdfName.Sig))
        {
            return "Signature";
        }
        else if (type.Equals(PdfName.Btn))
        {
            return GetButtonType(field);
        }

        return "Other";
    }

    private static string GetButtonType(PdfFormField field)
    {
        var ff = field.GetPdfObject().GetAsNumber(PdfName.Ff);
        int flags = ff != null ? ff.IntValue() : 0;

        if ((flags & PdfButtonFormField.FF_PUSH_BUTTON) != 0)
        {
            return "Button";
        }
        else if ((flags & PdfButtonFormField.FF_RADIO) != 0)
        {
            return "Radio";
        }

        return "CheckBox";
    }

    private static (string? ExportValue, string? CheckBoxStyle, bool? Checked)
        GetRadioCheckBoxValues(PdfFormField field)
    {
        var type = field.GetFormType();
        if (!type.Equals(PdfName.Btn))
        {
            return (null, null, null);
        }

        var ff = field.GetPdfObject().GetAsNumber(PdfName.Ff);
        int flags = ff != null ? ff.IntValue() : 0;

        if ((flags & PdfButtonFormField.FF_PUSH_BUTTON) != 0)
        {
            return (null, null, null);
        }
        else if ((flags & PdfButtonFormField.FF_RADIO) != 0)
        {
            var v = field.GetPdfObject().GetAsName(PdfName.V);
            string? exportValue = v?.ToString();
            bool? chk = v?.Equals(PdfName.OFF);
            return (exportValue, exportValue, chk);
        }
        else
        {
            var v = field.GetPdfObject().GetAsName(PdfName.V);
            string? exportValue = v?.ToString();
            bool? chk = v?.Equals(PdfName.OFF);
            string? checkStyle = null;

            var dic = field.GetPdfObject().GetAsDictionary(PdfName.AP);
            if (dic != null)
            {
                var normal = dic.GetAsDictionary(PdfName.N);
                if (normal != null)
                {
                    foreach (var state in normal.KeySet())
                    {
                        if (!state.Equals(PdfName.OFF))
                        {
                            return (state.ToString(), state.ToString(), chk);
                        }
                    }
                }
            }

            return (exportValue, checkStyle, chk);
        }
    }
}