namespace PDFRest.API.Models.Errors
{
    public sealed class PdfSizeExceededError : BadRequestError
    {
        public PdfSizeExceededError(long size)
        {
            Errors = new[] { $"Document must be up to {size} bytes." };
        }
    }
}
