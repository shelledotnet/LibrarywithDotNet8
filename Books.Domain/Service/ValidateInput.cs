using Books.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Service
{
    public static class ValidateInput
    {
        #region Reflection 

        //this is reflection it  return true if all property value are null
        public static bool ValidateUserInput(BookForCreationDto bookforCreationDto)
        {
            return bookforCreationDto.GetType()
                              .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                               .All(p => p.GetValue(bookforCreationDto) == null);

        }
        #endregion
    }
}
