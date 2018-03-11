using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    class CreatureModelData
    {
        private long FileSize;
        private string string_block;

        public DBC_Header header;
        public DBC_Body body;

        public bool loadDBCFile(string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                FileSize = fs.Length;

                // Read header
                int count = Marshal.SizeOf(typeof(DBC_Header));
                byte[] readBuffer = new byte[count];
                BinaryReader reader = new BinaryReader(fs);
                readBuffer = reader.ReadBytes(count);
                GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                header = (DBC_Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DBC_Header));
                handle.Free();

                // Prepare body
                body.records = new RecordMap[header.record_count];

                // Read body
                for (UInt32 i = 0; i < header.record_count; ++i)
                {
                    count = Marshal.SizeOf(typeof(Record));
                    readBuffer = new byte[count];
                    readBuffer = reader.ReadBytes(count);
                    handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                    body.records[i].record = (Record)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Record));
                    handle.Free();
                }

                // Read string block
                string_block = Encoding.UTF8.GetString(reader.ReadBytes(header.string_block_size));

                reader.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public bool SaveDBCFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);

            // Write header
            int count = Marshal.SizeOf(typeof(DBC_Header));
            byte[] buffer = new byte[count];
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(header, gcHandle.AddrOfPinnedObject(), true);
            writer.Write(buffer, 0, count);
            gcHandle.Free();

            // Write records
            for (UInt32 i = 0; i < header.record_count; ++i)
            {
                // Write main body
                count = Marshal.SizeOf(typeof(Record));
                buffer = new byte[count];
                gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Marshal.StructureToPtr(body.records[i].record, gcHandle.AddrOfPinnedObject(), true);
                writer.Write(buffer, 0, count);
                gcHandle.Free();
            }

            // Write string block
            writer.Write(Encoding.UTF8.GetBytes(string_block));

            writer.Close();
            fs.Close();

            return true;
        }

        public void modifyBitMasks(uint hasMask, uint modifyMask, bool remove, bool raceMask)
        {
            if (raceMask)
            {
                for (int i = 0; i < body.records.Length; ++i)
                {
                    for (int j = 0; j < body.records[i].record.raceMasks.Length; ++j)
                    {
                        if ((body.records[i].record.raceMasks[j] & hasMask) == hasMask)
                        {
                            if (remove)
                                body.records[i].record.raceMasks[j] &= ~modifyMask;
                            else
                                body.records[i].record.raceMasks[j] |= modifyMask;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < body.records.Length; ++i)
                {
                    for (int j = 0; j < body.records[i].record.classmasks.Length; ++j)
                    {
                        if ((body.records[i].record.classmasks[j] & hasMask) == hasMask)
                        {
                            if (remove)
                                body.records[i].record.classmasks[j] &= ~modifyMask;
                            else
                                body.records[i].record.classmasks[j] |= modifyMask;
                        }
                    }
                }
            }
        }
    }

    // Class instead of struct allows passing by reference
    public class VirtualStrTableEntry
    {
        public string value;
        public UInt32 newValue;
    };

    public struct DBC_Header
    {
        public UInt32 magic;
        public UInt32 record_count;
        public UInt32 field_count;
        public UInt32 record_size;
        public Int32 string_block_size;
    };

    public struct DBC_Body
    {
        public RecordMap[] records;
    };

    public struct RecordMap
    {
        public Record record;
        public string modelPath;
    };

    public struct Record
    {
        public UInt32 id;
        public Int32 repIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] raceMasks;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] classmasks;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int32[] reputationBase;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int32[] reputationFlags;
        public UInt32 parentFactionID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] parentFactionMod;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Int32[] parentFactionCap;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public UInt32[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public UInt32[] Description;
    };
}
