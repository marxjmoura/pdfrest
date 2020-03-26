namespace PDFRest.API.Models
{
    public sealed class FileSizeExceededError : BadRequestError
    {
        public FileSizeExceededError(long size)
        {
            Errors = new[] { $"File must be up to {size} bytes." };
        }
    }
}
