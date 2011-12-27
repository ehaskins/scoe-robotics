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
#define RECEIVE_BUFFER_SIZE 512
#define TRANSMIT_BUFFER_SIZE 512

//Receive states
#define READ 0
#define COMMAND 1
#define ESCAPE 2
#define WAIT 3
//Special chars
#define SPC_COMMAND 255
#define SPC_ESCAPE 254

//Command chars
#define CMD_NEWPACKET 255

//PacketStatus
#define READY 0
#define READING 1
#define CORRUPT 2

class ScoeComms {
public:
	Stream *commSerial;
	ScoeComms();
	void poll();
	unsigned int corruptPacketCount;
	unsigned int oversizePacketCount;
	unsigned int packetCount;
	RobotModel robotModel;
private:
	unsigned char receiveBuffer[RECEIVE_BUFFER_SIZE];
	unsigned char transmitBuffer[TRANSMIT_BUFFER_SIZE];
	unsigned int receiveBufferPosition;
	unsigned char readState;
	unsigned char packetState;
	unsigned int packetCrc;
	unsigned short packetDataLength;
	bool checkSerial();
	void sendStatus();
};

#endif /* SCOECOMMS_H_ */
