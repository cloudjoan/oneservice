using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Session;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

#region windows auth

//builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//   .AddNegotiate();

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = options.DefaultPolicy;
//});

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
		.AddNegotiate(options =>
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				options.EnableLdap(settings =>
				{
					//settings.Domain = "etatung.com.tw";
					//settings.MachineAccountName = "bpmadmin";
					//settings.MachineAccountPassword = "Bpm@dmin";

					settings.Domain = "etatung.com.tw";
					var ldapConnection = new LdapConnection(
						new LdapDirectoryIdentifier("139.223.14.100"),
						new NetworkCredential("bpmadmin", "Bpm@dmin"), AuthType.Basic);
					ldapConnection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
					settings.LdapConnection = ldapConnection;
				});
			}
		});

#endregion

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(3600 * 3);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
