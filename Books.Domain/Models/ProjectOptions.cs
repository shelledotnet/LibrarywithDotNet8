using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class ProjectOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string? XApiKey { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? SecreteKey { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string[]? ValidAudiences { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? ValidIssuer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public TimeSpan TokenLifeTime { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? SearchQuery { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? Ok { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? FilterQquery { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? NotFound { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? Conflict { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? InValidToken { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? ValidToken { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? FalseToken { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? TokenExpired { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? TokenRevoked { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? TokenUsed { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? TokenNotMatched { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? IsSuccess { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? OrderBadRequest { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? ContactURL { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? LicenseURL { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? UniBaseUrl { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? JokesUrl { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? XApiKeyMap { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? CompanyFailed { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? CompanyCreated { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? UpdateCompanyFailed { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? CompanyUpdated { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? DeleteCompanyFailed { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? CompanyDeleted { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? TokenUnavailable { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int Delay { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string? RegisterationFailed { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string? DefaultRole { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string[]? UserStatus { get; set; }







        #region MyRegion
        //[Required(AllowEmptyStrings = false)]
        //public string? Version { get; set; }

        ////[Range(0, 100)]
        ////public int? Mission { get; set; } = null;

        //[Required(AllowEmptyStrings = false)]
        //public string? Mission { get; set; }

        //[Required(AllowEmptyStrings = false)]
        //public string? CookieName { get; set; }

        //[Required(AllowEmptyStrings = false)]
        //public string? Description { get; set; }

        //[Required(AllowEmptyStrings = false)]
        //public string? Error { get; set; } 
        #endregion
    }

}
