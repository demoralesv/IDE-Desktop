using MySqlX.XDevAPI;
using Proyecto_Diseño.Net;
using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

public class ApiService
{
    private readonly HttpClient httpClient;
    private static ApiService instance;
    private string token;
    private ApiService()
    {
        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://sied.me/api/");
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "Kx7xLw0VPtZUqjDQwO0jFEKGfHQwdGaprPFFMPvFY6Ih5Ivg6BwfbVehxoWaTLsV4w788PIKIqFrGttftHFJ9fDZS415BQB7vrAAed1EoPqGyX3xXkkdUPyihP9AI7YqRK1kDpKtxB09VV1zX3sor1orv2k83CZosPIicGIOdAjkEArBTokSF9HqQlhu7hgVO8ACxDs");
    }
    public static ApiService getInstance()
    {
        if (instance == null)
        {
            instance = new ApiService();
        }
        return instance;
    }

    public async Task<string> PostUser(string correo, string pass)
    {
        using (SHA256 hash = SHA256.Create()) //creates the sign for the document 
        {
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(pass));
            StringBuilder sb = new StringBuilder();
            foreach (byte by in bytes)
            {
                sb.Append(by.ToString("x2"));
            }
            pass = sb.ToString();
        }
        var data = new Estudiante { correo = correo, password = pass };
        var jsondata = JsonSerializer.Serialize(data);
        StringContent jsonContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync("login", jsonContent);
        string jsonResponse = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            Result resulttoken = JsonSerializer.Deserialize<Result>(jsonResponse);
            token = resulttoken.data.token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return resulttoken.message;
        }
        else
        {
            Err error = JsonSerializer.Deserialize<Err>(jsonResponse);
            return error.message;
        }
    }


    public async Task<string> PostCreateUser(string nombre, string apellido, string correo, string pass)
    {
        using (SHA256 hash = SHA256.Create()) //creates the sign for the document 
        {
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(pass));
            StringBuilder sb = new StringBuilder();
            foreach (byte by in bytes)
            {
                sb.Append(by.ToString("x2"));
            }
            pass = sb.ToString();
        }
        var data = new Estudiante { nombre = nombre, apellido1 = apellido, correo = correo, password = pass };
        var jsondata = JsonSerializer.Serialize(data);
        StringContent jsonContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync("register", jsonContent);
        string jsonResponse = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            Result resulttoken = JsonSerializer.Deserialize<Result>(jsonResponse);
            token = resulttoken.data.token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            return resulttoken.message;
        }
        else
        {
            Err error = JsonSerializer.Deserialize<Err>(jsonResponse);
            return error.message;
        }
    }
    public async Task<List<CourseInfo>> GetCursos()
    {
        var jsondata = JsonSerializer.Serialize(token);
        StringContent jsonContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.GetAsync("courses");
        string jsonResponse = await response.Content.ReadAsStringAsync();
        Coursesjson result = JsonSerializer.Deserialize<Coursesjson>(jsonResponse);
         return result.data.courses;
    }
    public bool tokeninit()
    {
        return !string.IsNullOrEmpty(token);
    }
}