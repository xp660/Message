using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Messegebox.Service
{
    public class MailService
    {
        private string gmail_account = "imxp660@gmail.com";
        private string gmail_password = "a3344824";
        private string gmail_mail = "imxp660@gmail.com";     

        #region 寄會員驗證信
        //設定驗證碼字元陣列
        public string GetValidateCode()
        {
            string[] Code = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K"
                            , "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y"
                            , "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b"
                            , "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n"
                            , "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            string ValidateCode = string.Empty;

            Random rd = new Random();

            for (int i = 0; i < 10; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];

            }

            return ValidateCode;
        }


        //將使用者資料填入驗證信範本中
        public string GetRegisterMailBody(string TempString , string UserName,string ValidateUrl)
        {
            //將使用者資料填入
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);

            return TempString;
        }

        //寄驗證信的方法
        public void SendRegisterMail(string MailBody,string ToEMail)
        {
            //建立寄信用的Smtp物件，以Gmail為例
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com")
            {

                //設定使用者port，這裡設定Gmail所使用
                Port = 587,

                //建立使用者憑據，這裡設定我的Gmail帳戶
                Credentials = new System.Net.NetworkCredential(gmail_account, "nhiorbggtzmqpbjd"),

                //開啟SSL
                EnableSsl = true
            };

            //宣告信件物件內容
            MailMessage mail = new MailMessage
            {

                //設定來源信箱
                From = new MailAddress(gmail_mail)
            };

            //設定收信者信箱
            mail.To.Add(ToEMail);

            //信件主旨
            mail.Subject = "會員註冊確認信";

            //信件內容
            mail.Body = MailBody;

            //設定信件內容是HTML格式
            mail.IsBodyHtml = true;

            //送出信件
            smtpServer.Send(mail);
        }

        #endregion

    }
}