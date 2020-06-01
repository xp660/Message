using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messegebox.Service
{
    public class ForPaging
    {
        //當前頁數
        public int NowPage { get; set; }

        //最大頁數
        public int MaxPage { get; set; }

        //分頁項目在此設為唯讀
        //以後要修改個數在這裡修改
        public int ItemNum
        {
            get
            {
                return 3;
            }
        }

        public ForPaging()
        {
            //預設
            this.NowPage = 1;
        }
        public ForPaging(int Page)
        {
            this.NowPage = Page;
        }

        //設定正確頁數方法，避免傳入不正確的值
        public void SetRightPage()
        {
            if (this.NowPage < 1)
            {
                this.NowPage = 1;
            }
            else if (this.NowPage>this.MaxPage)
            {
                this.NowPage = MaxPage;
            }

            //無資料時的當前頁數重設為1
            if (this.NowPage.Equals(0))
            {
                this.NowPage = 1;
            }
        }


    }
}