using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PDFRest.API.Models.Errors
{
    public class BadRequestError : IActionResult
    {
        public BadRequestError() { }

        public BadRequestError(IEnumerable<ModelError> modelErrors)
        {
            Errors = modelErrors
                .Select(modelError => ErrorMessage(modelError))
                .ToList();
        }

        public IEnumerable<string> Errors { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var json = new JsonResult(this) { StatusCode = 400 };
            await json.ExecuteResultAsync(context);
        }

        private string ErrorMessage(ModelError modelError)
        {
            if (!string.IsNullOrWhiteSpace(modelError.ErrorMessage))
            {
                return modelError.ErrorMessage;
            }

            return "Could not read the request body.";
        }
    }
}
