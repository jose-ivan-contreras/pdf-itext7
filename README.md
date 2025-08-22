# pdf-itext7
This adjustments to the PDF file can be done in any order

## Set Control IDs
Control IDs use `|` character to separate words. You need to change Pershing documents to use such character instead of a period `.`  

Example:

`accountHeader.accountNumber` changes to `accountHeader|accountNumber`
    
To perform this task there is a method called **ChangeFieldNames** which receives two parameters: the pdf that needs to be adjusted and the result pdf file name.
This method creates a new file.

Example:

    string fileToBeChanged = @"C:\MyFile.pdf";
    string fileToBeCreated = @"C:\MyNewFileWithAdjustedIds.pdf";
    var pdf = new PdfMappings();
    pdf.ChangeFieldNames(fileToBeChanged, fileToBeCrated);

In this case, **fileToBeCreated** will have new ids on every control.

## Adjust Text Edit format and CheckBox 
**TextEdit** have a `Format tab`.  You need to set the right format on every control as needed.
**Checkboxes** have a `Export Value` input in which you can set the possible values coming from the data source.  If a Export Value is equal to the data source value, then the control will be checked.

## Change signature fields to Text Fields
If a field is going to be signed using `DocuSign` then you would need to change a Signature Field to a Text Field.  DocuSign cannot use Pdf Signature control to set the signature.

## Change Date functions to remove trailing `;`
If any field in the document has a Date format set, then Adobe will add a trailing `;`
To remove this you need to execute method **RemoveActionSemiColon**.  This takes two parameters: the actual pdf that wants to be adjusted and the result pdf.

    string fileToBeChanged = @"C:\MyFile.pdf";
    string fileToBeCreated = @"C:\MyNewFileWithAdjustedIds.pdf";
    var pdf = new PdfMappings();
    pdf.ChangeFieldNames(fileToBeChanged, fileToBeCrated);

In this case, **fileToBeCreated** will be corrected removing the trailing character.
