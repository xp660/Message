using Messegebox.Models;
using Messegebox.Service;
using Messegebox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Messegebox.Controllers
{
    public class GuestbooksController : Controller
    {
        //宣告Guestbooks資料表的service物件
        private readonly GuestbooksDBService GuestbookService = new GuestbooksDBService();

        // GET: Guestbooks
        public ActionResult Index(string Search,int Page=1)
        {
            //宣告一個頁面模型
            GuestbooksViewModel Data = new GuestbooksViewModel();
            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            //從service中取得頁面所需陣列資料
            Data.DataList = GuestbookService.GetDataList(Data.Paging, Data.Search);
            //將頁面資料傳入View中
            return View(Data);
        }

        #region 新增留言
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        //設定此Action只接受頁面POST資料傳入
        public ActionResult Create([Bind(Include ="Name,Content")]Guestbooks Data) 
        {
            GuestbookService.InsertGuestbooks(Data);
            return RedirectToAction("Index");
        }
        #endregion

        #region 修改留言
        public ActionResult Edit(int Id)
        {
            Guestbooks Data = GuestbookService.GetDataById(Id);
            return View(Data);
        }
        [HttpPost]
        public ActionResult Edit(int Id,[Bind(Include ="Name,Content")]Guestbooks UpdateData)
        {
            if (GuestbookService.CheckUpdate(Id))
            {
                UpdateData.Id = Id;
                GuestbookService.UpdateGuestbooks(UpdateData);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 回覆留言
        public ActionResult Reply(int Id)
        {
            Guestbooks Data = GuestbookService.GetDataById(Id);
            return View(Data);
        }
        [HttpPost]
        public ActionResult Reply(int Id,[Bind(Include ="Reply,ReplyTime")]Guestbooks ReplyData)
        {
            if (GuestbookService.CheckUpdate(Id))
            {
                ReplyData.Id = Id;
                GuestbookService.ReplyGuestbooks(ReplyData);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 刪除留言
        public ActionResult Delete(int Id)
        {
            GuestbookService.DeleteGuestbooks(Id);
            return RedirectToAction("Index");
        }
        #endregion
    }
}