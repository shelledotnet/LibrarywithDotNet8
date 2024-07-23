using Books.domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Books.API.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrorMessages(this ModelStateDictionary dictionary)
        {
            return dictionary.SelectMany(m => m.Value.Errors)
                             .Select(m => m.ErrorMessage)
                             .ToList();
        }

        public static ServiceBadResponse GetApiResponse(this ModelStateDictionary dictionary)
        {

            return new ServiceBadResponse { IsSuccess = false, Message =  GetErrorMessages(dictionary) };


        }
    }
}
