using System;
using System.IO;
using System.Linq;
using FsCheck;
using NUnit.Framework;

namespace Webium.Tests.Migration
{
    /// <summary>
    /// Property-based tests for setup script migration.
    /// Feature: 001-upm-package-migration, Property 1: Setup scripts contain no old submodule paths
    /// </summary>
    [TestFixture]
    public class SetupScriptProperties
    {
        private string _repoRoot;
        private string[] _setupPs1Lines;
        private string[] _setupShLines;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Walk up from the test output directory until we find setup.ps1
            var dir = TestContext.CurrentContext.TestDirectory;
            while (dir != null && !File.Exists(Path.Combine(dir, "setup.ps1")))
                dir = Path.GetDirectoryName(dir);

            _repoRoot = dir ?? Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", ".."));

            var ps1Path = Path.Combine(_repoRoot, "setup.ps1");
            var shPath = Path.Combine(_repoRoot, "setup.sh");

            Assert.That(File.Exists(ps1Path), Is.True,
                $"setup.ps1 not found at {ps1Path}");
            Assert.That(File.Exists(shPath), Is.True,
                $"setup.sh not found at {shPath}");

            _setupPs1Lines = File.ReadAllLines(ps1Path);
            _setupShLines = File.ReadAllLines(shPath);
        }

        /// <summary>
        /// Property 1: Setup scripts contain no old submodule paths
        ///
        /// For any line in setup.ps1, the line must not contain "Assets/Webium"
        /// (case-sensitive). This ensures the migration fully removed all
        /// references to the old submodule location.
        ///
        /// **Validates: Requirements 6.1, 6.3**
        /// </summary>
        [Test]
        public void SetupPs1_NoLineContainsOldSubmodulePath()
        {
            var config = Configuration.QuickThrowOnFailure;
            config.MaxNbOfTest = Math.Max(100, _setupPs1Lines.Length);

            var lineIndexGen = Gen.Choose(0, _setupPs1Lines.Length - 1);

            Prop.ForAll(
                lineIndexGen.ToArbitrary(),
                (lineIndex) =>
                {
                    var line = _setupPs1Lines[lineIndex];
                    Assert.That(line, Does.Not.Contain("Assets/Webium"),
                        $"setup.ps1 line {lineIndex + 1} contains old submodule path: {line}");
                }
            ).Check(config);
        }

        /// <summary>
        /// Property 1: Setup scripts contain no old submodule paths
        ///
        /// For any line in setup.sh, the line must not contain "Assets/Webium"
        /// (case-sensitive). This ensures the migration fully removed all
        /// references to the old submodule location.
        ///
        /// **Validates: Requirements 6.1, 6.3**
        /// </summary>
        [Test]
        public void SetupSh_NoLineContainsOldSubmodulePath()
        {
            var config = Configuration.QuickThrowOnFailure;
            config.MaxNbOfTest = Math.Max(100, _setupShLines.Length);

            var lineIndexGen = Gen.Choose(0, _setupShLines.Length - 1);

            Prop.ForAll(
                lineIndexGen.ToArbitrary(),
                (lineIndex) =>
                {
                    var line = _setupShLines[lineIndex];
                    Assert.That(line, Does.Not.Contain("Assets/Webium"),
                        $"setup.sh line {lineIndex + 1} contains old submodule path: {line}");
                }
            ).Check(config);
        }
    }
}
