using AutoMapper;
using Books.Domain.Entities;
using Books.domain.Models;
using Books.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Books.Domain.Data;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Books.Domain.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<EmployeeManagerDbContext> contextFactory;
        private readonly IMapper _mapper;
        private readonly ProjectOptions _projectOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserRepository> _logger;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public UserRepository(IDbContextFactory<EmployeeManagerDbContext> contextFactory, IMapper mapper,
            IOptionsMonitor<ProjectOptions> projectOptions, TokenValidationParameters tokenValidationParameters
            , IHttpContextAccessor httpContextAccessor, ILogger<UserRepository> logger)
        {
            this.contextFactory = contextFactory;
            _mapper = mapper;
            _projectOptions = projectOptions.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        //C:\Users\Mohammed.Shelle\AppData\Roaming\Microsoft\UserSecrets\c1d2563a-74d4-4d85-a9fe-13820fd35c88\secrets.json
        private static async Task DeActivateUser(Users user, EmployeeManagerDbContext context)
        {
           if((DateTime.Now > user.DateExpired) && user.Active)
            {
                user.Active = false;
                context.Users.Update(user);//this keep tract of changes in the entity
                await context.SaveChangesAsync();//this persist to DB
            }
        }
        
        
        public async Task<ServiceResponse<GenTokens>> Authenticate(LoginRequestDto loginRequestDto)
        {
            EmployeeManagerDbContext context = await contextFactory.CreateDbContextAsync();
            ServiceResponse<GenTokens> response = new();
            try
            {
                UsersDto usersDto = _mapper.Map<UsersDto>(loginRequestDto);



                Users? user = await context.Users.FirstOrDefaultAsync(u => u.Username!.Trim().Equals(usersDto.Username!.Trim(), StringComparison.CurrentCultureIgnoreCase) && u.Active && !u.Blocked);
                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Code = HttpStatusCode.NotFound;
                    response.Message = _projectOptions.NotFound;
                    return response;
                }
                else if (user is not null)
                    await DeActivateUser(user, context);

                #region CompareForPasswordSalt-Match
                //var passwordHash = HashingHelper.HashUsingPbkdf2(usersDto.Password, user.PasswordSalt); 
                //if (usersDto.Password != passwordHash)
                //{
                //    response.IsSuccess = false;
                //    response.Message = _projectOptions.NotFound;
                //}
                #endregion

                bool isPasswordatched = MatchPasswordHash(usersDto.Password, user.Password, user.PasswordKey);
                if (isPasswordatched)
                {

                    //Generate-AccessToken-jwt
                    CreateJWT(user, context, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token);

                    //RemovedUnused-RefeshToken-jwt
                    await RemovedUnusedRefeshToken(user, context);


                    //Generate-RefreshToken
                    //Log this output paramter referesh token in the immemory object
                    CreateRefreshToken(user, token, context, out string createToken, out RefereshTokenModels refershToken);

                    //set refereshtoken on cookie header response
                    //SetRefreshToken(refershToken, createToken);



                    GenTokens genTokens = new()
                    {
                        Token = tokenHandler.WriteToken(token),
                        RefreshToken = createToken,
                        Username = user.Username,
                        ValidTo = token.ValidTo,//DateTime.Now.Add(_projectOptions.TokenLifeTime),
                        ValidFrom = token.ValidFrom,
                        RefreshTokenExpireDate = refershToken.DateExpired,
                        Email = user.Email,
                        Role = GetUserRoles(user)

                    };

                    SetTokensInsideCookie(genTokens);

                    response.Data = genTokens;
                    response.Code=HttpStatusCode.Created;
                    response.IsSuccess = true;
                    response.Message = "IsSuccess";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Code=HttpStatusCode.Forbidden;
                    response.Message = _projectOptions.NotFound;
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex, "authenticate");
                response.IsSuccess = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }

        private bool MatchPasswordHash(string? password, byte[] passwordHash, byte[] passwordKey)
        {
            using var hmac = new HMACSHA512(passwordKey);
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        public string Base64Encode(string text)
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }
        public async Task<ServiceResponse<LoginResponseDto>> Register(RegisterRequestDto registerRequestDto)
        {
            EmployeeManagerDbContext? context = await contextFactory.CreateDbContextAsync();
            ServiceResponse<LoginResponseDto> response = new();
            try
            {

                #region DedubCheck
                bool isAnyUserActive = await UserAlreadyExists(registerRequestDto.Username!, registerRequestDto.Email!);
                if (isAnyUserActive)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.Conflict;
                    response.Code = HttpStatusCode.Conflict;
                    return response;
                }

                #endregion

                Users users = _mapper.Map<Users>(registerRequestDto);


                using var hmac = new HMACSHA512();
                users.PasswordKey = hmac.Key;
                users.Password = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerRequestDto.Password));
                users.Active = true;
                users.Blocked = false;
                users.DateExpired = DateTime.UtcNow.AddMonths(3);

                #region users.PasswordSalt

                //var passSalt = Base64Encode(Guid.NewGuid().ToString());

                //users.PasswordSalt = PassSalt
                //users.Password = HashingHelper.HashUsingPbkdf2(registerRequestDto.Password, PassSalt);

                #endregion



                context.Users.Add(users);
                int result = await context.SaveChangesAsync();
                if (result == 1)
                {

                    Role role = new()
                    {
                        Name = _projectOptions.DefaultRole,
                        UsersId = users.Id,
                    };
                    context.Role.Add(role);
                    await context.SaveChangesAsync();


                    response.Data = new LoginResponseDto { Username = registerRequestDto.Username, Role = GetUserRoles(users), Token = _projectOptions.TokenUnavailable };
                    response.IsSuccess = true;
                    response.Code = HttpStatusCode.Created;
                    response.Message = _projectOptions.Created;
                }
                else if (result == 0)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.BadRequest;
                    response.Code = HttpStatusCode.BadRequest;
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Register");
                response.IsSuccess = false;
                response.Message = ex!.InnerException?.Message != null ? ex.InnerException?.Message : ex.Message;
            }
            return response;

        }


        public async Task<ServiceResponse<GenTokens>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            EmployeeManagerDbContext? context = await contextFactory.CreateDbContextAsync();
            ServiceResponse<GenTokens> response = new();
            try
            {

                ClaimsPrincipal validatedToken = GetPrincipalFromToken(refreshTokenRequestDto.Token);

                if (validatedToken == null)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.InValidToken;
                    response.Code = HttpStatusCode.BadRequest;
                    return response;
                }
                long expireDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                DateTime expireDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                                             .AddSeconds(expireDateUnix);


                if (expireDate > DateTime.UtcNow)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.ValidToken;
                    return response;
                }
                var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                var storedRefershToken = await context.RefereshTokens.SingleOrDefaultAsync(x => x.Token ==
                SHA512Converter.GenerateSHA512String(refreshTokenRequestDto.RefreshToken!));
                if (storedRefershToken == null)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.FalseToken;
                    response.Code=HttpStatusCode.BadRequest;
                    return response;
                }
                if (DateTime.UtcNow > storedRefershToken.DateExpired)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.TokenExpired;
                    response.Code=HttpStatusCode.NotAcceptable;
                    return response;
                }
                if (storedRefershToken.IsRevoked)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.TokenRevoked;
                    response.Code=HttpStatusCode.Unauthorized;
                    return response;
                }
                if (storedRefershToken.IsUsed)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.TokenUsed;
                    response.Code=HttpStatusCode.Unauthorized;
                    return response;
                }
                if (storedRefershToken.JwtId != jti)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.TokenNotMatched;
                    response.Code=HttpStatusCode.NotAcceptable;
                    return response;
                }

                storedRefershToken.IsUsed = true;
                context.RefereshTokens.Update(storedRefershToken);
                await context.SaveChangesAsync();

                var userid = validatedToken.Claims.Single(x => x.Type == "Id").Value;
                //var user = await customersDbContext.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "Id").Value);ClaimTypes.NameIdentifier

                var user = await context.Users.FindAsync(Convert.ToInt32(userid));

                //Generate-AccessToken-jwt
                CreateJWT(user, context, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token);

                await RemovedUnusedRefeshToken(user, context);


                //Generate-RefreshToken
                //Log this output paramter referesh token in the immemory object
                CreateRefreshToken(user, token, context, out string createToken, out RefereshTokenModels refershToken);





                GenTokens genTokens = new()
                {
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = createToken,
                    Username = user.Username,
                    ValidTo = token.ValidTo,//DateTime.Now.Add(_projectOptions.TokenLifeTime),
                    ValidFrom = token.ValidFrom,
                    RefreshTokenExpireDate = refershToken.DateExpired,
                    Email = user.Email,
                    Role = GetUserRoles(user)

                };


                //set refereshtoken and token on cookie header response
                SetTokensInsideCookie(genTokens);

                response.Data = genTokens;
                response.IsSuccess = true;
                response.Message = "IsSuccess";
                response.Code = HttpStatusCode.Created;


            }
            catch (Exception ex)
            {

                string message = $"{ex}";
                Log.Error(message);
                response.IsSuccess = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }   
            return response;

        }



        public async Task<bool> UserAlreadyExists(string username, string email)
        {
            try
            {
                var context = await contextFactory.CreateDbContextAsync();
                return await context.Users.AnyAsync(u => u.Username!.ToLower() == username.Trim().ToLower()
                || u.Email!.ToLower() == email.ToLower());

            }
            catch (Exception ex)
            {
                Log.Error(ex,"UserAlreadyExists");
                return false;
            }
        }


        private void CreateJWT(Users? users, EmployeeManagerDbContext context, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token)
        {

            #region GenerateSecureSecret
            //public const string Issuer = "SecureApiByfcmbdigital";
            //public const string Audience = "SecureApiUser";

            //public const string Secret = "OFRC1j9aaR2BvADxNWlG2pmuD392UfQBZZLM1fuzDEzDlEpSsn+btrpJKd3FfY855OMA9oK4Mc8y48eYUrVUSw==";


            //Important note*******
            //The secret is a base64-encoded string, always make sure to use a secure long string so no one can guess it. ever!.
            //a very recommended approach to use is through the HMACSHA256() class, to generate such a secure secret, you can refer to the below function
            // you can run a small test by calling the GenerateSecureSecret() function to generate a random secure secret once, grab it, and use it as the secret above 
            // or you can save it into appsettings.json file and then load it from them, the choice is yours


            //public static string GenerateSecureSecret()
            //{
            //    var hmac = new HMACSHA256();
            //    return Convert.ToBase64String(hmac.Key);
            //}
            #endregion



            tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Convert.FromBase64String(_projectOptions.SecreteKey);  //convert to byte[] from base64

            ClaimsIdentity? claimsIdentity = new ClaimsIdentity(new[] {
               //endeavour not to use sensitive data  pwd

              // new Claim(ClaimTypes.NameIdentifier, users.Id.ToString()),
               new Claim("Id",users!.Id.ToString()),
               new Claim("IsBlocked", users.Blocked.ToString()),
                new Claim("IsActive", users.Active.ToString()),
               new Claim(JwtRegisteredClaimNames.Sub,users.Email ?? ""),
               new Claim(JwtRegisteredClaimNames.Email,users.Email ?? ""),
               new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), //to identify the refereshtoken id
               new Claim("UserName",users.Username ?? "")

           });

            foreach (Role? item in GetUserRole(users.Id, context))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, item.Name!));
            }



            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            //HmacSha256Signature  the bigger the number the longer the key character length

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                #region CustomeClaims
                Subject = claimsIdentity,
                #endregion


                #region GenericClaims
                Issuer = _projectOptions.ValidIssuer,
                Audience = _projectOptions.ValidAudiences?[0],
                Expires = DateTime.Now.Add(_projectOptions.TokenLifeTime),
                SigningCredentials = signingCredentials,  //this is atually used to sing the token and also to validate the token
                #endregion

            };
            token = tokenHandler.CreateToken(tokenDescriptor);

        }

        public void SetTokensInsideCookie(GenTokens genTokens)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Response.Cookies.Append("accessToken", genTokens.Token,
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                        HttpOnly = true,
                        IsEssential = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });

                context.Response.Cookies.Append("refreshToken", genTokens.RefreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = true,
                        IsEssential = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });
            }
        }
        private static void CreateRefreshToken(Users user, SecurityToken token, EmployeeManagerDbContext context, out string createToken, out RefereshTokenModels refershToken)
        {
           //createToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            createToken = RandomString(100);
            refershToken = new RefereshTokenModels()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UsersId = user.Id,
                DateCreated = DateTime.UtcNow,
                DateExpired = DateTime.UtcNow.AddMonths(3),
                Token = SHA512Converter.GenerateSHA512String(createToken)


            };

            //create RefreshToken and log it
            context.RefereshTokens.Add(refershToken);
            context.SaveChangesAsync();
        }

        private List<Role> GetUserRole(int UserId, EmployeeManagerDbContext context)
        {
            try
            {
                List<Role> rolesMasters = (from UM in context.Users
                                           join UR in context.Role on UM.Id equals UR.UsersId
                                           join RM in context.Role on UR.Id equals RM.Id
                                           where UM.Id == UserId
                                           select RM).ToList();
                return rolesMasters;
            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                Log.Error(message);

                return new List<Role>();
            }
        }

        private static async Task RemovedUnusedRefeshToken(Users user, EmployeeManagerDbContext context)
        {
            var refreshToken = context.RefereshTokens.Where(req => req.UsersId == user.Id && req.IsRevoked == false && req.IsUsed == false).FirstOrDefault();
            if (refreshToken != null)
            {

                context.RefereshTokens.Remove(refreshToken);
                await context.SaveChangesAsync();

            }

        }


        public List<string> GetUserRoles(Users user)
        {
            List<string> roles = new List<string>();
            foreach (var item in user.Roles)
            {
                roles.Add(item.Name.ToLower());
            }
            return roles;
        }


        //this validate token b4 using it to get referesh token
        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPrincipalFromToken");
                return null;
            }
        }


        //confirm the return type algorithm used to generate jwt
        public bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }


        private static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCjkDEFGH1JKLMNOPQYRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz" + Guid.NewGuid().ToString().ToUpper();
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(length)]).ToArray());
        }
    }
}
