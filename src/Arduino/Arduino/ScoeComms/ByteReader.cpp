/*
 * StreamReader.cpp
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#include "ByteReader.h"
#include "../Arduino.h"
unsigned char readUInt8(unsigned char data[], int offset){
	unsigned char out = data[offset];
	offset += 1;
	return out;
}
unsigned short readUInt16(unsigned char data[], int offset){
	unsigned short out = (data[offset + 1] << 8) + (data[offset]);

	return out;
}
unsigned long readUInt32(unsigned char data[], int offset){
	unsigned long out =
			((unsigned long)data[offset + 3] << 24) +
			((unsigned long)data[offset + 2] << 16) +
			((unsigned long)data[offset + 1] << 8) +
			((unsigned long)data[offset]);
	return out;
}
unsigned long long readUInt64(unsigned char data[], int offset){
	unsigned long long out =
			((unsigned long long)data[offset + 7] << 56) +
			((unsigned long long)data[offset + 6] << 48) +
			((unsigned long long)data[offset + 5] << 40) +
			((unsigned long long)data[offset + 4] << 32) +
			((unsigned long long)data[offset + 3] << 24) +
			((unsigned long long)data[offset + 2] << 16) +
			((unsigned long long)data[offset + 1] << 8) +
			((unsigned long long)data[offset + 0]);

	return out;
}
void readBytes(unsigned char data[], unsigned char out[], int count, int offset){
	for (int i = 0; i < count; i++){
		out[i] = data[i + offset];
	}
}
