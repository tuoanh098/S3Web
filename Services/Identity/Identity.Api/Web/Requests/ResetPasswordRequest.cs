namespace Identity.Api.Web.Requests;
public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
