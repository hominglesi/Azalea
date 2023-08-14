using System;
using System.IO;
using System.Security.Cryptography;

namespace Azalea.Extentions;

public static class ExtentionMethods
{
    public static T[][] ToJagged<T>(this T[,] rectangular)
    {
        ArgumentNullException.ThrowIfNull(rectangular);

        var jagged = new T[rectangular.GetLength(0)][];
        for (int r = 0; r < rectangular.GetLength(0); r++)
        {
            jagged[r] = new T[rectangular.GetLength(1)];
            for (int c = 0; c < rectangular.GetLength(1); c++)
                jagged[r][c] = rectangular[r, c];
        }

        return jagged;
    }

    public static string ComputeMD5Hash(this Stream stream)
    {
        string hash;

        stream.Seek(0, SeekOrigin.Begin);
        using var md5 = MD5.Create();
        hash = md5.ComputeHash(stream).toLowercaseHex();
        stream.Seek(0, SeekOrigin.Begin);

        return hash;
    }

    private static string toLowercaseHex(this byte[] bytes)
        => string.Create(bytes.Length * 2, bytes, (span, b) =>
        {
            for (int i = 0; i < b.Length; i++)
                _ = b[i].TryFormat(span[(i * 2)..], out _, "x2");
        });
}
