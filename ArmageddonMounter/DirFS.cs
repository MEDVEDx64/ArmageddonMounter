using DokanNet;
using Syroot.Worms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace ArmageddonMounter
{
    class DirFS : IDokanOperations
    {
        Archive arc;
        string arcPath;
        string volumeName;

        public DirFS(string arcPath)
        {
            this.arcPath = arcPath;
            arc = new Archive(arcPath);

            var path = arcPath.Split('\\');
            volumeName = path[path.Length - 1];
        }

        public NtStatus Save()
        {
            try
            {
                arc.Save(arcPath);
            }

            catch
            {
                return NtStatus.InternalError;
            }

            return DokanResult.Success;
        }

        NtStatus AllocateFile(string key)
        {
            if (key == "desktop.ini" || key == "Thumbs.db")
                // We don't need these files here
                return DokanResult.AccessDenied;

            if (!arc.ContainsKey(key))
                arc[key] = new byte[0];

            return DokanResult.Success;
        }

        string GetFileKey(string path)
        {
            if (path == "\\")
                return path;

            if (path.StartsWith("\\"))
                return path.Substring(1);

            return path;
        }

        // ----- Dokan interface methods -----

        public void Cleanup(string fileName, IDokanFileInfo info)
        {
        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {
            Save();
        }

        public NtStatus CreateFile(string fileName, DokanNet.FileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, IDokanFileInfo info)
        {
            fileName = GetFileKey(fileName);

            if ((mode == FileMode.Create || mode == FileMode.OpenOrCreate) && arc.ContainsKey(fileName))
            {
                return DokanResult.AlreadyExists;
            }

            return DokanResult.Success;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            return DokanResult.FileNotFound;
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            fileName = GetFileKey(fileName);
            if (arc.ContainsKey(fileName))
            {
                arc.Remove(fileName);
                return DokanResult.Success;
            }

            return DokanResult.FileNotFound;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new List<FileInformation>();

            foreach (var k in arc.Keys)
            {
                files.Add(new FileInformation()
                {
                    FileName = k,
                    Length = arc[k].Length,
                });
            }

            return DokanResult.Success;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new List<FileInformation>();
            return DokanResult.NotImplemented;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams = new List<FileInformation>();
            return NtStatus.NotImplemented;
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
        {
            return Save();
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, IDokanFileInfo info)
        {
            long bytes = 0;
            foreach(var k in arc.Keys)
            {
                bytes += arc[k].Length;
            }

            totalNumberOfBytes = uint.MaxValue;
            freeBytesAvailable = (bytes > totalNumberOfBytes ? 0 : (totalNumberOfBytes - bytes));
            totalNumberOfFreeBytes = freeBytesAvailable;

            return DokanResult.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, IDokanFileInfo info)
        {
            fileInfo = new FileInformation();

            if(fileName == "\\")
            {
                fileInfo.Attributes = FileAttributes.Directory;
                return DokanResult.Success;
            }

            fileName = GetFileKey(fileName);
            if (!arc.ContainsKey(fileName))
                return DokanResult.FileNotFound;

            fileInfo.FileName = fileName;
            fileInfo.Length = arc[fileName].Length;

            return DokanResult.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            security = null;
            return NtStatus.NotImplemented;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = volumeName;
            features = FileSystemFeatures.CasePreservedNames | FileSystemFeatures.CaseSensitiveSearch;
            fileSystemName = "WORMS2";
            maximumComponentLength = 255;

            return DokanResult.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus Mounted(IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            if (oldName == newName)
                return DokanResult.FileExists;

            oldName = GetFileKey(oldName);
            newName = GetFileKey(newName);

            arc[newName] = arc[oldName];
            arc.Remove(oldName);
            return DokanResult.Success;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            fileName = GetFileKey(fileName);
            if (!arc.ContainsKey(fileName))
            {
                bytesRead = 0;
                return DokanResult.FileNotFound;
            }

            var file = arc[fileName];
            int opSize = Math.Min(buffer.Length, file.Length - (int)offset);
            Array.Copy(file, offset, buffer, 0, opSize);
            bytesRead = opSize;

            return DokanResult.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            return SetEndOfFile(fileName, length, info);
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            fileName = GetFileKey(fileName);

            var status = AllocateFile(fileName);
            if (status != DokanResult.Success)
                return status;

            var file = new byte[length];
            Array.Copy(arc[fileName], file, Math.Min(length, arc[fileName].Length));
            arc[fileName] = file;

            return DokanResult.Success;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            // Attribures is not supported here, so just ignoring it.
            return DokanResult.Success;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, IDokanFileInfo info)
        {
            // Storing of timestamps is not supported
            return DokanResult.Success;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            return Save();
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            fileName = GetFileKey(fileName);

            var status = AllocateFile(fileName);
            if (status != DokanResult.Success)
            {
                bytesWritten = 0;
                return status;
            }

            if (offset == -1) // Append mode
                offset = arc[fileName].Length;

            if ((buffer.Length + offset) > arc[fileName].Length)
            {
                status = SetEndOfFile(fileName, buffer.Length + offset, info);
                if (status != DokanResult.Success)
                {
                    bytesWritten = 0;
                    return status;
                }
            }

            Array.Copy(buffer, 0, arc[fileName], offset, buffer.Length);
            bytesWritten = buffer.Length;

            return DokanResult.Success;
        }
    }
}
