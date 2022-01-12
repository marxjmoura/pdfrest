using System.IO;
using iText.Forms;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Signatures;

namespace PDFRest.API.Features.Signature
{
    public class AllPagesSignature
    {
        private readonly IExternalSignature _externalSignature;
        private readonly PdfSignatureAppearance _appearance;

        public AllPagesSignature(
            IExternalSignature externalSignature,
            PdfSignatureAppearance appearance)
        {
            _appearance = appearance;
            _externalSignature = externalSignature;
        }

        public byte[] Annotate(byte[] pdfBytes)
        {
            var stream = new MemoryStream(pdfBytes);
            var pdfReader = new PdfReader(stream);
            var pdfDocument = new PdfDocument(pdfReader);
            var signatureUtil = new SignatureUtil(pdfDocument);
            var acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);

            foreach (var name in signatureUtil.GetSignatureNames())
            {
                var field = acroForm.GetField(name);

                field.SetModified();
                // field.SetValue("BLABLABLA");

                for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                {
                    // var rectangle = new Rectangle(10, 10, 160, 120);
                    // var form = new PdfFormXObject(rectangle);

                    // var signField = PdfFormField.CreateSignature(pdfDocument);
                    // signField.SetFieldName(name);
                    // signField.Put(PdfName.V, cryptoDictionary.getPdfObject());
                    // signField.AddKid(widget);

                    var pdfPage = pdfDocument.GetPage(pageNumber);
                    var rectangle = new Rectangle(_appearance.GetPageRect());
                    var pdfWidgetAnnotation = new PdfWidgetAnnotation(rectangle);
                    // pdfWidgetAnnotation.setFlags(PdfAnnotation.PRINT | PdfAnnotation.LOCKED);

                    // pdfWidgetAnnotation.SetNormalAppearance(form.GetPdfObject());
                    pdfWidgetAnnotation.SetPage(pdfPage);

                    field.AddKid(pdfWidgetAnnotation);

                    pdfPage.AddAnnotation(pdfWidgetAnnotation);
                }
            }

            return _externalSignature.Sign(stream.ToArray());
        }
    }
}
