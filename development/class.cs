<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace MIP.Infrastructure.IO
{
	/// <summary>
	/// Service providing I/O operations.
	/// </summary>
	public class FileSystemService : IFileSystemService
	{
		#region Constants

		/// <summary>
		/// Backup files naming format
		/// </summary>
		private const string BACKUP_FILE_NAME_FORMAT = "_yyyy-MM-dd_HH-mm-jjjjжооооss;LKJ;KLJ;LKJ";

		/// <summary>
		/// Buffer size
		/// </summary>
		private const int BUFFER_SIZE = 4096;

		/// <summary>
		/// Compress level.
		/// </summary>
		private const int COMPRESSION_LEVEL = 5;

		#endregion

		#region IFileSystemService Members

			/// <summary>
		/// 	Reads specified file
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <returns> </returns>
		public string ReadFromFile(string filePath)
		{
			return File.ReadAllText(filePath);
		}


		/// <summary>
		/// Feature 2 Method
		/// </summary>
		/// <param name="archivePath">Feature 2 Method The archive path. </param>
		/// <param name="fileName">Feature 2 Method Name of the file.</param>
		/// <param name="content">Feature 2 Method The content.</param>
		public void WtriteFeature2(string archivePath, string fileName, string content)
		{
			var memoryStream = new MemoryStream();
			//TODO: implement Feature 2 Methods
		}


		/// <summary>
		/// 	Reads the bytes from file.
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <returns> </returns>
		public byte[] ReadBytesFromFile(string filePath)
		{
			if (File.Exists(filePath))
				return File.ReadAllBytes(filePath);
			return new byte[] { };
		}

		/// <summary>
		/// 	Writes content to the specified file.
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <param name="content"> The content. </param>
		public void WriteToFile(string filePath, string content)
		{
			File.WriteAllText(filePath, content);
		}

		/// <summary>
		/// Writes to archive.
		/// </summary>
		/// <param name="archivePath">The archive path.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="content">The content.</param>
		public void WriteToArchive(string archivePath, string fileName, string content)
		{
			var memoryStream = new MemoryStream();
			using (var streamWriter = new StreamWriter(memoryStream))
			{
				streamWriter.Write(content);
				streamWriter.Flush();

				memoryStream.Seek(0, SeekOrigin.Begin);
				using (var compressedStream = CompressMemoryStream(memoryStream, fileName))
				{
					WriteToFile(archivePath, compressedStream);
				}
			}
		}

		/// <summary>
		/// Reads from archive.
		/// </summary>
		/// <param name="archivePath">The archive path.</param>
		/// <returns></returns>
		public string ReadSingleFileFromArchive(string archivePath)
		{
			var archiveStream = GetFileStream(archivePath);
			return DecompressSingleFileFromStream(archiveStream);
		}

		/// <summary>
		/// 	Writes <see cref="byte">byte</see> array to file. (DTC Transaction supported)
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <param name="bytes"> <see cref="byte">Byte</see> array to be written. </param>
		public void WriteBytesToFile(string filePath, byte[] bytes)
		{
			File.WriteAllBytes(filePath, bytes);
		}

		/// <summary>
		/// 	Copies the file to stream.
		/// </summary>
		/// <param name="outputStream"> The output stream. </param>
		/// <param name="path"> The path. </param>
		public void CopyFileToStream(Stream outputStream, string path)
		{
			// Stream.CopyTo uses buffer. The default buffer size is 4096.
			using (Stream s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				s.CopyTo(outputStream);
			}
		}

		/// <summary>
		/// 	Copies the file to base64.
		/// </summary>
		/// <param name="outputData"> The output data. </param>
		/// <param name="path"> The path. </param>
		public void CopyFileToBase64(out string outputData, string path)
		{
			if (File.Exists(path))
			{
				using (Stream s = new FileStream(path, FileMode.Open))
				{
					var bytes = new byte[s.Length];
					s.Position = 0;
					s.Read(bytes, 0, (int)s.Length);
					outputData = Convert.ToBase64String(bytes);
				}
			}
			else
			{
				outputData = string.Empty;
			}
		}

		/// <summary>
		/// 	Deletes the file.
		/// </summary>
		/// <param name="path"> The path. </param>
		public void DeleteFile(string path)
		{
			File.Delete(path);
		}

		/// <summary>
		/// Deletes file if file exists in file system.
		/// </summary>
		/// <param name="path">Path to file to be removed</param>
		public void DeleteFileIfExists(string path)
		{
			if (FileExists(path))
			{
				DeleteFile(path);
			}
		}

		/// <summary>
		/// Deletes the folder if exist.
		/// </summary>
		/// <param name="folderToDelete">The folder to delete.</param>
		public void DeleteFolderIfExist(string folderToDelete)
		{
			new DirectoryInfo(folderToDelete).Delete(true);
		}

		/// <summary>
		/// 	Writes to file specified content
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <param name="content"> The content. </param>
		public void WriteToFile(string filePath, Stream content)
		{
			DeleteFileIfExists(filePath);

			using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				content.Seek(0, SeekOrigin.Begin);
				content.CopyTo(fileStream);
			}
		}

		/// <summary>
		/// Writes to file with progress.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="content">The content.</param>
		/// <param name="progressHandler">The progress handler.</param>
		public void WriteToFileWithProgress(string filePath, Stream content, Func<double, bool> progressHandler)
		{
			using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				content.Seek(0, SeekOrigin.Begin);
				var buffer = new byte[4096];

				while (content.Position < content.Length)
				{
					fileStream.Write(buffer, 0, content.Read(buffer, 0, buffer.Length));

					if (!progressHandler((double)content.Position / content.Length))
						break; // process has been cancelled
				}
			}
		}

		/// <summary>
		/// Copies an existing file to a new file.
		/// </summary>
		/// <param name="sourceFileName"> The File to copy. </param>
		/// <param name="destFileName"> The destination File </param>
		/// <param name="overwrite">true if the destination file can be overwritten; otherwise, false.</param>
		public void CopyFile(String sourceFileName, String destFileName, bool overwrite = false)
		{
			CreateDirectoryIfNotExists(Path.GetDirectoryName(destFileName));
			File.Copy(sourceFileName, destFileName, overwrite);
		}

		/// <summary>
		/// Moves the file.
		/// </summary>
		/// <param name="sourceFileName">Name of the source file.</param>
		/// <param name="destFileName">Name of the dest file.</param>
		public void MoveFile(string sourceFileName, string destFileName)
		{
			CreateDirectoryIfNotExists(Path.GetDirectoryName(destFileName));
			File.Move(sourceFileName, destFileName);
		}

		/// <summary>
		/// 	Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="directoryPath"> The directory path. </param>
		/// <returns> </returns>
		public bool DirectoryExists(string directoryPath)
		{
			return Directory.Exists(directoryPath);
		}

		/// <summary>
		/// 	Creates all directories and subdirectories as specified by path.
		/// </summary>
		/// <param name="directoryPath"> The directory path. </param>
		/// <returns> </returns>
		public DirectoryInfo CreateDirectory(string directoryPath)
		{
			return Directory.CreateDirectory(directoryPath);
		}

		/// <summary>
		/// 	Creates all directories and subdirectories as specified by path.
		/// </summary>
		/// <param name="directoryPath"> The directory path. </param>
		/// <returns> </returns>
		public DirectoryInfo CreateDirectoryIfNotExists(string directoryPath)
		{
			var directoryInfo = new DirectoryInfo(directoryPath);

			if (!DirectoryExists(directoryPath))
			{
				CreateDirectory(directoryPath);
			}

			return directoryInfo;
		}

		/// <summary>
		/// 	Makes backup file
		/// </summary>
		/// <param name="filePath"> The current file path. </param>
		public void BackUpFile(string filePath)
		{
			var currentDirectory = Path.GetDirectoryName(filePath);
			if (currentDirectory == null)
			{
				throw new DirectoryNotFoundException("Cannot find directory.");
			}

			var backUpFileName = String.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(filePath),
											   DateTime.UtcNow.ToString(BACKUP_FILE_NAME_FORMAT),
											   Path.GetExtension(filePath));
			var backupFilePath = Path.Combine(currentDirectory, backUpFileName);
			if (FileExists(backupFilePath))
			{
				throw new InvalidDataException("BackUp file already exists: " + backupFilePath);
			}

			CopyFile(filePath, backupFilePath);
		}

		/// <summary>
		/// 	Creates File
		/// </summary>
		/// <param name="filePath"> The current file path. </param>
		public FileStream CreateFile(string filePath)
		{
			return File.Create(filePath);
		}

		/// <summary>
		/// 	Gets filestream by file path
		/// </summary>
		/// <param name="filePath"> The current file path. </param>
		/// <returns> </returns>
		public Stream GetFileStream(string filePath)
		{
			return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		/// <summary>
		/// Compress files
		/// </summary>
		/// <param name="files">The files that will be included to the compress file.</param>
		/// <param name="destinationFilePath">The path to the compress file that will be created</param>
		public void CompressFiles(IDictionary<string, byte[]> files, string destinationFilePath)
		{
			using (var outStream = new FileStream(destinationFilePath, FileMode.OpenOrCreate))
			{
				using (var zipStream = new ZipOutputStream(outStream))
				{
					zipStream.SetLevel(COMPRESSION_LEVEL);

					foreach (var embeddedFile in files)
					{
						var zipEntity = new ZipEntry(embeddedFile.Key)
							{
								DateTime = DateTime.UtcNow,
								Size = embeddedFile.Value.Length
							};

						zipStream.PutNextEntry(zipEntity);

						var buffer = new byte[BUFFER_SIZE];
						using (var embeddedFileStream = new MemoryStream(embeddedFile.Value))
						{
							StreamUtils.Copy(embeddedFileStream, zipStream, buffer);
						}
					}

					zipStream.CloseEntry();

					zipStream.IsStreamOwner = true;
				}
			}
		}

		/// <summary>
		/// Decompresses the input stream.
		/// </summary>
		/// <param name="inputStream">The archive stream.</param>
		/// <returns></returns>
		public Dictionary<string, Stream> DecompressStream(Stream inputStream)
		{
			var result = new Dictionary<String, Stream>();

			using (var zipInputStream = new ZipInputStream(inputStream))
			{
				ZipEntry zipEntry = zipInputStream.GetNextEntry();
				while (zipEntry != null)
				{
					var buffer = new byte[BUFFER_SIZE];
					var stream = new MemoryStream((int)zipEntry.Size);

					StreamUtils.Copy(zipInputStream, stream, buffer);
					result.Add(zipEntry.Name, stream);

					zipEntry = zipInputStream.GetNextEntry();
				}
			}

			return result;
		}

		/// <summary>
		/// Decompresses the file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		public Dictionary<string, Stream> DecompressFile(string filePath)
		{
			return DecompressStream(GetFileStream(filePath));
		}

		/// <summary>
		/// Decompresses the file to directory.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="destinationDirectoryPath">The destination directory path.</param>
		public IEnumerable<string> DecompressFileToDirectory(string filePath, string destinationDirectoryPath = null)
		{
			var decompressedFile = DecompressFile(filePath);
			var result = new List<string>();

			foreach (var entry in decompressedFile)
			{
				var destinationPath = destinationDirectoryPath != null
									   ? Path.Combine(destinationDirectoryPath, entry.Key.GetFileName())
									   : Path.Combine(filePath.GetDirectoryName(), entry.Key);

				WriteToFile(destinationPath, entry.Value);
				entry.Value.Dispose();

				result.Add(destinationPath);
			}

			return result;
		}

		/// <summary>
		/// Decompresses the single file from stream.
		/// </summary>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public string DecompressSingleFileFromStream(Stream inputStream)
		{
			string result = String.Empty;

			using (var zipInputStream = new ZipInputStream(inputStream))
			{
				ZipEntry zipEntry = zipInputStream.GetNextEntry();

				if (zipEntry != null)
				{
					var buffer = new byte[BUFFER_SIZE];

					var stream = new MemoryStream((int)zipEntry.Size);
					StreamUtils.Copy(zipInputStream, stream, buffer);

					using (var sr = new StreamReader(stream))
					{
						stream.Seek(0, SeekOrigin.Begin);

						result = sr.ReadToEnd();
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Compress memoryStream
		/// </summary>
		/// <param name="memoryStream"></param>
		/// <param name="fileInArchive"></param>
		/// <returns></returns>
		public MemoryStream CompressMemoryStream(MemoryStream memoryStream, string fileInArchive)
		{
			var outStream = new MemoryStream();

			using (var zipStream = new ZipOutputStream(outStream))
			{
				zipStream.SetLevel(COMPRESSION_LEVEL); //0-9, 9 being the highest level of compression

				var newEntry = new ZipEntry(fileInArchive) { DateTime = DateTime.UtcNow };

				zipStream.PutNextEntry(newEntry);

				StreamUtils.Copy(memoryStream, zipStream, new byte[BUFFER_SIZE]);
				zipStream.CloseEntry();

				zipStream.IsStreamOwner = false; // False stops the Close also Closing the underlying stream.
			}

			outStream.Position = 0;
			return outStream;
		}

		/// <summary>
		/// Compress File
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="destinationFilePath">The path to the compress file that will be created</param>
		/// <param name="fileNameInArchive">This file name will be added to the archive.</param>
		public void CompressFile(String sourceFile, String destinationFilePath, String fileNameInArchive = null)
		{
			var outStream = new FileStream(destinationFilePath, FileMode.OpenOrCreate);
			using (var zipStream = new ZipOutputStream(outStream))
			{
				zipStream.SetLevel(COMPRESSION_LEVEL);

				var fileInfo = new FileInfo(sourceFile);
				var zipEntity = new ZipEntry(fileNameInArchive ?? sourceFile)
					{
						DateTime = DateTime.UtcNow,
						Size = fileInfo.Length
					};

				zipStream.PutNextEntry(zipEntity);

				var buffer = new byte[BUFFER_SIZE];
				using (FileStream streamReader = File.OpenRead(sourceFile))
				{
					StreamUtils.Copy(streamReader, zipStream, buffer);
				}

				zipStream.CloseEntry();

				zipStream.IsStreamOwner = true;
			}
		}

		/// <summary>
		/// Returns the names of files in the specified directory.
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <returns></returns>
		public string[] GetFiles(String directoryPath)
		{
			return Directory.GetFiles(directoryPath);
		}

		/// <summary>
		/// Read all bytes from stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public byte[] ReadAllBytesFromStream(Stream stream)
		{
			using (var memmoryStream = new MemoryStream())
			{
				stream.CopyTo(memmoryStream);
				return memmoryStream.ToArray();
			}
		}

		/// <summary>
		/// Zips the directory.
		/// </summary>
		/// <param name="directoryName">Name of the directory.</param>
		/// <param name="destinationFilePath">The destination file path.</param>
		public void ZipDirectory(string directoryName, string destinationFilePath)
		{
			new FastZip().CreateZip(destinationFilePath, directoryName, true, string.Empty);
		}

		#endregion
	}
}
=======
using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace MIP.Infrastructure.IO
{
	/// <summary>
	/// Service providing I/O operations.
	/// </summary>
	public class FileSystemService : IFileSystemService
	{

		#region Constants

		/// <summary>
		/// Backup files naming format
		/// </summary>
		private const string BACKUP_FILE_NAME_FORMAT = "_yyyy-MM-dd_HH-mm-ss";

		/// <summary>
		/// Buffer size
		/// </summary>
		private const int BUFFER_SIZE = 4096;

		/// <summary>
		/// Compress level.
		/// </summary>
		private const int COMPRESSION_LEVEL = 5;

		#endregion

		#region IFileSystemService Members


		
		/// <summary>
		/// Returns the names of files in the specified directory.
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <returns></returns>
		public string[] GetFiles(String directoryPath)
		{
			return Directory.GetFiles(directoryPath);
		}

		/// <summary>
		/// Read all bytes from stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public byte[] ReadAllBytesFromStream(Stream stream)
		{
			using (var memmoryStream = new MemoryStream())
			{
				stream.CopyTo(memmoryStream);
				return memmoryStream.ToArray();
			}
		}

		/// <summary>
		/// Zips the directory.
		/// </summary>
		/// <param name="directoryName">Name of the directory.</param>
		/// <param name="destinationFilePath">The destination file path.</param>
		public void ZipDirectory(string directoryName, string destinationFilePath)
		{
			new FastZip().CreateZip(destinationFilePath, directoryName, true, string.Empty);
		}

	


		/// <summary>
		/// 	Reads the bytes from file.
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <returns> </returns>
		public byte[] ReadBytesFromFile(string filePath)
		{
			if (File.Exists(filePath))
				return File.ReadAllBytes(filePath);
			return new byte[] { };
		}

		/// <summary>
		/// 	Reads specified file
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <returns> </returns>
		public string ReadFromFile(string filePath)
		{
			return File.ReadAllText(filePath);
		}

			/// <summary>
		/// 	Determines whether the specified file exists.
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <returns> </returns>
		public bool FileExists(string filePath)
		{
			return File.Exists(filePath);
		}


		/// <summary>
		/// 	Writes content to the specified file.
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <param name="content"> The content. </param>
		public void WriteToFile(string filePath, string content)
		{
			File.WriteAllText(filePath, content);
		}

		

		/// <summary>
		/// Reads from archive.
		/// </summary>
		/// <param name="archivePath">The archive path.</param>
		/// <returns></returns>
		public string ReadSingleFileFromArchive(string archivePath)
		{
			var archiveStream = GetFileStream(archivePath);
			return DecompressSingleFileFromStream(archiveStream);
		}

		/// <summary>
		/// 	Writes <see cref="byte">byte</see> array to file. (DTC Transaction supported)
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <param name="bytes"> <see cref="byte">Byte</see> array to be written. </param>
		public void WriteBytesToFile(string filePath, byte[] bytes)
		{
			File.WriteAllBytes(filePath, bytes);
		}

		/// <summary>
		/// Writes to archive.
		/// </summary>
		/// <param name="archivePath">The archive path.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="content">The content.</param>
		public void WriteToArchive(string archivePath, string fileName, string content)
		{
			var memoryStream = new MemoryStream();
			using (var streamWriter = new StreamWriter(memoryStream))
			{
				streamWriter.Write(content);
				streamWriter.Flush();

				memoryStream.Seek(0, SeekOrigin.Begin);
				using (var compressedStream = CompressMemoryStream(memoryStream, fileName))
				{
					WriteToFile(archivePath, compressedStream);
				}
			}
		}

		/// <summary>
		/// 	Copies the file to stream.
		/// </summary>
		/// <param name="outputStream"> The output stream. </param>
		/// <param name="path"> The path. </param>
		public void CopyFileToStream(Stream outputStream, string path)
		{
			// Stream.CopyTo uses buffer. The default buffer size is 4096.
			using (Stream s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				s.CopyTo(outputStream);
			}
		}

		/// <summary>
		/// 	Deletes the file.
		/// </summary>
		/// <param name="path"> The path. </param>
		public void DeleteFile(string path)
		{
			File.Delete(path);
		}

		/// <summary>
		/// 	Copies the file to base64.
		/// </summary>
		/// <param name="outputData"> The output data. </param>
		/// <param name="path"> The path. </param>
		public void CopyFileToBase64(out string outputData, string path)
		{
			if (File.Exists(path))
			{
				using (Stream s = new FileStream(path, FileMode.Open))
				{
					var bytes = new byte[s.Length];
					s.Position = 0;
					s.Read(bytes, 0, (int)s.Length);
					outputData = Convert.ToBase64String(bytes);
				}
			}
			else
			{
				outputData = string.Empty;
			}
		}

		

		/// <summary>
		/// Deletes file if file exists in file system.
		/// </summary>
		/// <param name="path">Path to file to be removed</param>
		public void DeleteFileIfExists(string path)
		{
			if (FileExists(path))
			{
				DeleteFile(path);
			}
		}


		/// <summary>
		/// 	Writes to file specified content
		/// </summary>
		/// <param name="filePath"> The file path. </param>
		/// <param name="content"> The content. </param>
		public void WriteToFile(string filePath, Stream content)
		{
			DeleteFileIfExists(filePath);

			using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				content.Seek(0, SeekOrigin.Begin);
				content.CopyTo(fileStream);
			}
		}


		/// <summary>
		/// Deletes the folder if exist.
		/// </summary>
		/// <param name="folderToDelete">The folder to delete.</param>
		public void DeleteFolderIfExist(string folderToDelete)
		{
			new DirectoryInfo(folderToDelete).Delete(true);
		}

/// <summary>
		/// Copies an existing file to a new file.
		/// </summary>
		/// <param name="sourceFileName"> The File to copy. </param>
		/// <param name="destFileName"> The destination File </param>
		/// <param name="overwrite">true if the destination file can be overwritten; otherwise, false.</param>
		public void CopyFile(String sourceFileName, String destFileName, bool overwrite = false)
		{
			CreateDirectoryIfNotExists(Path.GetDirectoryName(destFileName));
			File.Copy(sourceFileName, destFileName, overwrite);
		}

		/// <summary>
		/// Writes to file with progress.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="content">The content.</param>
		/// <param name="progressHandler">The progress handler.</param>
		public void WriteToFileWithProgress(string filePath, Stream content, Func<double, bool> progressHandler)
		{
			using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				content.Seek(0, SeekOrigin.Begin);
				var buffer = new byte[4096];

				while (content.Position < content.Length)
				{
					fileStream.Write(buffer, 0, content.Read(buffer, 0, buffer.Length));

					if (!progressHandler((double)content.Position / content.Length))
						break; // process has been cancelled
				}
			}
		}

		


		/// <summary>
		/// 	Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="directoryPath"> The directory path. </param>
		/// <returns> </returns>
		public bool DirectoryExists(string directoryPath)
		{
			return Directory.Exists(directoryPath);
		}

		/// <summary>
		/// 	Creates all directories and subdirectories as specified by path.
		/// </summary>
		/// <param name="directoryPath"> The directory path. </param>
		/// <returns> </returns>
		public DirectoryInfo CreateDirectory(string directoryPath)
		{
			return Directory.CreateDirectory(directoryPath);
		}


		/// <summary>
		/// Moves the file.
		/// </summary>
		/// <param name="sourceFileName">Name of the source file.</param>
		/// <param name="destFileName">Name of the dest file.</param>
		public void MoveFile(string sourceFileName, string destFileName)
		{
			CreateDirectoryIfNotExists(Path.GetDirectoryName(destFileName));
			File.Move(sourceFileName, destFileName);
		}

	/// <summary>
		/// Decompresses the file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		public Dictionary<string, Stream> DecompressFile(string filePath)
		{
			return DecompressStream(GetFileStream(filePath));
		}

		/// <summary>
		/// Decompresses the file to directory.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="destinationDirectoryPath">The destination directory path.</param>
		public IEnumerable<string> DecompressFileToDirectory(string filePath, string destinationDirectoryPath = null)
		{
			var decompressedFile = DecompressFile(filePath);
			var result = new List<string>();

			foreach (var entry in decompressedFile)
			{
				var destinationPath = destinationDirectoryPath != null
									   ? Path.Combine(destinationDirectoryPath, entry.Key.GetFileName())
									   : Path.Combine(filePath.GetDirectoryName(), entry.Key);

				WriteToFile(destinationPath, entry.Value);
				entry.Value.Dispose();

				result.Add(destinationPath);
			}

			return result;
		}

		/// <summary>
		/// Decompresses the single file from stream.
		/// </summary>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public string DecompressSingleFileFromStream(Stream inputStream)
		{
			string result = String.Empty;

			using (var zipInputStream = new ZipInputStream(inputStream))
			{
				ZipEntry zipEntry = zipInputStream.GetNextEntry();

				if (zipEntry != null)
				{
					var buffer = new byte[BUFFER_SIZE];

					var stream = new MemoryStream((int)zipEntry.Size);
					StreamUtils.Copy(zipInputStream, stream, buffer);

					using (var sr = new StreamReader(stream))
					{
						stream.Seek(0, SeekOrigin.Begin);

						result = sr.ReadToEnd();
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Compress memoryStream
		/// </summary>
		/// <param name="memoryStream"></param>
		/// <param name="fileInArchive"></param>
		/// <returns></returns>
		public MemoryStream CompressMemoryStream(MemoryStream memoryStream, string fileInArchive)
		{
			var outStream = new MemoryStream();

			using (var zipStream = new ZipOutputStream(outStream))
			{
				zipStream.SetLevel(COMPRESSION_LEVEL); //0-9, 9 being the highest level of compression

				var newEntry = new ZipEntry(fileInArchive) { DateTime = DateTime.UtcNow };

				zipStream.PutNextEntry(newEntry);

				StreamUtils.Copy(memoryStream, zipStream, new byte[BUFFER_SIZE]);
				zipStream.CloseEntry();

				zipStream.IsStreamOwner = false; // False stops the Close also Closing the underlying stream.
			}

			outStream.Position = 0;
			return outStream;
		}
		

		/// <summary>
		/// 	Makes backup file
		/// </summary>
		/// <param name="filePath"> The current file path. </param>
		public void BackUpFile(string filePath)
		{
			var currentDirectory = Path.GetDirectoryName(filePath);
			if (currentDirectory == null)
			{
				throw new DirectoryNotFoundException("Cannot find directory.");
			}

			var backUpFileName = String.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(filePath),
											   DateTime.UtcNow.ToString(BACKUP_FILE_NAME_FORMAT),
											   Path.GetExtension(filePath));
			var backupFilePath = Path.Combine(currentDirectory, backUpFileName);
			if (FileExists(backupFilePath))
			{
				throw new InvalidDataException("BackUp file already exists: " + backupFilePath);
			}

			CopyFile(filePath, backupFilePath);
		}

		/// <summary>
		/// 	Creates all directories and subdirectories as specified by path.
		/// </summary>
		/// <param name="directoryPath"> The directory path. </param>
		/// <returns> </returns>
		public DirectoryInfo CreateDirectoryIfNotExists(string directoryPath)
		{
			var directoryInfo = new DirectoryInfo(directoryPath);

			if (!DirectoryExists(directoryPath))
			{
				CreateDirectory(directoryPath);
			}

			return directoryInfo;
		}


		/// <summary>
		/// 	Creates File
		/// </summary>
		/// <param name="filePath"> The current file path. </param>
		public FileStream CreateFile(string filePath)
		{
			return File.Create(filePath);
		}

	

		/// <summary>
		/// Decompresses the input stream.
		/// </summary>
		/// <param name="inputStream">The archive stream.</param>
		/// <returns></returns>
		public Dictionary<string, Stream> DecompressStream(Stream inputStream)
		{
			var result = new Dictionary<String, Stream>();

			using (var zipInputStream = new ZipInputStream(inputStream))
			{
				ZipEntry zipEntry = zipInputStream.GetNextEntry();
				while (zipEntry != null)
				{
					var buffer = new byte[BUFFER_SIZE];
					var stream = new MemoryStream((int)zipEntry.Size);

					StreamUtils.Copy(zipInputStream, stream, buffer);
					result.Add(zipEntry.Name, stream);

					zipEntry = zipInputStream.GetNextEntry();
				}
			}

			return result;
		}

		/// <summary>
		/// 	Gets filestream by file path
		/// </summary>
		/// <param name="filePath"> The current file path. </param>
		/// <returns> </returns>
		public Stream GetFileStream(string filePath)
		{
			return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}




		/// <summary>
		/// Compress files
		/// </summary>
		/// <param name="files">The files that will be included to the compress file.</param>
		/// <param name="destinationFilePath">The path to the compress file that will be created</param>
		public void CompressFiles(IDictionary<string, byte[]> files, string destinationFilePath)
		{
			using (var outStream = new FileStream(destinationFilePath, FileMode.OpenOrCreate))
			{
				using (var zipStream = new ZipOutputStream(outStream))
				{
					zipStream.SetLevel(COMPRESSION_LEVEL);

					foreach (var embeddedFile in files)
					{
						var zipEntity = new ZipEntry(embeddedFile.Key)
							{
								DateTime = DateTime.UtcNow,
								Size = embeddedFile.Value.Length
							};

						zipStream.PutNextEntry(zipEntity);

						var buffer = new byte[BUFFER_SIZE];
						using (var embeddedFileStream = new MemoryStream(embeddedFile.Value))
						{
							StreamUtils.Copy(embeddedFileStream, zipStream, buffer);
						}
					}

					zipStream.CloseEntry();

					zipStream.IsStreamOwner = true;
				}
			}
		}

	

		/// <summary>
		/// Compress File
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="destinationFilePath">The path to the compress file that will be created</param>
		/// <param name="fileNameInArchive">This file name will be added to the archive.</param>
		public void CompressFile(String sourceFile, String destinationFilePath, String fileNameInArchive = null)
		{
			var outStream = new FileStream(destinationFilePath, FileMode.OpenOrCreate);
			using (var zipStream = new ZipOutputStream(outStream))
			{
				zipStream.SetLevel(COMPRESSION_LEVEL);

				var fileInfo = new FileInfo(sourceFile);
				var zipEntity = new ZipEntry(fileNameInArchive ?? sourceFile)
					{
						DateTime = DateTime.UtcNow,
						Size = fileInfo.Length
					};

				zipStream.PutNextEntry(zipEntity);

				var buffer = new byte[BUFFER_SIZE];
				using (FileStream streamReader = File.OpenRead(sourceFile))
				{
					StreamUtils.Copy(streamReader, zipStream, buffer);
				}

				zipStream.CloseEntry();

				zipStream.IsStreamOwner = true;
			}
		}


		#endregion
	}
}
>>>>>>> feature4
