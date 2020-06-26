﻿using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Messegebox.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Messegebox.ViewModels
{
    public class MemberRegisterViewModel
    {
        public Members newMember { get; set; }

        [DisplayName("密碼:")]
        [Required(ErrorMessage ="請輸入密碼")]
        public string Password { get; set; }


        [DisplayName("確認密碼:")]
        [Compare("Password", ErrorMessage = "兩次輸入密碼不一致")]
        [Required(ErrorMessage = "請輸入確認密碼")]
        public string PasswordCheck { get; set; }

    }
}