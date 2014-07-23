using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UpdateVersionInfo
{
    public static class VersionInfoResource
    {
        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr BeginUpdateResource(String pFileName, Boolean bDeleteExistingResources);

            [DllImport("kernel32.dll")]
            public static extern Boolean EndUpdateResource(IntPtr hUpdate, Boolean fDiscard);

            [DllImport("kernel32.dll")]
            public static extern Boolean UpdateResource(IntPtr hUpdate, IntPtr lpType, Int32 lpName, UInt16 wLanguage, Byte[] lpData, UInt32 cbData);

            [DllImport("version.dll")]
            public static extern Boolean GetFileVersionInfo(String lptstrFilename, Int32 dwHandleIgnored, Int32 dwLen, Byte[] lpData);

            [DllImport("version.dll")]
            public static extern Int32 GetFileVersionInfoSize(String sFileName, out IntPtr handle);
        }

        public static class Structures
        {
            public struct VS_VERSIONINFO
            {
                public UInt16 wLength;
                public UInt16 wValueLength;
                public UInt16 wType;

                public String szKey;

                public UInt16 Padding1;

                public VS_FIXEDFILEINFO Value;

                public UInt16 Padding2;

                public VarFileInfo VarFileInfo;
                public StringFileInfo StringFileInfo;

                public UInt16 GetLength()
                {
                    UInt16 result;

                    result = (UInt16)Marshal.SizeOf(this.wLength);
                    result += (UInt16)Marshal.SizeOf(this.wValueLength);
                    result += (UInt16)Marshal.SizeOf(this.wType);
                    result += (UInt16)(Encoding.Unicode.GetByteCount(this.szKey) + 2);
                    result += (UInt16)(result % 4);
                    result += (UInt16)(this.Value.GetLength());
                    result += (UInt16)(result % 4);
                    result += (UInt16)(this.VarFileInfo.GetLength());
                    result += (UInt16)(this.StringFileInfo.GetLength());

                    return result;
                }

                public UInt16 GetValueLength()
                {
                    return (UInt16)(this.Value.GetLength());
                }

                public void LoadFromStream(BinaryReader reader)
                {
                    Int32 startPosition = (Int32)reader.BaseStream.Position;

                    this.wLength = reader.ReadUInt16();
                    this.wValueLength = reader.ReadUInt16();
                    this.wType = reader.ReadUInt16();

                    // szKey
                    {
                        StringBuilder szKeyBuilder = new StringBuilder();
                        UInt16 @char;

                        while ((@char = reader.ReadUInt16()) != 0)
                        {
                            szKeyBuilder.Append(Convert.ToChar(@char));
                        }

                        this.szKey = szKeyBuilder.ToString();
                    }

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding1 = reader.ReadUInt16();
                    }

                    this.Value.LoadFromStream(reader);

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding2 = reader.ReadUInt16();
                    }

                    this.VarFileInfo.LoadFromStream(reader);
                    this.StringFileInfo.LoadFromStream(reader);
                }

                public void WriteToStream(BinaryWriter writer)
                {
                    Int32 startPosition = (Int32)writer.BaseStream.Position;

                    this.wLength = GetLength();
                    this.wValueLength = GetValueLength();
                    this.wType = 0;

                    writer.Write(this.wLength);
                    writer.Write(this.wValueLength);
                    writer.Write(this.wType);

                    // szKey
                    {
                        foreach (Char @char in this.szKey)
                        {
                            writer.Write(Convert.ToUInt16(@char));
                        }

                        writer.Write((UInt16)0);
                    }

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }

                    this.Value.WriteToStream(writer);

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }

                    this.VarFileInfo.WriteToStream(writer);
                    this.StringFileInfo.WriteToStream(writer);
                }
            }

            public struct VS_FIXEDFILEINFO
            {
                public UInt32 dwSignature;
                public UInt32 dwStructVersion;
                public UInt32 dwFileVersionMS;
                public UInt32 dwFileVersionLS;
                public UInt32 dwProductVersionMS;
                public UInt32 dwProductVersionLS;
                public UInt32 dwFileFlagsMask;
                public UInt32 dwFileFlags;
                public UInt32 dwFileOS;
                public UInt32 dwFileType;
                public UInt32 dwFileSubtype;
                public UInt32 dwFileDateMS;
                public UInt32 dwFileDateLS;

                public UInt16 GetLength()
                {
                    return sizeof(UInt32) * 13;
                }

                public void LoadFromStream(BinaryReader reader)
                {
                    this.dwSignature = reader.ReadUInt32();
                    this.dwStructVersion = reader.ReadUInt32();
                    this.dwFileVersionMS = reader.ReadUInt32();
                    this.dwFileVersionLS = reader.ReadUInt32();
                    this.dwProductVersionMS = reader.ReadUInt32();
                    this.dwProductVersionLS = reader.ReadUInt32();
                    this.dwFileFlagsMask = reader.ReadUInt32();
                    this.dwFileFlags = reader.ReadUInt32();
                    this.dwFileOS = reader.ReadUInt32();
                    this.dwFileType = reader.ReadUInt32();
                    this.dwFileSubtype = reader.ReadUInt32();
                    this.dwFileDateMS = reader.ReadUInt32();
                    this.dwFileDateLS = reader.ReadUInt32();
                }

                public void WriteToStream(BinaryWriter writer)
                {
                    writer.Write(this.dwSignature);
                    writer.Write(this.dwStructVersion);
                    writer.Write(this.dwFileVersionMS);
                    writer.Write(this.dwFileVersionLS);
                    writer.Write(this.dwProductVersionMS);
                    writer.Write(this.dwProductVersionLS);
                    writer.Write(this.dwFileFlagsMask);
                    writer.Write(this.dwFileFlags);
                    writer.Write(this.dwFileOS);
                    writer.Write(this.dwFileType);
                    writer.Write(this.dwFileSubtype);
                    writer.Write(this.dwFileDateMS);
                    writer.Write(this.dwFileDateLS);
                }
            }

            public struct VarFileInfo
            {
                public UInt16 wLength;
                public UInt16 wValueLength;
                public UInt16 wType;

                public String szKey;

                public UInt16 Padding;

                public Var Children;

                public UInt16 GetLength()
                {
                    UInt16 result;

                    result = (UInt16)Marshal.SizeOf(this.wLength);
                    result += (UInt16)Marshal.SizeOf(this.wValueLength);
                    result += (UInt16)Marshal.SizeOf(this.wType);
                    result += (UInt16)(Encoding.Unicode.GetByteCount(this.szKey) + 2);
                    result += (UInt16)(result % 4);
                    result += (UInt16)this.Children.GetLength();

                    return result;
                }

                public UInt16 GetValueLength()
                {
                    return 0;
                }

                public void LoadFromStream(BinaryReader reader)
                {
                    Int32 startPosition = (Int32)reader.BaseStream.Position;

                    this.wLength = reader.ReadUInt16();
                    this.wValueLength = reader.ReadUInt16();
                    this.wType = reader.ReadUInt16();

                    // szKey
                    {
                        StringBuilder szKeyBuilder = new StringBuilder();
                        UInt16 @char;

                        while ((@char = reader.ReadUInt16()) != 0)
                        {
                            szKeyBuilder.Append(Convert.ToChar(@char));
                        }

                        this.szKey = szKeyBuilder.ToString();
                    }

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding = reader.ReadUInt16();
                    }

                    this.Children.LoadFromStream(reader);
                }

                public void WriteToStream(BinaryWriter writer)
                {
                    Int32 startPosition = (Int32)writer.BaseStream.Position;

                    this.wLength = GetLength();
                    this.wValueLength = GetValueLength();
                    this.wType = 1;

                    writer.Write(this.wLength);
                    writer.Write(this.wValueLength);
                    writer.Write(this.wType);

                    // szKey
                    {
                        foreach (Char @char in this.szKey)
                        {
                            writer.Write(Convert.ToUInt16(@char));
                        }

                        writer.Write((UInt16)0);
                    }

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }

                    this.Children.WriteToStream(writer);
                }
            }

            public struct Var
            {
                public UInt16 wLength;
                public UInt16 wValueLength;
                public UInt16 wType;

                public String szKey;

                public UInt16 Padding;

                public UInt32 Value;

                public UInt16 GetLength()
                {
                    UInt16 result;

                    result = (UInt16)Marshal.SizeOf(this.wLength);
                    result += (UInt16)Marshal.SizeOf(this.wValueLength);
                    result += (UInt16)Marshal.SizeOf(this.wType);
                    result += (UInt16)(Encoding.Unicode.GetByteCount(this.szKey) + 2);
                    result += (UInt16)(result % 4);
                    result += (UInt16)Marshal.SizeOf(this.Value);

                    return result;
                }

                public UInt16 GetValueLength()
                {
                    return (UInt16)Marshal.SizeOf(this.Value);
                }

                public void LoadFromStream(BinaryReader reader)
                {
                    Int32 startPosition = (Int32)reader.BaseStream.Position;

                    this.wLength = reader.ReadUInt16();
                    this.wValueLength = reader.ReadUInt16();
                    this.wType = reader.ReadUInt16();

                    // szKey
                    {
                        StringBuilder szKeyBuilder = new StringBuilder();
                        UInt16 @char;

                        while ((@char = reader.ReadUInt16()) != 0)
                        {
                            szKeyBuilder.Append(Convert.ToChar(@char));
                        }

                        this.szKey = szKeyBuilder.ToString();
                    }

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding = reader.ReadUInt16();
                    }

                    this.Value = reader.ReadUInt32();
                }

                public void WriteToStream(BinaryWriter writer)
                {
                    Int32 startPosition = (Int32)writer.BaseStream.Position;

                    this.wLength = GetLength();
                    this.wValueLength = GetValueLength();
                    this.wType = 0;

                    writer.Write(this.wLength);
                    writer.Write(this.wValueLength);
                    writer.Write(this.wType);

                    // szKey
                    {
                        foreach (Char @char in this.szKey)
                        {
                            writer.Write(Convert.ToUInt16(@char));
                        }

                        writer.Write((UInt16)0);
                    }

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }

                    writer.Write(this.Value);
                }
            }

            public struct StringFileInfo
            {
                public UInt16 wLength;
                public UInt16 wValueLength;
                public UInt16 wType;

                public String szKey;

                public UInt16 Padding;

                public StringTable Children;

                public UInt16 GetLength()
                {
                    UInt16 result;

                    result = (UInt16)Marshal.SizeOf(this.wLength);
                    result += (UInt16)Marshal.SizeOf(this.wValueLength);
                    result += (UInt16)Marshal.SizeOf(this.wType);
                    result += (UInt16)(Encoding.Unicode.GetByteCount(this.szKey) + 2);
                    result += (UInt16)(result % 4);
                    result += this.Children.GetLength();

                    return result;
                }

                public UInt16 GetValueLength()
                {
                    return 0;
                }

                public void LoadFromStream(BinaryReader reader)
                {
                    Int32 startPosition = (Int32)reader.BaseStream.Position;

                    this.wLength = reader.ReadUInt16();
                    this.wValueLength = reader.ReadUInt16();
                    this.wType = reader.ReadUInt16();

                    // szKey
                    {
                        StringBuilder szKeyBuilder = new StringBuilder();
                        UInt16 @char;

                        while ((@char = reader.ReadUInt16()) != 0)
                        {
                            szKeyBuilder.Append(Convert.ToChar(@char));
                        }

                        this.szKey = szKeyBuilder.ToString();
                    }

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding = reader.ReadUInt16();
                    }

                    this.Children.LoadFromStream(reader);
                }

                public void WriteToStream(BinaryWriter writer)
                {
                    Int32 startPosition = (Int32)writer.BaseStream.Position;

                    this.wLength = GetLength();
                    this.wValueLength = GetValueLength();
                    this.wType = 1;

                    writer.Write(this.wLength);
                    writer.Write(this.wValueLength);
                    writer.Write(this.wType);

                    // szKey
                    {
                        foreach (Char @char in this.szKey)
                        {
                            writer.Write(Convert.ToUInt16(@char));
                        }

                        writer.Write((UInt16)0);
                    }

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }

                    this.Children.WriteToStream(writer);
                }
            }

            public struct StringTable
            {
                public UInt16 wLength;
                public UInt16 wValueLength;
                public UInt16 wType;

                public String szKey;

                public UInt16 Padding;

                public Dictionary<String, StringValue> Children;

                public UInt16 GetLength()
                {
                    UInt16 result;

                    result = (UInt16)Marshal.SizeOf(this.wLength);
                    result += (UInt16)Marshal.SizeOf(this.wValueLength);
                    result += (UInt16)Marshal.SizeOf(this.wType);
                    result += (UInt16)(Encoding.Unicode.GetByteCount(this.szKey) + 2);
                    result += (UInt16)(result % 4);

                    foreach (StringValue value in this.Children.Values)
                    {
                        result += value.GetLength();
                    }

                    return result;
                }

                public UInt16 GetValueLength()
                {
                    return 0;
                }

                public void LoadFromStream(BinaryReader reader)
                {
                    Int32 startPosition = (Int32)reader.BaseStream.Position;

                    this.wLength = reader.ReadUInt16();
                    this.wValueLength = reader.ReadUInt16();
                    this.wType = reader.ReadUInt16();

                    // szKey
                    {
                        StringBuilder szKeyBuilder = new StringBuilder();
                        UInt16 @char;

                        while ((@char = reader.ReadUInt16()) != 0)
                        {
                            szKeyBuilder.Append(Convert.ToChar(@char));
                        }

                        this.szKey = szKeyBuilder.ToString();
                    }

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding = reader.ReadUInt16();
                    }

                    // Values
                    {
                        this.Children = new Dictionary<String, StringValue>();

                        while (true)
                        {
                            StringValue value = new StringValue();
                            Int64 oldPosition = reader.BaseStream.Position;

                            value.LoadFromStream(reader);

                            if (value.wType == 1)
                            {
                                Children.Add(value.szKey, value);
                            }
                            else
                            {
                                reader.BaseStream.Seek(oldPosition - reader.BaseStream.Position, SeekOrigin.Current);
                                break;
                            }
                        }
                    }
                }

                public void WriteToStream(BinaryWriter writer)
                {
                    Int32 startPosition = (Int32)writer.BaseStream.Position;

                    this.wLength = GetLength();
                    this.wValueLength = GetValueLength();
                    this.wType = 1;

                    writer.Write(this.wLength);
                    writer.Write(this.wValueLength);
                    writer.Write(this.wType);

                    // szKey
                    {
                        foreach (Char @char in this.szKey)
                        {
                            writer.Write(Convert.ToUInt16(@char));
                        }

                        writer.Write((UInt16)0);
                    }

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }

                    // Values
                    foreach (StringValue value in this.Children.Values)
                    {
                        value.WriteToStream(writer);
                    }
                }
            }

            public struct StringValue
            {
                public UInt16 wLength;
                public UInt16 wValueLength;
                public UInt16 wType;

                public String szKey;

                public UInt16 Padding1;

                public String Value;

                public UInt16 Padding2;

                public UInt16 GetLength()
                {
                    UInt16 result;

                    result = (UInt16)Marshal.SizeOf(this.wLength);
                    result += (UInt16)Marshal.SizeOf(this.wValueLength);
                    result += (UInt16)Marshal.SizeOf(this.wType);
                    result += (UInt16)(Encoding.Unicode.GetByteCount(this.szKey) + 2);
                    result += (UInt16)(result % 4);
                    result += (UInt16)(Encoding.Unicode.GetByteCount(this.Value) + 2);
                    result += (UInt16)(result % 4);

                    return result;
                }

                public UInt16 GetValueLength()
                {
                    return (UInt16)(this.Value.Length + 1);
                }

                public void LoadFromStream(BinaryReader reader)
                {
                    Int32 startPosition = (Int32)reader.BaseStream.Position;

                    this.wLength = reader.ReadUInt16();
                    this.wValueLength = reader.ReadUInt16();
                    this.wType = reader.ReadUInt16();

                    // szKey
                    {
                        StringBuilder szKeyBuilder = new StringBuilder();
                        UInt16 @char;

                        while ((@char = reader.ReadUInt16()) != 0)
                        {
                            szKeyBuilder.Append(Convert.ToChar(@char));
                        }

                        this.szKey = szKeyBuilder.ToString();
                    }

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding1 = reader.ReadUInt16();
                    }

                    // Value
                    {
                        StringBuilder valueBuilder = new StringBuilder();
                        UInt16 @char;

                        while ((@char = reader.ReadUInt16()) != 0)
                        {
                            valueBuilder.Append(Convert.ToChar(@char));
                        }

                        this.Value = valueBuilder.ToString();
                    }

                    if ((reader.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        this.Padding2 = reader.ReadUInt16();
                    }
                }

                public void WriteToStream(BinaryWriter writer)
                {
                    Int32 startPosition = (Int32)writer.BaseStream.Position;

                    this.wLength = GetLength();
                    this.wValueLength = GetValueLength();
                    this.wType = 1;

                    writer.Write(this.wLength);
                    writer.Write(this.wValueLength);
                    writer.Write(this.wType);

                    // szKey
                    {
                        foreach (Char @char in this.szKey)
                        {
                            writer.Write(Convert.ToUInt16(@char));
                        }

                        writer.Write((UInt16)0);
                    }

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }

                    // Value
                    {
                        foreach (Char @char in this.Value)
                        {
                            writer.Write(Convert.ToUInt16(@char));
                        }

                        writer.Write((UInt16)0);
                    }

                    if ((writer.BaseStream.Position - startPosition) % 4 != 0)
                    {
                        writer.Write((UInt16)0);
                    }
                }
            }
        }
        
        static public Boolean LoadFromFile(String path, out Structures.VS_VERSIONINFO pVerInfo)
        {
            IntPtr dwHandle;
            Int32  dwSize;
            
            dwSize = NativeMethods.GetFileVersionInfoSize(path, out dwHandle);
            
            if (dwSize > 0)
            {
                Byte[] lpBuffer = new Byte[dwSize];
                
                if (NativeMethods.GetFileVersionInfo(path, 0, dwSize, lpBuffer) == true)
                {
                    using (BinaryReader bufferReader = new BinaryReader(new MemoryStream(lpBuffer)))
                    {
                        pVerInfo = new Structures.VS_VERSIONINFO();
                        pVerInfo.LoadFromStream(bufferReader);
                    }
                    
                    return true;
                }
            }
            
            pVerInfo = default(Structures.VS_VERSIONINFO);
            
            return false;
        }
        
        static public Boolean WriteToFile(String path, Structures.VS_VERSIONINFO pVerInfo)
        {
            IntPtr RT_VERSION = new IntPtr(16);
            IntPtr hResource = NativeMethods.BeginUpdateResource(path, false);
                
            if (hResource != IntPtr.Zero)
            {
                try
                {
                    Byte[] lpWriteData;
                    
                    using (BinaryWriter bufferWriter = new BinaryWriter(new MemoryStream()))
                    {
                        try
                        {
                            pVerInfo.WriteToStream(bufferWriter);
                        }
                        finally
                        {
                            lpWriteData = (bufferWriter.BaseStream as MemoryStream).ToArray();
                        }
                    }
                    
                    if (NativeMethods.UpdateResource(hResource, RT_VERSION, 1, (UInt16)0, lpWriteData, (UInt32)lpWriteData.Length) == true)
                    {
                        return true;
                    }
                }
                finally
                {
                    NativeMethods.EndUpdateResource(hResource, false);
                }
            }
            
            return false;
        }
    }
}
