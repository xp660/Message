using Messegebox.Models;
using Messegebox.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Services.Discovery;

namespace Messegebox
{
    public class GuestbooksDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["no"].ConnectionString;

        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        //取得陣列資料方法
        //public List<Guestbooks> GetDataList()
        //{
        //    List<Guestbooks> DataList = new List<Guestbooks>();

        //    string sql = @"SELECT * FROM Guestbooks";

        //    try
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand(sql, conn);
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        while (dr.Read()) //獲得下一筆資料直到沒有
        //        {
        //            Guestbooks Data = new Guestbooks
        //            {
        //                Id = Convert.ToInt32(dr["Id"]),
        //                Name = dr["Name"].ToString(),
        //                Content = dr["Content"].ToString(),
        //                CreateTime = Convert.ToDateTime(dr["CreateTime"])
        //            };
        //            //確定此則留言是否回覆，且不允許空白
        //            //因C#是強型別語言，所以轉換時Datetime型態不允許存取null
        //            if (!dr["ReplyTime"].Equals(DBNull.Value))
        //            {
        //                Data.Reply = dr["Reply"].ToString();
        //                Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
        //            }
        //            DataList.Add(Data);

        //        }

        //    }
        //    catch(Exception e)
        //    {
        //        throw new Exception(e.Message.ToString());
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //    return DataList;
        //}

        #region 新增資料
        public void InsertGuestbooks(Guestbooks newData)
        {
            string sql = $@"INSERT INTO Guestbooks(Name,Content,CreateTime) VALUES
                        ('{newData.Name}' ,'{newData.Content}',
                            '{DateTime.Now:yyyy - MM - dd HH: mm: ss}' ) ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

        }
        #endregion

        #region 查詢一筆資料
        public Guestbooks GetDataById(int Id)
        {
            Guestbooks Data = new Guestbooks();

            string sql = $@"SELECT * FROM Guestbooks WHERE Id={Id};";

            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Name = dr["Name"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                //確定此則留言是否回覆，且不允許空白
                if (!string.IsNullOrWhiteSpace(dr["Reply"].ToString()))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                }
            }
            catch (Exception e)
            {
                //查無資料
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳根據編號所取得的資料
            return Data;
        }
        #endregion

        #region 修改留言
        public void UpdateGuestbooks(Guestbooks UpdateData)
        {
            string sql = $@"UPDATE Guestbooks SET Name='{UpdateData.Name}',
                            Content='{UpdateData.Content}' WHERE Id={UpdateData.Id};";

            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }

        }
        #endregion

        #region 回覆留言
        public void ReplyGuestbooks(Guestbooks ReplyData)
        {
            string sql= $@"UPDATE Guestbooks SET Reply ='{ReplyData.Reply}',
                            ReplyTime='{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' WHERE Id = { ReplyData.Id }; ";

            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 檢查相關
        //修改資料判斷的方法
        public bool CheckUpdate(int Id)
        {
            //取得要修改的資料
            Guestbooks Data = GetDataById(Id);
            //判斷是否有資料及是否有回覆
            return (Data != null && Data.ReplyTime == null);
        }
        #endregion

        #region 刪除留言
        public void DeleteGuestbooks(int Id)
        {
            string sql = $@"DELETE FROM Guestbooks WHERE Id={Id};";

            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 查詢陣列資料
        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋條件時
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
            }
            else
            {
                //無搜尋條件時
                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
            }

            return DataList;

            return DataList;
        }
        #endregion


        #region 設定最大頁數方法
        //無搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Guestbooks ";
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }

            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();

        }

        //有搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging,string Search)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Guestbooks Where Name like '%{Search}%' 
                            or Content like '%{Search}%' or Reply like '%{Search}%' ";
            
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }
        #endregion

        #region 搜尋資料方法
        //無搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging)
        {

            List<Guestbooks> DataList = new List<Guestbooks>();
            string sql = $@"SELECT * FROM (SELECT row_number() OVER(order by Id) AS sort,* FROM Guestbooks)
                        m WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";

            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    //確定此則留言是否回覆，且不允許空白
                    //因C#是強型別語言，所以轉換時Datetime型態不允許存取null
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }

        //有搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging,string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            string sql = $@"SELECT * FROM (SELECT row_number() OVER(order by Id) AS sort,* FROM Guestbooks
                            WHERE Name LIKE '%{Search}%' OR Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%') m
                        WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";


            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    //確定此則留言是否回覆，且不允許空白
                    //因C#是強型別語言，所以轉換時Datetime型態不允許存取null
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }
    }
    #endregion
}