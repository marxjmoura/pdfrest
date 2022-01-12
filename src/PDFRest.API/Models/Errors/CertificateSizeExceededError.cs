namespace PDFRest.API.Models.Errors
{
    public sealed class CertificateSizeExceededError : BadRequestError
    {
        public CertificateSizeExceededError(long size)
        {
            Errors = new[] { $"Certificate must be up to {size} bytes." };
        }
    }
}
