using System;
using System.Runtime.CompilerServices;

namespace Azalea.Extentions.EnumExtentions;

public static class EnumExtentions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool HasFlagFast<T>(this T enumValue, T flag) where T : unmanaged, Enum
    {
        if (sizeof(T) == 1)
        {
            byte value1 = Unsafe.As<T, byte>(ref enumValue);
            byte value2 = Unsafe.As<T, byte>(ref flag);
            return (value1 & value2) == value2;
        }

        if (sizeof(T) == 2)
        {
            short value1 = Unsafe.As<T, short>(ref enumValue);
            short value2 = Unsafe.As<T, short>(ref flag);
            return (value1 & value2) == value2;
        }

        if (sizeof(T) == 4)
        {
            int value1 = Unsafe.As<T, int>(ref enumValue);
            int value2 = Unsafe.As<T, int>(ref flag);
            return (value1 & value2) == value2;
        }

        if (sizeof(T) == 8)
        {
            long value1 = Unsafe.As<T, long>(ref enumValue);
            long value2 = Unsafe.As<T, long>(ref flag);
            return (value1 & value2) == value2;
        }

        throw new ArgumentException($"Invalid enum type provided: {typeof(T)}.");
    }
}
