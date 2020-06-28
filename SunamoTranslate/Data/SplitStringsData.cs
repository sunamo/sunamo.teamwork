using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Is allowed to created only in ListBoxResultContextMenuHandler. 
/// In all other location (like APSHelper) must pass through method
/// </summary>
public class SplitStringsData
{
    int serieOfInstance = 0;
    int serieOfInstanceStatic = 0;
    public static SplitStringsData Instance = new SplitStringsData();

    public bool makeAnyChanges = false;
    public bool reallyChanged = false;

    private SplitStringsData()
    {
        serieOfInstanceStatic++;
        serieOfInstance = serieOfInstanceStatic;
    }

    /// <summary>
    /// Only translateable strings as is filled in GetBetween
    /// </summary>
    public CollectionWithoutDuplicates<string> bet = new CollectionWithoutDuplicates<string>();
    /// <summary>
    /// Replace all call ctor
    /// </summary>
    public CollectionWithoutDuplicates<string> notToTranslate = new CollectionWithoutDuplicates<string>();
    public Dictionary<string, StringPaddingData> v = new Dictionary<string, StringPaddingData>();

    StringBuilder sbMessage = new StringBuilder();

    public string GenerateMessage()
    {
        sbMessage.Clear();
        if (makeAnyChanges)
        {
            sbMessage.Append("In memory was some changes. ");
            if (reallyChanged)
            {
                sbMessage.Append("File is really different than on begin and saved to drive.");
            }
            else
            {
                sbMessage.Append("File wasn't different, therefore won't be saved to drive.");
            }
        }
        else
        {
            // keep as
        }

        return sbMessage.ToString();
    }
}

