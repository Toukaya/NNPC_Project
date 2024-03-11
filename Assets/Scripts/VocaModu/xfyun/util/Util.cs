using System;
using System.Security.Cryptography;
using System.Text;

namespace VocaModu.xfyun.util
{
    public static class Util
    {
        public static string GetUrl(string url, string apiSecret, string apiKey)
        {
            var uri = new Uri(url);
            var date = DateTime.Now.ToString("r"); //官方文档要求时间必须是UTC+0或GMT时区，RFC1123格式(Thu, 01 Aug 2019 01:53:21 GMT)。
            var authorization = ComposeAuthUrl(uri,date, apiSecret, apiKey);
            var uriStr = $"{uri}?authorization={authorization}&date={date}&host={uri.Host}"; //生成最终鉴权
            return uriStr;
        }

        /// <summary>
        /// 组装生成鉴权
        /// </summary>
        private static string ComposeAuthUrl(Uri uri,string date, string apiSecret, string apiKey)
        {
            var signatureOrigin = string.Format("host: " + uri.Host + "\ndate: " + date + "\nGET " + uri.AbsolutePath + " HTTP/1.1");
            var signatureSHA = HmacSHA256(signatureOrigin, apiSecret); 
            var auth = "api_key=\"{0}\", algorithm=\"{1}\", headers=\"{2}\", signature=\"{3}\"";
            var authorizationOrigin = string.Format(auth, apiKey, "hmac-sha256", "host date request-line", signatureSHA); 
            return ToBase64String(authorizationOrigin);
        }

        private static string HmacSHA256(string secret, string signKey)
        {
            using HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(signKey));
            byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(secret));
            var signRet = Convert.ToBase64String(hash);
            return signRet;
        }
        
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }

            return encode;
        }

        public static string ToBase64String(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
        
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
        
        
        //byte[]转16进制格式string
        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes == null) return hexString;
            StringBuilder strB = new StringBuilder();
            foreach (byte b in bytes)
                strB.AppendFormat("{0:x2}", b);

            hexString = strB.ToString();

            return hexString;
        }
        
           
        /// <summary>
        /// byte[]数组转化为AudioClip可读取的float[]类型
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static float[] BytesToFloat(byte[] byteArray)
        {
            float[] soundData = new float[byteArray.Length / 2];
            for (int i = 0; i < soundData.Length; i++)
                soundData[i] = BytesToFloat(byteArray[i * 2], byteArray[i * 2 + 1]);
            
            return soundData;
        }

        private static float BytesToFloat(byte firstByte, byte secondByte)
        {
            // convert two bytes to one short (little endian)
            //小端和大端顺序要调整
            short s;
            if (BitConverter.IsLittleEndian)
                s = (short)((secondByte << 8) | firstByte);
            else
                s = (short)((firstByte << 8) | secondByte);
            // convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }
    }
}