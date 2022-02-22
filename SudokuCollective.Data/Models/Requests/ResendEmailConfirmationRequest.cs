using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class ResendEmailConfirmationRequest : IResendEmailConfirmationRequest
    {
        private string _license = string.Empty;
        private readonly GuidValidatedAttribute _guidRegexAttribute = new();

        [Required, GuidValidated(ErrorMessage = AttributeMessages.InvalidLicense)]
        public string License
        {
            get
            {
                return _license;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _guidRegexAttribute.IsValid(value))
                {
                    _license = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidLicense);
                }
            }
        }
        public int RequestorId { get; set; }
        public int AppId { get; set; }

        public ResendEmailConfirmationRequest() { }

        public ResendEmailConfirmationRequest(string license, int requestorId, int appId)
        {
            License = license;
            RequestorId = requestorId;
            AppId = appId;
        }
    }
}
