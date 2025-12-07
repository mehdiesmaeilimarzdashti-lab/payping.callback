
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/callback", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    var json = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

    string refId = json["refId"].ToString();
    string clientTrackId = json["clientTrackId"].ToString();
    int amount = int.Parse(json["amount"].ToString());

    // ارسال پیام به تلگرام
    string chatId = clientTrackId;
    string message = $"پرداخت موفق بود 🎉\nکد پیگیری: {refId}\nمبلغ: {amount} تومان";

    await SendMessageToTelegram(chatId, message);

    return Results.Ok("OK");
});

app.Run();

static async Task SendMessageToTelegram(string chatId, string text)
{
    string token = "BOT_TOKEN";

    using var client = new HttpClient();
    var url = $"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatId}&text={text}";

    await client.GetAsync(url);
}
