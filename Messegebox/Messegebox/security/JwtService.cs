using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace Messegebox.security
{
    public class JwtService
    {
        #region 製作Token
        public string GenerationToken(string Account,string Role)
        {
            JwtObject jwtObject = new JwtObject
            {
                Account = Account,
                Role = Role,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"])).ToString()
            };

            //從web.config取得密鑰
            string secretKey = WebConfigurationManager.AppSettings["secretKey"].ToString();

            //JWT內容
            var payload = jwtObject;

            //將資料加密為Token
            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(secretKey), JwsAlgorithm.HS512);

            return token;

        }
        #endregion
    }
}