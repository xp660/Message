using Jose;
using Messegebox.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Messegebox
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //���g�v�����ҫe����ʧ@
        //�b���Ω�]�w����(Role)
        protected void Application_OnPostAuthenticateRequest(object sender,EventArgs e)
        {
            //�����ШD���
            HttpRequest httpRequest = HttpContext.Current.Request;

            //�]�wJWT�K�_
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();

            //�]�wcookie
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

            //�ˬdCookie���e�O�_�s��Token
            if(httpRequest.Cookies[cookieName] != null)
            {
                //�NToken�٭�
                JwtObject jwtObject = JWT.Decode<JwtObject>(Convert.ToString(httpRequest.Cookies[cookieName].Value), Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);

                //�N�ϥΪ̨�����X�A���Φ��}�C
                string[] roles = jwtObject.Role.Split(new char[] { ',' });

                //�ۦ�إ�Identity���NHttpContext.Current.User��Identity
                //�N��ƶ�iClaim�����]�p
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name,jwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier,jwtObject.Account)
                };

                var claimsIdentity = new ClaimsIdentity(claims, cookieName);

                //�[�Jidentityprovider�o��Claim�ϱo�ϥ�_�y�J@html.antiForgeryToken()��q�L
                claimsIdentity.AddClaim(
                    new Claim(@"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider"
                        , "My Indentity", @"http://www.w3.org/2001/XMLSchema#string"));

                //���������ثeHttpContext��User����h
                HttpContext.Current.User = new GenericPrincipal(claimsIdentity, roles);

                Thread.CurrentPrincipal = HttpContext.Current.User;
            }


        }


    }
}
