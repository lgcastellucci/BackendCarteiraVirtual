using System.Net;
using System.Text;

namespace BackendCarteiraVirtual
{
    public class HttpService
    {
        public async Task<HttpStatusCode> EnviaTransacao(double valor)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string apiUrl = "http://localhost:42702/mock/transaction";

                // Conte�do que voc� deseja enviar no corpo da solicita��o (JSON neste exemplo)
                string jsonContent = "{\"value\": " + valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "}";

                // Crie o conte�do a partir do JSON
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Execute a solicita��o POST
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                    // Verifique se a solicita��o foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        // Lide com a resposta, se necess�rio
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (responseBody == "true")
                            return HttpStatusCode.OK;
                        else
                            return HttpStatusCode.Forbidden;
                    }
                    else
                    {
                        return HttpStatusCode.NotAcceptable;
                    }
                }
                catch
                {
                    return HttpStatusCode.InternalServerError;
                }
            }
        }
    }
}