using System;
using System.Net;

namespace am
{

public static class AmNetUtil
{

    /*=========================================================================================*/
 
    // 低精度のタイムスタンプ用
    public static uint timestamp {
	get {
	    return (uint)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
	}
    }

    /*=========================================================================================*/

    // @todo unsafe使ったバイト操作関数に置き換える。
    public static short htons(short src){ return IPAddress.HostToNetworkOrder(src); }
    public static int   htonl(int src)  { return IPAddress.HostToNetworkOrder(src); }
    public static long  htonll(long src){ return IPAddress.HostToNetworkOrder(src); }
    public static short ntohs(short src){ return IPAddress.NetworkToHostOrder(src); }
    public static int   ntohl(int src)  { return IPAddress.NetworkToHostOrder(src); }
    public static long  ntohll(long src){ return IPAddress.NetworkToHostOrder(src); }
    public static ushort htons(ushort src){ return (ushort)IPAddress.HostToNetworkOrder((short)src); }
    public static uint   htonl(uint src)  { return (uint)IPAddress.HostToNetworkOrder((int)src); }
    public static ulong  htonll(ulong src){ return (ulong)IPAddress.HostToNetworkOrder((long)src); }
    public static ushort ntohs(ushort src){ return (ushort)IPAddress.NetworkToHostOrder((short)src); }
    public static uint   ntohl(uint src)  { return (uint)IPAddress.NetworkToHostOrder((int)src); }
    public static ulong  ntohll(ulong src){ return (ulong)IPAddress.NetworkToHostOrder((long)src); }

    // 引数は、ネットワークバイトオーダー！！
    public static string ConvIpD2S(int nip){
	int    ip  = ntohl(nip);
	string ret = "";
	ret  =       ((ip >> 24) & 0x000000FF).ToString();
	ret += "." + ((ip >> 16) & 0x000000FF).ToString();
	ret += "." + ((ip >> 8) & 0x000000FF).ToString();
	ret += "." + ((ip >> 0) & 0x000000FF).ToString();
	return ret;
    }

    /*=========================================================================================*/

    delegate void ConvB2Hex_t(ref string dst, int b);

    // 省略したら、配列の長さをそのまま渡す
    public static string HexDump(AmByteArray src, int length){ return HexDump(src.data, length); }
    public static string HexDump(byte[] src){ return HexDump(src, src.Length); }
    public static string HexDump(byte[] src, int length){
	string ret = "";

	ConvB2Hex_t f = (ref string dst, int b) => {  
	    switch(b){
		case 0:  dst += "0"; break;
		case 1:  dst += "1"; break;
		case 2:  dst += "2"; break;
		case 3:  dst += "3"; break;
		case 4:  dst += "4"; break;
		case 5:  dst += "5"; break;
		case 6:  dst += "6"; break;
		case 7:  dst += "7"; break;
		case 8:  dst += "8"; break;
		case 9:  dst += "9"; break;
		case 10: dst += "A"; break;
		case 11: dst += "B"; break;
		case 12: dst += "C"; break;
		case 13: dst += "D"; break;
		case 14: dst += "E"; break;
		case 15: dst += "F"; break;
	    }
	};

	for(int idx = 0; idx < length; ++idx){
	    f(ref ret, (src[idx] >> 4) & 0x0F);
	    f(ref ret, src[idx]  & 0x0F);

	    if((idx % 4) == 3){ ret += " "; }
	    if((idx % 20) == 19){ if((idx+1) != length){ ret += "\n"; }}
	}

	return ret;
    }
  
    /*=========================================================================================*/
  
}
}

