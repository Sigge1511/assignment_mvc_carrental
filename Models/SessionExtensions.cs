namespace assignment_mvc_carrental.Models
{
    public static class SessionExtensions //vi ska bara ha en av denna därför static
    {
        public static void SetObject<T>(this ISession session, string key, T value) //sparar info i vår session
        {           
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }
        
        public static T Get<T>(this ISession session, string key) //hämtar info från vår session
        {
            var json = session.GetString(key);

            if (string.IsNullOrEmpty(json)) //om sessionen är tom sträng, returnera defaultvärde
            {
                return default(T);
            }
            else
            {
                return json == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
        }

    }
}
