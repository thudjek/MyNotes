﻿namespace SharedModels.Requests.Auth;
public class ConfirmEmailRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}