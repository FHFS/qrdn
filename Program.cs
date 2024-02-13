using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Net;

namespace QRdn
{
    public class QRCodeController : Controller
    {

        public static async Task<Bitmap> DownloadBitmapFromUrl(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        return new Bitmap(stream);
                    }
                }
            }
        }

        public QRCodeController()
        {
            SetIconAsync().Wait();
        }
        private async Task SetIconAsync()
        {
            string imageUrl = "https://th.bing.com/th/id/OIP.upQhneQriWKNEvZZDinfswHaHa?rs=1&pid=ImgDetMain";
            icon = await DownloadBitmapFromUrl(imageUrl);
        }

        private Bitmap icon;

        public IActionResult Index(string data)
        {
            // Check if data is provided
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest("Data parameter is missing.");
            }

            // Generate QRCode image
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.DarkRed, Color.PaleGreen, icon, 15, 6, true);

            // Convert Bitmap to byte array
            byte[] byteArray;
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byteArray = stream.ToArray();
            }

            // Return the image as a FileResult
            return File(byteArray, "image/png");
        }
    }
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "qrcode",
                    pattern: "a",
                    defaults: new { controller = "QRCode", action = "Index" });
            });
        }
    }

    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:8080");
            });

        public static void Main(string[] args)
        {
            Console.WriteLine("QR dotNet started");
            CreateHostBuilder(args).Build().Run();
            Console.ReadLine();
        }

    }
}
