/*
 * ScoeComms.cpp
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */
#include <Arduino.h>
#include <ScoeComms\ScoeComms.h>
#include <ScoeComms\CRC32.h>
#include <ScoeComms\ByteReader.h>
#include <ScoeComms\ByteWriter.h>
ScoeComms::ScoeComms() {
	init(&Serial);
}
void ScoeComms::init(Stream *stream) {
	robotModel.init();
	lastDataReceived = 0;
	receiveBufferPosition = 0;
	isReceiving = false;
	lastByte = 0;
}
void ScoeComms::poll() {
	unsigned long now = millis();
	//Handle overflow, shouldn't happen, but...
	if (now < lastDataReceived) {
		lastDataReceived = 0;
	}
	if (checkSerial()) {
		parsePacket();
		lastDataReceived = now;
		sendStatus();
	}
	unsigned long safeTime = lastDataReceived + RECEIVE_SAFTEY_DELAY;
	bool tripped = now > safeTime;
	robotModel.loop(tripped);
}

void ScoeComms::sendStatus() {
	unsigned int transmitOffset = 0;
	transmitBuffer[transmitOffset++] = lastPacketType == PT_COMMAND ? PT_STATUS : PT_ECHO;
	if (lastPacketType == PT_COMMAND)
		robotModel.getStatus(transmitBuffer, &transmitOffset);

	unsigned long packetCrc = crc(transmitBuffer, transmitOffset);

	//2 total length, 4 crc, 1 version,
	unsigned char headerBytes[11];
	int headerOffset = 0;
	headerOffset = writeUInt16(headerBytes, transmitOffset + 9, headerOffset);
	headerOffset = writeUInt32(headerBytes, packetCrc, headerOffset);
	headerBytes[headerOffset++] = 3; //version
	headerOffset = writeUInt16(headerBytes, lastPacketIndex, headerOffset);
	headerOffset = writeUInt16(headerBytes, transmitOffset, headerOffset);

	commStream->write(SPC_COMMAND);
	commStream->write(CMD_NEWPACKET);
	for (int i = 0; i < 11; i++) {
		writeByte(headerBytes[i]);
	}

	for (unsigned int i = 0; i < transmitOffset; i++) {
		writeByte(transmitBuffer[i]);
	}
}

void ScoeComms::writeByte(uint8_t byte){
	if (byte >= 254) {
		commStream->write(SPC_ESCAPE);
	}
	commStream->write(byte);
}

bool ScoeComms::checkSerial() {
	/*while (commStream->available() > 0) {
		unsigned char byte = commStream->read();

		if (!isWaiting) {
			if (byte == SPC_ESCAPE && lastByte != SPC_ESCAPE) {
				//Wait until next loop
			} else if (sizeBufferPosition < 2) {
				sizeBuffer[sizeBufferPosition++] = byte;
				if (sizeBufferPosition == 2){
					int offset = 0;
					packetLength = readUInt16(sizeBuffer, &offset);
				}
			} else if (receiveBufferPosition < packetLength) {
				receiveBuffer[receiveBufferPosition++] = byte;
				if (receiveBufferPosition == packetLength) {
					isWaiting = true;
					return true;
				}
			}
		}
		if (byte == CMD_NEWPACKET && lastByte == SPC_COMMAND) {
			isWaiting = false;
			receiveBufferPosition = 0;
			sizeBufferPosition = 0;
		}

		lastByte = byte;
	}*/

	while (commStream->available() > 0){
		uint8_t thisByte = commStream->read();
		if (isReceiving){
			if (thisByte >= 254 && lastByte != SPC_ESCAPE){}
			else if (sizeBufferPosition < 2){
				sizeBuffer[sizeBufferPosition++] = thisByte;
				if (sizeBufferPosition == 2){
					packetLength = readUInt16(sizeBuffer, 0);
					if (packetLength > RECEIVE_BUFFER_SIZE)
						isReceiving = false;
				}
			}
			else if (receiveBufferPosition < packetLength){
				receiveBuffer[receiveBufferPosition++] = thisByte;
				if (receiveBufferPosition == packetLength){
					return true;
				}
			}
		}

		if (lastByte == SPC_COMMAND && thisByte == CMD_NEWPACKET){
			isReceiving = true;
			receiveBufferPosition = 0;
			sizeBufferPosition = 0;
		}

			lastByte = thisByte;
	}
	return false;
}

void ScoeComms::parsePacket() {
	int offset = 0;
	uint32_t packetCrc = readUInt32(receiveBuffer, offset); offset += 4;
	uint8_t version = receiveBuffer[offset++];
	lastPacketIndex = readUInt16(receiveBuffer, offset); offset += 4;
	uint16_t contentLength = readUInt16(receiveBuffer, offset); offset += 4;
	int contentOffset = offset;

	unsigned char *contentPtr = receiveBuffer;
	contentPtr += contentOffset;

	uint32_t calcCrc = crc(contentPtr, contentLength);


	if (version == 3 && packetCrc == calcCrc)
		parseContent(contentOffset, contentLength);

}

void ScoeComms::parseContent(int contentOffset, uint16_t contentLength){
	int offset = contentOffset;
	lastPacketType = receiveBuffer[offset++];

	if (lastPacketType == PT_COMMAND)
		robotModel.update(receiveBuffer, offset, contentLength);
}
