using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebSale.Controllers
{
    public class VNpayController : Controller
    {
        // GET: VNpay
        public ActionResult RedirectToVNPay()
        {
            // Define payment details
            string vnp_Amount = (1000000 * 100).ToString(); // Convert to VNPay's required format
            string vnp_OrderInfo = HttpUtility.UrlEncode("Thanh toán đơn hàng VNPay");
            string vnp_ReturnUrl = HttpUtility.UrlEncode("http://localhost:44338/thanks");
            string vnp_TmnCode = "1K87LDMO";
            string vnp_TxnRef = DateTime.Now.Ticks.ToString(); // Unique transaction reference

            // Additional VNPay-required parameters
            string vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string vnp_IpAddr = "127.0.0.1";
            string vnp_CurrCode = "VND";
            string vnp_Locale = "vn";
            string vnp_Command = "pay";
            string vnp_Version = "2.0.0";

            // Retrieve the VNPay hash secret from web.config
            string vnp_HashSecret = ConfigurationManager.AppSettings["VNPay_HashSecret"];

            // Construct data string in VNPay’s required format
            string data = $"vnp_Amount={vnp_Amount}&vnp_Command={vnp_Command}&vnp_CreateDate={vnp_CreateDate}&vnp_CurrCode={vnp_CurrCode}&vnp_IpAddr={vnp_IpAddr}&vnp_Locale={vnp_Locale}&vnp_OrderInfo={vnp_OrderInfo}&vnp_ReturnUrl={vnp_ReturnUrl}&vnp_TmnCode={vnp_TmnCode}&vnp_TxnRef={vnp_TxnRef}&vnp_Version={vnp_Version}";

            // Generate HMAC SHA256 signature
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(vnp_HashSecret)))
            {
                signature = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
            }

            // Construct final payment URL with the signature
            string paymentUrl = $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?{data}&vnp_SecureHash={signature}";

            // Redirect to VNPay
            return Redirect(paymentUrl);
        }
    }
}