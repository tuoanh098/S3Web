namespace Identity.Api.Web.Requests;
public sealed record RegisterRequest(string Email, string Password, string Role);