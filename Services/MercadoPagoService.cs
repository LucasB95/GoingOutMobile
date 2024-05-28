using GoingOutMobile.Services.Interfaces;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private static string AccessToken = "TEST-3348115962794649-122019-c9ee6765f5a7255a0467e2b671e07506-37419455";


        public async Task<List<string>> preparandoMP()
        {

            List <string> resultados = new List<string>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);


                var requestContent = new JObject
                {
                    {"external_reference", "F 0001"},
                    {"items",new JArray
                      {
                        new JObject
                        {
                            {"title","Producto 1" },
                            {"quantity",1 },
                            {"unit_price",1500 }
                        }
                      }
                    },
                    {"payer", new JObject
                      {
                        {"type","customer" }
                      }
                    },
                    {"payment_method_id","cardPayment" },
                    {"back_urls", new JObject
                      {
                        {"success","customer" }
                      }
                    },
                    {"auto_return", "all"}



                };

                var response = await httpClient.PostAsync("https://api.mercadopago.com/checkout/preferences", new StringContent(requestContent.ToString()));
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var order = JObject.Parse(responseContent);
                resultados.Add(order["id"].ToString());
                resultados.Add(order["sandbox_init_point"].ToString());
                return resultados;

            }
           

        }


        public async Task<List<string>> MPCorto()
        {
            List<string> resultados = new List<string>();

            MercadoPagoConfig.AccessToken = "TEST-3348115962794649-122019-c9ee6765f5a7255a0467e2b671e07506-37419455";

            var request = new PaymentCreateRequest
            {
                TransactionAmount = 10,
                Token = Guid.NewGuid().ToString(),
                Description = "Payment description",
                Installments = 1,
                PaymentMethodId = "visa",
                Payer = new PaymentPayerRequest
                {
                    Email = "test.payer@email.com",
                }
            };

            var client = new PaymentClient();
            Payment payment = await client.CreateAsync(request);

            return resultados;
        }
    }
}
