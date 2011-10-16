/*
 * StatusData.cpp
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#include "StatusData.h"
#include "ByteWriter.h"
#include "Configuration.h"
#include "CRC32.h"
void StatusData::updateResponse(ControlData *data) {
	replyId = data->packetId;
	mode = data->mode;
}
void StatusData::getResponseBytes(unsigned char data[]) {
	Configuration *config = Configuration::getInstance();
	for (int i = 0; i < STATUS_PACKET_SIZE; i++){
		data[i] = 0;
	}
	int offset = 0;

	offset = writeByte(data, mode.data.data, offset);
	offset = writeByte(data, batteryVoltageInt, offset);
	offset = writeByte(data, batteryVoltageFrac, offset);
	offset = writeByte(data, dsOutputs.data, offset);

	for (int i = 0; i < 4; i++) {
		offset = writeByte(data, 0, offset);
	}

	offset = writeUInt16(data, teamNumber, offset);

	for (int i = 0; i < 6; i++) {
		offset = writeByte(data, config->mac[i], offset);
	}

	for (int i = 0; i < 8; i++) {
		offset = writeByte(data, fpgaVersion[i], offset);
	}

	for (int i = 0; i < 6; i++) { //Unknown pad
		offset = writeByte(data, 0, offset);
	}

	offset = writeUInt16(data, replyId, offset);

	unsigned long c = crc(data, STATUS_PACKET_SIZE);
	writeUInt32(data, c, STATUS_PACKET_SIZE - 4);
}

