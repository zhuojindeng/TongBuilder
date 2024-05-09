using SiweiSoft.Application;
using System.Text;
using TongBuilder.SSOServer.Models;

namespace TongBuilder.SSOServer.Services
{
    /// <summary>
    /// 短信相关服务类
    /// </summary>
    public class SmsService
    {
        private readonly IConfiguration _configuration;
        private static Dictionary<string, SmsCodeRecord>? _smsCodeRecords;
        private SmsServiceConfig _smsServiceConfig;

        /// <summary>
        /// 获取记录中所有手机号和对应的短信验证码
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, SmsCodeRecord> GetCodeRecords()
        {
            if (_smsCodeRecords == null)
            {
                _smsCodeRecords = new Dictionary<string, SmsCodeRecord>();
            }
            return _smsCodeRecords;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smsServiceConfig = new SmsServiceConfig()
            {
                AccessKeyId = _configuration.GetValue<string>("AccessKeyId"),
                AccessKeySecret = _configuration.GetValue<string>("AccessKeySecret"),
                MessageSignature = _configuration.GetValue<string>("MessageSignature"),
                MessageTemplateId = _configuration.GetValue<string>("MessageTemplateId")
            };
        }

        /// <summary>
        /// 给特定号码发送一个新的短信验证码，返回验证码
        /// </summary>
        /// <param name="phoneNmuber"></param>
        /// <returns></returns>
        public OperationResult<string> SendNewCode(string phoneNmuber)
        {           
            ClearExpiredCode();
            var record = new SmsCodeRecord()
            {
                PhoneNumber = phoneNmuber,
                CreateTime = DateTime.Now,
                Code = GetRandomCode(6),
                ExpirationTime = DateTime.Now.AddMinutes(15)
            };

            if (GetCodeRecords().ContainsKey(phoneNmuber))
            {
                //检测一下是否请求太频繁，60秒可请求一次
                if (_smsCodeRecords[phoneNmuber].CreateTime.AddSeconds(60) > DateTime.Now)
                {                    
                    return OperationResult<string>.Failed("Failed", "获取验证码操作太频繁，请稍后再试");
                }
                _smsCodeRecords[phoneNmuber] = record;
            }
            else
            {
                _smsCodeRecords.Add(phoneNmuber, record);
            }

            if (SendCodeByMessage(phoneNmuber, record.Code))
            {
                
                return OperationResult<string>.Success(record.Code);
            }            
           
            return OperationResult<string>.Failed("Failed", "短信平台发送失败");

        }

        /// <summary>
        /// 调用平台API接口将code发送给指定手机号
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool SendCodeByMessage(string phoneNumber, string code)
        {
            AlibabaCloud.SDK.Dysmsapi20170525.Client client = CreateClient(_smsServiceConfig.AccessKeyId, _smsServiceConfig.AccessKeySecret);
            AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest sendSmsRequest = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest
            {
                SignName = _smsServiceConfig.MessageSignature,
                TemplateCode = _smsServiceConfig.MessageTemplateId,
                PhoneNumbers = phoneNumber,
                TemplateParam = $"{{\"code\":\"{code}\"}}",
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsResponse resp = client.SendSmsWithOptions(sendSmsRequest, runtime);
            return resp.Body.Code == "OK";
            
        }

        /// <summary>
        /// 获取特定位数的随机码，只含数字0-9
        /// </summary>
        /// <param name="size">随机码位数</param>
        /// <returns></returns>
        public string GetRandomCode(int size)
        {
            if (size <= 0) size = 6;
            var code = new StringBuilder(size);
            Random ran = new Random();

            for (int i = 0; i < size; i++)
            {
                code.Append(ran.Next(0, 9).ToString());
            }

            return code.ToString();
        }

        /// <summary>
        /// 清除已过期的验证码记录
        /// </summary>
        public void ClearExpiredCode()
        {
            if (GetCodeRecords().Count != 0)
            {
                foreach (var item in GetCodeRecords())
                {
                    if (item.Value.ExpirationTime < DateTime.Now)
                    {
                        _smsCodeRecords.Remove(item.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 验证手机和验证码是否正确
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <param name="type">4=首次认证，5=选择租户时认证</param>
        /// <returns></returns>
        public static bool VerifyCode(string phoneNumber, string code, string type)
        {
            if (GetCodeRecords().ContainsKey(phoneNumber))
            {
                var record = _smsCodeRecords[phoneNumber];
                if (record.ExpirationTime >= DateTime.Now && record.Code == code && ((record.IsVerified == 0 && type == "4") || (record.IsVerified == 1 && type == "5")))
                {
                    //_smsCodeRecords.Remove(phoneNumber);
                    _smsCodeRecords[phoneNumber].IsVerified = type == "4" ? 1 : 2;
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 使用AK SK初始化账号Client
        /// </summary>
        /// <param name="accessKeyId">应用ID号</param>
        /// <param name="accessKeySecret">应用秘钥</param>
        /// <returns></returns>
        public static AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = accessKeyId,
                AccessKeySecret = accessKeySecret,
            };
            // 访问的域名
            config.Endpoint = "dysmsapi.aliyuncs.com";
            return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
        }

        /// <summary>
        /// 初始化账号Client
        /// 阿里官方推进通过STS鉴权方式进一步提高秘钥安全性
        /// 请参见：https://help.aliyun.com/document_detail/378671.html
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="accessKeySecret"></param>
        /// <param name="securityToken"></param>
        /// <returns></returns>
        public static AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClientWithSTS(string accessKeyId, string accessKeySecret, string securityToken)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 必填，您的 AccessKey ID
                AccessKeyId = accessKeyId,
                // 必填，您的 AccessKey Secret
                AccessKeySecret = accessKeySecret,
                // 必填，您的 Security Token
                SecurityToken = securityToken,
                // 必填，表明使用 STS 方式
                Type = "sts",
            };
            // 访问的域名
            config.Endpoint = "dysmsapi.aliyuncs.com";
            return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
        }
        

        /// <summary>
        /// 测试使用
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, SmsCodeRecord> GetRecordsForTest()
        {
            return _smsCodeRecords;
        }

    }
}
