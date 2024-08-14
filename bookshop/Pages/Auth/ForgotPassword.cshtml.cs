using bookshop.Myhelper; // Import the namespace for EmailSender
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace bookshop.Pages.Auth
{
	[RequireNoAuth]
	public class ForgotPasswordModel : PageModel
	{
		[BindProperty, Required(ErrorMessage = "The Email is required"), EmailAddress]
		public string Email { get; set; } = "";

		public string errorMessage = "";
		public string successMessage = "";

		public async Task OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				errorMessage = "Data validation failed";
				return;
			}

			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=bookstore;Integrated Security=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync(); // Use async OpenAsync() for better performance

					string sql = "SELECT * FROM users WHERE email=@Email";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@Email", Email);

						using (SqlDataReader reader = await command.ExecuteReaderAsync())
						{
							if (await reader.ReadAsync())
							{
								string firstname = reader.GetString(1);
								string lastname = reader.GetString(2);

								string token = Guid.NewGuid().ToString();

								// Save the token in the database
								SaveToken(Email, token);

								// Send the token by email to the user
								string resetUrl = $"https://localhost:7112/Auth/ResetPassword?token={token}";
								string username = firstname + " " + lastname;
								string subject = "Password Reset";
								string message = $"Dear {username},\n\n" +
												 $"You can reset your password using the following link:\n\n" +
												 $"{resetUrl}\n\n" +
												 "Best regards";

								// Call EmailSender to send email
								await EmailSender.SendEmail(Email, username, subject, message);

								successMessage = "Please check email and click on the reset password link";
							}
							else
							{
								errorMessage = "We have no user with this email address";
								return;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message; // Handle exceptions
				return;
			}
		}

		private void SaveToken(string email, string token)
		{
			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=bookstore;Integrated Security=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();

					// Delete any old token for this email address from the database
					string sql = "DELETE FROM password_resets WHERE email=@Email";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@Email", email);

						command.ExecuteNonQuery();
					}

					// Add token to the database
					sql = "INSERT INTO password_resets (email, token) VALUES (@Email, @Token)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@Email", email);
						command.Parameters.AddWithValue("@Token", token);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message); // Handle database errors
			}
		}
	}
}
