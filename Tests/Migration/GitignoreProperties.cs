using System;
using System.IO;
using System.Linq;
using FsCheck;
using NUnit.Framework;

namespace Webium.Tests.Migration
{
    /// <summary>
    /// Property-based tests for .gitignore cleanup.
    /// Feature: 001-upm-package-migration, Property 4: .gitignore contains no obsolete submodule rules
    /// </summary>
    [TestFixture]
    public class GitignoreProperties
    {
        private string _repoRoot;
        private string[] _gitignoreLines;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Walk up from the test output directory until we find .gitignore
            var dir = TestContext.CurrentContext.TestDirectory;
            while (dir != null && !File.Exists(Path.Combine(dir, ".gitignore")))
                dir = Path.GetDirectoryName(dir);

            _repoRoot = dir ?? Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", ".."));

            var gitignorePath = Path.Combine(_repoRoot, ".gitignore");

            Assert.That(File.Exists(gitignorePath), Is.True,
                $".gitignore not found at {gitignorePath}");

            _gitignoreLines = File.ReadAllLines(gitignorePath);
            Assert.That(_gitignoreLines.Length, Is.GreaterThan(0),
                ".gitignore is empty");
        }

        /// <summary>
        /// Property 4: .gitignore contains no obsolete submodule rules
        ///
        /// For any line in the .gitignore file, the line must not match the
        /// pattern "Assets/Webium" (case-insensitive, to catch patterns like
        /// [Aa]ssets/Webium). This ensures the cleanup fully removed all
        /// submodule-era ignore rules.
        ///
        /// **Validates: Requirements 9.1**
        /// </summary>
        [Test]
        public void Gitignore_NoLineContainsObsoleteSubmoduleRule()
        {
            var config = Configuration.QuickThrowOnFailure;
            config.MaxNbOfTest = Math.Max(100, _gitignoreLines.Length);

            var lineIndexGen = Gen.Choose(0, _gitignoreLines.Length - 1);

            Prop.ForAll(
                lineIndexGen.ToArbitrary(),
                (lineIndex) =>
                {
                    var line = _gitignoreLines[lineIndex];
                    Assert.That(
                        line.IndexOf("Assets/Webium", StringComparison.OrdinalIgnoreCase),
                        Is.LessThan(0),
                        $".gitignore line {lineIndex + 1} contains obsolete submodule rule: {line}");
                }
            ).Check(config);
        }
    }
}
