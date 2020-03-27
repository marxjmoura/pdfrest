var bytes = File.ReadAllBytes("/path/to/file.pdf");
var pdfFile = new ByteArrayContent(bytes);

pdfFile.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Pdf);

var formData = new MultipartFormDataContent();
formData.Add(pdfFile, "file", "file");
formData.Add("PDF_A_2B", "conformanceLevel");
formData.Add("My Title", "title");
formData.Add("Author name", "author");
formData.Add(new DateTime().ToString("o"), "creationDate");
formData.Add("MyProperty", "customProperties[0].name");
formData.Add("My Property Value", "customProperties[0].value");

var client = new HttpClient();
var response = await _client.PostAsync("/pdfa", formData);
