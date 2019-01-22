using System;   
using UnityEngine;   

namespace am
{

public static class AmByteExtention
{
    
    unsafe public static void Memcpy(this byte[] dstBuf, byte[] srcBuf, int bufSize){
	int size = bufSize;
	fixed(byte *dst = &dstBuf[0])
	{
	    fixed(byte *src = &srcBuf[0])
	    {
		for (int idx = 0; idx < size; ++idx){
		    dst[idx] = src[idx];
		}
	    }
	}
    }
    
}
}

/*
 * Local variables:
 * compile-command: "make -C../"
 * End:
 */
