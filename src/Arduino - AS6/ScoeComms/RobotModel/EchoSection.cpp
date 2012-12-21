/*
 * EchoSection.cpp
 *
 *  Created on: Feb 21, 2012
 *      Author: EHaskins
 */

#include "EchoSection.h"

#include "..\Utils\ByteReader.h"
#include "..\Utils\ByteWriter.h"

EchoSection::EchoSection(){
	sectionId = 255;
}
void EchoSection::update(unsigned char data[], unsigned int offset){
	length = readUInt16(data, &offset);
	for(uint16_t i= 0; i < length; i++){
		echoData[i] = data[offset++];
	}
}
void EchoSection::getStatus(unsigned char data[], unsigned int *offset){
	*offset = writeUInt16(data, length, *offset);
	for (uint16_t i = 0; i < length; i++){
		data[(*offset)++] = echoData[i];
	}
}
