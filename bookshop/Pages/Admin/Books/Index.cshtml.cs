using bookshop.Myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace bookshop.Pages.Admin.Books
{
    [RequireAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<BookInfo> listBooks = new List<BookInfo>();
        public string search = "";

        public int page =1; //the corrent of html page
        public int totalPages = 0;
        private readonly int  pageSize = 5; // books per page

        public string column = "id";
        public string order = "desc"; 

        public void OnGet()
        {
            search = Request.Query["search"];
            if (search == null) search = "";

            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page =1;
                }
            }
            string[] validColumns = { "id", "title", "authors", "num_pages", "price", "category", "created_at" };
            column = Request.Query["column"];
            if (column == null || !validColumns.Contains(column))
            {
                column = "id";
            }

            order = Request.Query["order"];
            if (order == null || !order.Equals("asc"))
            {
                order = "desc";
            }
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=bookstore;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM books";
                    if (search.Length > 0)
                    {
                        sqlCount += " WHERE title LIKE @search OR authors LIKE @search";
                    }

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");

                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = " SELECT * FROM books";
                    if (search.Length > 0)
                    {
                        sql += " WHERE title LIKE @search OR authors LIKE @search";
                    }
                    sql += " ORDER BY " + column + " " + order;//" ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

					using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search +  "%");
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);   

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BookInfo bookinfo = new BookInfo();
                                bookinfo.Id = reader.GetInt32(0 );
                                bookinfo.Title = reader.GetString(1);
                                bookinfo.Authtors = reader.GetString(2);
                                bookinfo.Isbn = reader.GetString(3);
                                bookinfo.NumPages = reader.GetInt32(4);
                                bookinfo.Price= reader.GetDecimal(5);
                                bookinfo.Category = reader.GetString(6);
                                bookinfo.Description = reader.GetString(7);
                                bookinfo.ImageFileName = reader.GetString(8);
                                bookinfo.CreateAt = reader.GetDateTime(9).ToString("MM/dd/yyyy");

                                listBooks.Add(bookinfo);
                            }
                        }
                    }
                }
			}
            catch (Exception ex)
            {
                Console.WriteLine();
            }
        }
    }

    public class BookInfo
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Authtors { get; set; } = "";
        public string Isbn { get; set; } = "";
        public int NumPages { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageFileName { get; set; } = "";
        public string CreateAt { get; set; } = "";
        public string Authors { get; internal set; }
    }
}
