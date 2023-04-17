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
		public static string UserEmail = string.Empty;

		// Database Configuration
		public TextEditorController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		private SqlConnection _connection;
		private List<TextEditorModel> _contentList = new List<TextEditorModel>();


		// Connection to Database
		public void Connection()
		{
			string conn = _configuration.GetConnectionString("TextEditorDB");
			_connection = new SqlConnection(conn);
			_connection.Open();
		}
		// GET: TextEditorController  Index Function for listing the Content
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



			return View();
		}


		[HttpPost]
		// Http : Post
		// To get userEmail from user
		// Param : UserEmail
		public IActionResult Index(string userEmail)
		{
			UserEmail = userEmail.Trim();
			return RedirectToAction("Index");
		}

		// GET: TextEditorController1/Create To view the create page
		public ActionResult Create()
		{
            
            return View();
		}


		[HttpPost]
		// Post request to insert data into database
		public ActionResult Create(TextEditorModel textEditorModel)
		{
			Connection();
			string addJobQuery = $"INSERT INTO TextEditor VALUES('{textEditorModel.Title}','{textEditorModel.TextDescription}','{textEditorModel.ShortDescription}','{textEditorModel.UserEmail}','{textEditorModel.SharedUsers}')";


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




		// GET:
		// Delete a record in the DB using Id
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
		// HTTP : Get
		// Get a particular Record in the DB 
		// Param : ID
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
						contentModel.Id = id;
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
		// Http : Post
		// Update a record in the DB
		// Param : TextEditorModel
		public ActionResult Update(TextEditorModel textEditorModel)
		{
			Connection();

			string updateQuery = $"UPDATE TextEditor SET Title='{textEditorModel.Title}',ShortDescription='{textEditorModel.ShortDescription}'," +
				$"TextDescription='{textEditorModel.TextDescription}',ShareUsers='{textEditorModel.SharedUsers}' WHERE " +
				$"Id = {textEditorModel.Id}";

			try
			{
				using (SqlCommand cmd = new SqlCommand(updateQuery, _connection))
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
	}
}
