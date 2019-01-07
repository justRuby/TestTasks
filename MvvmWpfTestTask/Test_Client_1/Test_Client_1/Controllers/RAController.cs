using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Test_Client_1.Controllers
{
    using Newtonsoft.Json;
    using System.Net.Sockets;
    using Test_Client_1.Models;

    public class RAController
    {
        private const string NAME = "token";
        private const string DOMAIN = @"https://localhost:44315/";

        private static RAController _instance;
        private static readonly object syncRoot = new Object();

        private RAController() { }

        public static RAController GetInstance()
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new RAController();
                }
            }
            return _instance;
        }

        public async Task<AnswerModel> CheckServer()
        {
            AnswerModel result = new AnswerModel();

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, App.UserToken));

                    HttpResponseMessage answer = null;
                    try
                    {
                        answer = await client.GetAsync(DOMAIN + "Alive");
                        answer.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException ex)
                    {
                        result.errorMessageList.Add(ex.Message);
                    }
                    catch(SocketException ex)
                    {
                        result.errorMessageList.Add(ex.Message);
                    }
                    catch (WebException ex)
                    {
                        WebExceptionStatus status = ex.Status;
                        if (status == WebExceptionStatus.ProtocolError)
                        {
                            HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                            result.errorMessageList.Add(string.Format("Статусный код ошибки: {0} - {1}", (int)httpResponse.StatusCode, httpResponse.StatusCode));
                        }
                    }

                    if (answer != null && answer.StatusCode == HttpStatusCode.OK)
                    {
                        result.Result = true;
                    }
                }
            }

            return await Task.FromResult(result);
        }

        public async Task<List<NoteModel>> GetNotesAsync()
        {
            string content = string.Empty;

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, App.UserToken));

                    var answer = await client.GetAsync(DOMAIN + "Get");

                    if (answer.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = answer.Content;
                        content = await con.ReadAsStringAsync();
                    }
                }
            }

            List<NoteModel> result = null;

            if (content != "")
                result = JsonConvert.DeserializeObject<List<NoteModel>>(content);

            return await Task.FromResult(result);
        }

        public async Task AddNoteAsync(NoteModel model)
        {
            string content = string.Empty;
            string json = JsonConvert.SerializeObject(model);

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, App.UserToken));

                    HttpResponseMessage result = await client.GetAsync(DOMAIN + "Add");

                    if (result != null && result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        content = await con.ReadAsStringAsync();

                        if (content.Equals("t"))
                        {
                            //res.Result = true;
                        }
                    }
                }
            }
        }

        public async Task EditNoteAsync(NoteModel model)
        {
            bool answer = false;
            string content = string.Empty;
            string json = JsonConvert.SerializeObject(model);

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, App.UserToken));

                    var result = await client.PostAsync(DOMAIN + "Edit", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        content = await con.ReadAsStringAsync();

                        if (content.Equals("t"))
                            answer = true;
                        else
                            answer = false;
                    }
                }
            }
        }

        public async Task DeleteNoteAsync(NoteModel model)
        {
            bool answer = false;
            string content = string.Empty;
            string json = JsonConvert.SerializeObject(model);

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent("f" + json);
            
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, App.UserToken));

                    var result = await client.PostAsync(DOMAIN + "Del", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        content = await con.ReadAsStringAsync();
                        answer = true;
                    }
                }
            }
        }

        public async Task DeleteNotesAsync(List<NoteModel> modelList)
        {
            bool answer = false;
            string content = string.Empty;
            string json = JsonConvert.SerializeObject(modelList);

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent("t" + json);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, App.UserToken));

                    var result = await client.PostAsync(DOMAIN + "Del", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        content = await con.ReadAsStringAsync();
                        answer = true;
                    }
                }
            }
        }

    }
}
