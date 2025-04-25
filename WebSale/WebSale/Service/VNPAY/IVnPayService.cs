using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSale.Models.VNPAY;
using WebSale.Models;
using System.Web;

namespace WebSale.Service.VNPAY
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContextBase context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
