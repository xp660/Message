using Messegebox.security;
using Messegebox.Service;
using Messegebox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Messegebox.Controllers
{
    public class MembersController : Controller
    {
        //宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();
       
        //宣告寄信用的Service物件
        private readonly MailService mailService = new MailService();

        // GET: Members
        public ActionResult Index()
        {
            return View();
        }

        #region 註冊
        //註冊一開始顯示頁面
        public ActionResult Register()
        {
            //判斷使用者是否已經登入驗證
            if (User.Identity.IsAuthenticated)
                //已登入則重新導向
                return RedirectToAction("Index", "Guestbooks");
                
            //否則進入註冊畫面
            return View();
            
        }

        //傳入註冊資料的Action
        [HttpPost]
        public ActionResult Register(MemberRegisterViewModel RegisterMember) 
        {
            //判斷頁面資料是否經過驗證
            if (ModelState.IsValid)
            {
                // 將頁面資料中的密碼欄位填入
                RegisterMember.newMember.Password = RegisterMember.Password;

                //取得信箱驗證碼
                string AuthCode = mailService.GetValidateCode();

                //將信箱驗證碼填入
                RegisterMember.newMember.AuthCode = AuthCode;

                //呼叫Serrvice註冊新會員
                membersService.Register(RegisterMember.newMember);

                //取得寫好的驗證信範本內容
                string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));

                //宣告Email驗證用Url
                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("EmailValidate", "Members"
                        , new
                        {
                            Account = RegisterMember.newMember.Account,
                            AuthCode = AuthCode
                        })
                };

                //藉由Service將使用者資料夾填入驗證信範本
                string MailBody = mailService.GetRegisterMailBody(TempMail, RegisterMember.newMember.Name,
                                                    ValidateUrl.ToString().Replace("%3F", "?"));

                //呼叫Service寄出驗證信
                mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);

                //用TempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";

                //重新導向頁面
                return RedirectToAction("RegisterResult");

            }
            //未經驗證清空密碼相關欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            //將資料回填至View中
            return View(RegisterMember);


        }
           

        //註冊結果顯示頁面
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷註冊帳號是否被註冊過Action
        public JsonResult AccountCheck(MemberRegisterViewModel RegisterMember)
        {
            //呼叫Service來判斷回傳結果
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account), JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳入
        public ActionResult EmailValidate(string Account,string AuthCode)
        {
            //用ViewData儲存，使用Service進行信箱驗證後結果訊息
            ViewData["EmailData"] = membersService.EmailValidate(Account, AuthCode);

            return View();
        }

        #endregion

        #region 登入
        //登入一開始載入畫面
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Guestbooks"); //以登入則重新導向
            return View();//否則進入登入畫面
        }

        //傳入登入資料
        [HttpPost]
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);
            //判斷驗證後結果是否有錯誤訊息
            if (String.IsNullOrWhiteSpace(ValidateStr))
            {
                string RoleData = membersService.GetRole(LoginMember.Account);

                JwtService jwtService = new JwtService();

                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

                string Token = jwtService.GenerationToken(LoginMember.Account, RoleData);

                //產生一個cookie
                HttpCookie cookie = new HttpCookie(cookieName);

                cookie.Value = Server.UrlEncode(Token);

                //寫入用戶端
                Response.Cookies.Add(cookie);

                //設定cookie期限
                Response.Cookies["cookieName"].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));


                return RedirectToAction("Index", "Guestbooks");


            }
            else
            {
                //有驗證錯誤訊息，加入頁面模型
                ModelState.AddModelError("", ValidateStr);
                return View(LoginMember);
            }
        }
        #endregion

        #region 登出
        [Authorize] //設定此Action需登入
        public ActionResult Logout()
        {
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

            //清除cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);

            return RedirectToAction("Login");
        }
        #endregion


        #region 修改密碼
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel ChangeData)
        {
            //判斷頁面資料是否都經過驗證
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }

            return View();
        }
        #endregion

    }


}