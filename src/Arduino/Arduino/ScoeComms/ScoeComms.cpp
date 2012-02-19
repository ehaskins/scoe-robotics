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
	//commSerial = &Serial;
	init(&Serial);
}
void ScoeComms::init(Stream *stream) {
	commStream = stream;
	robotModel.init();
	lastDataReceived = 0;
	receiveBufferPosition = 0;
	isWaiting = true;
	lastByte = 0;
}
void ScoeComms::poll() {
	unsigned long now = millis();
	//Handle rollover, shouldn't happen, but...
	if (now < lastDataReceived) {
		lastDataReceived = 0;
	}
	if (checkSerial()) {
		lastDataReceived = now;
		robotModel.update(receiveBuffer, 6, packetDataLength);
		sendStatus();
	}
	unsigned long safeTime = lastDataReceived + RECEIVE_SAFTEY_DELAY;
	bool tripped = now > safeTime;
	robotModel.loop(tripped);
}

void ScoeComms::sendStatus() {
	unsigned int offset = 0;
	robotModel.getStatus(transmitBuffer, &offset);

	unsigned long packetCrc = crc(transmitBuffer, offset);
	unsigned char headerBytes[8] = { SPC_COMMAND, CMD_NEWPACKET, 0, 0, 0, 0, 0,
			0 };
	writeUInt32(headerBytes, packetCrc, 2);
	writeUInt16(headerBytes, offset, 6);

	for (int i = 0; i < 8; i++) {
		commStream->write(headerBytes[i]);
	}

	for (unsigned int i = 0; i < offset; i++) {
		unsigned char byte = transmitBuffer[i];

		if (byte > 254) {
			commStream->write(SPC_ESCAPE);
		}
		commStream->write(byte);
	}
}

bool ScoeComms::checkSerial() {
	while (commStream->available() > 0) {
		unsigned char byte = commStream->read();

		if (!isWaiting) {
			if (byte == SPC_ESCAPE && lastByte != SPC_ESCAPE) {
				//Wait until next loop
			} else if (receiveBufferPosition < RECEIVE_BUFFER_SIZE - 6) {
				receiveBuffer[receiveBufferPosition++] = byte;
				if (receiveBufferPosition == 4) {
					int position = 0;
					packetCrc = readUInt32(receiveBuffer, &position);
				} else if (receiveBufferPosition == 6) {
					int position = 4;
					packetDataLength = readUInt16(receiveBuffer, &position);
					if (packetDataLength > (RECEIVE_BUFFER_SIZE - 6)) {
						isWaiting = true;
					}
				} else if (receiveBufferPosition == packetDataLength + 6) {
					isWaiting = true;

					unsigned char *bptr = receiveBuffer;
					unsigned long calculatedCrc = crc(bptr + 6,
							packetDataLength);
					if (calculatedCrc == packetCrc) {
						return true;
					}
				}
			}
		}
		if (byte == CMD_NEWPACKET && lastByte == SPC_COMMAND) {
			isWaiting = false;
			receiveBufferPosition = 0;
		}

		lastByte = byte;
	}
	return false;
}
