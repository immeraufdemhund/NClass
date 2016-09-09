// NClass - Free class diagram editor
// Copyright (C) 2006-2009 Balazs Tihanyi
// 
// This program is free software; you can redistribute it and/or modify it under 
// the terms of the GNU General Public License as published by the Free Software 
// Foundation; either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// this program; if not, write to the Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using log4net;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NClass.Core
{
    public sealed class LanguageManager
    {
        public static readonly LanguageManager Instance = new LanguageManager();
        private static readonly ILog Log = LogManager.GetLogger(typeof(LanguageManager));

        private readonly ICollection<IExposeLanguage> _loadedLanguages;
        private LanguageManager()
        {
            _loadedLanguages = new List<IExposeLanguage>();
        }

        public void LoadExposedLanguages(DirectoryInfo directoryInfo)
        {
            if(!directoryInfo.Exists)
            {
                Log.Warn($"{directoryInfo.FullName} does not exist so it will not be checked for Exposed Languages");
                return;
            }

            foreach (var file in directoryInfo.EnumerateFiles())
            {
                try
                {
                    var type = typeof(IExposeLanguage);
                    var loadedAssembly = Assembly.LoadFile(file.FullName);
                    foreach (var exposedLanguageType in loadedAssembly.GetTypes().Where(t => type.IsAssignableFrom(t)))
                    {
                        Log.Info($"Trying to activate {exposedLanguageType.FullName}");
                        var exposedLanguage = (IExposeLanguage) Activator.CreateInstance(exposedLanguageType);
                            _loadedLanguages.Add(exposedLanguage);
                    }
                }
                catch (FileLoadException) { Log.Warn($"{file.FullName} has already been loaded"); }
                catch (BadImageFormatException) { Log.Debug($"{file.FullName} is not an assembly."); }
                catch (ReflectionTypeLoadException ex) { Log.Error($"Error Activating a type in {file.FullName}", ex); }
            }
        }

        public IEnumerable<string> GetAll()
        {
            return _loadedLanguages.Select(x=>x.LanguageName);
        }

        public Language GetLanguageInstance(string name)
        {
            return _loadedLanguages.SingleOrDefault(x=>x.LanguageName == name).Instance;
        }
    }
}