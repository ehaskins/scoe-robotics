/*
 * ByteWriter.cpp
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#include "ByteWriter.h"
int writeByte(uint8_t data[], uint8_t val, uint16_t offset){
	data[offset] = val;
	return offset + 1;
}
int writeUInt16(uint8_t data[], uint16_t val, uint16_t offset){

	data[offset + 1] = (uint8_t)(val >> 8);
	data[offset + 0] = (uint8_t)val;
	return offset + 2;
}
int writeUInt32(uint8_t data[], uint32_t val, uint16_t offset){
	data[offset + 3] = (uint8_t)(val >> 24);
	data[offset + 2] = (uint8_t)(val >> 16);
	data[offset + 1] = (uint8_t)(val >> 8);
	data[offset + 0] = (uint8_t)val;
	return offset + 4;
}

int writeUInt32ForCrc(uint8_t data[], uint32_t val, uint16_t offset){
	data[offset + 3] = (uint8_t)(val >> 24);
	data[offset + 2] = (uint8_t)(val >> 16);
	data[offset + 1] = (uint8_t)(val >> 8);
	data[offset] = (uint8_t)val;
	return offset + 4;
}

int writeBytes(uint8_t data[], uint16_t dataOffset, uint8_t val[], uint16_t count, uint16_t valOffset){
	for (uint16_t i = 0; i < count; i++){
		data[i + dataOffset] = val[i + valOffset];
	}
	return dataOffset + count;
}
