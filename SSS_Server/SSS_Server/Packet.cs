using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SSS_Server
{
    public class Packet : IDisposable
    {
        private bool disposed = false;
        private List<Byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        //packet types (server)
        public enum ServerPackets
        {
            welcome = 1,
            spawnPlayer,
            playerPosition,
            playerRotation
        }

        //packet types (client)
        public enum ClientPackets
        {
            welcome = 1,
            playerMovement
        }

        //constructors
        public Packet()
        {
            buffer = new List<byte>();
            readPos = 0;
        }
        public Packet(byte[] _data) //receive - read
        {
            buffer = new List<Byte>();
            readPos = 0;

            SetBytes(_data);
        }
        public Packet(int _id) //send
        {
            buffer = new List<byte>();
            readPos = 0; 

            Write(_id); 
        }

        //ultilities
        public void Reset(bool _shouldReset = true) 
        {
            if (_shouldReset)
            {
                buffer.Clear(); 
                readableBuffer = null;
                readPos = 0;
            }
            else
            {
                readPos -= 4;
            }
        }
        public int UnreadLength()
        {
            return Length() - readPos; // return the remaining length (unread)
        }
        public void SetBytes(byte[] _data) //convert to array to read
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }
        public byte[] ToArray() //convert to array
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }
        public int Length() //return buffer length
        {
            return buffer.Count; 
        }

        //write
        public void Write(byte _value)
        {
            buffer.Add(_value);
        }
        public void Write(byte[] _value)
        {
            buffer.AddRange(_value);
        }
        public void Write(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(string _value)
        {
            Write(_value.Length); 
            buffer.AddRange(Encoding.ASCII.GetBytes(_value));
        }
        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
        }
        public void Write(Vector3 _value)
        {
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
        }
        public void Write(Quaternion _value)
        {
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
            Write(_value.W);
        }

        //read
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte _value = readableBuffer[readPos];
                if (_moveReadPos)
                {
                    readPos += 1; 
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }
        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] _value = buffer.GetRange(readPos, _length).ToArray(); 
                if (_moveReadPos)
                {

                    readPos += _length; 
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }
        public short ReadShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short _value = BitConverter.ToInt16(readableBuffer, readPos); 
                if (_moveReadPos)
                {
                    readPos += 2;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }
        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int _value = BitConverter.ToInt32(readableBuffer, readPos); 
                if (_moveReadPos)
                {    
                    readPos += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }
        public long ReadLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long _value = BitConverter.ToInt64(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 8;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }
        public float ReadFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float _value = BitConverter.ToSingle(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 4;
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }
        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos); 
                if (_moveReadPos)
                {
                    readPos += 1; 
                }
                return _value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }
        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt();
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length);
                if (_moveReadPos && _value.Length > 0)
                {
              
                    readPos += _length; 
                }
                return _value; 
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        public Vector3 ReadVector3(bool _moveReadPos = true)
        {
            return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        public Quaternion ReadQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }

        //garbage collection
        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
