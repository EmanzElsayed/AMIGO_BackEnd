using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.QueryParams
{
    public record GetAllCurrencyQuery
    (
         string? CurrencyCode ,
         string? Name ,
         string? Language 

    );
}
