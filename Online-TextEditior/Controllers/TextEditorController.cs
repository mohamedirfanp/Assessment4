using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Online_TextEditior.Models;

namespace Online_TextEditior.Controllers
{
    public class TextEditorController : Controller
    {
		private readonly IConfiguration _configuration;
        private static string UserEmail = string.Empty;
		public TextEditorController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		private SqlConnection _connection;
		private List<TextEditorModel> _contentList = new List<TextEditorModel>();

		public void Connection()
		{
			string conn = _configuration.GetConnectionString("TextEditorDB");
			_connection = new SqlConnection(conn);
			_connection.Open();
		}
		// GET: TextEditorController1
		[HttpGet]
        public ActionResult Index()
        {
            Connection();
            string selectQuery = "";
            if(!UserEmail.Equals(""))
            {
			    selectQuery = "GetTextContentsByEmail";

            }
            else
            {
                selectQuery = "SELECT * FROM TextEditor";
            }

			string opnMode = "Normal";
			string jobIdStr = "";



			using (SqlCommand cmd = new SqlCommand(selectQuery, _connection))
			{
                if(!UserEmail.Equals(""))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email",System.Data.SqlDbType.NVarChar).Value = UserEmail;
                }
				SqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					TextEditorModel contentModel = new TextEditorModel();
					contentModel.Id = (int)reader[0];
                    contentModel.Title = reader[1].ToString().Trim();
                    contentModel.TextDescription = reader[2].ToString().Trim();
                    contentModel.ShortDescription = reader[3].ToString().Trim();
                    contentModel.UserEmail = reader[4].ToString().Trim();
                    contentModel.SharedUsers = reader[5].ToString().Trim();
                    _contentList.Add(contentModel);
				}
				reader.Close();
			}

			ViewBag.contentList = _contentList;

			ViewBag.status = opnMode;
			ViewBag.contentId = jobIdStr;


			return View();
        }

        // GET: TextEditorController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TextEditorController1/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(TextEditorModel textEditorModel)
        {
            Connection();
            string addJobQuery = $"INSERT INTO TextEditor VALUES('{textEditorModel.Title}','{textEditorModel.TextDescription}','{textEditorModel.ShortDescription}','{textEditorModel.UserEmail}','{textEditorModel.SharedUsers}')";

            Console.WriteLine(addJobQuery);

            try
            {
                using (SqlCommand cmd = new SqlCommand(addJobQuery, _connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }

        // GET: TextEditorController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TextEditorController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TextEditorController1/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Connection();
            string deleteJobQuery = $"DELETE FROM TextEditor WHERE Id = {id}";

            try
            {
                using (SqlCommand cmd = new SqlCommand(deleteJobQuery, _connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            Connection();
            string getJobQuery = $"SELECT * FROM TextEditor WHERE Id = {id}";

            TextEditorModel contentModel = new TextEditorModel();
            try
            {
                using (SqlCommand cmd = new SqlCommand(getJobQuery, _connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        contentModel.Id = (int)reader[0];
                        contentModel.Title = reader[1].ToString().Trim();
                        contentModel.TextDescription = reader[2].ToString().Trim();
                        contentModel.ShortDescription = reader[3].ToString().Trim();
                        contentModel.UserEmail = reader[4].ToString().Trim();
                        contentModel.SharedUsers = reader[5].ToString().Trim();
                        _contentList.Add(contentModel);

                    }
                }
                ViewBag.content = _contentList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(ViewBag);
        }

        [HttpPost]
        public ActionResult Update(TextEditorModel textEditorModel)
        {
            Connection();
            return View();
        }

        
    }
}
