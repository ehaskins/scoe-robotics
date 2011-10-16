/*
 * StreamReader.cpp
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#include "ByteReader.h"
#include "../WProgram.h"
/*
void ReadBytes(unsigned char data[], int *offset, int count, unsigned char out[]) {
	for (int i = 0; i < count; i++){
		out[i] = data[*offset];
		*offset++;
	}
	Serial.print("ReadBytes: Offset:");
	Serial.println(*offset);
}*/

unsigned char readUInt8(unsigned char data[], int *offset){
	unsigned char out = data[*offset];
	*offset += 1;
	return out;
}
unsigned short readUInt16(unsigned char data[], int *offset){
	unsigned short out = (data[*offset] << 8) + (data[*offset + 1]);
	/*unsigned char byteCount = 2;
	for (int i = 0; i < byteCount; i++){
		out += data[*offset + byteCount - i] << (8 * i);
	}*/
	*offset += 2;
	return out;
}
unsigned long readUInt32(unsigned char data[], int *offset){
	unsigned long out =
			((unsigned long)data[*offset] << 24) +
			((unsigned long)data[*offset + 1] << 16) +
			((unsigned long)data[*offset + 2] << 8) +
			((unsigned long)data[*offset + 3]);
	/*unsigned char byteCount = 2;
	for (int i = 0; i < byteCount; i++){
		out += data[*offset + byteCount - i] << (8 * i);
	}*/
	*offset += 4;
	return out;
}
unsigned long long readUInt64(unsigned char data[], int *offset){
	unsigned long long out =
			((unsigned long long)data[*offset] << 56) +
			((unsigned long long)data[*offset + 1] << 48) +
			((unsigned long long)data[*offset + 2] << 40) +
			((unsigned long long)data[*offset + 3] << 32) +
			((unsigned long long)data[*offset + 4] << 24) +
			((unsigned long long)data[*offset + 5] << 16) +
			((unsigned long long)data[*offset + 6] << 8) +
			((unsigned long long)data[*offset + 7]);
	/*unsigned char byteCount = 2;
	for (int i = 0; i < byteCount; i++){
		out += data[*offset + byteCount - i] << (8 * i);
	}*/
	*offset += 8;
	return out;
}
void readBytes(unsigned char data[], unsigned char out[], int count, int *offset){
	for (int i = 0; i < count; i++){
		out[i] = data[i + *offset];
	}
	*offset += count;
}
