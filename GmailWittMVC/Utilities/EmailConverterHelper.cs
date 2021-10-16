using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace GmailWittMVC.Utilities
{
    public static class EmailConverterHelper
    {
        public static byte[] Base64UrlDecode(string arg)
        {
            // Convert from base64url string to base64 string
            string s = arg;
            s = s.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 0:
                    break;              // No pad chars in this case
                case 2:
                    s += "==";
                    break;              // Two pad chars
                case 3:
                    s += "=";
                    break;              // One pad char
                default:
                    throw new Exception("Illegal base64url string!");
            }

            return Convert.FromBase64String(s);
        }
        
        public static string GetUntilOrEmpty(this string text, string stopAt = "<html")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(charLocation);
                }
            }

            return String.Empty;
        }
        
        
         public static string MsgNestedParts(IList<MessagePart> Parts)
        {
            string str = string.Empty;
            if (Parts.Count() < 0)
            {
                return string.Empty;
            }
            else
            {
                IList<MessagePart> PlainTestMail = Parts.Where(x => x.MimeType == "text/plain").ToList();
                IList<MessagePart> AttachmentMail = Parts.Where(x => x.MimeType == "multipart/alternative").ToList();

                if (PlainTestMail.Count() > 0)
                {
                    foreach (MessagePart EachPart in PlainTestMail)
                    {
                        if (EachPart.Parts == null)
                        {
                            if (EachPart.Body != null && EachPart.Body.Data != null)
                            {
                                str += EachPart.Body.Data;
                            }
                        }
                        else
                        {
                            return MsgNestedParts(EachPart.Parts);
                        }
                    }
                }
                if (AttachmentMail.Count() > 0)
                {
                    foreach (MessagePart EachPart in AttachmentMail)
                    {
                        if (EachPart.Parts == null)
                        {
                            if (EachPart.Body != null && EachPart.Body.Data != null)
                            {
                                str += EachPart.Body.Data;
                            }
                        }
                        else
                        {
                            return MsgNestedParts(EachPart.Parts);
                        }
                    }
                }
                return str;
            }
        }
         
         
         public static string Base64Decode(string Base64Test)
         {
             string EncodTxt = string.Empty;
             //STEP-1: Replace all special Character of Base64Test
             EncodTxt = Base64Test.Replace("-", "+");
             EncodTxt = EncodTxt.Replace("_", "/");
             EncodTxt = EncodTxt.Replace(" ", "+");
             EncodTxt = EncodTxt.Replace("=", "+");

             //STEP-2: Fixed invalid length of Base64Test
             if (EncodTxt.Length % 4 > 0) { EncodTxt += new string('=', 4 - EncodTxt.Length % 4); }
             else if (EncodTxt.Length % 4 == 0)
             {
                 EncodTxt = EncodTxt.Substring(0, EncodTxt.Length - 1);
                 if (EncodTxt.Length % 4 > 0) { EncodTxt += new string('+', 4 - EncodTxt.Length % 4); }
             }

             //STEP-3: Convert to Byte array
             byte[] ByteArray = Convert.FromBase64String(EncodTxt);

             //STEP-4: Encoding to UTF8 Format
             return Encoding.UTF8.GetString(ByteArray);
         }
         
         public static byte[] Base64ToByte(string Base64Test)
         {
             string EncodTxt = string.Empty;
             //STEP-1: Replace all special Character of Base64Test
             EncodTxt = Base64Test.Replace("-", "+");
             EncodTxt = EncodTxt.Replace("_", "/");
             EncodTxt = EncodTxt.Replace(" ", "+");
             EncodTxt = EncodTxt.Replace("=", "+");

             //STEP-2: Fixed invalid length of Base64Test
             if (EncodTxt.Length % 4 > 0) { EncodTxt += new string('=', 4 - EncodTxt.Length % 4); }
             else if (EncodTxt.Length % 4 == 0)
             {
                 EncodTxt = EncodTxt.Substring(0, EncodTxt.Length - 1);
                 if (EncodTxt.Length % 4 > 0) { EncodTxt += new string('+', 4 - EncodTxt.Length % 4); }
             }

             //STEP-3: Convert to Byte array
             return Convert.FromBase64String(EncodTxt);
         }
        
        
    }
}