using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSale.Models.VNPAY;
using Microsoft.AspNetCore.Http;
using System.Configuration;
using WebSale.Models;
namespace WebSale.Service.VNPAY
{
    public class VnPayService :IVnPayService
    {

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContextBase context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();

            // URL callback from web.config
            var urlCallBack = ConfigurationManager.AppSettings["Vnpay_PaymentBackReturnUrl"];

            // Adding request data using values from ConfigurationManager
            pay.AddRequestData("vnp_Version", ConfigurationManager.AppSettings["Vnpay_Version"]);
            pay.AddRequestData("vnp_Command", ConfigurationManager.AppSettings["Vnpay_Command"]);
            pay.AddRequestData("vnp_TmnCode", ConfigurationManager.AppSettings["Vnpay_TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", ConfigurationManager.AppSettings["Vnpay_CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", ConfigurationManager.AppSettings["Vnpay_Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            // Generate the payment URL using the base URL and hash secret from web.config
            var paymentUrl = pay.CreateRequestUrl(ConfigurationManager.AppSettings["Vnpay_BaseUrl"], ConfigurationManager.AppSettings["Vnpay_HashSecret"]);

            return paymentUrl;
        }
        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, ConfigurationManager.AppSettings["Vnpay_HashSecret"]);

            return response;
        }
    }
}