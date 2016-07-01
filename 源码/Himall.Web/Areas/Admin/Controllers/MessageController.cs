using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Net.Mail; 
namespace Himall.Web.Areas.Admin.Controllers
{
	public class MessageController : BaseAdminController
	{
		public MessageController()
		{
		}

		public ActionResult Edit(string pluginId)
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) => {
				dynamic expandoObjects = new ExpandoObject();
				expandoObjects.name = item.PluginInfo.DisplayName;
				expandoObjects.pluginId = item.PluginInfo.PluginId;
				expandoObjects.enable = item.PluginInfo.Enable;
				expandoObjects.status = item.Biz.GetAllStatus();
				return expandoObjects;
			});
			ViewBag.messagePlugins = objs;
			ViewBag.Id = pluginId;
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			ViewBag.Name = plugin.PluginInfo.DisplayName;
			ViewBag.ShortName = plugin.Biz.ShortName;
			FormData formData = plugin.Biz.GetFormData();
			Plugin<ISMSPlugin> plugin1 = PluginsManagement.GetPlugins<ISMSPlugin>().FirstOrDefault<Plugin<ISMSPlugin>>();
			ViewBag.ShowSMS = false;
			ViewBag.ShowBuy = false;
			if (plugin1 != null && pluginId == plugin1.PluginInfo.PluginId)
			{
				ViewBag.ShowSMS = true;
				ViewBag.LoginLink = plugin1.Biz.GetLoginLink();
				ViewBag.BuyLink = plugin1.Biz.GetBuyLink();
				if (plugin1.Biz.IsSettingsValid)
				{
					ViewBag.Amount = plugin1.Biz.GetSMSAmount();
					ViewBag.ShowBuy = true;
				}
			}
			return View(formData);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Enable(string pluginId, MessageTypeEnum messageType, bool enable)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			if (!enable)
			{
				plugin.Biz.Disable(messageType);
			}
			else
			{
				plugin.Biz.Enable(messageType);
			}
			return Json(new { success = true });
		}

		public ActionResult Management()
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) => {
				dynamic expandoObjects = new ExpandoObject();
				expandoObjects.name = item.PluginInfo.DisplayName;
				expandoObjects.pluginId = item.PluginInfo.PluginId;
				expandoObjects.enable = item.PluginInfo.Enable;
				expandoObjects.status = item.Biz.GetAllStatus();
				return expandoObjects;
			});
			return View(objs);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Save(string pluginId, string values)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			IEnumerable<KeyValuePair<string, string>> keyValuePairs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(values);
			plugin.Biz.SetFormValues(keyValuePairs);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Send(string pluginId, string destination)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			if (string.IsNullOrEmpty(destination))
			{
				Result result = new Result()
				{
					success = false,
					msg = string.Concat("你填写的", plugin.Biz.ShortName, "不能为空！")
				};
				return Json(result);
			}
			if (!plugin.Biz.CheckDestination(destination))
			{
				Result result1 = new Result()
				{
					success = false,
					msg = string.Concat("你填写的", plugin.Biz.ShortName, "不正确")
				};
				return Json(result1);
			}
			string siteName = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().SiteName;
			//string str = plugin.Biz.SendTestMessage(destination, string.Concat("该条为测试信息，请勿回复!【", siteName, "】"), "这是一封测试邮件");
			string str= SendTestMessage(destination, string.Concat("该条为测试信息，请勿回复!【", siteName, "】"), "这是一封测试邮件");
           // string str = SendMail();
            if (str == "发送成功")
			{
				return Json(new { success = true });
			}
			Result result2 = new Result()
			{
				success = false,
				msg = str
			};
			return Json(result2);
		}
        public string SendTestMessage(string destination, string content, string title = "")
        {
            string str = "发送成功";
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(destination);

            /*  
            * msg.To.Add("b@b.com");  
            * msg.To.Add("b@b.com");  
            * msg.To.Add("b@b.com");可以发送给多人  
             */

            msg.From = new MailAddress("15950560518@163.com", "yinzhen", System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = "这是测试邮件";//邮件标题   
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码   
            msg.Body = "邮件内容";//邮件内容   
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码   
            msg.IsBodyHtml = false;//是否是HTML邮件   
            msg.Priority = MailPriority.High;//邮件优先级   

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("15950560518@163.com", "10201116yy");
            //在71info.com注册的邮箱和密码   
            client.Host = "smtp.163.com";
            client.Port = 110;
            object userState = msg;
            try
            {
               // client.SendAsync(msg, userState);
                client.Send(msg);   
                //  MessageBox.Show("发送成功");   
                str = "发送成功";
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                // MessageBox.Show(ex.Message, "发送邮件出错");  
                str = "发送邮件出错";
            }
            return str;
        }
      /* private string SendMail() 
       { 
          try 
          { 
           //邮件发送类 
           MailMessage mail = new MailMessage(); 
           //是谁发送的邮件 
           mail.From = new MailAddress("1446878469@qq.com"); 
           //发送给谁 
           mail.To.Add("yinzhen931020@163.com"); 
           //标题 
           mail.Subject = "Himall"; 
           mail.SubjectEncoding = System.Text.Encoding.UTF8; 
           //发送优先级 
           mail.Priority = MailPriority.High; 
           //邮件内容 
           mail.Body = "这是一封测试邮件";
           mail.BodyEncoding = System.Text.Encoding.UTF8; 
           //是否HTML形式发送 
           mail.IsBodyHtml = true; 
           //邮件服务器和端口 
           SmtpClient smtp = new SmtpClient("smtp.qq.com", 25); 
           smtp.UseDefaultCredentials = true; 
           //指定发送方式 
           smtp.DeliveryMethod = SmtpDeliveryMethod.Network; 
           //指定登录名和密码 
           smtp.Credentials = new System.Net.NetworkCredential("1446878469@qq.com", "10200610yz"); 
           //超时时间 
          // smtp.Timeout = 10000;
           
           smtp.Send(mail); 
           return "发送成功"; 
         } 
       catch(Exception exp) 
       { 
           return "发送邮件失败"; 
       } 
      }*/

	}
}