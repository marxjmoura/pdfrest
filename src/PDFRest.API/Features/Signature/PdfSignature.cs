using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iText.Forms;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkcs;

namespace PDFRest.API.Features.Signature
{
    public class PdfSignature
    {
        public DateTime SignDate { get; set; }

        public string Location { get; set; }

        public string Reason { get; set; }

        public byte[] Sign(byte[] pdfBytes, byte[] certificateBytes, string password)
        {
            var signedPdf = new MemoryStream();
            var reader = new PdfReader(new MemoryStream(pdfBytes));
            var document = new PdfDocument(new PdfReader(new MemoryStream(pdfBytes)));

            var store = new Pkcs12Store(new MemoryStream(certificateBytes), password.ToCharArray());

            var alias = store.Aliases.OfType<string>()
                .Where(alias => store.IsKeyEntry(alias) && store.GetKey(alias).Key.IsPrivate)
                .SingleOrDefault();

            var chain = store.GetCertificateChain(alias)
                .Select(entry => entry.Certificate)
                .ToArray();

            var privateKey = store.GetKey(alias).Key;

            var stampingProperties = new StampingProperties().UseAppendMode();
            var signer = new PdfSigner(reader, signedPdf, stampingProperties);

            var signDateText = SignDate.ToString("dd/MM/yyyy HH:mm:ss");

            var subject = store.GetCertificate(alias).Certificate.SubjectDN;
            var subjectName = subject.GetValueList(X509Name.CN).OfType<string>().SingleOrDefault();

            Reason = "Eu estou aprovando este documento com minha assinatura de vinculação legal";
            Location = "Aracaju/SE";

            var layerText = new StringBuilder()
                .AppendLine($"Assinado digitalmente por {subjectName}")
                .AppendLine($"DN: {subject}")
                .AppendLine($"Razão: {Reason}")
                .AppendLine($"Local: {Location}")
                .AppendLine($"Data: {signDateText}")
                .ToString();

            // signer.SetFieldLockDict();

            signer.SetSignDate(SignDate);

            var appearance = signer.GetSignatureAppearance()
                .SetPageRect(new Rectangle(10, 10, 160, 120)) // Font 6 and 7
                // .SetPageRect(new Rectangle(10, 10, 200, 150)) // Font 8
                .SetReason(Reason)
                .SetLocation(Location)
                .SetLayer2Text(layerText)
                .SetLayer2FontSize(7)
                .SetPageNumber(document.GetNumberOfPages());

            var signature = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);

            // Reason:
            // - Eu sou o autor deste documento
            // - Eu revisei este documento
            // - Eu estou aprovando este documento
            // - Eu estou aprovando este documento com minha assinatura de vinculação legal
            // - Eu atesto a precisão e a integridade deste documento
            // - Eu concordo com os termos definidos por minha assinatura neste documento
            // - Eu concordo com partes específicas deste documento

            signer.SignDetached(signature, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);

            // var pdfSignatureAnnotation = new AllPagesSignature(signature, appearance);
            // pdfSignatureAnnotation.Annotate(signedPdf.ToArray());

            return signedPdf.ToArray();
        }

        public IEnumerable<PdfSignatureValidation> Validate(byte[] pdfBytes)
        {
            var pdfStream = new MemoryStream(pdfBytes);
            var pdfReader= new PdfReader(pdfStream);
            var pdfDocument = new PdfDocument(pdfReader);
            var signatureUtil = new SignatureUtil(pdfDocument);
            var signatureNames = signatureUtil.GetSignatureNames();

            return signatureNames
                .Select(name =>
                {
                    var pkcs7 = signatureUtil.ReadSignatureData(name);
                    var signature = signatureUtil.GetSignature(name);
                    var certificate = pkcs7.GetSigningCertificate();

                    var authentication = new PdfSignatureValidation();

                    authentication.SubjectName = certificate.SubjectDN
                        .GetValueList(X509Name.CN).OfType<string>().SingleOrDefault();

                    authentication.SubjectEmail = certificate.SubjectDN
                        .GetValueList(X509Name.EmailAddress).OfType<string>().SingleOrDefault();

                    authentication.IssuerName = certificate.IssuerDN
                        .GetValueList(X509Name.CN).OfType<string>().SingleOrDefault();

                    authentication.IssuerEmail = certificate.IssuerDN
                        .GetValueList(X509Name.EmailAddress).OfType<string>().SingleOrDefault();

                    authentication.Location = signature.GetLocation();
                    authentication.Reason = signature.GetReason();
                    authentication.SignDate = pkcs7.GetSignDate();
                    authentication.IntegrityChecked = pkcs7.VerifySignatureIntegrityAndAuthenticity();
                    authentication.CoverWholeDocument = signatureUtil.SignatureCoversWholeDocument(name);

                    return authentication;
                })
                .ToList();
        }
    }
}
