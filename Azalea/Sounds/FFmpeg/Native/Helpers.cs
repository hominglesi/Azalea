using System;

namespace Azalea.Sounds.FFmpeg.Native;

internal interface IFixedArray
{
	int Length { get; }
}

internal interface IFixedArray<T> : IFixedArray
{
	T this[uint index] { get; set; }
	T[] ToArray();
	void UpdateFrom(T[] array);
}

#pragma warning disable IDE0044, IDE0251 // Make members and fields 'readonly'

internal unsafe struct AVBufferRef_ptrArray8 : IFixedArray
{
	public const int Size = 8;
	public readonly int Length => Size;

	private AVBufferRef* _0;
	private AVBufferRef* _1;
	private AVBufferRef* _2;
	private AVBufferRef* _3;
	private AVBufferRef* _4;
	private AVBufferRef* _5;
	private AVBufferRef* _6;
	private AVBufferRef* _7;

	public AVBufferRef* this[uint i]
	{
		get
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(i, Size);
			fixed (AVBufferRef** p0 = &_0)
				return *(p0 + i);
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(i, Size);
			fixed (AVBufferRef** p0 = &_0)
				*(p0 + i) = value;
		}
	}

	public readonly AVBufferRef*[] ToArray()
	{
		fixed (AVBufferRef** p0 = &_0)
		{
			var a = new AVBufferRef*[Size];
			for (uint i = 0; i < Size; i++)
				a[i] = *(p0 + i);

			return a;
		}
	}

	public void UpdateFrom(AVBufferRef*[] array)
	{
		fixed (AVBufferRef** p0 = &_0)
		{
			uint i = 0;
			foreach (var value in array)
			{
				*(p0 + i++) = value;
				if (i >= Size)
					return;
			}
		}
	}

	public static implicit operator AVBufferRef*[](AVBufferRef_ptrArray8 @struct)
		=> @struct.ToArray();
}

#pragma warning restore IDE0044, IDE0251 // Make members and fields 'readonly'

internal unsafe struct byte_array4 : IFixedArray<byte>
{
	public const int Size = 4;
	public readonly int Length => Size;

	private fixed byte _[Size];

	public byte this[uint i]
	{
		get => _[i];
		set => _[i] = value;
	}
	public byte[] ToArray()
	{
		var a = new byte[Size];
		for (uint i = 0; i < Size; i++)
			a[i] = _[i];

		return a;
	}

	public void UpdateFrom(byte[] array)
	{
		uint i = 0;
		foreach (var value in array)
		{
			_[i++] = value;
			if (i >= Size)
				return;
		}
	}

	public static implicit operator byte[](byte_array4 @struct) => @struct.ToArray();
}

internal unsafe struct byte_array16 : IFixedArray<byte>
{
	public const int Size = 16;
	public readonly int Length => Size;

	private fixed byte _[Size];

	public byte this[uint i]
	{
		get => _[i];
		set => _[i] = value;
	}

	public byte[] ToArray()
	{
		var a = new byte[Size];
		for (uint i = 0; i < Size; i++)
			a[i] = _[i];

		return a;
	}

	public void UpdateFrom(byte[] array)
	{
		uint i = 0;
		foreach (var value in array)
		{
			_[i++] = value;
			if (i >= Size)
				return;
		}
	}

	public static implicit operator byte[](byte_array16 @struct) => @struct.ToArray();
}

internal unsafe struct byte_ptrArray8 : IFixedArray
{
	public const int Size = 8;
	public readonly int Length => Size;

	private byte* _0;
	private byte* _1;
	private byte* _2;
	private byte* _3;
	private byte* _4;
	private byte* _5;
	private byte* _6;
	private byte* _7;

	public byte* this[uint i]
	{
		get
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(i, Size);
			fixed (byte** p0 = &_0)
				return *(p0 + i);
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(i, Size);
			fixed (byte** p0 = &_0)
				*(p0 + i) = value;
		}
	}

	public byte*[] ToArray()
	{
		fixed (byte** p0 = &_0)
		{
			var a = new byte*[Size];
			for (uint i = 0; i < Size; i++)
				a[i] = *(p0 + i);

			return a;
		}
	}
	public void UpdateFrom(byte*[] array)
	{
		fixed (byte** p0 = &_0)
		{
			uint i = 0;
			foreach (var value in array)
			{
				*(p0 + i++) = value;
				if (i >= 8)
					return;
			}
		}
	}

	public static implicit operator byte*[](byte_ptrArray8 @struct) => @struct.ToArray();
}

internal unsafe struct int_array8 : IFixedArray<int>
{
	public const int Size = 8;
	public readonly int Length => Size;
	fixed int _[Size];

	public int this[uint i]
	{
		get => _[i];
		set => _[i] = value;
	}
	public int[] ToArray()
	{
		var a = new int[Size];
		for (uint i = 0; i < Size; i++)
			a[i] = _[i];

		return a;
	}

	public void UpdateFrom(int[] array)
	{
		uint i = 0;
		foreach (var value in array)
		{
			_[i++] = value;
			if (i >= Size)
				return;
		}
	}

	public static implicit operator int[](int_array8 @struct) => @struct.ToArray();
}

internal unsafe struct ulong_array8 : IFixedArray<ulong>
{
	public const int Size = 8;
	public readonly int Length => Size;
	fixed ulong _[Size];

	public ulong this[uint i]
	{
		get => _[i];
		set => _[i] = value;
	}
	public ulong[] ToArray()
	{
		var a = new ulong[Size];
		for (uint i = 0; i < Size; i++)
			a[i] = _[i];

		return a;
	}

	public void UpdateFrom(ulong[] array)
	{
		uint i = 0;
		foreach (var value in array)
		{
			_[i++] = value;
			if (i >= Size)
				return;
		}
	}

	public static implicit operator ulong[](ulong_array8 @struct) => @struct.ToArray();
}
