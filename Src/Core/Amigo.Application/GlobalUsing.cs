#region Packages

global using FluentResults;
global using FluentValidation;
global using Microsoft.AspNetCore.Identity;

global using Microsoft.Extensions.Configuration;
global using Microsoft.IdentityModel.Tokens;

global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Security.Claims;
global using System.Text;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using MimeKit;

#endregion

#region References
global using Amigo.Application.Abstraction;
global using Amigo.Application.Abstraction.Services.Authentication;
global using Amigo.Domain.Abstraction.Repositories;
global using Amigo.Domain.DTO.Authentication;
global using Amigo.Domain.Entities.Identity;

global using Amigo.Domain.Errors.BusinessErrors;
global using Amigo.Domain.Extension;
global using Amigo.SharedKernal.DTOs.Authentication;

global using Amigo.Application.Abstraction.Services;
global using Amigo.SharedKernal.DTOs.Enum;


#endregion