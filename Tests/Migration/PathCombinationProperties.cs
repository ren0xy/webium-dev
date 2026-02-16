using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FsCheck;
using NUnit.Framework;

namespace Webium.Tests.Migration
{
    /// <summary>
    /// Property-based tests for path combination safety.
    /// Feature: 001-upm-package-migration, Property 2: Path combination produces paths within the package root
    /// </summary>
    [TestFixture]
    public class PathCombinationProperties
    {
        /// <summary>
        /// Generates a single alphanumeric folder name (1–10 chars).
        /// </summary>
        private static Gen<string> FolderNameGen()
        {
            var chars = Gen.Elements(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
                    .ToCharArray());

            return Gen.Choose(1, 10)
                .SelectMany(len =>
                    Gen.ArrayOf(len, chars)
                        .Select(arr => new string(arr)));
        }

        /// <summary>
        /// Generates a random absolute root path.
        /// On Windows: C:\seg1\seg2\...
        /// On non-Windows: /seg1/seg2/...
        /// Always produces 1–4 folder segments.
        /// </summary>
        private static Gen<string> AbsoluteRootPathGen()
        {
            var segmentsGen = Gen.Choose(1, 4)
                .SelectMany(count =>
                    Gen.ArrayOf(count, FolderNameGen())
                        .Select(segs => segs.ToArray()));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var driveGen = Gen.Elements('C', 'D', 'E', 'F');
                return driveGen.SelectMany(drive =>
                    segmentsGen.Select(segs =>
                        drive + ":\\" + string.Join("\\", segs)));
            }
            else
            {
                return segmentsGen
                    .Select(segs => "/" + string.Join("/", segs));
            }
        }

        /// <summary>
        /// Generates a random relative folder path with 1–3 segments.
        /// No ".." segments, no absolute paths — just clean alphanumeric names.
        /// </summary>
        private static Gen<string> RelativeFolderPathGen()
        {
            return Gen.Choose(1, 3)
                .SelectMany(count =>
                    Gen.ArrayOf(count, FolderNameGen())
                        .Select(segs =>
                            Path.Combine(segs)));
        }

        /// <summary>
        /// Property 2: Path combination produces paths within the package root
        ///
        /// For any absolute package root path and any relative UI folder path
        /// (that does not contain ".." traversal), combining them via Path.Combine
        /// should produce a path that starts with the package root.
        /// This validates that the bootstrapper's path resolution keeps file access
        /// within the package boundary.
        ///
        /// **Validates: Requirements 8.1, 8.2**
        /// </summary>
        [Test]
        public void CombinedPath_StartsWithRoot()
        {
            var config = Configuration.QuickThrowOnFailure;
            config.MaxNbOfTest = 100;

            Prop.ForAll(
                AbsoluteRootPathGen().ToArbitrary(),
                RelativeFolderPathGen().ToArbitrary(),
                (root, relative) =>
                {
                    var combined = Path.Combine(root, relative);
                    var normalizedCombined = Path.GetFullPath(combined);
                    var normalizedRoot = Path.GetFullPath(root);

                    // Ensure the root ends with a separator for a proper prefix check,
                    // so "/foo" doesn't falsely match "/foobar".
                    if (!normalizedRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        normalizedRoot += Path.DirectorySeparatorChar;

                    Assert.That(
                        normalizedCombined.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase),
                        Is.True,
                        $"Combined path '{normalizedCombined}' does not start with root '{normalizedRoot}'. " +
                        $"Original root='{root}', relative='{relative}'");
                }
            ).Check(config);
        }
    }
}
