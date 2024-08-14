using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client; // เพิ่มการนำเข้าไฟล์นี้
using sib_api_v3_sdk.Model;
using System.Collections.Generic; // เพิ่มการนำเข้าไฟล์นี้
using System.Diagnostics;
using System.Threading.Tasks;

namespace bookshop.Myhelper
{
	public class EmailSender
	{
		private static IConfigurationRoot Configuration { get; set; }

		static EmailSender()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");

			Configuration = builder.Build();
		}

		public static async System.Threading.Tasks.Task SendEmail(string receiverEmail, string receiverName, string subject, string message)
		{
			var apiKey = Configuration["BrevoApi:Apikey"];
			var senderEmail = Configuration["BrevoApi:SenderEmail"];
			var senderName = Configuration["BrevoApi:SenderName"];

			// ตั้งค่าคีย์ API สำหรับการยืนยัน
			//Configuration.Default.ApiKey.Add("api-key", apiKey);

			var apiInstance = new TransactionalEmailsApi();
			SendSmtpEmailSender sender = new SendSmtpEmailSender(senderName, senderEmail);
			SendSmtpEmailTo receiver1 = new SendSmtpEmailTo(receiverEmail, receiverName);
			List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo> { receiver1 };
			string HtmlContent = null;
			string TextContent = message;

			try
			{
				var sendSmtpEmail = new SendSmtpEmail(sender, To, null, null, HtmlContent, TextContent, subject);
				CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
				Console.WriteLine("Brevo Response: " + result.ToJson());
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				Console.WriteLine(e.Message);
				throw; // โยนข้อผิดพลาดให้ผู้เรียกจัดการ
			}
		}
	}
}
