using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BTS__User__Mangement__API.Models
{

    public class NewToken
    {
        private const string _alg = "HmacSHA256";
        private const string _salt = "rz8LuOtFBXphj9WQfvFh";

        public static string GenerateToken(string username, string password, string ip, string userAgent, long ticks)
        {
            string hash = string.Join(":", new string[] { username, ip, userAgent, ticks.ToString() });
            string hashLeft = "";
            string hashRight = "";
            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(password));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join(":", new string[] { username, ticks.ToString() });
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
        }
        public static string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, _salt });
            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(_salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hmac.Hash);
            }
        }



       // private const int _expirationMinutes = 10;
        public static bool IsTokenValid(string token, string ip, string userAgent)
        {
            bool result = false;
            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                // Split the parts.
                string[] parts = key.Split(new char[] { ':' });
                if (parts.Length == 3)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    string username = parts[1];
                    long ticks = long.Parse(parts[2]);
                    DateTime timeStamp = new DateTime(ticks);
                    var response = UserToken.GetTokenRow(ip, userAgent, token);
                    // Ensure the timestamp is valid.
                    if (response != null)
                    {
                        bool expired = Math.Abs((DateTime.UtcNow - timeStamp).TotalMinutes) > response.ExpiryDateInMinutes;
                        if (!expired)
                        {
                            // Lookup the user's account from the db.
                            //Users GetUserDetails = Users.VerifyLoginDetails(username, AppId);

                            if (username == "")
                            {
                                string password = "";
                                // Hash the message with the key to generate a token.
                                string computedToken = GenerateToken(username, password, ip, userAgent, ticks);
                                // Compare the computed token with the one supplied and ensure they match.
                                result = (token == computedToken);
                                //If Token is still valid renew the token

                            }
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
            }
            return result;
        }


        public static string GetIPAddress()
        {
            HttpContext context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static long ConvertDateTimeToTicks(DateTime dtInput)
        {
            long ticks = 0;
            ticks = dtInput.Ticks;
            return ticks;
        }


    }
}