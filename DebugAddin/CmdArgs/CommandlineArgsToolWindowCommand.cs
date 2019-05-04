﻿using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DebugAddin.CmdArgs
  {
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class CommandlineArgsToolWindowCommand
    {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0100;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("0a89b57b-a57a-4f93-9588-8570d141738a");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly Package package;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandlineArgsToolWindowCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private CommandlineArgsToolWindowCommand(Package package)
      {
      if (package == null)
        {
        throw new ArgumentNullException("package");
        }

      this.package = package;

      OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
      if (commandService != null)
        {
        var menuCommandID = new CommandID(CommandSet, CommandId);
        var menuItem = new MenuCommand(this.ShowToolWindow, menuCommandID);
        commandService.AddCommand(menuItem);
        }
      }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static CommandlineArgsToolWindowCommand Instance
      {
      get;
      private set;
      }

    /// <summary>
    /// Gets the service provider from the owner package.
    /// </summary>
    private IServiceProvider ServiceProvider
      {
      get
        {
        return this.package;
        }
      }

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static void Initialize(Package package)
      {
      Instance = new CommandlineArgsToolWindowCommand(package);
      }

    /// <summary>
    /// Shows the tool window when the menu item is clicked.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    private void ShowToolWindow(object sender, EventArgs e)
      {
      ThreadHelper.ThrowIfNotOnUIThread();
      // Get the instance number 0 of this tool window. This window is single instance so this instance
      // is actually the only one.
      // The last flag is set to true so that if the tool window does not exists it will be created.
      ToolWindowPane window = this.package.FindToolWindow(typeof(CommandlineArgsToolWindow), 0, true);
      if ((null == window) || (null == window.Frame))
        {
        throw new NotSupportedException("Cannot create tool window");
        }

      IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
      Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
      }
    }
  }
