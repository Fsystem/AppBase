﻿using Avalonia.Input;

namespace CarinaStudio.Input
{
	/// <summary>
	/// Extensions for <see cref="IDataObject"/>.
	/// </summary>
	public static class DataObjectExtensions
	{
		/// <summary>
		/// Clone <see cref="IDataObject"/> as <see cref="DataObject"/>.
		/// </summary>
		/// <param name="data"><see cref="IDataObject"/>.</param>
		/// <returns><see cref="DataObject"/>.</returns>
		public static DataObject Clone(this IDataObject data) => new DataObject().Also(it =>
		{
			foreach (var format in data.GetDataFormats())
			{
				var value = data.Get(format);
				if (value != null)
					it.Set(format, value);
			}
		});


		/// <summary>
		/// Check whether at least one file name is contained in <see cref="IDataObject"/> or not.
		/// </summary>
		/// <param name="data"><see cref="IDataObject"/>.</param>
		/// <returns>True if at least one file name is contained in <see cref="IDataObject"/>.</returns>
		public static bool HasFileNames(this IDataObject data) => Global.RunOrDefault(() =>
		{
			return data.GetFileNames()?.Let(it =>
			{
				foreach (var fileName in it)
				{
					if (!string.IsNullOrEmpty(fileName))
						return true;
				}
				return false;
			}) ?? false;
		});


		/// <summary>
		/// Try getting the data with given format and type.
		/// </summary>
		/// <typeparam name="T">Type of data.</typeparam>
		/// <param name="dataObject"><see cref="IDataObject"/>.</param>
		/// <param name="format">Format.</param>
		/// <param name="data">Data.</param>
		/// <returns>True if data got successfully.</returns>
		public static bool TryGetData<T>(this IDataObject dataObject, string format, out T? data) where T : class
		{
			var rawData = dataObject.Get(format);
			if (rawData is T dataT)
			{
				data = dataT;
				return true;
			}
			data = default;
			return false;
		}


		/// <summary>
		/// Get the only file name contained in <see cref="IDataObject"/>.
		/// </summary>
		/// <param name="data"><see cref="IDataObject"/>.</param>
		/// <param name="fileName">File name contained in <see cref="IDataObject"/>.</param>
		/// <returns>True if only one file name contained in <see cref="IDataObject"/>, or false if no file name or more than one file names are contained.</returns>
		public static bool TryGetSingleFileName(this IDataObject data, out string? fileName)
		{
			fileName = Global.RunOrDefault(() =>
			{
				return data.GetFileNames()?.Let(it =>
				{
					var fileName = default(string);
					foreach (var candidate in it)
					{
						if (!string.IsNullOrEmpty(candidate))
						{
							if (fileName is null)
								fileName = candidate;
							else
								return null;
						}
					}
					return fileName;
				});
			});
			return (fileName is not null);
		}


		/// <summary>
		/// Try getting the value type data with given format and type.
		/// </summary>
		/// <typeparam name="T">Type of data.</typeparam>
		/// <param name="dataObject"><see cref="IDataObject"/>.</param>
		/// <param name="format">Format.</param>
		/// <param name="value">Value.</param>
		/// <returns>True if value got successfully.</returns>
		public static bool TryGetValue<T>(this IDataObject dataObject, string format, out T value) where T : struct
		{
			var rawData = dataObject.Get(format);
			if (rawData is T targetValue)
			{
				value = targetValue;
				return true;
			}
			value = default;
			return false;
		}
	}
}
