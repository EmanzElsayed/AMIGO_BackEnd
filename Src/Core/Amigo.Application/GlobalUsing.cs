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

global using Microsoft.EntityFrameworkCore;

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


global using Amigo.Application.Abstraction.MappingInterfaces;
global using Amigo.Application.Mapping;
global using Amigo.Application.Specifications.DestinationSpecification;
global using Amigo.Application.Validation.Common.Rules;
global using Amigo.Domain.Abstraction;
global using Amigo.Domain.DTO.Destination;
global using Amigo.Domain.Entities;
global using Amigo.Domain.Entities.TranslationEntities;
global using Amigo.Domain.Enum;
global using Amigo.Domain.Errors;

global using Amigo.SharedKernal.DTOs.Destination;
global using Amigo.SharedKernal.DTOs.Results;
global using Amigo.SharedKernal.QueryParams;

global using Amigo.Application.Abstraction.Services.Admin;
global using Amigo.Application.Specifications.DestinationSpecification.User;
 global using Amigo.Application.Specifications.DestinationSpecification.Admin;

#endregion