﻿using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugAddin.CmdArgs
  {
  static class Utils
    {
    public static IEnumerable<T[]> TupleUp<T>(this IEnumerable<T> source, int count)
      {
      using (var iterator = source.GetEnumerator())
        {
        while (true)
          {
          T[] array = new T[count];
          for (int i=0; i<count; ++i)
            {
            if (iterator.MoveNext() == false)
              goto End;
            array[i] = iterator.Current;
            }
          yield return array;
          }
        }
      End: ;
      }

    static private Dictionary<string, Guid> panes = new Dictionary<string, Guid>();
    static public void PrintMessage(string from, string message, bool activate = false)
      {
      Guid guid;
      IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
      if (panes.TryGetValue(from, out guid) == false)
        {
        guid = Guid.NewGuid();
        outWindow.CreatePane(ref guid, from, 1, 1);
        panes.Add(from, guid);
        }
      IVsOutputWindowPane debugPane;
      outWindow.GetPane(ref guid, out debugPane);
      debugPane.OutputString(message + "\n");
      if (activate)
        debugPane.Activate();
      }

    static public IList<Project> GetAllProjectsInSolution()
      {
      DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));
      Projects projects = dte.Solution.Projects;
      List<Project> list = new List<Project>();
      var item = projects.GetEnumerator();
      while (item.MoveNext())
        {
        var project = item.Current as Project;
        if (project == null)
          {
          continue;
          }

        if (project.Kind == EnvDTE.Constants.vsProjectKindSolutionItems)
          {
          list.AddRange(GetSolutionFolderProjects(project));
          }
        else
          {
          list.Add(project);
          }
        }

      return list;
      }

    static private IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder)
      {
      List<Project> list = new List<Project>();
      for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
        {
        var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
        if (subProject == null)
          {
          continue;
          }

        // If this is another solution folder, do a recursive call, otherwise add
        if (subProject.Kind == EnvDTE.Constants.vsProjectKindSolutionItems)
          {
          list.AddRange(GetSolutionFolderProjects(subProject));
          }
        else
          {
          list.Add(subProject);
          }
        }
      return list;
      }

    static public UIHierarchyItem FindUIHierarchyItem(Project pi)
      {
      UIHierarchyItem retVal = null;
      DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));
      foreach (UIHierarchyItem hierarchyItem in dte.ToolWindows.SolutionExplorer.UIHierarchyItems)
        {
        if (hierarchyItem.Object == pi)
          retVal = hierarchyItem;
        else
          retVal = Find(hierarchyItem, pi);
        if (retVal != null)
          break;
        }
      return retVal;
      }

    static private UIHierarchyItem Find(UIHierarchyItem hierarchyItem, Project pi)
      {
      UIHierarchyItem retVal = null;
      foreach (UIHierarchyItem childItem in hierarchyItem.UIHierarchyItems)
        {
        if (childItem.Object == pi)
          retVal = childItem;
        else
          retVal = Find(childItem, pi);
        if (retVal != null)
          break;
        }
      return retVal;
      }
    }
  }
