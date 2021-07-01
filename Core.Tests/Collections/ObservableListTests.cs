﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CarinaStudio.Collections
{
	/// <summary>
	/// Tests of <see cref="ObservableList{T}"/>.
	/// </summary>
	[TestFixture]
	class ObservableListTests
	{
		// Fields.
		readonly Random random = new Random();


		/// <summary>
		/// Test for collection changed event.
		/// </summary>
		[Test]
		public void CollectionChangedEventTest()
		{
			for (var t = 0; t < 10; ++t)
			{
				// prepare
				var list = new ObservableList<int>();
				var reflectedList = new List<int>();
				list.CollectionChanged += (_, e) =>
				{
					switch (e.Action)
					{
						case NotifyCollectionChangedAction.Add:
							{
								var newItems = e.NewItems;
								var elements = new int[newItems.Count].Also((it) => newItems.CopyTo(it, 0));
								reflectedList.InsertRange(e.NewStartingIndex, elements);
							}
							break;
						case NotifyCollectionChangedAction.Remove:
							reflectedList.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
							break;
						case NotifyCollectionChangedAction.Reset:
							reflectedList.Clear();
							reflectedList.AddRange(list);
							break;
						default:
							throw new AssertionException($"Uncovered collection change action: {e.Action}.");
					}
				};

				// add random elements
				for (var i = 0; i < 100; ++i)
					list.Add(this.random.Next());
				Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after adding elements.");
				for (var i = 0; i < 100; ++i)
				{
					var count = this.random.Next(1, 101);
					var elements = this.GenerateRandomArray(count);
					list.AddRange(elements);
				}
				Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after adding elements.");

				// insert random elements
				for (var i = 0; i < 100; ++i)
					list.Insert(this.random.Next(list.Count + 1), this.random.Next());
				Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after inserting elements.");
				for (var i = 0; i < 100; ++i)
				{
					var count = this.random.Next(1, 101);
					var elements = this.GenerateRandomArray(count);
					list.InsertRange(this.random.Next(list.Count + 1), elements);
				}
				Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after inserting elements.");

				// remove random elements
				for (var i = 0; i < 100; ++i)
					list.RemoveAt(this.random.Next(list.Count));
				Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after removing elements.");
				if (list.RemoveAll(element => (element % 3) == 0) > 0)
					Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after removing elements.");
				for (var i = 0; i < 100 && list.IsNotEmpty(); ++i)
				{
					var index = this.random.Next(list.Count - 1);
					var count = this.random.Next(1, 11);
					if (index + count > list.Count)
						count = list.Count - index;
					list.RemoveRange(index, count);
				}
				Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after removing elements.");

				// Clear
				list.Clear();
				Assert.IsTrue(list.SequenceEqual(reflectedList), "List built by collection change event is incorrect after clearing.");
			}
		}


		// Generate array with random value.
		int[] GenerateRandomArray(int count) => new int[count].Also((it) =>
		{
			for (var i = count - 1; i >= 0; --i)
				it[i] = this.random.Next(0, count / 2);
		});


		/// <summary>
		/// Test for RemoveAll().
		/// </summary>
		[Test]
		public void RemovingAllTest()
		{
			for (var t = 0; t < 10; ++t)
			{
				// prepare
				var refList = new List<int>(this.GenerateRandomArray(10240));
				var list = new ObservableList<int>(refList);
				Assert.IsTrue(list.SequenceEqual(refList), "List built by initial elements is different from reference list.");

				// remove all
				var predicate = new Predicate<int>(n => (n & 0x1) == 0);
				if (refList.RemoveAll(predicate) > 0)
				{
					list.RemoveAll(predicate);
					Assert.IsTrue(list.SequenceEqual(refList), "List is different from reference list after removing elements.");
				}
			}
		}
	}
}
