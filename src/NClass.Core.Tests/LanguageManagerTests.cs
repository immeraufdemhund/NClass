using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace NClass.Core
{
    [TestFixture]
    public class LanguageManagerTests
    {
        private static readonly LanguageManager _languageManager = LanguageManager.Instance;

        [Test]
        public void WithEmptyFolder_NoLanguagesAreLoaded()
        {
            var languages = GetAllExposedProgrammingLanguages(@"C:\IDontExist");

            Assert.That(languages, Is.Empty);
        }

        [Test]
        public void WhenDirectoryHasAssemblyWithLanguageMarkerInterface_LanguageNameIsReturned()
        {
            var expectedLanguageList = new string[] { "TestLanguage" };

            var languages = GetAllExposedProgrammingLanguages(TestContext.CurrentContext.TestDirectory);

            Assert.That(languages, Is.EquivalentTo(expectedLanguageList));
        }

        private static List<string> GetAllExposedProgrammingLanguages(string path)
        {
            _languageManager.LoadExposedLanguages(new DirectoryInfo(path));

            var installedLanguages = _languageManager.GetAll().ToList();
            return installedLanguages;
        }
    }

    public class TestExposeLanguage : IExposeLanguage
    {
        public Language Instance => null;

        public string LanguageName => "TestLanguage";
    }
}
