/*
 * ScoeComms.cpp
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#include "ScoeComms.h"
#include <FRCComms\CRC32.h>
#include <FRCComms\ByteReader.h>
#include <FRCComms\ByteWriter.h>
ScoeComms::ScoeComms() {
	// TODO Auto-generated constructor stub

}

void ScoeComms::poll(){
	if (checkSerial()){
		robotModel.update(receiveBuffer, 6, packetDataLength);

	}
}

void ScoeComms::sendStatus(){
	unsigned short length;
	robotModel.getStatusData(transmitBuffer, 0, &length);

	unsigned long packetCrc = crc(transmitBuffer, length);
	unsigned char headerBytes[8] = {SPC_COMMAND, CMD_NEWPACKET, 0, 0, 0, 0, 0, 0};
	writeUInt32(headerBytes, crcBytes, 2);
	writeUInt16(headerBytes, length, 6);

	for(int i = 0; i < 6; i++){
		commSerial->write(headerBytes[i]);
	}

	for(int i = 0; i < length; i++){
		unsigned char byte = transmitBuffer[i];

		if (byte > 254){
			commSerial->write(CMD_ESCAPE);
		}
		commSerial->write(byte);
	}
}

bool ScoeComms::checkSerial(){
	while (commSerial->available() > 0){
		unsigned char byte = commSerial->read();
		switch (readState){
		case COMMAND:
			switch (byte){
			case CMD_NEWPACKET:
				readState = READ;
				receiveBufferPosition = 0;
				break;
			default:
				readState = WAIT;
				this->packetState = CORRUPT;
				corruptPacketCount++;
				break;
			}
			break;
		case ESCAPE:
			if (receiveBufferPosition <= RECEIVE_BUFFER_SIZE)
				receiveBuffer[receiveBufferPosition++] = byte;
			break;
		case READ:
			if (byte == SPC_COMMAND)
				readState = COMMAND;
			else if (byte == SPC_ESCAPE)
				readState = ESCAPE;
			else if (receiveBufferPosition <= RECEIVE_BUFFER_SIZE)
				receiveBuffer[receiveBufferPosition++] = byte;
			break;
		case WAIT:
			if (byte == SPC_COMMAND)
				readState = COMMAND;
			break;
		}

		if (receiveBufferPosition == 4){
			int position = 0;
			packetCrc = readUInt32(receiveBuffer, &position);
		}
		else if (receiveBufferPosition == 6){
			int position = 4;
			packetDataLength = readUInt16(receiveBuffer, &position);
			if (packetDataLength > RECEIVE_BUFFER_SIZE - 6){
				readState = WAIT;
				oversizePacketCount++;
			}
		}
		else if (receiveBufferPosition == packetDataLength + 6){
			unsigned char *bptr = receiveBuffer;
			unsigned long calculatedCrc = crc(bptr + 6, packetDataLength);
			if (calculatedCrc == packetCrc){
				packetCount++;
				return true;
			}
		}
	}
	return false;
}
