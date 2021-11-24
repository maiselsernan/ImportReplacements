using System.Collections.Generic;

namespace ImportReplacement.Web.Shared
{
   public static class Utils
    {
        public static IEnumerable<string> GetDescriptions(long code)
        {
            if ((code & 0x100000L) == 0x100000L) yield return "המד נעלם (21)";
            if ((code & 0x000001L) == 0x000001L) yield return "כתובת לא נכונה (01)";
            if ((code & 0x000002L) == 0x000002L) yield return "מקום סגור 9 (02)";
            if ((code & 0x000004L) == 0x000004L) yield return "אין גישה (03)";
            if ((code & 0x000008L) == 0x000008L) yield return "כלב (04)";
            if ((code & 0x001000L) == 0x001000L) yield return "המונה מנותק מהצנרת (13)";
            if ((code & 0x010000L) == 0x010000L) yield return "ניתוק מד גינתי (17)";
            if ((code & 0x002000L) == 0x002000L) yield return "גניבת מים מרשת משותפת (14)";
            if ((code & 0x004000L) == 0x004000L) yield return "מונה חדש (15)";
            if ((code & 0x000010L) == 0x000010L) yield return "המונה מזיע";
            if ((code & 0x080000L) == 0x080000L) yield return "מד פגום";
            if ((code & 0x000020L) == 0x000020L) yield return "ברז לא סוגר";
            if ((code & 0x000040L) == 0x000040L) yield return "אין ברז";
            if ((code & 0x000080L) == 0x000080L) yield return "חלודה";
            if ((code & 0x008000L) == 0x008000L) yield return "בוררות";
            if ((code & 0x000100L) == 0x000100L) yield return "עצור (9)";
            if ((code & 0x000200L) == 0x000200L) yield return "אין שימוש";
            if ((code & 0x000400L) == 0x000400L) yield return "התמונה לא של המד";
            if ((code & 0x020000L) == 0x020000L) yield return "המונה הפוך";
            if ((code & 0x040000L) == 0x040000L) yield return "אין תקשורת";
            if ((code & 0x200000L) == 0x200000L) yield return "ברז סגור";
            if ((code & 0x400000L) == 0x400000L) yield return "התקנה לפי דרישה";
            if ((code & 0x800000L) == 0x800000L) yield return "ניתוק לפי דרישה";
            if ((code & 0x0000100000000L) == 0x0000100000000L) yield return "הוספת פשטיק";
            if ((code & 0x0000200000000L) == 0x0000200000000L) yield return "הקטנת קוטר";
            if ((code & 0x0000400000000L) == 0x0000400000000L) yield return "היעדר גישה – מד גבוה המצריך סולם";
            if ((code & 0x0001000000000L) == 0x0001000000000L) yield return "הוספת הארקה";
            if ((code & 0x0004000000000L) == 0x0004000000000L) yield return "צנרת ארוכה – דורש  עבודת קיצור ";
            if ((code & 0x0008000000000L) == 0x0008000000000L) yield return "צנרת קצרה – דורש עבודת הארכה";
            if ((code & 0x0010000000000L) == 0x0010000000000L) yield return "ברגים מרותכים לצנרת";
            if ((code & 0x0020000000000L) == 0x0020000000000L) yield return "רקורד תקוע";
            if ((code & 0x0040000000000L) == 0x0040000000000L) yield return "התקנת ברז לפני / אחרי המד";
            if ((code & 0x0080000000000L) == 0x0080000000000L) yield return "כמות חורים לא מתאימה בפלאנץ''";
            if ((code & 0x0100000000000L) == 0x0100000000000L) yield return "התקנת לוכד אבנים";
        }

    }
}
