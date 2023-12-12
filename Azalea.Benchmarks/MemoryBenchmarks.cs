using BenchmarkDotNet.Attributes;

namespace Azalea.Benchmarks;

[MemoryDiagnoser]
public class MemoryBenchmarks
{
	private class TestObject
	{
		public int Prop1 { get; set; }
		public int Prop2 { get; set; }
		public int Prop3 { get; set; }
	}

	private class TestStruct
	{
		public int Prop1;
		public int Prop2;
		public int Prop3;
	}

	private TestObject[] _sequentialObjects = new TestObject[1000];
	private TestObject[] _sequentialObjects2 = new TestObject[1000];
	private TestObject[] _nonSequentialObjects = new TestObject[1000];
	private TestObject[] _nonSequentialObjects2 = new TestObject[1000];
	private TestObject[] _sequencedObjects = new TestObject[1000];
	private TestObject[] _trashObjects = new TestObject[10000];
	private TestObject[] _trashObjects2 = new TestObject[10000];
	private TestStruct[] _structs = new TestStruct[1000];
	private TestStruct[] _structs2 = new TestStruct[1000];

	private TestObject createTestObject(int num) => new()
	{
		Prop1 = num,
		Prop2 = num * 2,
		Prop3 = num * 3,
	};

	[GlobalSetup]
	public void Setup()
	{
		for (int i = 0; i < _sequentialObjects.Length; i++)
		{
			_sequentialObjects[i] = createTestObject(i);
		}


		for (int i = 0; i < _nonSequentialObjects.Length; i++)
		{
			_nonSequentialObjects[i] = createTestObject(i);

			for (int j = 0; j < 10; j++)
			{
				_trashObjects[(i * 10) + j] = createTestObject(j);
			}
		}

		for (int i = 0; i < _sequentialObjects2.Length; i++)
		{
			_sequentialObjects2[i] = createTestObject(i);
		}

		for (int i = 0; i < _nonSequentialObjects2.Length; i++)
		{
			_nonSequentialObjects2[i] = createTestObject(i);

			for (int j = 0; j < 10; j++)
			{
				_trashObjects2[(i * 10) + j] = createTestObject(j);
			}
		}

		for (int i = 0; i < _nonSequentialObjects.Length; i++)
		{
			_sequencedObjects[i] = _nonSequentialObjects[i];
		}

		for (int i = 0; i < _structs.Length; i++)
		{
			_structs[i] = new TestStruct()
			{
				Prop1 = i,
				Prop2 = i * 2,
				Prop3 = i * 3,
			};
		}

		for (int i = 0; i < _structs2.Length; i++)
		{
			_structs2[i] = new TestStruct()
			{
				Prop1 = i,
				Prop2 = i * 2,
				Prop3 = i * 3,
			};
		}
	}

	[Benchmark]
	public int ReadObjectsSequentialy()
	{
		var sum = 0;
		for (int i = 0; i < 1000; i++)
		{
			sum += _sequentialObjects[i].Prop1;
			sum += _sequentialObjects[i].Prop2;
			sum += _sequentialObjects[i].Prop3;
		}

		sum += _trashObjects[0].Prop1;
		sum += _trashObjects2[0].Prop1;

		return sum;
	}

	[Benchmark]
	public int ReadObjectsSequentialyWithJumping()
	{
		var sum = 0;
		for (int i = 0; i < 1000; i += 2)
		{
			sum += _sequentialObjects[i].Prop1;
			sum += _sequentialObjects2[i].Prop2;
			sum += _sequentialObjects[i].Prop3;
			sum += _sequentialObjects2[i].Prop1;
			sum += _sequentialObjects[i].Prop2;
			sum += _sequentialObjects2[i].Prop3;
		}

		sum += _trashObjects[0].Prop1;
		sum += _trashObjects2[0].Prop1;

		return sum;
	}

	[Benchmark]
	public int ReadObjectsNonSequentially()
	{
		var sum = 0;
		for (int i = 0; i < 1000; i++)
		{
			sum += _nonSequentialObjects[i].Prop1;
			sum += _nonSequentialObjects[i].Prop2;
			sum += _nonSequentialObjects[i].Prop3;
		}

		sum += _trashObjects[0].Prop1;

		return sum;
	}

	[Benchmark]
	public int ReadObjectsNonSequentiallyWithJumping()
	{
		var sum = 0;
		for (int i = 0; i < 1000; i += 2)
		{
			sum += _nonSequentialObjects[i].Prop1;
			sum += _nonSequentialObjects2[i].Prop2;
			sum += _nonSequentialObjects[i].Prop3;
			sum += _nonSequentialObjects2[i].Prop1;
			sum += _nonSequentialObjects[i].Prop2;
			sum += _nonSequentialObjects2[i].Prop3;
		}

		sum += _trashObjects[0].Prop1;
		sum += _trashObjects2[0].Prop1;

		return sum;
	}

	[Benchmark]
	public int ReadObjectsSequenced()
	{
		var sum = 0;
		for (int i = 0; i < 1000; i++)
		{
			sum += _sequencedObjects[i].Prop1;
			sum += _sequencedObjects[i].Prop2;
			sum += _sequencedObjects[i].Prop3;
		}

		sum += _trashObjects[0].Prop1;

		return sum;
	}

	[Benchmark]
	public int ReadStructs()
	{
		var sum = 0;
		for (int i = 0; i < 1000; i++)
		{
			sum += _structs[i].Prop1;
			sum += _structs[i].Prop2;
			sum += _structs[i].Prop3;
		}

		sum += _trashObjects[0].Prop1;

		return sum;
	}

	[Benchmark]
	public int ReadStructsWithJumping()
	{
		var sum = 0;
		for (int i = 0; i < 1000; i += 2)
		{
			sum += _structs[i].Prop1;
			sum += _structs2[i].Prop2;
			sum += _structs[i].Prop3;
			sum += _structs2[i].Prop1;
			sum += _structs[i].Prop2;
			sum += _structs2[i].Prop3;
		}

		sum += _trashObjects[0].Prop1;
		sum += _trashObjects2[0].Prop1;

		return sum;
	}
}
