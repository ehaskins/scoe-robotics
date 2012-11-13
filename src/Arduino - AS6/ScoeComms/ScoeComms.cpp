/*
* ScoeComms.cpp
*
*  Created on: Dec 26, 2011
*      Author: EHaskins
*/
#include <Arduino.h>
#include "ScoeComms.h"
#include "CRC32.h"
#include "ByteReader.h"
#include "ByteWriter.h"
SerialInterface::SerialInterface() {
	//commSerial = &Serial;
	init(&Serial);
}
void SerialInterface::init(Stream *stream) {
	commStream = stream;
	robotModel.init();
	lastDataReceived = 0;
	receiveBufferPosition = 0;
	isWaiting = true;
	lastByte = 0;
	packetIndex = 0;
	isConnected = false;
	sizeBufferPosition = 0;
	packetSize = 0;
}
void SerialInterface::poll() {
	unsigned long now = millis();
	//Handle rollover. Vehicle should never be on that long, but...
	if (now < lastDataReceived) {
		lastDataReceived = 0;
	}
	if (checkSerial() && processCommand()) {
		lastDataReceived = now;
		robotModel.update(receiveBuffer, CONTENT_POS, packetDataLength);
		sendStatus();
	}
	unsigned long safeTime = lastDataReceived + RECEIVE_SAFTEY_DELAY;
	isConnected = now > safeTime;
	robotModel.loop(isConnected);
}


void SerialInterface::sendStatus() {
	unsigned int offset = 0;
	
	PacketType type = isConnected ? Echo : Status;
	
	if (type == Status){
		robotModel.getStatus(transmitBuffer, &offset);
	}
	
	unsigned long packetCrc = crc(transmitBuffer, offset);
	
	byte startBytes[4] = {(byte)CommandBegin, (byte)NewPacket, 0, 0};
	writeUInt16(startBytes, HEADER_LENGTH + offset, 2);

	byte headerBytes[HEADER_LENGTH] = {0, 0, 0, 0, PACKET_VERSION, 0, 0, 0, 0, 0 };
	writeUInt32(headerBytes, packetCrc, CRC_POS);
	headerBytes[TYPE_POS] = (byte)type;
			
	writeUInt16(headerBytes, packetIndex, INDEX_POS);
	writeUInt16(headerBytes, offset, CONTENT_LENGTH_POS);


	
	for (int i = 0; i < 4; i++) {
		commStream->write(startBytes[i]);
	}
	for (int i = 0; i < HEADER_LENGTH; i++) {
		unsigned char byte = headerBytes[i];

		if (byte >= 254) {
			commStream->write(EscapeNext);
		}
		commStream->write(byte);
	}

	for (unsigned int i = 0; i < offset; i++) {
		unsigned char byte = transmitBuffer[i];

		if (byte >= 254) {
			commStream->write(EscapeNext);
		}
		commStream->write(byte);
	}
}

bool SerialInterface::processCommand(){
	unsigned int offset = 0;
	unsigned long packetCrc = readUInt32(receiveBuffer, &offset); offset = 4;
	byte version = receiveBuffer[offset++];
	
	if (version == PACKET_VERSION){
		packetIndex = readUInt16(receiveBuffer, &offset);
		PacketType type = (PacketType)receiveBuffer[offset++];
		isConnected = true;
		packetDataLength = readUInt16(receiveBuffer, &offset);
		unsigned char *bptr = receiveBuffer;
		unsigned long calculatedCrc = crc(bptr + CONTENT_POS, packetDataLength);	
		
		bool crcOk = calculatedCrc == packetCrc;

		return crcOk;
	}
	return false;
}

bool SerialInterface::checkSerial() {
	while (commStream->available() > 0) {
		unsigned char thisByte = commStream->read();

		if (!isWaiting) {
			if (thisByte == (byte)CommandBegin && lastByte != (byte)EscapeNext) {
				//Wait until next loop
			}
			else if (sizeBufferPosition < 2)
			{
				sizeBuffer[sizeBufferPosition++] = thisByte;
				if (sizeBufferPosition == 2)
				{
					packetSize = readUInt16(sizeBuffer, 0);
					if (packetSize > RECEIVE_BUFFER_SIZE) //Don't try to process over sized packets;
						isWaiting = true; 
				}					
			} 
			else if (receiveBufferPosition < packetSize && receiveBufferPosition < RECEIVE_BUFFER_SIZE) 
			{
				receiveBuffer[receiveBufferPosition++] = thisByte;
				
				if (receiveBufferPosition == packetSize)
				{
					isWaiting = true;
					return true;
				}
			}
			

		}
		
		if (thisByte == (byte)NewPacket && lastByte == (byte)CommandBegin) {			
			isWaiting = false;
			receiveBufferPosition = 0;
			sizeBufferPosition = 0;
			
		}

		lastByte = thisByte;
	}
	return false;
}
