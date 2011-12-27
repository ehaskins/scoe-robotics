/*
 * CommandData.cpp
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#include "ControlData.h"
#include "ByteReader.h"
#include "CRC32.h"
	/*CommandData::CommandData(){
		for (int i = 0; i < NUM_JOYSTICKS; i++){
			Joystick *temp = new Joystick();
			Joysticks[i] = temp;
		}
	}*/

	void ControlData::parse(unsigned char data[]){
		int crcOffset = CONTROL_PACKET_SIZE - 4;
		int temp = crcOffset;
		unsigned long dataCrc = readUInt32(data, &temp);
		for (int i = crcOffset; i < crcOffset + 4; i++){
			data[i] = 0;
		}
		unsigned long calculatedCrc = crc(data, CONTROL_PACKET_SIZE);

		isValid = calculatedCrc == dataCrc;

		if (isValid)
		{
			int offset = 0;
			packetId = readUInt16(data, &offset);
			mode.data.data = readUInt8(data, &offset);
			dsInputs = readUInt8(data, &offset);
			teamNumber = readUInt16(data, &offset);
			alliance = readUInt8(data, &offset);
			position = readUInt8(data, &offset);
			//TODO:Read joysticks.
			for (int i = 0; i < 4; i++){
				joysticks[i].parse(data, &offset);
			}

			cRioChecksum = readUInt64(data, &offset);
			fpgaChecksum0 = readUInt32(data, &offset);
			fpgaChecksum1 = readUInt32(data, &offset);
			fpgaChecksum2 = readUInt32(data, &offset);
			fpgaChecksum3 = readUInt32(data, &offset);

			//TODO:Read version.
		}
	}
