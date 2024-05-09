using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Models;
using TongBuilder.SSOServer.Constants;
using TongBuilder.SSOServer.Models;
using TongBuilder.SSOServer.ViewModels;


namespace TongBuilder.SSOServer.Services
{
    public class UserLoginService
    {        
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private string _userServer = "";//读取配置的用户管理服务地址
        private string UserServer
        {
            get
            {
                if (string.IsNullOrEmpty(_userServer))
                {
                    _userServer = _configuration.GetValue<string>("UserService");
                }
                return _userServer;
            }
        }
        public UserLoginService(IAuthService authService, IConfiguration configuration) 
        {
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// 登录系统获取用户信息
        /// </summary>
        /// <param name="userLoginModel"></param>
        /// <returns></returns>
        public async Task<UserLoginResultModel> LoginAsync(UserLoginModel userLoginModel)
        {
            var result = await _authService.Login(userLoginModel);
            return result;
        }

        /// <summary>
        /// 验证登录
        /// LoginType=1 钉钉，=2 微信，=3 手机号+密码，=4 手机号+短信，=5 手机号+短信的选择租户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserLoginResultModel> CheckAccountAsync(LoginViewModel model)
        {
            UserLoginResultModel result = new UserLoginResultModel();
            if (model.LoginType == "3")//手机号+密码登录
            {
                //result = new UserLoginResultModel() { OpenId = "B46F7BA32EA7A0D9EF53FDCD3A3CE7FE", Tenants = new List<string>() { "上海思伟", "砼车信息" }, Succeeded = true };//测试写死
                result = await LoginAsync(new UserLoginModel()
                {
                    PhoneNumber = model.Username,
                    Password = model.Password
                });
                if (!result.Succeeded)
                {
                    //result.Description = string.Join(";", result.Errors.Select(e => e.Description).ToList());
                }
            }
            else if (model.LoginType == "4" || model.LoginType == "5")//手机号+短信验证码登录
            {
                //增加一步认证服务临时手机短信验证，可以在用户管理服务验证后，这步可以删除
                var verifyCodeResult = SmsService.VerifyCode(model.Username, model.Password, model.LoginType);
                if (verifyCodeResult)
                {
                    result = await LoginAsync(new UserLoginModel()
                    {
                        PhoneNumber = model.Username,
                        VerificationCode = model.Password,
                        LoginMode = "PhoneNumberWithSms"
                    });
                }
                else
                {
                    //result.Description = "短信验证码认证失败";
                }
            }
            else if (model.LoginType == "1")
            {
                //这里model.Username参数传的是unionid
                var userInfo = await GetLoginInfoAsync(model.Username, 2);
                result.Succeeded = userInfo.IsSuccess;
                //result.Tenants = userInfo.Tenants;
                result.OpenId = userInfo.OpenId;
            }
            else if (model.LoginType == "2")
            {
                //这里model.Username参数传的是unionid
                var userInfo = await GetLoginInfoAsync(model.Username, 3);
                result.Succeeded = userInfo.IsSuccess;
                //result.Tenants = userInfo.Tenants;
                result.OpenId = userInfo.OpenId;
            }
            else
            {
                result.Succeeded = false;
                //result.Description = "登录类型错误，此类型不存在";
                return result;
            }           
            return result;
        }

        /// <summary>
        /// 验证用户信息，验证类型type 1=openid，2=钉钉unionid，3=微信unionid，4=手机号（待补充完整，调用接口需要使用token），5=砼行APP手机号
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<UserInfoResult> GetLoginInfoAsync(string keyword, int type)
        {
            try
            {
                //测试token，待用户管理服务参数确定后改为根据用户openid和issuer服务器生成
                var token = Keys.Token;
                
                if (type > 5 || type < 1 || type == 4)
                {
                    //测试数据，待替换为手机验证码登录获取用户信息
                    return new UserInfoResult() { NickName = keyword, OpenId = "B46F7BA32EA7A0D9EF53FDCD3A3CE7FE", PhoneNumber = "15801112222", Tenants = new List<TenantModel>() { new TenantModel() { Code = "Siweisoft", Name = "上海思伟", IsDefault = true } }, IsSuccess = true };
                }
                else if (type == 2 || type == 3 || type == 5)
                {
                    using var client = new HttpClient();
                    //var thirdPartyName = type == 2 ? "dingtalk" : "wechat";
                    var thirdPartyName = "";
                    switch (type)
                    {
                        case 2:
                            thirdPartyName = "dingtalk";
                            break;
                        case 3:
                            thirdPartyName = "wechat";
                            break;
                        default:
                            thirdPartyName = "TongHangApp";
                            break;
                    }

                    _authService.SetToken(Keys.Token);
                    //通过平台unionid获取用户openid标识
                    var ret = await _authService.GetExternalUserAsync(thirdPartyName, keyword);
                    if (ret != null && ret.Succeeded)
                    {
                        var userOpenid = ret.Data.OpenId;
                        //通过用户openid标识获取其租户列表
                        var ret1= await _authService.GetTenantsByUserIdAsync(userOpenid);
                        if(ret1 != null && ret1.Succeeded)
                        {
                            return new UserInfoResult()
                            {
                                OpenId = userOpenid,
                                IsSuccess = true,
                                Tenants = ret1.Data.Select(t => new TenantModel() { Code = t.Code, Name = t.Name, IsDefault = t.IsDefault }).ToList()
                            };
                        }
                    }
                }
                else
                {
                    _authService.SetToken(Keys.Token);
                    //使用查询租户列表接口确认账号续存状态                    

                    var response = await _authService.GetTenantsByUserIdAsync(keyword);                                     

                    if (response!=null && response.Succeeded)
                    {
                        return new UserInfoResult()
                        {
                            OpenId = keyword,
                            IsSuccess = true
                        };
                    }                   
                    
                }
            }
            catch (Exception)
            {
                return new UserInfoResult()
                {
                    IsSuccess = false
                };

            }
            return new UserInfoResult()
            {
                IsSuccess = false
            };
        }

        /// <summary>
        /// 根据用户token获取用户基本信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<UserInfoResult> GetUserInfoAsync(string token)
        {
            string responseText = "";
            try
            {
                _authService.SetToken(Keys.Token);                         

                var result = await _authService.GetUserInfoAsync();

                if (result != null && result.Succeeded)
                {
                    return new UserInfoResult()
                    {
                        OpenId = result.Data.OpenId,
                        NickName = result.Data.NickName,
                        PhoneNumber = result.Data.PhoneNumber,
                        IsSuccess = true
                    };
                }                
                
            }
            catch (Exception ex)
            {
                //待写入日志
                return new UserInfoResult()
                {
                    IsSuccess = false,
                    Description = responseText
                };
            }
            return new UserInfoResult()
            {
                IsSuccess = false
            };
        }
    }
}
