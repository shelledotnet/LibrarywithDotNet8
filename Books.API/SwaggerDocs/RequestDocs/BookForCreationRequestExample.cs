// Ignore Spelling: API

using Bogus;
using Books.Domain.Dto;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace Books.API.SwaggerDocs.RequestDocs;

public class BookForCreationRequestExample : IExamplesProvider<BookForCreationDto>
{
    public BookForCreationDto GetExamples()
    {
        return new BookForCreationDto(Guid.NewGuid(), GenerateRandomName(), GenerateRandomString(30));
        
    }

    static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*() _-+=<>?"; // Characters allowed in the string
        StringBuilder stringBuilder = new StringBuilder(length);
        Random random = new Random();

        for (int i = 0; i < length; i++)
        {
            char randomChar = chars[random.Next(chars.Length)];
            stringBuilder.Append(randomChar);
        }

        return stringBuilder.ToString();
    }

    static string GenerateRandomName()
    {
        return new Faker("en").Company.CompanyName();
    }
}
