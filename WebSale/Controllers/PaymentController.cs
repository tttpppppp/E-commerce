using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebSale.Models.VNPAY;
using WebSale.Service.VNPAY;

public class PaymentController : Controller
{
   


    private const string Endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
    private const string PartnerCode = "MOMOBKUN20180529";
    private const string AccessKey = "klm05TvNBzhg7h7j";
    private const string SecretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
    public async Task<ActionResult> OnlineCheckout()
    {
        string orderInfo = "Thanh toán qua MoMo";
        string amount = "10000";
        string orderId = new Random().Next(0, 9999).ToString(); 
        string redirectUrl = "https://localhost:44338/thanks"; 
        string ipnUrl = "https://localhost:44338/thanks"; 
        string extraData = "";

        string requestId = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
        string requestType = "payWithATM";

        string rawHash = $"accessKey={AccessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={PartnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

        string signature;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
        {
            signature = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash))).Replace("-", "").ToLower();
        }
        var data = new
        {
            partnerCode = PartnerCode,
            partnerName = "Test",
            storeId = "MomoTestStore",
            requestId = requestId,
            amount = amount,
            orderId = orderId,
            orderInfo = orderInfo,
            redirectUrl = redirectUrl,
            ipnUrl = ipnUrl,
            lang = "vi",
            extraData = extraData,
            requestType = requestType,
            signature = signature
        };
        using (var client = new HttpClient())
        {
            try
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Endpoint, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    return View("Error", new { message = "Error processing payment request." });
                }

                var jsonResult = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                return Redirect(jsonResult.payUrl.ToString());
            }
            catch (Exception ex)
            {
                return View("Error", new { message = ex.Message });
            }
        }
    }

    [HttpPost]
    public ActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
    {
        VnPayService vnPayService = new VnPayService();
        var url = vnPayService.CreatePaymentUrl(model, HttpContext);
        return Redirect(url);
    }
}
