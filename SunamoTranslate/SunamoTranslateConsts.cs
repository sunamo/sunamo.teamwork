﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sunamo.Values;

public class SunamoTranslateConsts
{


    /// <summary>
    /// new google.maps.LatLng ( etc.
    /// </summary>
    public static List<string> newL = new List<string>(26);
    public static List<string> newU = new List<string>(26);

    static SunamoTranslateConsts()
    {
        allBasicTypes = Types.allBasicTypes.Select(d => d.Name).ToList();
        allBasicTypesFull = Types.allBasicTypes.Select(d => d.FullName).ToList();
    }

    public static PpkOnDrive alwaysStringsToTranslate = new PpkOnDrive(AppData.ci.GetFile(AppFolders.Settings, "alwaysStringsTranslateStrings.txt"));
    public static PpkOnDrive stringsNotToTranslate = new PpkOnDrive(AppData.ci.GetFile(AppFolders.Settings, "notToTranslateStrings.txt"));
    public static PpkOnDrive extensionsAvailableToTranslate = new PpkOnDrive(AppData.ci.GetFile(AppFolders.Settings, "extensionsAvailableToTranslate.txt"));

    public static List<string> sqlKeywords
    {
        get
        {
            return new List<string>(s_sqlKeywords);
        }
    }

    public static List<string> allBasicTypes = null;
    public static List<string> allBasicTypesFull = null;

    /// <summary>
    /// Every here have to be delimited in code by space
    /// none string can contains space (event inner join and so)
    /// NO (,) and other non letter chars
    /// </summary>
    private static List<string> s_sqlKeywords = CA.ToListString("sum", "id", "object", "objectproperty", "select", "in", "update", "delete", "insert", "inner", "join", "from", "group", "by", "top", "sysobjects", "*", "where", "and", "exec", "set", "newid");
}
