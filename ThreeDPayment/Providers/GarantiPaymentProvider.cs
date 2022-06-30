using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ThreeDPayment.Requests;
using ThreeDPayment.Results;

namespace ThreeDPayment.Providers
{
    public class GarantiPaymentProvider : IPaymentProvider
    {
        private readonly HttpClient client;

        public GarantiPaymentProvider(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        public Task<PaymentGatewayResult> ThreeDGatewayRequest(PaymentGatewayRequest request)
        {
            try
            {
                //request.OrderNumber = request.OrderNumber.Substring(0, 14).Replace('-', '_');

                string terminalUserId = request.BankParameters["terminalUserId"];
                string terminalId = request.BankParameters["terminalId"];
                string terminalMerchantId = request.BankParameters["terminalMerchantId"];
                string terminalProvUserId = request.BankParameters["terminalProvUserId"];
                string terminalProvPassword = request.BankParameters["terminalProvPassword"];
                string storeKey = request.BankParameters["storeKey"];//garanti sanal pos ekranından üreteceğimiz güvenlik anahtarı
                string mode = request.BankParameters["mode"];//PROD | TEST
                string txntype = request.BankParameters["txntype"];

                var parameters = new Dictionary<string, object>();

                if (!request.CommonPaymentPage)
                {
                    parameters.Add("cardnumber", request.CardNumber);
                    parameters.Add("cardcvv2", request.CvvCode);//kart güvenlik kodu
                    parameters.Add("cardexpiredatemonth", request.ExpireMonth);//kart bitiş ay'ı
                    parameters.Add("cardexpiredateyear", request.ExpireYear);//kart bitiş yıl'ı
                }

                parameters.Add("secure3dsecuritylevel", request.BankParameters["secure3dsecuritylevel"]);//SMS onaylı ödeme modeli 3DPay olarak adlandırılıyor.
                parameters.Add("mode", mode);
                parameters.Add("apiversion", request.BankParameters["apiversion"]);
                parameters.Add("terminalprovuserid", terminalProvUserId);
                parameters.Add("terminaluserid", terminalUserId);
                parameters.Add("terminalmerchantid", terminalMerchantId);
                parameters.Add("terminalid", terminalId);
                parameters.Add("txntype", txntype);//direk satış
                //parameters.Add("txncurrencycode", request.BankParameters["CurrencyIsoCode"]);//TL ISO code | EURO 978 | Dolar 840
                parameters.Add("txncurrencycode", request.CurrencyIsoCode);//TL ISO code | EURO 978 | Dolar 840

                parameters.Add("lang", request.BankParameters["LanguageIsoCode"]);
                //parameters.Add("motoind", "N");
                parameters.Add("customeripaddress", request.CustomerIpAddress);
                parameters.Add("orderaddressname1", request.CardHolderName);
                parameters.Add("orderid", request.OrderNumber);//sipariş numarası

                //işlem başarılı da olsa başarısız da olsa callback sayfasına yönlendirerek kendi tarafımızda işlem sonucunu kontrol ediyoruz
                parameters.Add("successurl", request.CallbackUrl);//başarılı dönüş adresi
                parameters.Add("errorurl", request.CallbackUrl);//hatalı dönüş adresi

                //garanti bankasında tutar bilgisinde nokta, virgül gibi değerler istenmiyor. 1.10 TL'lik işlem 110 olarak gönderilmeli. Yani tutarı 100 ile çarpabiliriz.
                string amount = (request.TotalAmount * 100m).ToString("0.##", new CultureInfo("en-US"));//virgülden sonraki sıfırlara gerek yok
                parameters.Add("txnamount", amount);

                string installment = request.Installment.ToString();
                if (request.Installment < 2)
                    installment = string.Empty;//0 veya 1 olması durumunda taksit bilgisini boş gönderiyoruz

                parameters.Add("txninstallmentcount", installment);//taksit sayısı | boş tek çekim olur


                //garanti tarafından terminal numarasını 9 haneye tamamlamak için başına sıfır eklenmesi isteniyor.
                string _terminalid = string.Format("{0:000000000}", int.Parse(terminalId));

                string SecurityData = GetSHA1(terminalProvPassword + _terminalid).ToUpper();
                string hashData = GetSHA512(terminalId + request.OrderNumber + amount + request.CurrencyIsoCode +  request.CallbackUrl + request.CallbackUrl + txntype + installment + storeKey + SecurityData).ToUpper();

                parameters.Add("secure3dhash", hashData);



                return Task.FromResult(PaymentGatewayResult.Successed(parameters, request.BankParameters["gatewayUrl"]));
            }
            catch (Exception ex)
            {
                return Task.FromResult(PaymentGatewayResult.Failed(ex.ToString()));
            }
        }

        public async Task<VerifyGatewayResult> VerifyGateway(VerifyGatewayRequest request, PaymentGatewayRequest gatewayRequest, IFormCollection form)
        {
            if (form == null)
            {
                return  VerifyGatewayResult.Failed("Form verisi alınamadı.");
            }

            var mdStatus = form["mdstatus"].ToString();
            if (string.IsNullOrEmpty(mdStatus))
            {
                return VerifyGatewayResult.Failed(form["mderrormessage"], form["procreturncode"]);
            }

            var response = form["response"];
            //mdstatus 1,2,3 veya 4 olursa 3D doğrulama geçildi anlamına geliyor
            if (!mdStatusCodes.Contains(mdStatus))
            {
                return VerifyGatewayResult.Failed($"{response} - {form["mderrormessage"]}", form["procreturncode"]);
            }

            if (StringValues.IsNullOrEmpty(response) || !response.Equals("Approved"))
            {
                return VerifyGatewayResult.Failed($"{response} - {form["errmsg"]}", form["procreturncode"]);
            }

            //var hashBuilder = new StringBuilder();
            //hashBuilder.Append(request.BankParameters["terminalId"]);
            //hashBuilder.Append(form["oid"].FirstOrDefault());
            //hashBuilder.Append(form["authcode"].FirstOrDefault());
            //hashBuilder.Append(form["procreturncode"].FirstOrDefault());
            //hashBuilder.Append(form["response"].FirstOrDefault());
            //hashBuilder.Append(form["mdstatus"].FirstOrDefault());
            //hashBuilder.Append(form["cavv"].FirstOrDefault());
            //hashBuilder.Append(form["eci"].FirstOrDefault());
            //hashBuilder.Append(form["md"].FirstOrDefault());
            //hashBuilder.Append(form["rnd"].FirstOrDefault());
            //hashBuilder.Append(request.BankParameters["storeKey"]);

            //var hashData = GetSHA1(hashBuilder.ToString());

            var _terminalid = request.BankParameters["terminalId"];

            string SecurityData = GetSHA1(form["terminalProvPassword"].FirstOrDefault() + _terminalid).ToUpper();
            //string hashData = GetSHA1(_terminalid + request.OrderNumber + amount + request.CallbackUrl + request.CallbackUrl + txntype + installment + storeKey + SecurityData).ToUpper();

            //string hashData = GetSHA1(
            //    _terminalid
            //    + form["orderid"].FirstOrDefault()
            //    + form["amount"].FirstOrDefault()
            //    + form["CurrencyIsoCode"].FirstOrDefault()
            //    + form["CallbackUrl"].FirstOrDefault()
            //    + form["CallbackUrl"].FirstOrDefault()
            //    + form["txntype"].FirstOrDefault()
            //    + form["installment"].FirstOrDefault()
            //    + form["storeKey"].FirstOrDefault()
            //    + SecurityData
            //    ).ToUpper();

            //if (!form["HASH"].Equals(hashData))
            //{
            //    return VerifyGatewayResult.Failed("Güvenlik imza doğrulaması geçersiz.");
            //}

            int.TryParse(form["txninstallmentcount"], out int installment);

            return VerifyGatewayResult.Successed(form["transid"], form["hostrefnum"], installment, 0, response, form["procreturncode"], form["campaignchooselink"]);
        }

        public async Task<CancelPaymentResult> CancelRequest(CancelPaymentRequest request)
        {
            string terminalUserId = request.BankParameters["terminalUserId"];
            string terminalId = request.BankParameters["terminalId"];
            string terminalMerchantId = request.BankParameters["terminalMerchantId"];
            string cancelUserId = request.BankParameters["cancelUserId"];
            string cancelUserPassword = request.BankParameters["cancelUserPassword"];
            string mode = request.BankParameters["mode"];//PROD | TEST

            //garanti tarafından terminal numarasını 9 haneye tamamlamak için başına sıfır eklenmesi isteniyor.
            string _terminalid = string.Format("{0:000000000}", int.Parse(terminalId));

            //garanti bankasında tutar bilgisinde nokta, virgül gibi değerler istenmiyor. 1.10 TL'lik işlem 110 olarak gönderilmeli. Yani tutarı 100 ile çarpabiliriz.
            string amount = (request.TotalAmount * 100m).ToString("N");//virgülden sonraki sıfırlara gerek yok

            //provizyon şifresi ve 9 haneli terminal numarasının birleşimi ile bir hash oluşturuluyor
            string securityData = GetSHA1($"{cancelUserPassword}{_terminalid}");

            //ilgili veriler birleştirilip hash oluşturuluyor
            string hashstr = GetSHA1($"{request.OrderNumber}{terminalId}{amount}{securityData}");

            string requestXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                                        <GVPSRequest>
                                            <Mode>{mode}</Mode>
                                            <Version>v0.01</Version>
                                            <ChannelCode></ChannelCode>
                                            <Terminal>
                                                <ProvUserID>{cancelUserId}</ProvUserID>
                                                <HashData>{hashstr}</HashData>
                                                <UserID>{terminalUserId}</UserID>
                                                <ID>{terminalId}</ID>
                                                <MerchantID>{terminalMerchantId}</MerchantID>
                                            </Terminal>
                                            <Customer>
                                                <IPAddress>{request.CustomerIpAddress}</IPAddress>
                                                <EmailAddress></EmailAddress>
                                            </Customer>
                                            <Order>
                                                <OrderID>{request.OrderNumber}</OrderID>
                                                <GroupID></GroupID>
                                            </Order>
                                            <Transaction>
                                                <Type>void</Type>
                                                <InstallmentCnt>{request.Installment}</InstallmentCnt>
                                                <Amount>{amount}</Amount>
                                                <CurrencyCode>{request.CurrencyIsoCode}</CurrencyCode>
                                                <CardholderPresentCode>0</CardholderPresentCode>
                                                <MotoInd>N</MotoInd>
                                                <OriginalRetrefNum>{request.ReferenceNumber}</OriginalRetrefNum>
                                            </Transaction>
                                        </GVPSRequest>";

            var response = await client.PostAsync(request.BankParameters["verifyUrl"], new StringContent(requestXml, Encoding.UTF8, "text/xml"));
            string responseContent = await response.Content.ReadAsStringAsync();

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(responseContent);

            if (xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ReasonCode") == null ||
                xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ReasonCode").InnerText != "00" ||
                xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ReasonCode").InnerText != "0000")
            {
                string errorMessage = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ErrorMsg")?.InnerText ?? string.Empty;
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "Bankadan hata mesajı alınamadı.";

                return CancelPaymentResult.Failed(errorMessage);
            }

            string transactionId = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/RetrefNum")?.InnerText;
            return CancelPaymentResult.Successed(transactionId, transactionId);
        }

        public async Task<RefundPaymentResult> RefundRequest(RefundPaymentRequest request)
        {
            string terminalUserId = request.BankParameters["terminalUserId"];
            string terminalId = request.BankParameters["terminalId"];
            string terminalMerchantId = request.BankParameters["terminalMerchantId"];
            string refundUserId = request.BankParameters["refundUserId"];
            string refundUserPassword = request.BankParameters["refundUserPassword"];
            string mode = request.BankParameters["mode"];//PROD | TEST

            //garanti terminal numarasını 9 haneye tamamlamak için başına sıfır eklenmesini istiyor.
            string _terminalid = string.Format("{0:000000000}", int.Parse(terminalId));

            //garanti tutar bilgisinde nokta, virgül gibi değerler istenmiyor. 1.10 TL'lik işlem 110 olarak gönderilmeli. Yani tutarı 100 ile çarpabiliriz.
            string amount = (request.TotalAmount * 100m).ToString("N");//virgülden sonraki sıfırlara gerek yok

            //provizyon şifresi ve 9 haneli terminal numarasının birleşimi ile bir hash oluşturuluyor
            string securityData = GetSHA1($"{refundUserPassword}{_terminalid}");

            //ilgili veriler birleştirilip hash oluşturuluyor
            string hashstr = GetSHA1($"{request.OrderNumber}{terminalId}{amount}{securityData}");

            string requestXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                                        <GVPSRequest>
                                            <Mode>{mode}</Mode>
                                            <Version>v0.01</Version>
                                            <ChannelCode></ChannelCode>
                                            <Terminal>
                                                <ProvUserID>{refundUserId}</ProvUserID>
                                                <HashData>{hashstr}</HashData>
                                                <UserID>{terminalUserId}</UserID>
                                                <ID>{terminalId}</ID>
                                                <MerchantID>{terminalMerchantId}</MerchantID>
                                            </Terminal>
                                            <Customer>
                                                <IPAddress>{request.CustomerIpAddress}</IPAddress>
                                                <EmailAddress></EmailAddress>
                                            </Customer>
                                            <Order>
                                                <OrderID>{request.OrderNumber}</OrderID>
                                                <GroupID></GroupID>
                                            </Order>
                                            <Transaction>
                                                <Type>refund</Type>
                                                <InstallmentCnt>{request.Installment}</InstallmentCnt>
                                                <Amount>{amount}</Amount>
                                                <CurrencyCode>{request.CurrencyIsoCode}</CurrencyCode>
                                                <CardholderPresentCode>0</CardholderPresentCode>
                                                <MotoInd>N</MotoInd>
                                                <OriginalRetrefNum>{request.ReferenceNumber}</OriginalRetrefNum>
                                            </Transaction>
                                        </GVPSRequest>";

            var response = await client.PostAsync(request.BankParameters["verifyUrl"], new StringContent(requestXml, Encoding.UTF8, "text/xml"));
            string responseContent = await response.Content.ReadAsStringAsync();

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(responseContent);

            if (xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ReasonCode") == null ||
                xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ReasonCode").InnerText != "00" ||
                xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ReasonCode").InnerText != "0000")
            {
                string errorMessage = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ErrorMsg")?.InnerText ?? string.Empty;
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "Bankadan hata mesajı alınamadı.";

                return RefundPaymentResult.Failed(errorMessage);
            }

            string transactionId = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/RetrefNum")?.InnerText;
            return RefundPaymentResult.Successed(transactionId, transactionId);
        }

        public async Task<PaymentDetailResult> PaymentDetailRequest(PaymentDetailRequest request)
        {
            string terminalUserId = request.BankParameters["terminalUserId"];
            string terminalId = request.BankParameters["terminalId"];
            string terminalMerchantId = request.BankParameters["terminalMerchantId"];
            string terminalProvUserId = request.BankParameters["terminalProvUserId"];
            string terminalProvPassword = request.BankParameters["terminalProvPassword"];
            string mode = request.BankParameters["mode"];//PROD | TEST

            //garanti terminal numarasını 9 haneye tamamlamak için başına sıfır eklenmesini istiyor.
            string _terminalid = string.Format("{0:000000000}", int.Parse(terminalId));

            //provizyon şifresi ve 9 haneli terminal numarasının birleşimi ile bir hash oluşturuluyor
            string securityData = GetSHA1($"{terminalProvPassword}{_terminalid}");

            string amount = "100";//sabit 100 gönderin dediler. Yani 1 TL.

            //ilgili veriler birleştirilip hash oluşturuluyor
            string hashstr = GetSHA1($"{request.OrderNumber}{terminalId}{amount}{securityData}");

            string requestXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                                        <GVPSRequest>
                                           <Mode>{mode}</Mode>
                                           <Version>v0.01</Version>
                                           <ChannelCode />
                                           <Terminal>
                                              <ProvUserID>{terminalProvUserId}</ProvUserID>
                                              <HashData>{hashstr}</HashData>
                                              <UserID>{terminalUserId}</UserID>
                                              <ID>{terminalId}</ID>
                                              <MerchantID>{terminalMerchantId}</MerchantID>
                                           </Terminal>
                                           <Customer>
                                              <IPAddress>{request.CustomerIpAddress}</IPAddress>
                                              <EmailAddress></EmailAddress>
                                           </Customer>
                                           <Card>
                                              <Number />
                                              <ExpireDate />
                                              <CVV2 />
                                           </Card>
                                           <Order>
                                              <OrderID>{request.OrderNumber}</OrderID>
                                              <GroupID />
                                           </Order>
                                           <Transaction>
                                              <Type>orderinq</Type>
                                              <InstallmentCnt />
                                              <Amount>{amount}</Amount>
                                              <CurrencyCode>{request.CurrencyIsoCode}</CurrencyCode>
                                              <CardholderPresentCode>0</CardholderPresentCode>
                                              <MotoInd>N</MotoInd>
                                           </Transaction>
                                        </GVPSRequest>";

            var response = await client.PostAsync(request.BankParameters["verifyUrl"], new StringContent(requestXml, Encoding.UTF8, "text/xml"));
            string responseContent = await response.Content.ReadAsStringAsync();

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(responseContent);

            string finalStatus = xmlDocument.SelectSingleNode("GVPSResponse/Order/OrderInqResult/Status")?.InnerText ?? string.Empty;
            string transactionId = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/RetrefNum")?.InnerText;
            string referenceNumber = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/RetrefNum")?.InnerText;
            string cardPrefix = xmlDocument.SelectSingleNode("GVPSResponse/Order/OrderInqResult/CardNumberMasked")?.InnerText;
            string installment = xmlDocument.SelectSingleNode("GVPSResponse/Order/OrderInqResult/InstallmentCnt")?.InnerText ?? "0";
            string bankMessage = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/Message")?.InnerText;
            string responseCode = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ReasonCode")?.InnerText;

            if (finalStatus.Equals("APPROVED", StringComparison.OrdinalIgnoreCase))
            {
                return PaymentDetailResult.PaidResult(transactionId, referenceNumber, cardPrefix, int.Parse(installment), 0, bankMessage, responseCode);
            }
            else if (finalStatus.Equals("VOID", StringComparison.OrdinalIgnoreCase))
            {
                return PaymentDetailResult.CanceledResult(transactionId, referenceNumber, bankMessage, responseCode);
            }
            else if (finalStatus.Equals("REFUNDED", StringComparison.OrdinalIgnoreCase))
            {
                return PaymentDetailResult.RefundedResult(transactionId, referenceNumber, bankMessage, responseCode);
            }

            var bankErrorMessage = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/SysErrMsg")?.InnerText ?? string.Empty;
            var errorMessage = xmlDocument.SelectSingleNode("GVPSResponse/Transaction/Response/ErrorMsg")?.InnerText ?? string.Empty;
            if (string.IsNullOrEmpty(errorMessage))
                errorMessage = "Bankadan hata mesajı alınamadı.";

            return PaymentDetailResult.FailedResult(bankErrorMessage, responseCode, errorMessage);
        }

        private string OldGetSHA1(string text)
        {
            var cryptoServiceProvider = new SHA1CryptoServiceProvider();
            var inputbytes = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(text));

            var builder = new StringBuilder();
            for (int i = 0; i < inputbytes.Length; i++)
            {
                builder.Append(string.Format("{0,2:x}", inputbytes[i]).Replace(" ", "0"));
            }

            return builder.ToString().ToUpper();
        }


        //sha512
        public static string GetSHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        public string GetSHA1(string SHA1Data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = SHA1Data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            return GetHexaDecimal(inputbytes);
        }


        public string GetHexaDecimal(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;
            for (int n = 0; n <= length - 1; n++)
            {
                s.Append(String.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
            }
            return s.ToString();
        }

        public Dictionary<string, string> TestParameters => new Dictionary<string, string>
        {
            { "terminalUserId", "1" },
            { "terminalId", "1" },
            { "terminalMerchantId", "1" },
            { "terminalProvUserId", "1" },
            { "terminalProvPassword", "1" },
            { "storeKey", "1" },
            { "mode", "TEST" },
            { "gatewayUrl", "https://sanalposprovtest.garanti.com.tr/servlet/gt3dengine" },
            { "verifyUrl", "https://sanalposprovtest.garanti.com.tr/servlet/gt3dengine" }
        };

        private static readonly string[] mdStatusCodes = new[] { "1", "2", "3", "4" };
    }
}