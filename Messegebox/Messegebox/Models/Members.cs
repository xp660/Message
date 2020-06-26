﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Messegebox.Models
{
    public class Members
    {
        [DisplayName("帳號:")]
        [Required(ErrorMessage ="請輸入帳號")]
        [StringLength(30,MinimumLength =6,ErrorMessage ="帳號長度介於6-30字元")]
        [Remote("AccountCheck","Members",ErrorMessage ="此帳號已被註冊過")]
        public string Account { get; set; }

        //密碼
        public string Password { get; set; }

        [DisplayName("姓名:")]
        [StringLength(20, ErrorMessage = "姓名長度最多20字元")]
        [Required(ErrorMessage = "請輸入姓名")]
        public string Name { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(200, ErrorMessage = "Email長度最多200字元")]
        [EmailAddress(ErrorMessage ="這不是Email格式")]
        public string Email { get; set; }

        //信箱驗證碼
        public string AuthCode { get; set; }

        //使否為管理者
        public bool IsAdmin { get; set; }
    }
}