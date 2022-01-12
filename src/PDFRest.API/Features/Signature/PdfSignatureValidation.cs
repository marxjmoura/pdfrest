using System;

namespace PDFRest.API.Features.Signature
{
    public class PdfSignatureValidation
    {
        public string SubjectName { get; set; }

        public string SubjectEmail { get; set; }

        public string IssuerName { get; set; }

        public string IssuerEmail { get; set; }

        public string Location { get; set; }

        public string Reason { get; set; }

        public DateTime SignDate { get; set; }

        public bool IntegrityChecked { get; set; }

        public bool CoverWholeDocument { get; set; }

        public bool SelfSigned => IssuerName == SubjectName;
    }
}
