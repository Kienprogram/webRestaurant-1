namespace ApiAuth.Service
{
    public class UserService
    {
        // This method will be used to hash passwords during user registration or creation
        public static string HashPassword(string plainPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.HashPassword(plainPassword);
            }
            catch (Exception ex)
            {
                // Log exception and handle error gracefully
                throw new InvalidOperationException("Password hashing failed", ex);
            }
        }
    }
}
