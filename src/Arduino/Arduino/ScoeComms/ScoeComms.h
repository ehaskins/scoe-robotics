/*
 * ScoeComms.h
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#ifndef SCOECOMMS_H_
#define SCOECOMMS_H_
#include <Stream.h>
#include "RobotModel.h"
#define RECEIVE_BUFFER_SIZE 200
#define TRANSMIT_BUFFER_SIZE 200

//Time in ms before output is disabled due to no communication.
#define RECEIVE_SAFTEY_DELAY 500 //TODO: Set back to reasonable level

//Receive states
#define READ 0
#define COMMAND 1
#define ESCAPE 2
#define WAIT 3

//Packet Types
#define PT_PROBE 0
#define PT_ECHO 1
#define PT_COMMAND 2
#define PT_STATUS 3
//Special chars
#define SPC_COMMAND 255
#define SPC_ESCAPE 254

//Command chars
#define CMD_NEWPACKET 255

class ScoeComms {
public:
	ScoeComms();
	void init(Stream * commStream);
	void poll();
	RobotModel robotModel;
private:
	Stream *commStream;
	uint8_t receiveBuffer[RECEIVE_BUFFER_SIZE];
	uint8_t sizeBuffer[2];
	uint8_t sizeBufferPosition;
	uint8_t lastPacketType;
	uint16_t lastPacketIndex;
	unsigned char transmitBuffer[TRANSMIT_BUFFER_SIZE];
	unsigned int receiveBufferPosition;
	unsigned char lastByte;
	bool isReceiving;
	unsigned long packetCrc;
	unsigned short packetLength;

	unsigned long lastDataReceived;
	bool checkSerial();
	void sendStatus();
	void parsePacket();
	void parseContent(int contentOffset, uint16_t contentLength);
	void writeByte(uint8_t byte);
};

#endif /* SCOECOMMS_H_ */
