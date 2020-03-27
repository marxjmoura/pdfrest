using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using PDFRest.API.Models;

namespace PDFRest.Tests.Factories
{
    public static class PdfFormDataFactory
    {
        public static PdfFormData WithConformanceLevel(this PdfFormData model, string conformanceLevel)
        {
            model.ConformanceLevel = conformanceLevel;

            return model;
        }

        public static PdfFormData WithTitle(this PdfFormData model, string title)
        {
            model.Title = title;

            return model;
        }

        public static PdfFormData WithAuthor(this PdfFormData model, string author)
        {
            model.Author = author;

            return model;
        }

        public static PdfFormData WithCreationDate(this PdfFormData model, DateTime creationDate)
        {
            model.CreationDate = creationDate;

            return model;
        }

        public static PdfFormData AddCustomProperty(this PdfFormData model, string name, string value)
        {
            model.CustomProperties = model.CustomProperties ?? new List<CustomPropertyModel>();

            model.CustomProperties.Add(new CustomPropertyModel
            {
                Name = name,
                Value = value
            });

            return model;
        }

        public static MultipartFormDataContent Upload(this PdfFormData model, byte[] bytes)
        {
            var file = new ByteArrayContent(bytes);
            file.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Pdf);

            var formData = new MultipartFormDataContent();
            formData.Add(file, nameof(PdfFormData.File), "dummy.pdf");

            if (model.ConformanceLevel != null)
            {
                formData.Add(new StringContent(model.ConformanceLevel), nameof(PdfFormData.ConformanceLevel));
            }

            if (model.Title != null)
            {
                formData.Add(new StringContent(model.Title), nameof(PdfFormData.Title));
            }

            if (model.Author != null)
            {
                formData.Add(new StringContent(model.Author), nameof(PdfFormData.Author));
            }

            if (model.CreationDate != null)
            {
                var dateTimeIso8601 = model.CreationDate.Value.ToString("o");
                formData.Add(new StringContent(dateTimeIso8601), nameof(PdfFormData.CreationDate));
            }

            if (model.CustomProperties != null)
            {
                for (var i = 0; i < model.CustomProperties.Count; i++)
                {
                    var customProperty = model.CustomProperties.ElementAt(i);
                    var customProperyName = new StringContent(customProperty.Name);
                    var customProperyValue = new StringContent(customProperty.Value);

                    formData.Add(customProperyName, $"{nameof(PdfFormData.CustomProperties)}[{i}][Name]");
                    formData.Add(customProperyValue, $"{nameof(PdfFormData.CustomProperties)}[{i}][Value]");
                }
            }

            return formData;
        }
    }
}
