
using sunamo.Essential;
using sunamo.Html;
using sunamo.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class TranslateAbleHelper
{
    public static CollectionWithoutDuplicates<string> toTranslate = new CollectionWithoutDuplicates<string>();
    public static List<string> notToTranslate = new List<string>();

    public static bool outsideReplaceBadChars = false;

    private static bool s_result;
    private static bool result
    {
        set
        {
            if (!value)
            {
                if (outsideReplaceBadChars)
                {
                    notToTranslate.Add(s_between);
                }
            }

            s_result = value;
        }
        get
        {
            return s_result;
        }
    }

    private static string s_between = null;
    private static SplitStringsData s_splitStringsData = null;
    public static BoolString isNameOfControl = null;
    public static bool IsToTranslate(string text)
    {
        return IsToTranslate(null, text, 0, null);
    }
}