MvcBlanket
==========

ASP.NET MVC compliant captcha generation library.

Features
--------

The following features are supported:

* Fully customizable rendering engine
* AJAX-enabled captcha image refresh
* Integrates into ASP.NET MVC workflow by providing the special CaptchaResult action result and Captcha controller's extension method.
* Provides support of entered captcha code validation using declarative approach by ValidateCaptcha validation attribute

Using
--------

###Generating captcha image

Write the following code to the controller's action method:

		public ActionResult GetCaptcha()
		{
			return this.Captcha();
		}

		
###Validating captcha code

Write following code to the model you use:

		[ValidateCaptcha]
		public string Captcha {get;set;}
		
The validation then runs as usual and you can check is the model valid by accessing `ModelState.IsValid` property in your action method.

###Rendering to the HTML

Write the following code to the view:

		@Html.CaptchaFor(m => m.Captcha, "GetCaptcha", "Is captcha unintelligible? Reload new one.")
		@Html.ValidationMessageFor(m => m.Captcha)

Copy `captcha.js` file to your web application project. 
Add reference to the javascript like this:

		<script type="text/javascript" src="@Url.Content("~/Scripts/captcha.js")"></script>

###Customization

You can customize rendering by setting a number of properies of CaptchaImage instance.
Also you can place a number of captches onto the single page by providing unique captcha identifiers to `Captcha` method and `ValidateCaptcha` attribute.