using System.Security.Cryptography;
using System.Text;

namespace ApiAuth.Service
{
    public class XApiSecretMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiSecretKey;
        private readonly string _encryptionKey;

        public XApiSecretMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _apiSecretKey = configuration.GetValue<string>("ApiSecretKey");
            _encryptionKey = configuration.GetValue<string>("EncryptionKey");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //if (!context.Request.Headers.TryGetValue("X-Api-Secret-Key", out var encryptedApiKey))
            //{
            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    await context.Response.WriteAsync("Missing X-Api-Secret-Key.");
            //    return;
            //}

            //string decryptedApiKey = Decrypt(encryptedApiKey, _encryptionKey);

            //if (!_apiSecretKey.Equals(decryptedApiKey))
            //{
            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    await context.Response.WriteAsync("Invalid X-Api-Secret-Key.");
            //    return;
            //}

            if (!context.Request.Headers.TryGetValue("X-Api-Secret-Key", out var providedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing X-Api-Secret-Key.");
                return;
            }

            if (!providedApiKey.Equals(_apiSecretKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid X-Api-Secret-Key.");
                return;
            }

            await _next(context); // Call the next middleware
        }

        private string Decrypt(string cipherText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = keyBytes; // Use the same key as IV (for simplicity)
                ICryptoTransform decryptor = aes.CreateDecryptor();

                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }
}