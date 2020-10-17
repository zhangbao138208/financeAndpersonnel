/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using System;
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
using EasySlideVerification.Model;
using EasySlideVerification;
using EasySlideVerification.Common;
using System.Drawing.Imaging;
using System.Linq;
using DncZeus.Api.Cache;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.Util;

namespace DncZeus.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    
    [ApiController]
    public class OauthController : ControllerBase
    {
        private readonly AppAuthenticationSettings _appSettings;
        private readonly DncZeusDbContext _dbContext;
        private readonly RSAHelper _rSaHelper;
        private readonly ISlideVerifyService _verifyService;
        
        
        public OauthController(
            IOptions<AppAuthenticationSettings> appSettings, 
            DncZeusDbContext dbContext,
            RSAHelper rSaHelper,
            ISlideVerifyService verifyService)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _rSaHelper = rSaHelper;
            _verifyService = verifyService;
        }
       
        /// <summary>
        /// 登录认证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Auth( AuthModel model)
        {
            var username = model.userName;
            var password = model.password;
            var response = ResponseModelFactory.CreateInstance;
            DncUser user;
            if (CacheManager.GetCache<string>(model.slideCookie??"")!="true")
            {
                response.SetFailed("验证失败或者验证已超时请刷新页面重新验证");
                return Ok(response);
            }
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

                var mapping = await _dbContext.DncUserRoleMapping.
                    Where(x => x.UserGuid == user.Guid).
                    Select(x=>x.RoleCode).
                    ToListAsync();
                var roles = string.Join(',', mapping);
                var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("guid",user.Guid.ToString()),
                    new Claim("avatar",""),
                    new Claim("displayName",user.DisplayName),
                    new Claim("loginName",user.LoginName),
                    new Claim("emailAddress",""),
                    new Claim("guid",user.Guid.ToString()),
                    new Claim("userType",((int)user.UserType).ToString()),
                    new Claim("roles",roles),
                });
                var token = JwtBearerAuthenticationExtension.GetJwtAccessToken(_appSettings, claimsIdentity);

                response.SetData(token);
                return Ok(response);
            }
            
           
        }

        /// <summary>
        /// 创建图片滑动数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/[controller]/[action]")]
        public ActionResult<SlideVerificationPlainInfo> GetVerification()
        {
            var data = _verifyService.Create();
            var result = ConvertToBase64PlainInfo(data);
            return result;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/[controller]/[action]")]
        public IActionResult Verify([FromBody] VerifyParam param)
        {
            var ret = _verifyService.Validate(param);
            var cookie = Guid.NewGuid().ToString("N");
            CacheManager.SetCache(cookie, ret,DateTimeOffset.Now.AddSeconds(30));
            return Ok(new { code=ret,cookie=cookie });
        }

        /// <summary>
        /// 图片由byte[]转换为base64字符串
        /// </summary>
        private SlideVerificationPlainInfo ConvertToBase64PlainInfo(SlideVerificationInfo data)
        {
            SlideVerificationPlainInfo result = SlideVerificationPlainInfo.From(data);
            if (result != null)
            {
                result.BackgroundImg = ImageUtil.ImageToBase64(data.BackgroundImg, ImageFormat.Jpeg);
                result.SlideImg = ImageUtil.ImageToBase64(data.SlideImg, ImageFormat.Png);
                //PositionX 不能输出到前端
                result.PositionX = 0;
            };

            return result;
        }
    }
}