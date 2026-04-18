using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Enum
{
    public enum RefundStatus
    {
        Pending = 1,      // تم إنشاء الطلب
        Processing = 2,   // جاري التنفيذ مع بوابة الدفع
        Succeeded = 3,    // تم رد المبلغ
        Failed = 4,       // فشل
        Rejected = 5,     // مرفوض حسب السياسة
        Cancelled = 6,    // تم إلغاء طلب الاسترجاع
        Partial = 7       // استرجاع جزئي
    }
}
