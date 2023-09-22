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

                // Conteúdo que você deseja enviar no corpo da solicitação (JSON neste exemplo)
                string jsonContent = "{\"value\": " + valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "}";

                // Crie o conteúdo a partir do JSON
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Execute a solicitação POST
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                    // Verifique se a solicitação foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        // Lide com a resposta, se necessário
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