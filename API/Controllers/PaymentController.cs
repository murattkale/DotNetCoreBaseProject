using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ThreeDPayment;
using ThreeDPayment.Requests;
using ThreeDPayment.Results;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class PaymentController : Controller
    {
        private const string PaymentSessionName = "PaymentInfo";
        private const string PaymentResultSessionName = "PaymentResult";

        private readonly IUnitOfWork<myDBContext> _uow;
        private readonly ICreditCardService _ICreditCardService;
        private readonly IBankPrefixService _IBankPrefixService;
        private readonly IBankService _bankService;
        private readonly IBankParameterService _IBankParameterService;
        private readonly IPaymentTransactionService _IPaymentTransactionService;
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public PaymentController(
            IUnitOfWork<myDBContext> _uow,
            IBankPrefixService _IBankPrefixService,
            ICreditCardService _ICreditCardService,
            IBankService bankService,
            IBankParameterService _IBankParameterService,
            IPaymentTransactionService _IPaymentTransactionService,
            IPaymentProviderFactory paymentProviderFactory)
        {
            this._uow = _uow;
            this._ICreditCardService = _ICreditCardService;
            this._IBankPrefixService = _IBankPrefixService;
            this._bankService = bankService;
            this._IBankParameterService = _IBankParameterService;
            this._IPaymentTransactionService = _IPaymentTransactionService;
            this._paymentProviderFactory = paymentProviderFactory;
        }



        [HttpPost("Index")]
        public IActionResult Index(PaymentViewModel model)
        {
            var res = new RModel<PaymentTransaction>();
            try
            {
                //gateway request
                PaymentGatewayRequest gatewayRequest = new PaymentGatewayRequest
                {
                    CardHolderName = model.CardHolderName,
                    //clear credit card unnecessary characters
                    CardNumber = model.CardNumber?.Replace(" ", string.Empty).Replace(" ", string.Empty),
                    //ExpireMonth = Convert.ToInt32(model.ExpireMonth) < 10 ? ("0" + model.ExpireMonth.ToString()) : model.ExpireMonth.ToString(),
                    ExpireMonth = model.ExpireMonth,
                    ExpireYear = model.ExpireYear,
                    CvvCode = model.CvvCode,
                    CardType = model.CardType,
                    Installment = model.Installment,
                    TotalAmount = Convert.ToDecimal(model.TotalAmount),
                    OrderNumber = model.OrderNumber,
                    CurrencyIsoCode = model.CurrencyIsoCode,
                    //LanguageIsoCode = "en",
                    CustomerIpAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                };

                //bank
                int bankid = model.BankId.ToInt();
                var bankRow = _bankService.Get(o => o.Id == bankid);
                var bank = bankRow.ResultRow;
                gatewayRequest.BankName = Enum.Parse<BankNames>(bank.SystemName);

                //bank parameters
                var bankParameters = _IBankParameterService.WhereList(o => o.BankId == bank.Id).ResultList;
                gatewayRequest.BankParameters = bankParameters.ToDictionary(key => key.Key, value => value.Value);

                //create payment transaction
                PaymentTransaction payment = new PaymentTransaction
                {
                    OrderNumber = gatewayRequest.OrderNumber,
                    UserIpAddress = gatewayRequest.CustomerIpAddress,
                    UserAgent = HttpContext.Request.Headers[HeaderNames.UserAgent],
                    BankId = model.BankId.Value,
                    CardPrefix = gatewayRequest.CardNumber.Substring(0, 6),
                    CardHolderName = gatewayRequest.CardHolderName,
                    Installment = model.Installment,
                    TotalAmount = Convert.ToDecimal(model.TotalAmount),
                    BankRequest = JsonConvert.SerializeObject(gatewayRequest)
                };

                //mark as created
                payment.MarkAsCreated();

                //insert payment transaction
                var resRow = _IPaymentTransactionService.InsertOrUpdate(payment);
                var resSave = _uow.SaveChanges();

                res.ResultRow = payment;
                res.Message = resSave.Message;
                res.RType = resSave.RType;


                return Ok(res);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                return Ok(new { errorMessage = "İşlem sırasında bir hata oluştu." });
            }
        }

        [HttpGet("Confirm")]
        public async Task<IActionResult> Confirm(string paymentId, string Callback)
        {
            if (paymentId == string.Empty)
            {
                VerifyGatewayResult failModel = VerifyGatewayResult.Failed("Ödeme bilgisi geçersiz.");
                return Ok(failModel);
            }

            //get transaction by identifier
            var paymentRow = _IPaymentTransactionService.Get(o => o.OrderNumber == paymentId);
            var payment = paymentRow.ResultRow;
            if (payment == null)
            {
                VerifyGatewayResult failModel = VerifyGatewayResult.Failed("Ödeme bilgisi geçersiz.");
                return Ok(failModel);
            }

            PaymentGatewayRequest bankRequest = JsonConvert.DeserializeObject<PaymentGatewayRequest>(payment.BankRequest);
            if (bankRequest == null)
            {
                VerifyGatewayResult failModel = VerifyGatewayResult.Failed("Ödeme bilgisi geçersiz.");
                return Ok(failModel);
            }

            if (!IPAddress.TryParse(bankRequest.CustomerIpAddress, out IPAddress ipAddress))
            {
                bankRequest.CustomerIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            if (bankRequest.CustomerIpAddress == "::1")
            {
                bankRequest.CustomerIpAddress = "127.0.0.1";
            }

            IPaymentProvider provider = _paymentProviderFactory.Create(bankRequest.BankName);

            //var uri = $"{Callback}{Url.RouteUrl("Callback", new { paymentId = payment.OrderNumber })}";

            //set callback url
            bankRequest.CallbackUrl = new Uri(Callback + $"Payment/Callback/{bankRequest.OrderNumber}");


            //gateway request
            PaymentGatewayResult gatewayResult = await provider.ThreeDGatewayRequest(bankRequest);

            //check result status
            if (!gatewayResult.Success)
            {
                VerifyGatewayResult failModel = VerifyGatewayResult.Failed(gatewayResult.ErrorMessage);
                return Ok(failModel);
            }

            //html content
            if (gatewayResult.HtmlContent)
            {
                return Ok(gatewayResult.HtmlFormContent);
            }

            //create form submit with parameters
            string model = _paymentProviderFactory.CreatePaymentFormHtml(gatewayResult.Parameters, gatewayResult.GatewayUrl);
            return Ok(model);
        }

        [HttpPost("GetInstallments")]
        public IActionResult GetInstallments([FromBody] InstallmentViewModel model)
        {
            RModel<InstallmentViewModel> res = new RModel<InstallmentViewModel>();
            //add cash option
            model.AddCashRate(model.TotalAmount);

            //get card prefix by prefix
            var creditCardRow = _ICreditCardService.GetCreditCardByPrefix(model.Prefix, true);
            var creditCard = creditCardRow.ResultRow;
            if (creditCard == null)
            {
                //get default bank
                //var defaultBankRow = _bankService.Get(o => o.DefaultBank);
                //var defaultBank = defaultBankRow.ResultRow;

                //if (defaultBank == null || !defaultBank.Active)
                //{
                //    res.Message = "Ödeme için aktif banka bulunamadı.";
                //    res.RType = RType.Error;

                //}

                //model.BankId = defaultBank.Id;
                //model.BankLogo = defaultBank.LogoPath;
                //model.BankName = defaultBank.Name;
                res.ResultRow = model;
                res.RType = RType.OK;

            }
            else
            {
                //get bank by identifier
                var bankRow = _bankService.Get(o => o.Id == creditCard.BankId);
                var bank = bankRow.ResultRow;

                //get default bank
                if (bank == null || !bank.Active)
                {
                    var bankRowItem = _bankService.Get(o => o.DefaultBank);
                    bank = bankRowItem.ResultRow;
                }
                else
                {
                    if (bank == null || !bank.Active)
                    {
                        res.Message = "Ödeme için aktif banka bulunamadı.";
                        res.RType = RType.Error;
                        return Ok(res);
                    }
                    else
                    {
                        //prepare installment model
                        foreach (CreditCardInstallment installment in creditCard.Installments)
                        {
                            decimal installmentAmount = model.TotalAmount;
                            decimal installmentTotalAmount = installmentAmount;

                            if (installment.InstallmentRate > 0)
                            {
                                installmentTotalAmount = Math.Round(model.TotalAmount + ((model.TotalAmount * installment.InstallmentRate) / 100), 2, MidpointRounding.AwayFromZero);
                            }

                            installmentAmount = Math.Round(installmentTotalAmount / installment.Installment, 2, MidpointRounding.AwayFromZero);

                            model.InstallmentRates.Add(new InstallmentViewModel.InstallmentRate
                            {
                                Text = $"{installment.Installment} Taksit",
                                Installment = installment.Installment,
                                Rate = installment.InstallmentRate,
                                Amount = installmentAmount.ToString("N2"),
                                AmountValue = installmentAmount,
                                TotalAmount = installmentTotalAmount.ToString("N2"),
                                TotalAmountValue = installmentTotalAmount
                            });
                        }

                        //set manufacturer card flag
                        model.BankId = bank.Id;
                        model.BankLogo = bank.LogoPath;
                        model.BankName = bank.Name;


                        res.ResultRow = model;
                        res.RType = RType.OK;

                    }


                }


            }

            //var prefix = model.Prefix.Trim();

            //res.ResultRow.BankPrefix = _IBankPrefixService.Where(o => o.Prefix.Equals(prefix)).Result.FirstOrDefault();

            return Ok(res);
        }

        [HttpPost("Callback")]
        //[IgnoreAntiforgeryToken]
        public async Task<IActionResult> Callback(IFormCollection form)
        {
            var res = new RModel<VerifyGatewayResult>();
            var paymentId = form["orderid"].ToStr();
            if (paymentId == string.Empty)
            {
                VerifyGatewayResult failModel = VerifyGatewayResult.Failed("Ödeme bilgisi geçersiz.");
                res.ResultRow = failModel;
                return Ok(res);
            }

            //get transaction by identifier
            var paymentRow = _IPaymentTransactionService.Get(o => o.OrderNumber == paymentId, false);
            var payment = paymentRow.ResultRow;

            if (payment == null)
            {
                VerifyGatewayResult failModel = VerifyGatewayResult.Failed("Ödeme bilgisi geçersiz.");
                res.ResultRow = failModel;
                return Ok(res);
            }

            PaymentGatewayRequest bankRequest = JsonConvert.DeserializeObject<PaymentGatewayRequest>(payment.BankRequest);
            if (bankRequest == null)
            {
                VerifyGatewayResult failModel = VerifyGatewayResult.Failed("Ödeme bilgisi geçersiz.");
                res.ResultRow = failModel;
                return Ok(res);
            }

            //create provider
            IPaymentProvider provider = _paymentProviderFactory.Create(bankRequest.BankName);
            VerifyGatewayRequest verifyRequest = new VerifyGatewayRequest
            {
                BankName = bankRequest.BankName,
                BankParameters = bankRequest.BankParameters
            };

            VerifyGatewayResult verifyResult = await provider.VerifyGateway(verifyRequest, bankRequest, form);
            verifyResult.OrderNumber = bankRequest.OrderNumber;

            //save bank response
            payment.BankResponse = JsonConvert.SerializeObject(new
            {
                verifyResult,
                parameters = form.Keys.ToDictionary(key => key, value => form[value].ToString())
            });

            payment.TransactionNumber = verifyResult.TransactionId;
            payment.ReferenceNumber = verifyResult.ReferenceNumber;
            payment.BankResponse = verifyResult.Message;

            if (verifyResult.Installment > 1)
            {
                payment.Installment = verifyResult.Installment;
            }

            if (verifyResult.ExtraInstallment > 1)
            {
                payment.ExtraInstallment = verifyResult.ExtraInstallment;
            }

            if (verifyResult.Success)
            {
                //mark as paid
                payment.MarkAsPaid(DateTime.Now);
                _IPaymentTransactionService.Update(payment);
                _uow.SaveChanges();

                res.ResultRow = verifyResult;
                return Ok(res);
            }

            //mark as not failed(it's mean error)
            payment.MarkAsFailed(verifyResult.ErrorMessage, $"{verifyResult.Message} - {verifyResult.ErrorCode}");

            //update payment transaction
            _IPaymentTransactionService.Update(payment);
            _uow.SaveChanges();

            res.ResultRow = verifyResult;
            return Ok(res);
        }

        [HttpGet("Completed")]
        public IActionResult Completed(string orderNumber)
        {
            var res = new RModel<CompletedViewModel>();
            //get order by order number
            var paymentRow = _IPaymentTransactionService.Get(o => o.OrderNumber == orderNumber, true, false, o => o.Bank);
            var payment = paymentRow.ResultRow;
            if (payment == null)
            {
                res.Message = "İşleminiz yarım kalmıştır lütfen yetkililere ulaşınız.";
                res.RType = RType.Error;
                return Ok(null);
            }

            //create completed view model
            CompletedViewModel model = new CompletedViewModel
            {
                OrderNumber = payment.OrderNumber,
                TransactionNumber = payment.TransactionNumber,
                ReferenceNumber = payment.ReferenceNumber,
                BankId = payment.BankId,
                BankName = payment.Bank?.Name,
                CardHolderName = payment.CardHolderName,
                Installment = payment.Installment,
                TotalAmount = payment.TotalAmount,
                PaidDate = payment.PaidDate,
                CreateDate = payment.CreateDate
            };
            res.ResultRow = model;
            return Ok(res);
        }
    }
}