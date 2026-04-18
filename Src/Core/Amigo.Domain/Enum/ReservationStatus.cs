using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Enum
{
    public enum ReservationStatus
    {
        Pending = 0,     // تم عمل reservation لكن لم يتم الدفع
        Confirmed = 1,   // الدفع تم بنجاح وتم تثبيت الحجز
        Cancelled = 2,   // المستخدم ألغى أو الدفع فشل
        Expired = 3      // انتهى وقت الحجز (15 min timeout)
    }
}
