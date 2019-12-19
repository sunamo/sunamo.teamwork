using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Construction;
using SlnGen.Common;

public class SunamoSlnGenHelper
    {
    /// <summary>
    /// Working wit Microsoft.Build.Construction.SolutionFile which cant add new files
    /// </summary>
    /// <param name="solutionFilePath"></param>
    /// <returns></returns>
    public static List<string> GetProjects(string solutionFilePath)
    {
        SolutionFile solutionFile = SolutionFile.Parse(solutionFilePath);


        var p = solutionFile.ProjectsByGuid;
        return p.Select(d => d.Value.AbsolutePath).ToList();   
    }

    public static List<string> s()
    {
        SlnFile sln = new SlnFile();
        sln.Lo
    }
}

