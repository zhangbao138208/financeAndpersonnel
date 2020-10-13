/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using DncZeus.Api.Entities;
using DncZeus.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using DncZeus.Api.Auth;
using static DncZeus.Api.Entities.Enums.CommonEnum;
using DncZeus.Api.Models;
using DncZeus.Api.Utils.Encryption;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DncZeus.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OauthController : ControllerBase
    {
        private readonly AppAuthenticationSettings _appSettings;
        private readonly DncZeusDbContext _dbContext;
        private readonly RSAHelper _rSaHelper;
        public OauthController(
            IOptions<AppAuthenticationSettings> appSettings, 
            DncZeusDbContext dbContext,
            RSAHelper rSaHelper)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _rSaHelper = rSaHelper;
        }
       
        /// <summary>
        /// 登录认证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Auth( AuthModel model)
        {
            var username = model.userName;
            var password = model.password;
            var response = ResponseModelFactory.CreateInstance;
            DncUser user;
            //RSAHelper rSAHelper = new RSAHelper
            //       (RSAType.RSA, Encoding.UTF8, CeyhConfiguration.TheRSASetting.Private, CeyhConfiguration.TheRSASetting.Public);
            await using (_dbContext)
            {
                user = await _dbContext.DncUser.FirstOrDefaultAsync(x => x.LoginName == username.Trim());
                
                if (user == null || user.IsDeleted == IsDeleted.Yes)
                {
                    response.SetFailed("用户不存在");
                    return Ok(response);
                }
                var userP= _rSaHelper.Decrypt(user.Password);
                var modelP= _rSaHelper.Decrypt(password.Trim());
                //var s1 = rSAHelper.Decrypt(password.Trim());
                if (userP != modelP)
                {
                    response.SetFailed("密码不正确");
                    return Ok(response);
                }
                if (user.IsLocked == IsLocked.Locked)
                {
                    response.SetFailed("账号已被锁定");
                    return Ok(response);
                }
                if (user.Status == UserStatus.Forbidden)
                {
                    response.SetFailed("账号已被禁用");
                    return Ok(response);
                }
            }
            var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("guid",user.Guid.ToString()),
                    new Claim("avatar",""),
                    new Claim("displayName",user.DisplayName),
                    new Claim("loginName",user.LoginName),
                    new Claim("emailAddress",""),
                    new Claim("guid",user.Guid.ToString()),
                    new Claim("userType",((int)user.UserType).ToString())
                });
            var token = JwtBearerAuthenticationExtension.GetJwtAccessToken(_appSettings, claimsIdentity);

            response.SetData(token);
            return Ok(response);
        }
    }
}