using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string url = "http://localhost:5000/";
        using HttpListener listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();

        Console.WriteLine($"Server is listening on {url}");

        while (true)
        {
            try
            {
                HttpListenerContext context = await listener.GetContextAsync(); 
                _ = Task.Run(() => HandleRequest(context)); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static async Task HandleRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;

        Console.WriteLine($"\n=== Got Request ===");
        Console.WriteLine($"Method: {request.HttpMethod}");
        Console.WriteLine($"URL: {request.Url}");
        Console.WriteLine($"Headers:");

        foreach (string key in request.Headers.AllKeys)
        {
            Console.WriteLine($"{key}: {request.Headers[key]}");
        }

        if (request.HasEntityBody)
        {
            using var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
            string body = await reader.ReadToEndAsync();
            Console.WriteLine($"Request Body:\n{body}");
        }

        HttpListenerResponse response = context.Response;
        string responseString = "Request Accepted!";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.Close();
    }
}
