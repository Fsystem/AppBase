﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarinaStudio.Collections
{
	/// <summary>
	/// Tests of <see cref="ListExtensions"/>.
	/// </summary>
	[TestFixture]
	class ListExtensionsTests
	{
		// Fields.
		readonly Random random = new Random();


		/// <summary>
		/// Test binary search.
		/// </summary>
		[Test]
		public void BinarySearchTest()
		{
			// prepare comparison function
			var comparison = new Comparison<int>((x, y) => x - y);

			// search in empty list
			var testList = (IList<int>)new int[0];
			Assert.AreEqual(~0, testList.BinarySearch(1));
			Assert.AreEqual(~0, testList.BinarySearch(1, comparison));

			// search in list with same elements
			for (var i = 0; i < 10; ++i)
			{
				var count = this.random.Next(1, 101);
				testList = new List<int>(count).Also((it) =>
				{
					for (var j = count; j > 0; --j)
						it.Add(1);
				});
				Assert.AreEqual(~0, testList.BinarySearch(0));
				Assert.AreEqual(~0, testList.BinarySearch(0, comparison));
				Assert.AreEqual(~count, testList.BinarySearch(2));
				Assert.AreEqual(~count, testList.BinarySearch(2, comparison));
			}

			// search in sorted list
			for (var i = 0; i < 100; ++i)
			{
				// build list
				var count = this.random.Next(10, 101);
				testList = new List<int>(count).Also((it) =>
				{
					for (var j = 1; j <= count; ++j)
						it.Add(j * 2);
				});

				// search value which should be placed before head
				Assert.AreEqual(~0, testList.BinarySearch(testList[0] - 1));
				Assert.AreEqual(~0, testList.BinarySearch(testList[0] - 1, comparison));

				// search first value
				Assert.AreEqual(0, testList.BinarySearch(testList[0]));
				Assert.AreEqual(0, testList.BinarySearch(testList[0], comparison));

				// search last value
				Assert.AreEqual(count - 1, testList.BinarySearch(testList[count - 1]));
				Assert.AreEqual(count - 1, testList.BinarySearch(testList[count - 1], comparison));

				// search value which should be placed after tail
				Assert.AreEqual(~count, testList.BinarySearch(testList[count - 1] + 1));
				Assert.AreEqual(~count, testList.BinarySearch(testList[count - 1] + 1, comparison));

				// search existent value
				for (var j = 0; j < 10; ++j)
				{
					var index = this.random.Next(0, count);
					Assert.AreEqual(index, testList.BinarySearch(testList[index]));
					Assert.AreEqual(index, testList.BinarySearch(testList[index], comparison));
				}

				// search non-existent value
				for (var j = 0; j < 10; ++j)
				{
					var index = this.random.Next(0, count);
					Assert.AreEqual(~index, testList.BinarySearch(testList[index] - 1));
					Assert.AreEqual(~index, testList.BinarySearch(testList[index] - 1, comparison));
				}
			}
		}


		/// <summary>
		/// Test for copying elements of list to array.
		/// </summary>
		[Test]
		public void CopyingToArrayTest()
		{
			// prepare
			var testList = (IList<int>)new int[1024].Also((it) =>
			{
				for (var i = it.Length - 1; i >= 0; --i)
					it[i] = this.random.Next(int.MinValue, int.MaxValue);
			});
			var refList = new List<int>(testList);

			// copy head of list
			var array = new int[testList.Count / 10];
			var refArray = new int[array.Length];
			testList.CopyTo(0, array, 0, array.Length);
			refList.CopyTo(0, refArray, 0, refArray.Length);
			Assert.IsTrue(array.SequenceEqual(refArray), "Copied elements are incorrect.");

			// copy tail of list
			testList.CopyTo(testList.Count - array.Length, array, 0, array.Length);
			refList.CopyTo(refList.Count - refArray.Length, refArray, 0, refArray.Length);
			Assert.IsTrue(array.SequenceEqual(refArray), "Copied elements are incorrect.");

			// copy middle of list
			var index = this.random.Next(1, testList.Count - array.Length);
			testList.CopyTo(index, array, 0, array.Length);
			refList.CopyTo(index, refArray, 0, refArray.Length);
			Assert.IsTrue(array.SequenceEqual(refArray), "Copied elements are incorrect.");
		}
	}
}
