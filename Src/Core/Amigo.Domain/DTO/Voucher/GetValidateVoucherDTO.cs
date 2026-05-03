using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Voucher
{
    public record GetValidateVoucherDTO
    (
      string VoucherNumber,
      string Status,
      DateTime IssuedAt
    );
}
