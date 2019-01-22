using System;

namespace am
{
    
/// <summary>
///  AS3 のByteArrayの C# 移植版 
///  @attention バッファオーバーラン時にエラー扱いにして例外吐くとかいう部分は未実装
/// </summary> 
public class AmByteArray
{
    
    // position をセットすると、必要に応じて延長される
    public uint length;

    public uint bytesAvailable { get { return length - position; } }
    public uint position {
	get { return m_position; }
	set {
	    m_position = value;
	    //Logger.Log(value.ToString());
	    if(value > length){ length = value; }
	}
    }

    private   byte[] m_buf;
    protected uint   m_position;
    private const int DEFAULT_BUF_SIZE = 1024; // 仮

    /// <summary>
    ///   このメソッドはposition, lengthを触らないので、seekの調整は呼びだし元が責任をもって行なう事。
    /// </summary>
    unsafe public static void Memcpy(AmByteArray dstBuf, AmByteArray srcBuf, int size){
	fixed(byte *dst = &dstBuf.data[dstBuf.position])
	{
	    fixed(byte *src = &srcBuf.data[srcBuf.position])
	    {		
		for (int idx = 0; idx < size; ++idx){
		    dst[idx] = src[idx];
		}
	    }
	}	
    }

    /// <summary>
    ///   このメソッドはposition, lengthを触らないので、seekの調整は呼びだし元が責任をもって行なう事。
    /// </summary>
    unsafe public static void Memcpy(AmByteArray dstBuf, byte [] srcBuf, int size){
	fixed(byte *dst = &dstBuf.data[dstBuf.position])
	{
	    fixed(byte *src = &srcBuf[0])
	    {		
		for (int idx = 0; idx < size; ++idx){
		    dst[idx] = src[idx];
		}
	    }
	}	
    }
    
    /// <summary>
    ///   このメソッドはposition, lengthを触らないので、seekの調整は呼びだし元が責任をもって行なう事。
    /// </summary>
    unsafe public static void Memcpy(byte [] dstBuf, AmByteArray srcBuf, int size){
	fixed(byte *dst = &dstBuf[0])
	{
	    fixed(byte *src = &srcBuf.data[srcBuf.position])
	    {		
		for (int idx = 0; idx < size; ++idx){
		    dst[idx] = src[idx];
		}
	    }
	}	
    }
    
    /// <summary>
    ///   このメソッドはposition, lengthを触らないので、seekの調整は呼びだし元が責任をもって行なう事。
    /// </summary>
    unsafe public static int Memcmp(AmByteArray s1, AmByteArray s2, int n){
	int ret           = 0;

	fixed(byte *sa = &s1.data[s1.position])
	{
	    fixed(byte *sb = &s2.data[s2.position])
	    {		
		for (int idx = 0; idx < n; ++idx){
		    if(sa[idx] != sb[idx]){ ret = -1; break; }
		}
	    }
	}	
	return ret;
    }

    public AmByteArray(){ 
	m_buf = new byte[DEFAULT_BUF_SIZE];		    
	clear();
    }
    public AmByteArray(int len){
	m_buf = new byte[len];		    
	clear();
    }

    // 強制的にlengthをのばしたい時にも使う（RecvBufferの準備とか
    public void extend(uint size){ 
	if(length >= size){ return; } // 既に規定サイズ以上
	if(size > m_buf.Length){
	    m_buf = null;
	    m_buf = new byte[size];
	}
	length = size;
    }
  
    /// <summary>
    ///   Clearという名の初期化関数
    /// </summary>
    public void clear(){
	position       = 0;
	length         = 0;
	if(m_buf.Length != DEFAULT_BUF_SIZE){ // Default サイズのままならGC発生しないので高速
	    m_buf          = null;
	    m_buf          = new byte[DEFAULT_BUF_SIZE];		    
	}
    }
    
    // @todo sizeを超過したら、倍のバッファを確保してコピー、、を再帰で繰り返す
    // private void ExtendBuffer(){}

    public bool readBoolean(){
	bool ret = false;	
	ret = (m_buf[position] == 1) ? true : false;
	position = position + 1;
	return ret;
    }

    public int readByte(){
	byte ret = 0;
	ret = m_buf[position];
	position = position + 1;
	return ret;
    }

    public int readUnsignedByte(){
	byte ret = 0;
	ret = m_buf[position];
	position = position + 1;
	return ret;
    }

    unsafe public void readBytes(AmByteArray bytes, uint _offset = uint.MaxValue, uint _length = 0){
	int     valueOffset = (_offset != uint.MaxValue) ? (int)_offset : (int)bytes.position;
	int     valueLen    = (_length != 0) ? (int)_length : (int)bytesAvailable;
	byte [] valueBytes  = bytes.data;
	fixed(byte *dst = &valueBytes[valueOffset])
	{
	    fixed(byte *src = &m_buf[position])
	    {		
		for (int idx = 0; idx < valueLen; ++idx){
		    dst[idx] = src[idx];		
		}
		
		bytes.position = bytes.position + (uint)valueLen;
		
		// 読込み元のpositionも変わる
		position = position + (uint)valueLen;
	    }
	}	
    }

    unsafe  public uint readUnsignedInt(){
	uint ret = 0;
	fixed(byte *p = &m_buf[position])
	{
	    uint *src  = (uint*)p;
	    ret       = *src;
	    position  = position + sizeof(uint);
	}
	return ret;
    }

    unsafe  public int readInt(){
	int ret = 0;
	fixed(byte *p = &m_buf[position])
	{
	    int *src  = (int*)p;
	    ret       = *src;
	    position  = position + sizeof(int);
	}
	return ret;
    }
    
    unsafe public short readShort(){
	short ret = 0;
	fixed(byte *p = &m_buf[position])
	{
	    short *src  = (short*)p;
	    ret         = *src;
	    position    = position + sizeof(short);
	}
	return ret;
    }

    unsafe public ushort readUnsignedShort(){
	ushort ret = 0;
	fixed(byte *p = &m_buf[position])
	{
	    ushort *src = (ushort*)p;
	    ret         = *src;
	    position    = position + sizeof(ushort);
	}
	return ret;
    }

    public string readString(uint _length){ 
	string ret;

	ret = System.Text.Encoding.ASCII.GetString(m_buf, (int)position, (int)_length);
	position = position + _length;
	
	return ret; 
    }

    // @todo 
    // public string readUTFBytes(uint length){ return ""; }

    public void writeBoolean(bool value){
	m_buf[position] = (value == true) ? (byte)1 : (byte)0;
	position = position + 1; 
    }

    public void writeByte(int value){
	m_buf[position] = (byte)value;
	position = position + 1;
    }

    unsafe public void writeInt(int value){
	fixed(byte *p = &m_buf[position])
	{
	    int *dst  = (int*)p;
	    *dst      = value;
	    position  = position + sizeof(int);
	}
    }

    unsafe public void writeUnsignedInt(uint value){
	fixed(byte *p = &m_buf[position])
	{
	    uint *dst  = (uint*)p;
	    *dst      = value;
	    position  = position + sizeof(uint);
	}
    }

    unsafe public void writeShort(short value){
	fixed(byte *p = &m_buf[position])
	{
	    short *dst  = (short*)p;
	    *dst        = (short)value;
	    position    = position + sizeof(short);
	}
    }

    unsafe public void writeUnsignedShort(ushort value){
	fixed(byte *p = &m_buf[position])
	{
	    ushort *dst  = (ushort*)p;
	    *dst         = (ushort)value;
	    position     = position + sizeof(ushort);
	}
    }
    
    unsafe public void writeString(string value){
	int     valueLen   = value.Length;
	byte [] valueBytes = System.Text.Encoding.ASCII.GetBytes(value);
	fixed(byte *dst = &m_buf[position])
	{
	    fixed(byte *src = &valueBytes[0])
	    {		
		for (int idx = 0; idx < valueLen; ++idx){
		    dst[idx] = src[idx];		
		}
		position = position + (uint)valueLen;
	    }
	}	
    }

    // @todo
    //public void writeUTFBytes(string value){}

    unsafe public void writeBytes(AmByteArray bytes, uint _offset = uint.MaxValue, uint _length = 0){
	int     valueOffset = (_offset != uint.MaxValue) ? (int)_offset : (int)bytes.position;
	int     valueLen    = (_length != 0) ? (int)_length : (int)(bytes.length - valueOffset);
	byte [] valueBytes  = bytes.data;
	fixed(byte *dst = &m_buf[position])
	{
	    fixed(byte *src = &valueBytes[valueOffset])
	    {		
		for (int idx = 0; idx < valueLen; ++idx){
		    dst[idx] = src[idx];		
		}

		position = position + (uint)valueLen;

		// 注意：WriteのSoruceとなった方のByteArrayのPositionは変わらない（readと微妙に違うので注意）
	    }
	}
    }

    public byte [] data { get { return m_buf; } }

}
}

