using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Net.Json;
using System.Net;
using System.IO;

namespace SecureVote
{
    public class SecureProtocol
    {
        
        private const String URL = "http://kd2kr.iptime.org";
        private static String Kp = "";
        private static String RN = "";
        private static String Session_Key = "";

        public static bool Req_PublicKey(String strID)
        {
            try
            {
                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic["user_id"] = strID;
                JsonObjectCollection col = (JsonObjectCollection)PostRequest("get_public", dic);
                String res = (String)col["Result"].GetValue();
                if (res.Equals("FALSE"))
                    throw new Exception("Failed");

                Kp = (String)col["Kp"].GetValue();
                RN = (String)col["RN"].GetValue();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed in Req_PublicKey() : ");
            }
            return false;
        }

        public static bool Req_Auth(String strID, String strPW)
        {
            try
            {
                //request
                SHA256 mySHA256 = SHA256Managed.Create();
                int iHashing = (new Random()).Next(10,100);
                Byte[] hashed_RN = Encoding.UTF8.GetBytes(RN);
                for(int i=0; i<iHashing; i++)
                {
                    hashed_RN = mySHA256.ComputeHash(hashed_RN);
                }
                Session_Key = Utility.BytesToHexString(hashed_RN);

                JsonObjectCollection json = new JsonObjectCollection();
                json.Add(new JsonStringValue("user_id", strID));
                json.Add(new JsonStringValue("user_pw", strPW));
                json.Add(new JsonStringValue("RN", RN));
                json.Add(new JsonStringValue("session_key", Session_Key));

                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic["cipher"] = RSAEncrypt(json.ToString(), Kp);
                

                //response
                //result
                JsonObjectCollection col = (JsonObjectCollection)PostRequest("auth", dic);
                String res = (String)col["Result"].GetValue();
                if (res.Equals("FALSE"))
                    throw new Exception("Failed");

                //cipher
                String res_cipher = (String)col["Cipher"].GetValue();
                String res_plain = AESDecrypt256(res_cipher, Session_Key);
                JsonTextParser parser = new JsonTextParser();
                //encrypted jspm
                col = parser.Parse(res_plain);
                RN = (String)col["RN"].GetValue();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed in Req_Auth()");
            }
            return false;
        }

        public static JsonObjectCollection Req_Info(String strID, String strVoteID)
        {

            try
            {
                JsonObjectCollection json = new JsonObjectCollection();
                json.Add(new JsonStringValue("user_id", strID));
                json.Add(new JsonStringValue("vote_id", strVoteID));
                json.Add(new JsonStringValue("RN", RN));

                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic["cipher"] = AESEncrypt256(json.ToString(), Session_Key);

                JsonObjectCollection col = (JsonObjectCollection)PostRequest("vote_info", dic);
                String res = (String)col["Result"].GetValue();
                if (res.Equals("FALSE"))
                    throw new Exception("Failed");

                String res_cipher = (String)col["Cipher"].GetValue();
                String res_plain = AESDecrypt256(res_cipher, Session_Key);
                JsonTextParser parser = new JsonTextParser();
                //encrypted jspm
                return parser.Parse(res_plain);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed in Req_Info()");
            }
            return null;
        }

        public static bool Req_Choice(String strID, String VoteID, String strChoiceIdx)
        {
            try
            {

                JsonObjectCollection json = new JsonObjectCollection();
                json.Add(new JsonStringValue("user_id", strID));
                json.Add(new JsonStringValue("vote_id", strVoteID));
                json.Add(new JsonStringValue("choice_idx", strChoiceIdx));

                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic["cipher"] = AESEncrypt256(json.ToString(), Session_Key);

                JsonObjectCollection col = (JsonObjectCollection)PostRequest("choice", dic);

                String res = (String)col["Result"].GetValue();
                if (res.Equals("FALSE"))
                    throw new Exception("Failed");


            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed in Req_Choice()");
            }
            return false;
        }

        private static String RSAEncrypt(string getValue, string pubKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //암호화
            rsa.FromXmlString(pubKey);
 
            //암호화할 문자열을 UFT8인코딩
            byte[] inbuf = (new UTF8Encoding()).GetBytes(getValue);
 
            //암호화
            byte[] encbuf = rsa.Encrypt(inbuf, false);
 
            //암호화된 문자열 Base64인코딩
            return Convert.ToBase64String(encbuf);
        }

        private static String AESEncrypt256(String Input, String key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Convert.ToBase64String(xBuff);
            return Output;
        }


        //AES_256 복호화
        private static String AESDecrypt256(String Input, String key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Encoding.UTF8.GetString(xBuff);
            return Output;
        }
 


        private static Object PostRequest(String api, Dictionary<String, String> dic)
        {
            String strUri = URL + "/" + api;

            // POST, GET 보낼 데이터 입력
            StringBuilder dataParams = new StringBuilder();
            bool first = true;
            foreach (String key in dic.Keys)
            {
                if (!first)
                    dataParams.Append("&");
                else
                    first = false;
                dataParams.Append(key + "=" + dic[key]);
            }

            // 요청 String -> 요청 Byte 변환
            byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(dataParams.ToString());

            /////////////////////////////////////////////////////////////////////////////////////
            /* POST */
            // HttpWebRequest 객체 생성, 설정
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUri);
            request.Method = "POST";    // 기본값 "GET"
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;

            //////////////////////////////////////////////////////////////////////////////////////

            // 요청 Byte -> 요청 Stream 변환
            Stream stDataParams = request.GetRequestStream();
            stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
            stDataParams.Close();

            // 요청, 응답 받기
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // 응답 Stream 읽기
            Stream stReadData = response.GetResponseStream();
            StreamReader srReadData = new StreamReader(stReadData, Encoding.Default);
            String res = srReadData.ReadToEnd();
            try
            {
                JsonTextParser parser = new JsonTextParser();
                JsonObjectCollection col = (JsonObjectCollection)parser.Parse(res);
                return col;
            }
            catch (Exception ex)
            {

            }
            return res;
        }
    }
}
