// FILE: TestDataPath.cs
// ROLE: Resolves absolute paths to TestData JSON files at runtime.

using System;
using System.IO;

namespace Task1.Support
{
    public static class TestDataPath
    {
        public static string Resolve(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, "TestData", fileName);
        }
    }
}
