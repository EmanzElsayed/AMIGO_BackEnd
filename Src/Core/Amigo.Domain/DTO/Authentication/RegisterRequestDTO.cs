using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Authentication;

public record RegisterRequestDTO
(
     string Email,
     string Password,
     string ConfirmPassword,
     string PhoneNumber,
     string UserName,
     string FullName,
     string? Image,
     string Gender,
     DateOnly BirthDate,
     string Nationality,
     string Language,
     int BuildingNumber,
     string City,
     string Country,
     bool TermsAccepted
);
