using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public interface IHttpClientWrapper
{
    RModel<T> Get<T>(string url);
    Task<RModel<T>> GetAsync<T>(string url);
    Task<RModel<T>> GetStringAsync<T>(string url);
    Task<RModel<T>> PostAsync<T>(string url, dynamic postModel);
    Task<RModel<T>> PostStringAsync<T>(string url, dynamic postModel);
    Task<RModel<T>> PostFormAsync<T>(string url, IFormCollection postModel);
    RModel<T> Post<T>(string url, dynamic postModel);

}
public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _client;
    public HttpClient Client => _client;
    IConfiguration _IConfiguration;
    string apiUrl = "";
    AppSettings appSettings;
    IBaseModel _IBaseModel;



    public HttpClientWrapper(HttpClient _client, IConfiguration _IConfiguration, IBaseModel _IBaseModel)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;

        this._client = new HttpClient(httpClientHandler);
        this._IConfiguration = _IConfiguration;

        var appSettingsSection = _IConfiguration.GetSection("AppSettings");
        appSettings = appSettingsSection.Get<AppSettings>();
        this._IBaseModel = _IBaseModel;

        this.apiUrl = appSettings.apiUrl;

    }


    void setConf()
    {
        appSettings.CreaUser = _IBaseModel?.CreaUser.ToStr();
        appSettings.LanguageId = _IBaseModel?.LanguageId.ToStr();
    }

    public async Task<RModel<T>> GetAsync<T>(string url)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            var response = await _client.GetAsync(this.apiUrl + url);
            string respnoseText = response.Content.ReadAsStringAsync().Result;

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.MessageListJson = response.RequestMessage.ToString();
                result.Message = respnoseText;
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }

    public async Task<RModel<T>> GetStringAsync<T>(string url)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            var response = await _client.GetAsync(this.apiUrl + url);
            string respnoseText = response.Content.ReadAsStringAsync().Result;

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.MessageListJson = response.RequestMessage.ToString();
                result.Message = respnoseText;
                result.RType = RType.Error;
            }
            else
            {
                result.Message = respnoseText;
            }
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }


    public async Task<RModel<T>> PostStringAsync<T>(string url, dynamic postModel)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postModel);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(this.apiUrl + url, httpContent);
            var respnoseText = await response.Content.ReadAsStringAsync();

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.MessageListJson = response.RequestMessage.ToString();
                result.Message = respnoseText;
                result.RType = RType.Error;
            }
            else
            {
                result.Message = respnoseText;
            }
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }



    public async Task<RModel<T>> PostAsync<T>(string url, dynamic postModel)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postModel);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(this.apiUrl + url, httpContent);
            var respnoseText = await response.Content.ReadAsStringAsync();

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.MessageListJson = response.RequestMessage.ToString();
                result.Message = respnoseText;
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }

    public async Task<RModel<T>> PostFormAsync<T>(string url, IFormCollection postModel)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));


            var form = postModel.Keys.ToDictionary(k => k, v => postModel[v].ToStr());
            var multipart = new MultipartFormDataContent();
            foreach (var item in form)
            {
                var httpContent = new StringContent(item.Value, Encoding.UTF8, "multipart/form-data");
                multipart.Add(httpContent, item.Key);
            }

            //var Content = postModel.ToString();
            //var fileContent = new MemoryStream(Encoding.ASCII.GetBytes(Content));

            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(postModel);
            //var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            //var multipart = new MultipartFormDataContent();
            //multipart.Add(httpContent,"form");



            var response = await _client.PostAsync(this.apiUrl + url, multipart);
            var respnoseText = await response.Content.ReadAsStringAsync();

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.MessageListJson = response.RequestMessage.ToString();
                result.Message = respnoseText;
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }

    public RModel<T> Post<T>(string url, dynamic postModel)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));


            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postModel);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _client.PostAsync(this.apiUrl + url, httpContent);
            var respnoseText = response.Result.Content.ReadAsStringAsync().Result;

            if (respnoseText == "" || response.Result.ReasonPhrase != "OK")
            {
                result.MessageListJson = response.Result.RequestMessage.ToString();
                result.Message = respnoseText;
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }


    public RModel<T> Get<T>(string url)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            var response = _client.GetAsync(this.apiUrl + url).Result;
            string respnoseText = response.Content.ReadAsStringAsync().Result;

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.MessageListJson = response.RequestMessage.ToString();
                result.Message = respnoseText;
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }



}

//public class JsonContent : StringContent
//{
//    public JsonContent(object obj) :
//        base(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json")
//    { }
//}



