using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;




namespace swed64
{
    public class swed
    {

        #region imports

        [DllImport("Kernel32.dll")]

        static extern bool ReadProcessMemory(
           IntPtr hProcess,
           IntPtr lpBaseAddress,
           [Out] byte[] lpBuffer,
           int nSize,
           IntPtr lpNumberOfBytesRead
           );

        [DllImport("kernel32.dll")]

        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int size,
            IntPtr lpNumberOfBytesWritten
            );




        #endregion


        #region globals


        public static Process proc;


        #endregion


        public Process GetProcess(string procname)
        {
            proc = Process.GetProcessesByName(procname)[0];
            return proc;
        }
        public IntPtr GetModuleBase(string modulename)
        {
            if (string.IsNullOrEmpty(modulename))
            {
                return IntPtr.Zero;
            }

            if (proc == null)
            {
                return IntPtr.Zero;
            }

            try
            {
                if (modulename.Contains(".exe"))
                {
                    return proc.MainModule.BaseAddress;
                }

                foreach (ProcessModule module in proc.Modules)
                {
                    if (module.ModuleName == modulename)
                    {
                        return module.BaseAddress;
                    }
                }
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }

            return IntPtr.Zero;
        }


        // Main methods 
        public IntPtr ReadPointer(IntPtr addy)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(buffer);
        }
        public IntPtr ReadPointer(IntPtr addy, int offset)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(proc.Handle, IntPtr.Add(addy, offset), buffer, buffer.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(buffer);
        }

        public static byte[] ReadBytes(IntPtr addy, int bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public byte[] ReadBytes(IntPtr addy, int offset, int bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public bool WriteBytes(IntPtr address, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address, newbytes, newbytes.Length, IntPtr.Zero);
        }
        public bool WriteBytes(IntPtr address, int offset, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address + offset, newbytes, newbytes.Length, IntPtr.Zero);
        }

        // different return types 
        public int ReadInt(IntPtr address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4));
        }
        public int ReadInt(IntPtr address, int offset)
        {
            return BitConverter.ToInt32(ReadBytes(address + offset, 4));
        }

        public IntPtr ReadLong(IntPtr address)
        {
            return (IntPtr)BitConverter.ToInt64(ReadBytes(address, 8));
        }
        public IntPtr ReadLong(IntPtr address, int offset)
        {
            return (IntPtr)BitConverter.ToInt64(ReadBytes(address + offset, 8));
        }

        public float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(ReadBytes(address, 4));
        }
        public float ReadFloat(IntPtr address, int offset)
        {
            return BitConverter.ToSingle(ReadBytes(address + offset, 4));
        }

        public double ReadDouble(IntPtr address)
        {
            return BitConverter.ToDouble(ReadBytes(address, 8));
        }
        public double ReadDouble(IntPtr address, int offset)
        {
            return BitConverter.ToDouble(ReadBytes(address + offset, 4));
        }




        public Vector3 ReadVec(IntPtr address)
        {
            var bytes = ReadBytes(address, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(bytes, 0),
                Y = BitConverter.ToSingle(bytes, 4),
                Z = BitConverter.ToSingle(bytes, 8)
            };
        }

        public Vector3 ReadVec(IntPtr address, int offset)
        {
            var bytes = ReadBytes(address + offset, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(bytes, 0),
                Y = BitConverter.ToSingle(bytes, 4),
                Z = BitConverter.ToSingle(bytes, 8)
            };
        }

        public short ReadShort(IntPtr address)
        {
            return BitConverter.ToInt16(ReadBytes(address, 2));
        }

        public short ReadShort(IntPtr address, int offset)
        {
            return BitConverter.ToInt16(ReadBytes(address + offset, 2));
        }

        public ushort ReadUShort(IntPtr address)
        {
            return BitConverter.ToUInt16(ReadBytes(address, 2));
        }

        public ushort ReadUShort(IntPtr address, int offset)
        {
            return BitConverter.ToUInt16(ReadBytes(address + offset, 2));
        }

        public uint ReadUInt(IntPtr address)
        {
            return BitConverter.ToUInt32(ReadBytes(address, 4));
        }

        public uint ReadUInt(IntPtr address, int offset)
        {
            return BitConverter.ToUInt32(ReadBytes(address + offset, 4));
        }


        public ulong ReadULong(IntPtr address)
        {
            return BitConverter.ToUInt64(ReadBytes(address, 8));
        }

        public ulong ReadULong(IntPtr address, int offset)
        {
            return BitConverter.ToUInt64(ReadBytes(address + offset, 8));
        }

        public bool ReadBool(IntPtr address)
        {
            return BitConverter.ToBoolean(ReadBytes(address, 1));
        }

        public bool ReadBool(IntPtr address, int offset)
        {
            return BitConverter.ToBoolean(ReadBytes(address + offset, 1));
        }

        public string ReadString(IntPtr address, int length)
        {
            return Encoding.UTF8.GetString(ReadBytes(address, length));
        }

        public string ReadString(IntPtr address, int offset, int length)
        {
            return Encoding.UTF8.GetString(ReadBytes(address + offset, length));
        }

        public char ReadChar(IntPtr address)
        {
            return BitConverter.ToChar(ReadBytes(address, 2));
        }

        public char ReadChar(IntPtr address, int offset)
        {
            return BitConverter.ToChar(ReadBytes(address + offset, 2));
        }

        public float[] ReadMatrix(IntPtr address)
        {
            var bytes = ReadBytes(address, 4 * 16);
            var matrix = new float[bytes.Length];

            matrix[0] = BitConverter.ToSingle(bytes, 0 * 4);
            matrix[1] = BitConverter.ToSingle(bytes, 1 * 4);
            matrix[2] = BitConverter.ToSingle(bytes, 2 * 4);
            matrix[3] = BitConverter.ToSingle(bytes, 3 * 4);

            matrix[4] = BitConverter.ToSingle(bytes, 4 * 4);
            matrix[5] = BitConverter.ToSingle(bytes, 5 * 4);
            matrix[6] = BitConverter.ToSingle(bytes, 6 * 4);
            matrix[7] = BitConverter.ToSingle(bytes, 7 * 4);

            matrix[8] = BitConverter.ToSingle(bytes, 8 * 4);
            matrix[9] = BitConverter.ToSingle(bytes, 9 * 4);
            matrix[10] = BitConverter.ToSingle(bytes, 10 * 4);
            matrix[11] = BitConverter.ToSingle(bytes, 11 * 4);

            matrix[12] = BitConverter.ToSingle(bytes, 12 * 4);
            matrix[13] = BitConverter.ToSingle(bytes, 13 * 4);
            matrix[14] = BitConverter.ToSingle(bytes, 14 * 4);
            matrix[15] = BitConverter.ToSingle(bytes, 15 * 4);

            return matrix;

        }


        // write methods 

        public bool WriteInt(IntPtr address, int value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteInt(IntPtr address, int offset, int value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteShort(IntPtr address, short value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteShort(IntPtr address, int offset, short value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteUShort(IntPtr address, ushort value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteUShort(IntPtr address, int offset, ushort value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteUInt(IntPtr address, uint value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteUInt(IntPtr address, int offset, uint value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteLong(IntPtr address, long value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteLong(IntPtr address, int offset, long value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteULong(IntPtr address, ulong value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteULong(IntPtr address, int offset, ulong value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteFloat(IntPtr address, float value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteFloat(IntPtr address, int offset, float value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteDouble(IntPtr address, double value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteDouble(IntPtr address, int offset, double value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteBool(IntPtr address, bool value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteBool(IntPtr address, int offset, bool value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteString(IntPtr address, string value)
        {
            return WriteBytes(address, Encoding.UTF8.GetBytes(value));
        }

        public bool WriteVec(IntPtr address, Vector3 value)
        {
            byte[] bytes = new byte[12];
            byte[] xBytes = BitConverter.GetBytes(value.X);
            byte[] yBytes = BitConverter.GetBytes(value.Y);
            byte[] zBytes = BitConverter.GetBytes(value.Z);
            xBytes.CopyTo(bytes, 0);
            yBytes.CopyTo(bytes, 4);
            zBytes.CopyTo(bytes, 8);
            return WriteBytes(address, bytes);
        }

        public bool WriteVec(IntPtr address, int offset, Vector3 value)
        {
            byte[] bytes = new byte[12];
            byte[] xBytes = BitConverter.GetBytes(value.X);
            byte[] yBytes = BitConverter.GetBytes(value.Y);
            byte[] zBytes = BitConverter.GetBytes(value.Z);
            xBytes.CopyTo(bytes, 0);
            yBytes.CopyTo(bytes, 4);
            zBytes.CopyTo(bytes, 8);
            return WriteBytes(address + offset, bytes);
        }
    }
}