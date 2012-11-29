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

#define PACKET_VERSION 4
#define HEADER_LENGTH 10
#define CRC_POS 0
#define TYPE_POS 5
#define INDEX_POS 6
#define CONTENT_LENGTH_POS 8
#define CONTENT_POS 10
#define VERSION_POS 4


//Time in ms before output is disabled due to no communication.
#define RECEIVE_SAFTEY_DELAY 1900 //TODO: Set back to reasonable level

enum SerialSpecialChar {CommandBegin = 255, EscapeNext = 254};
enum ReceiveState {ReadData = 0, ProcessCommand = 1, Escape = 2, WaitForPacket = 3};

enum SerialCommandChar {NewPacket = 255};

class SerialInterface {
	public:
	SerialInterface();
	void init(Stream * commStream);
	void poll();
	RobotModel robotModel;
	private:
	Stream *commStream;
	unsigned char receiveBuffer[RECEIVE_BUFFER_SIZE];
	unsigned char transmitBuffer[TRANSMIT_BUFFER_SIZE];
	byte sizeBuffer[2];
	int sizeBufferPosition;
	unsigned int receiveBufferPosition;
	unsigned char lastByte;
	bool isWaiting;
	bool isConnected;
	unsigned long packetCrc;
	unsigned int packetDataLength;
	unsigned int packetSize;
	
	unsigned int packetIndex;

	unsigned long lastDataReceived;
	bool checkSerial();
	void packageStatus();
	void sendStatus();
	bool processCommand();
};

enum PacketType {Probe = 0, Echo = 1, Command = 2, Status = 3};

#endif /* SCOECOMMS_H_ */
