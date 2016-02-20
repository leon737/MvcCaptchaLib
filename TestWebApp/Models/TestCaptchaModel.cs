using CaptchaLib;

namespace TestWebApp.Models
{
    public class TestCaptchaModel
    {
        [ValidateCaptcha]
        public string Captcha { get; set; }
    }
}