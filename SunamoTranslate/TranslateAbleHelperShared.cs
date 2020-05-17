using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using sunamo.Essential;

public partial class TranslateAbleHelper
{
    /// <summary>
    /// Before usage is needed replace all chars like ¢
    /// A3 is to avoid creating still TextBox
    /// A2/3 can be null - in any case dont use, strange behaviour
    /// </summary>
    /// <param name="between"></param>
    /// <param name="charIndex"></param>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static bool IsToTranslate(SplitStringsData splitStringsData, string between, int charIndex, List<string> lines, AllowStrings allowString = null)
    {
        if (allowString == null)
        {
            allowString = new AllowStrings();
        }

        between = between.Trim();
        result = true;
        s_between = between;
        s_splitStringsData = splitStringsData;
        Dictionary<string, StringPaddingData> v = null;

        if (splitStringsData != null)
        {
            v = splitStringsData.v;
        }

        if (SH.ContainsNewLine(between) && between.Contains("|"))
        {
            // string with data 
            result = false; return result;
        }

        if (between.Length == 0)
        {
            result = false; return result;
        }

        // REPLACEMENT CHARACTER
        // used to replace an incoming character whose value is unknown or unrepresentable in Unicode
        // compare the use of U + 001A as a control character to indicate the substitute function
        if (between.Contains('\uFFFD'.ToString()))
        {
            result = false; return result;
        }

        ThisApp.check = false;

        bool isCzech = false;
        // extremely expensive for memory and cpu, instead of several second do the same a few minutes
        //isCzech = TextLang.IsCzech(between); 
        isCzech = SH.ContainsDiacritic(between);

        var lower = between.ToLower();
        var lowerT = lower.Trim();

        if (SunamoTranslateConsts.alwaysStringsToTranslate.Contains(lowerT))
        {
            result = true; return result;
        }

        // TODO: Should to manually decide, can be name of class but also (czech) word 
        // Commented coz also words like Hello, beautiful marked as not to translate
        //if (ConvertCamelConventionWithNumbers.IsCamelWithNumber(between) && !SH.ContainsDiacritic(between))
        //{
        //    result = false; return result;
        //}

        foreach (var item in SunamoTranslateConsts.newU)
        {
            if (between.Contains(item))
            {
                result = false;
                return result;
            }
        }

        foreach (var item in SunamoTranslateConsts.newL)
        {
            if (between.Contains(item))
            {
                result = false;
                return result;
            }
        }

        if (HtmlHelperText.IsHtmlEntity(between))
        {
            result = false; return result;
        }

        var deli = CA.ToList<char>(SH.spaceAndPuntactionChars);
        deli.Add('(');
        deli.Add(')');

        var tokens = SH.Split(between, deli);

        if (!isCzech)
        {
            bool isAllCamel = true;

            foreach (var item in tokens)
            {
                if (!ConvertCamelConventionWithNumbers.IsCamelWithNumber(item))
                {
                    isAllCamel = false;
                    break;
                }
            }

            if (isAllCamel)
            {
                result = false; return result;
            }
        }

        var tokensL = SH.Split(lower, deli);

        

        var betweenT = between.Trim();

        if (betweenT.Length < 4)
        {
            result = false; return result;
        }

        #region Wildcard
       
        #endregion

        #region Special formats - uri, guid, etc.
        #region Contains mail
        if (tokens.Count < 4)
        {
            foreach (var item in tokens)
            {
                if (RegexHelper.IsEmail(item))
                {
                    result = false; return result;
                }
            }
        }
        #endregion

        if (RegexHelper.rHtmlTag.IsMatch(between))
        {
            result = false; return result;
        }

        if (RegexHelper.IsUri(between))
        {
            result = false; return result;
        }

        if (RegexHelper.isGuid.IsMatch(between))
        {
            result = false; return result;
        }
        #endregion

        #region Code
        for (int i = 0; i < betweenT.Length; i++)
        {
            if (char.IsLetterOrDigit(betweenT[i]))
            {
                continue;
            }
            else
            {
                if (betweenT[i] == AllChars.colon)
                {
                    result = false;
                    return result;
                }
                else
                {
                    break;
                }
            }
        }
        #endregion

        Dictionary<char, int> lettersCount = SH.StatisticLetterChars(lower, StatisticLetterCharsStrategy.IgnoreCompletely, '.', AllChars.space, AllChars.slash, 'm', 'd', 'y');
        if (lettersCount.Count == 0)
        {
            result = false; return result;
        }
    
            // Must again coz in first I ignored slash
            lettersCount = SH.StatisticLetterChars(lower, StatisticLetterCharsStrategy.AddAsFirst,  AllChars.bs, AllChars.slash);

            if (lettersCount.ContainsKey(AllChars.slash))
            {
                // /i/Apps
                
                    result = false; return result;
                
            }
            else if (lettersCount.ContainsKey(AllChars.bs))
            {
                
                    result = false; return result;
                
            }

        // Height(4,1), Height (4,1)
        //if (Wildcard.IsMatch(between, Regex.Escape("*(*,*)")))
        var lettersSqlTable = CA.ToList<char>(AllChars.comma, AllChars.lb, AllChars.rb);
        lettersCount = SH.StatisticLetterChars(lower, StatisticLetterCharsStrategy.AddAsFirst, lettersSqlTable.ToArray());
        if(lettersCount[AllChars.comma] == 1 && lettersCount[AllChars.lb] == 1 && lettersCount[AllChars.rb] ==1)
        {
            foreach (var item in lettersSqlTable)
            {
                lettersCount.Remove(item);
            }

            bool allLettersDigitOrSpace = true;

            foreach (var item in lettersCount)
            {
                if (!char.IsLetterOrDigit(item.Key) && item.Key != AllChars.space)
                {
                    allLettersDigitOrSpace = false;
                    break;
                }
            }

            if (allLettersDigitOrSpace)
            {
                result = false; return result;
            }
            
        }


        #region ComplexInfoString
        ComplexInfoString s = new ComplexInfoString(between);

        // Dont contains any letter
        if (!(s.QuantityUpperChars > 1 || s.QuantityLowerChars > 1))
        {
            result = false; return result;
        }

        // Pascal convention - name of class
        if ((s.QuantityUpperChars > 1 && s.QuantityLowerChars > 1 && !between.Contains(AllStrings.space)))
        {
            result = false; return result;
        }

        // 
        //if (s.QuantityUpperChars > 0 && s.QuantitySpecialChars == 0 && s.QuantityNumbers == 0 && s.QuantityLowerChars == 0)
        //{
        //    result = false; return result;
        //}

        // 40 chars ID, NSLT, SELECT FROM {0} ...
        //&& between.Length == 40 && !between.Contains(" ")
        if (s.QuantityLowerChars == 0 && s.QuantityUpperChars > 0 || (s.QuantityUpperChars > 0 && between.Contains("@p0")))
        {
            result = false; return result;
        }
        #endregion

        #region Contains
        if (!between.Contains(AllStrings.space))
        {
            if (between.Contains(AllStrings.equals) || between.Contains(AllStrings.amp))
            {
                // Dont contains space, I can afford. url arguments
                result = false; return result;
            }
        }

        var lt = CA.ToListString(CA.JoinVariableAndArray(AllChars.dot, AllChars.numericChars));

        CA.RemoveStartingWith("n'", tokensL);
        tokensL.RemoveAll(d => BTS.IsInt(d));

        var includedSqlKeywords = CA.CompareList(tokensL, SunamoTranslateConsts.sqlKeywords);

        if (between.Contains("data-"))
        {
            result = false; return result;
        }

        if (between.Contains(" + "))
        {
            result = false; return result;
        }

        // strings are often splitted, so > 1 must be succufient
        if (includedSqlKeywords.Count > 1 && tokensL.Count <= includedSqlKeywords.Count)
        {
            result = false; return result;
        }

        if (includedSqlKeywords.Count > 0 && !lowerT.Contains(" "))
        {
            result = false; return result;
        }

        // ; expires=Fri, 31 Dec 9999 23:59:59 GMT
        if (lowerT.Contains("expires="))
        {
            result = false; return result;
        }

        // Contains only numbers and/or dot
        if (SH.ContainsAny(between, false, lt).Count == between.Length())
        {
            result = false; return result;
        }

        //if (SH.ContainsAny(between, false, CA.ToListString(")
        //{

        //}

        //&gt;nbsp;
        if (between.Contains("&") && between.Contains(";"))
        {
            result = false; return result;
        }

        // curly bracket 
        if (between.Contains(AllStrings.lcub) && between.Contains(AllStrings.rcub))
        {
            var occl = SH.ReturnOccurencesOfString(between, AllStrings.lcub);
            var occr = SH.ReturnOccurencesOfString(between, AllStrings.rcub);
            if (occl.Count == occr.Count)
            {
                for (int i = 0; i < occl.Count; i++)
                {
                    var from = occl[i];
                    var to = occr[i];

                    if (from > to)
                    {
                        result = false; return result;
                        break;
                    }

                    var bet2 = SH.GetTextBetweenTwoChars(between, from, to);
                    if (bet2.Contains(" "))
                    {
                        result = false; return result;
                        break;
                    }
                }
            }
            else
            {
                result = false; return result;
            }
            if (!between.Contains("{0"))
            {
                // Interpolation strings with variebles are not to translate - most of then is in english
                result = false; return result;
            }
        }

        // backslash
        if (between.Contains(AllStrings.bs))
        {
            result = false; return result;
        }

        if (between.Contains("$(") || between.Contains("jQuery("))
        {
            result = false; return result;
        }

        if (between.Contains(AllStrings.colon))
        {
            var partsColon = SH.Split(SH.Trim(between, ";"), AllStrings.colon);
            CA.Trim(partsColon);
            var decl = partsColon[0];
            if (HtmlHelperText.IsCssDeclarationName(decl))
            {
                result = false; return result;
            }
        }
        #endregion

        #region Special equal
        if (CA.IsEqualToAnyElement<string>(lowerT, CA.ToListString("true", "false", "unplated")))
        {
            result = false; return result;
        }

        if (HtmlHelperText.IsCssDeclarationName(SH.Trim(between, ";")))
        {
            result = false; return result;
        }

        if (SunamoTranslateConsts.allBasicTypes.Contains(between))
        {
            result = false; return result;
        }

        if (SunamoTranslateConsts.allBasicTypesFull.Contains(between))
        {
            result = false; return result;
        }
        #endregion

        #region Comments
        // Wont check for comments, it's return to me nonsense
        //if (lines != null)
        //{
        //    var text = SH.Join(lines, Environment.NewLine);
        //    var line = SH.GetLineIndexFromCharIndex(text, charIndex);
        //    var ba = SH.CharsBeforeAndAfter(text, text[charIndex].ToString(), charIndex, 10, 10);

        //    var ls = lines[line].Trim();

        //    // If i is comment
        //    if (ls.StartsWith("//"))
        //    {
        //        result = false;return result;
        //    }
        //} 
        #endregion

        #region Start/End with
        // Contains awesome or targetsize
        if (CA.StartWith(CA.ToListString("./", "exec", "cmd", "\\uf", "targetsize", "mail:", "asp:", "image-", "http", "og:", "no-", "mif-", "data-", "text/", "fg-", "bg-", "application/", "javascript:"
            ), lower) != null)
        {
            result = false; return result;
        }
        

        if (CA.AnyElementEndsWith(betweenT, ";") && !SH.ContainsDiacritic(betweenT))
        {
            result = false; return result;
        }

        if (!between.Contains(AllStrings.space))
        {
            int from = between.IndexOf(AllChars.lb);
            int to = between.IndexOf(AllChars.rb);

            if (from != -1 && to != -1)
            {
                string b = SH.GetTextBetweenTwoChars(between, from, to);

                if (BTS.IsInt(b))
                {
                    result = false; return result;
                }
            }

            if (between.EndsWith("/"))
            {
                result = false; return result;
            }

            // Nope_
            if (between.EndsWith("_"))
            {
                result = false; return result;
            }

            // Tables.
            if (between.EndsWith(".") && char.IsUpper(between[0]))
            {
                result = false; return result;
            }

            // color
            if (between.StartsWith("#"))
            {
                result = false; return result;
            }

            if (between.StartsWith("0x"))
            {
                result = false; return result;
            }
            else if (between.StartsWith("/") && between.Contains(";"))
            {
                result = false; return result;
            }
            else if (between.StartsWith("mailto:"))
            {
                result = false; return result;
            }
            


            //if (!SH.ContainsOnlyCase(between, true) && !SH.ContainsOnlyCase(between, false))
            //{
            //    // equalTo etc.
            //    result = false;return result;
            //}

        }
        else
        {
             if (between.EndsWith(" ("))
            {
                //Tag (
                result = false; return result;
            }
        }
        #endregion

        

        var lone = lower[0];
        var ltwo = lower[1];
        var one = between[0];
        var two = between[1];

        //<ItemsPanelTemplate   xmlns, <br /, Needed before replace show strings which will be traslate and not - swithc between them
        if (AllChars.lt == one && !allowString.tag)
        {
            result = false; return result;
        }

        // user32.dll
        var ext = FS.GetExtension(lower).TrimStart(AllChars.dot);

        if (AllExtensionsHelper.allExtensionsWithoutDot.ContainsKey(ext))
        {
            result = false; return result;
        }

        if (lowerT == string.Empty)
        {
            result = false; return result;
        }

        if (lower.Contains(".aspx"))
        {
            result = false; return result;
        }

        if (result)
        {
            if (AllHtmlAttrs.list.Contains(lower))
            {
                result = false; return result;
            }
        }

        #region System.Windows.Controls
        if (isNameOfControl.Invoke(between))
        {
            result = false; return result;
        }
        #endregion

        ////DebugLogger.Instance.WriteLine(between);
        ////DebugLogger.Instance.WriteLine("");

        if (outsideReplaceBadChars)
        {
            toTranslate.Add(between);
            if (v != null)
            {
                if (v.ContainsKey(between))
                {
                    v.Add(between, new StringPaddingData());
                }
            }
        }

        return result;
    }
}