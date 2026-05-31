using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

public class ApiService
{
    private readonly HttpClient httpClient;

    public ApiService()
    {
        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://sied.me/api/");
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "Kx7xLw0VPtZUqjDQwO0jFEKGfHQwdGaprPFFMPvFY6Ih5Ivg6BwfbVehxoWaTLsV4w788PIKIqFrGttftHFJ9fDZS415BQB7vrAAed1EoPqGyX3xXkkdUPyihP9AI7YqRK1kDpKtxB09VV1zX3sor1orv2k83CZosPIicGIOdAjkEArBTokSF9HqQlhu7hgVO8ACxDs");
    }

    public async Task<string> GetUsuario()
    {
        HttpResponseMessage response = await httpClient.GetAsync("usuarios/getUsers.php");//parametro
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
    public async Task<string> PostUser(string correo, string pass)
    {
        var data = new Estudiante { cor = correo, pass = pass };
        var jsondata = JsonSerializer.Serialize(data);
        StringContent jsonContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync("usuarios/getUsers.php", jsonContent);
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        return jsonResponse;
    }
}