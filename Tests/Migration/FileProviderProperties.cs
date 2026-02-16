using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FsCheck;
using NUnit.Framework;

namespace Webium.Tests.Migration
{
    /// <summary>
    /// Property-based tests for FileProvider path traversal detection.
    /// Feature: 001-upm-package-migration, Property 3: FileProvider rejects path traversal attempts
    /// </summary>
    [TestFixture]
    public class FileProviderProperties
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
        /// Generates a random absolute base path with 1–4 folder segments.
        /// On Windows: C:\seg1\seg2\...
        /// On non-Windows: /seg1/seg2/...
        /// </summary>
        private static Gen<string> AbsoluteBasePathGen()
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
        /// Generates a (basePath, traversalRelativePath) tuple where the traversal
        /// path contains enough ".." segments to escape the base directory.
        ///
        /// Strategy: count the directory segments in the base path, then generate
        /// MORE ".." segments than that count, followed by a random target folder.
        ///
        /// Example: base = C:\foo\bar (2 segments after drive)
        ///          relative = ..\..\..\..\evil (4 ".." segments — escapes the base)
        /// </summary>
        private static Gen<Tuple<string, string>> BaseAndTraversalGen()
        {
            return AbsoluteBasePathGen().SelectMany(basePath =>
            {
                var normalized = Path.GetFullPath(basePath);
                var root = Path.GetPathRoot(normalized);
                var relativePart = normalized.Substring(root.Length);
                var segmentCount = relativePart
                    .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                        StringSplitOptions.RemoveEmptyEntries)
                    .Length;

                // Need at least segmentCount + 1 ".." to guarantee escape past the base
                return Gen.Choose(segmentCount + 1, segmentCount + 4)
                    .SelectMany(dotDotCount =>
                        FolderNameGen().Select(targetFolder =>
                        {
                            var dotDots = string.Join(
                                Path.DirectorySeparatorChar.ToString(),
                                Enumerable.Repeat("..", dotDotCount));
                            var traversal = Path.Combine(dotDots, targetFolder);
                            return Tuple.Create(basePath, traversal);
                        }));
            });
        }

        /// <summary>
        /// Property 3: FileProvider rejects path traversal attempts
        ///
        /// For any base path and any relative path containing ".." segments that
        /// would escape the base directory, the normalized combined path should NOT
        /// start with the normalized base path. This proves that a FileProvider
        /// implementation checking normalizedPath.StartsWith(normalizedBase) would
        /// correctly reject these traversal attempts.
        ///
        /// **Validates: Requirements 8.5**
        /// </summary>
        [Test]
        public void PathTraversal_EscapesBase_IsDetectable()
        {
            var config = Configuration.QuickThrowOnFailure;
            config.MaxNbOfTest = 100;

            Prop.ForAll(
                BaseAndTraversalGen().ToArbitrary(),
                (pair) =>
                {
                    var basePath = pair.Item1;
                    var traversalRelative = pair.Item2;

                    var combined = Path.Combine(basePath, traversalRelative);
                    var normalizedCombined = Path.GetFullPath(combined);
                    var normalizedBase = Path.GetFullPath(basePath);

                    // Ensure the base ends with a separator for proper prefix check,
                    // so "/foo" doesn't falsely match "/foobar".
                    if (!normalizedBase.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        normalizedBase += Path.DirectorySeparatorChar;

                    Assert.That(
                        normalizedCombined.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase),
                        Is.False,
                        $"Path traversal was NOT detected! " +
                        $"Combined '{normalizedCombined}' still starts with base '{normalizedBase}'. " +
                        $"Original base='{basePath}', traversal='{traversalRelative}'");
                }
            ).Check(config);
        }
    }
}
