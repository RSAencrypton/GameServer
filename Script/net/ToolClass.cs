using System;
namespace Tool
{
    public class ByteArray {
        const int DEFAULT_SIZE = 1024;
        int initSize = 0;
        int capacity = 0;

        public byte[] bytes;
        public int readIndex;
        public int writeIndex;
        public int length { get { return writeIndex - readIndex; } }
        public int remain { get { return capacity - writeIndex; } }

        //the constructor if having byte stream
        public ByteArray(byte[] input) {
            bytes = input;
            readIndex = 0;
            writeIndex = bytes.Length;
            initSize = bytes.Length;
            capacity = bytes.Length;
        }

        //the constuctor if no byte stream
        public ByteArray(int size = DEFAULT_SIZE) {
            bytes = new byte[size];
            readIndex = 0;
            writeIndex = 0;
            initSize = size;
            capacity = size;
        }

        public void Resize(int size) {
            if (size < initSize) return;
            if (size < length) return;

            int n = 1;
            while (n < size) n *= 2;
            capacity = n;
            byte[] newBytes = new byte[n];
            Array.Copy(bytes, readIndex, newBytes, 0, writeIndex - readIndex);
            bytes = newBytes;
            writeIndex = length;
            readIndex = 0;
        }

        public void CheckAndMoveBytes() {
            if (length < 8) {
                MoveBytes();
            }
        }
        public void MoveBytes() {
            Array.Copy(bytes, readIndex, bytes, 0, length);
            writeIndex = length;
            readIndex = 0;
        }

        public int Add(byte[] input, int offset, int count) {
            if (remain < count) {
                Resize(count + length);
            }

            Array.Copy(input, offset, bytes, writeIndex, count);
            writeIndex += count;
            return count;
        }

        public int Read(byte[] input, int offset, int count) {
            count = Math.Min(count, length);
            Array.Copy(bytes, 0, input, offset, count);
            readIndex += count;
            CheckAndMoveBytes();
            return count;
        }

        public Int16 ReadInt16() {
            if (length < 2) return 0;
            Int16 res = (Int16)((bytes[1] << 8) | bytes[0]);
            readIndex += 2;
            CheckAndMoveBytes();
            return res;
        }

        public Int32 ReadInt32() {
            if (length < 4) return 0;
            Int32 res = (Int32)((bytes[3] << 24) |
                (bytes[2] << 16) |
                (bytes[1] << 8) |
                bytes[0]);
            readIndex += 4;
            CheckAndMoveBytes();
            return res;
        }
    }
}

