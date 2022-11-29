using AdvansioInteractive.Service.Internal.Lendng.Data;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;
using AdvansioInteractive.Service.Internal.Lendng.Interfaces;
using AdvansioInteractive.Service.Internal.Lendng.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdvansioInteractive.Service.Internal.Lendng.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly MongoDbConfig _mongoConfig;
        private readonly IMongoCollection<Wallet> _walletCollection;
        private readonly ILogger _logger;
        private readonly JwtSettings _jwtSettings;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<MongoDbConfig> mongoConfig, IDbClient dbClient, ILogger<AccountService> logger, JwtSettings jwtSettings)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _mongoConfig = mongoConfig.Value ?? throw new ArgumentNullException(nameof(mongoConfig));
            _walletCollection = dbClient.GetWalletCollection();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }
        public async Task<GenericResponse<LoginResponse>> Login(LoginDto request)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if(userExists is null)
                {
                    _logger.LogInformation($"{nameof(Login)} LOGIN Failed because user is invalid...");
                    return new GenericResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "Failed. Invalid user.",
                        Success = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userExists.Id.ToString()),
                    new Claim(ClaimTypes.Name, userExists.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userExists.Id.ToString()),

                };
                var roles = await _userManager.GetRolesAsync(userExists);
                var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
                claims.AddRange(roleClaims);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var nbf = DateTime.Now;
                var expires = nbf.AddMinutes(10);
                var tokenObj = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    notBefore: nbf,
                    expires: expires,
                    signingCredentials: credential
                    );
                var token = new JwtSecurityTokenHandler().WriteToken(tokenObj);
                return new GenericResponse<LoginResponse>
                {
                    Data = new LoginResponse { AccessToken = token, Email = userExists?.Email, UserId = userExists.Id.ToString(), },
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Login successful"
                };


            }
            catch (Exception ex)
            {

                _logger.LogError($"{nameof(Login)} ERROR OCCURRED => {ex.Message } INNER EXCEPTION => {ex.InnerException} STACKTRACE => {ex.StackTrace}");
                Console.WriteLine(ex);
                return new GenericResponse<LoginResponse>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                };
            }
        }

        public async Task<GenericResponse<RegisterResponse>> Register(RegisterDto request)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if(userExists is not null)
                {
                    return new GenericResponse<RegisterResponse>
                    {
                        Data = null,
                        Message = "Failed. User already exists.",
                        Success = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }


                userExists = new ApplicationUser()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.Username ?? request.Email,
                    
                };
                var createUserResult = await _userManager.CreateAsync(userExists, request.Password);
                if (!createUserResult.Succeeded)
                {
                    //failed.
                    return new GenericResponse<RegisterResponse>
                    {
                        Data = null,
                        Message = $"Failed. User creation failed with errors {createUserResult.Errors.First()?.Description} .",
                        Success = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }

                //add newly created user to role...
                var addUserToRoleResult = await _userManager.AddToRoleAsync(userExists, _mongoConfig.DefaultRole);
                if (!addUserToRoleResult.Succeeded)
                {
                    //failed.
                    return new GenericResponse<RegisterResponse>
                    {
                        Data = null,
                        Message = $"Failed. User creation successful BUT role creation failed with errors {addUserToRoleResult.Errors.First()?.Description} .",
                        Success = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }

                //now add 
                decimal.TryParse(_mongoConfig.FirstTimeWalletBonus, out var firstTimeWalletBonus);
                var walletForNewUser = new Wallet
                {
                    IsActive = true,
                    UserId = userExists.Id,
                    Balance = firstTimeWalletBonus,
                    WalletCurrency = _mongoConfig.DefaultCurrency,
                    TotalCummulativeTransactions = firstTimeWalletBonus,    
                    WalletId = await GenerateUniqueWalletId(),
                    WalletHolderName = string.Format("{0} {1}", userExists?.FirstName, userExists?.LastName),
                    WalletHolderEmail = userExists?.Email ?? _mongoConfig.DefaultEmail,
                };
                await _walletCollection.InsertOneAsync(walletForNewUser);

                return new GenericResponse<RegisterResponse>
                {
                    Data = null,
                    Message = "Registration successful",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Success = true,
                };




            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Register)} ERROR OCCURRED => {ex.Message } INNER EXCEPTION => {ex.InnerException} STACKTRACE => {ex.StackTrace}");
                Console.WriteLine(ex);
                return new GenericResponse<RegisterResponse>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                };
            }
        }
        private async Task<string> GenerateUniqueWalletId()
        {
            var index = 0;
            var sb = new StringBuilder();
            sb.Append("LNDG00000");
            var finalWalletId = string.Empty;   
            var lastWalletID = (await _walletCollection.Find(x => x.IsActive).ToListAsync()).LastOrDefault()?.WalletId;
            if(lastWalletID is null)
            {
                index = 1;
                sb.Append(index.ToString());
                return sb.ToString();
            }
            
            var lastIndex = Convert.ToInt32(lastWalletID?.Split("LNDG00000")?.Last());
            index = lastIndex + 1;

            sb.Append(index.ToString());
            return sb.ToString();

        }
    }
}
