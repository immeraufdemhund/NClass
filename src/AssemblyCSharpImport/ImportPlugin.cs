using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using NClass.AssemblyCSharpImport.Lang;
using NClass.CSharp;
using NClass.DiagramEditor.ClassDiagram;
using NClass.GUI;

namespace NClass.AssemblyCSharpImport
{
    /// <summary>
    ///     Implements the PlugIn-Interface of NClass.
    /// </summary>
    public class ImportPlugin : Plugin
    {
        /// <summary>
        ///     Gets a value indicating whether the plugin can be executed at the moment.
        /// </summary>
        public override bool IsAvailable => Workspace.HasActiveProject;

        /// <summary>
        ///     Gets the menu item used to start the plugin.
        /// </summary>
        public sealed override ToolStripItem MenuItem { get; }

        /// <summary>
        ///     Starts the export.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Additional information.</param>
        private void menuItem_Click(object sender, EventArgs e)
        {
            Launch();
        }

        /// <summary>
        ///     Set up the current culture for the strings.
        /// </summary>
        static ImportPlugin()
        {
            try
            {
                Strings.Culture = CultureInfo.GetCultureInfo(Settings.Default.UILanguage);
            }
            catch (ArgumentException)
            {
                //Culture is not supported, maybe the setting is "default".
            }
        }

        /// <summary>
        ///     Constructs a new instance of NETImportPlugin.
        /// </summary>
        /// <param name="environment">An instance of NClassEnvironment.</param>
        public ImportPlugin(NClassEnvironment environment)
            : base(environment)
        {
            MenuItem = new ToolStripMenuItem
            {
                Text = Strings.Menu_Title,
                ToolTipText = Strings.Menu_ToolTip
            };
            MenuItem.Click += menuItem_Click;
        }

        /// <summary>
        ///     Starts the functionality of the plugin.
        /// </summary>
        protected void Launch()
        {
            if (!Workspace.HasActiveProject)
                return;

            var settings = new ImportSettings();
            using (var settingsForm = new ImportSettingsForm(settings))
            {
                if (settingsForm.ShowDialog() != DialogResult.OK)
                    return;
            }
            var diagram = new Diagram(CSharpLanguage.Instance);

            // Is it a file or a folder?
            foreach (var item in settings.Items)
            {
                // Analyse items to know if it is :
                // a C# source file
                // a folder
                // a .NET assembly
                if (Path.HasExtension(item))
                {
                    switch (Path.GetExtension(item))
                    {
                        case ".cs":
                            if (File.Exists(item))
                            {
                                if (settings.NewDiagram)
                                    diagram = new Diagram(CSharpLanguage.Instance);

                                ImportCSharpFile(diagram, item);
                            }
                            break;
                        case ".dll":
                        case ".exe":
                            if (File.Exists(item))
                            {
                                if (settings.NewDiagram)
                                    diagram = new Diagram(CSharpLanguage.Instance);

                                ImportAssembly(diagram, settings, item);
                            }
                            break;
                        case ".sln":
                            if (File.Exists(item))
                            {
                                if (settings.NewDiagram)
                                    diagram = new Diagram(CSharpLanguage.Instance);

                                ImportVisualStudioSolution(diagram, settings, item);
                            }
                            break;
                        case ".csproj":
                            if (File.Exists(item))
                            {
                                if (settings.NewDiagram)
                                    diagram = new Diagram(CSharpLanguage.Instance);

                                ImportVisualStudioProject(diagram, settings, item);
                            }
                            break;
                    }
                }
                else
                {
                    if (Directory.Exists(item))
                        ImportFolder(diagram, item);
                }
            }
        }

        /// <summary>
        ///     Import a C# code source file.
        /// </summary>
        private void ImportCSharpFile(Diagram diagram, string fileName)
        {
            var importer = new CSharpImport(diagram);

            if (importer.ImportSourceCode(fileName))
                Workspace.ActiveProject.Add(diagram);
        }

        /// <summary>
        ///     Import all C# code source files in a folder and its subfolders.
        /// </summary>
        private void ImportFolder(Diagram diagram, string folderName)
        {
            // All C# code source file in this directory 
            foreach (var file in Directory.EnumerateFiles(folderName, "*.cs"))
                ImportCSharpFile(diagram, file);

            // All subfolders in this directory 
            foreach (var folder in Directory.EnumerateDirectories(folderName))
                ImportCSharpFile(diagram, folder);
        }

        /// <summary>
        ///     Import a .NET assembly.
        /// </summary>
        private void ImportAssembly(Diagram diagram, ImportSettings settings, string fileName)
        {
            var importer = new NETImport(diagram, settings);

            if (importer.ImportAssembly(fileName))
            {
                Workspace.ActiveProject.Add(diagram);
            }
        }

        /// <summary>
        ///     Import a Visual Studio Solution.
        /// </summary>
        private void ImportVisualStudioSolution(Diagram diagram, ImportSettings settings, string fileName)
        {
            // TODO
            // http://stackoverflow.com/questions/707107/library-for-parsing-visual-studio-solution-files
            // MonoDevelop.Projects.Formats.MSBuild
            // SlnFileFormat.cs
        }

        /// <summary>
        ///     Import a Visual Studio Project.
        /// </summary>
        private void ImportVisualStudioProject(Diagram diagram, ImportSettings settings, string fileName)
        {
            // TODO
        }
    }
}