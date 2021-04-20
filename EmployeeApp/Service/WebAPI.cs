using EmpClient.Utilities;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EmpClient.Service
{
    class WebAPI
    {

        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> GetEmployeesData(int pageId = 1)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string apiUrl = ConfigurationManager.AppSettings["BaseAPIURL"] + "?page=" + pageId.ToString();
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.GetAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Search employee details
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> SearchEmployeesData(int empId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string apiUrl = ConfigurationManager.AppSettings["BaseAPIURL"] + "/" + empId;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["AuthenticationToken"]);
                    var response = client.GetAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// creates a new employee
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> CreateEmployeeData<T>(T model) where T : class
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string apiUrl = ConfigurationManager.AppSettings["BaseAPIURL"];
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["AuthenticationToken"]);
                    var response = client.PostAsJsonAsync(apiUrl, model);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates employees details
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> UpdateEmployeeData<T>(int empId, T model) where T : class
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string apiUrl = ConfigurationManager.AppSettings["BaseAPIURL"] + "/" + empId;
                var jsonRequest = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["AuthenticationToken"]);
                    var response = client.PatchAsync(apiUrl, content);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete employee's record
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> DeleteEmployeeData(int empId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string apiUrl = ConfigurationManager.AppSettings["BaseAPIURL"] + "/" + empId;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["AuthenticationToken"]);
                    var response = client.DeleteAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
