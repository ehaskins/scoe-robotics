/*
 * Configuration.cpp
 *
 *  Created on: Feb 18, 2011
 *      Author: EHaskins
 */

#include "Configuration.h"
#include "ByteWriter.h"
#include "ByteReader.h"
#include "../EEPROM.h"

Configuration *Configuration::instance;
Configuration * Configuration::getInstance(){
	if (!instance)
		instance = new Configuration();
	return instance;
}

void Configuration::init(void) {
	offset = 3;
	if (getFormatVersion() == CONFIG_CURRENT_FORMAT_VERSION) {
		loadData(getCurrentSet());
	} else {
		initStorage();
		loadData(getCurrentSet());
	}
	Serial.print("Device ID: 0x");
	Serial.println(getDeviceId(), 16);
}
void Configuration::netInit(void) {
	for (int i = 0; i < 4; i++)
		broadcastIp[i] = 0xff;
	Serial.print("Configuration socket #");
	Serial.println(socket.begin(1000));
}
void Configuration::initStorage() {
	Serial.print("EEPROM Initializing...");
	randomSeed(analogRead(0));
	setFormatVersion(CONFIG_CURRENT_FORMAT_VERSION);
	setDeviceId((unsigned short)random(0xFFFF));
	setSetCount(1);

	mac[0] = 0x90;
	mac[1] = 0xA2;
	mac[2] = 0xDA;
	mac[3] = 0x00;
	mac[4] = 0x30;
	mac[5] = 0x98;

	network = 172;
	robotHostIp = 198;

	subnetMask[0] = 255;
	subnetMask[1] = 255;
	subnetMask[2] = 255;
	subnetMask[3] = 0;

	gatewayIp[0] = 0;
	gatewayIp[1] = 0;
	gatewayIp[2] = 0;
	gatewayIp[3] = 0;

	teamNumber = 1692;

	statusTransmitPort = 1110;
	controlReceivePort = 1140;

	writeData(0);

	Serial.println(" COMPLETE!");
}
void Configuration::writeData() {
	writeData(getCurrentSet());
}
void Configuration::writeData(unsigned char set) {
	unsigned char data[CONFIG_DATA_SIZE];
	int dataOffset = 0;
	dataOffset = writeUInt16(data, teamNumber, dataOffset);
	dataOffset = writeBytes(data, dataOffset, mac, 6, 0);
	data[dataOffset] = network;
	dataOffset++;
	data[dataOffset] = robotHostIp;
	dataOffset++;
	dataOffset = writeBytes(data, dataOffset, gatewayIp, 4, 0);
	dataOffset = writeBytes(data, dataOffset, subnetMask, 4, 0);
	dataOffset = writeUInt16(data, statusTransmitPort, dataOffset);
	dataOffset = writeUInt16(data, controlReceivePort, dataOffset);

	writeData(data, set);
}
void Configuration::writeData(unsigned char data[], unsigned char set) {
	int offset = getSetOffset(set);

	for (unsigned int i = 0; i < CONFIG_DATA_SIZE; i++) {
		EEPROM.write(i + offset, data[i]);
	}
}
void Configuration::loadData(unsigned char set) {
	int offset = getSetOffset(set);
	byte data[CONFIG_DATA_SIZE];
	for (unsigned int i = 0; i < CONFIG_DATA_SIZE; i++) {
		data[i] = EEPROM.read(i + offset);
	}
	loadData(data);
}
void Configuration::loadData(unsigned char data[]) {
	int dataOffset = 0;

	teamNumber = readUInt16(data, &dataOffset);
	readBytes(data, mac, 6, &dataOffset);
	network = data[dataOffset];
	dataOffset++;
	robotHostIp = data[dataOffset];
	dataOffset++;
	readBytes(data, subnetMask, 4, &dataOffset);
	readBytes(data, gatewayIp, 4, &dataOffset);
	statusTransmitPort = readUInt16(data, &dataOffset);
	controlReceivePort = readUInt16(data, &dataOffset);

	robotIp[0] = network;
	robotIp[1] = teamNumber / 100;
	robotIp[2] = teamNumber % 100;
	robotIp[3] = robotHostIp;
}

void Configuration::poll(void) {
	int requestLength = socket.available();
	requestLength -= 8;
	if (requestLength >= CONFIG_PACKET_HEADER_LENGTH) {
		unsigned char requestData[CONFIG_MAX_RX];
		int requestOffset = 0;
		socket.readPacket((char *) requestData, requestLength, remoteIp,
				remotePort);

		unsigned char command = requestData[requestOffset];
		requestOffset++;
		unsigned short deviceId = readUInt16(requestData, &requestOffset);
		switch (command) {
		case 0x01://Discovery
			if (deviceId == 0 || deviceId == getDeviceId()) {
				Serial.println("Configuration discovery packet received.");

				unsigned char response[] = { 0, 0, CONFIG_RESPONSE_MAC, mac[0], mac[1],
						mac[2], mac[3], mac[4],
						mac[5] };
				writeUInt16(response, getDeviceId(), 0);
				socket.sendPacket(response, 9, broadcastIp, 1001);
			}
			break;
		case 0x02: //Read data at slot x
			if (deviceId == 0 || deviceId == getDeviceId()) {
				unsigned char set = requestData[requestOffset];
				requestOffset++;
				Serial.println("Configuration read request received.");
				int configDataLength = CONFIG_DATA_SIZE;
				unsigned char responseData[CONFIG_DATA_SIZE + 3];

				writeUInt16(responseData, getDeviceId(), 0);
				responseData[2] = CONFIG_RESPONSE_DATA;
				responseData[3] = set;
				int offset = getSetOffset(set);
				for (int i = 0; i < configDataLength; i++) {
					responseData[i + 3] = EEPROM.read(i + offset);
				}

				socket.sendPacket(responseData, configDataLength + 3,
						broadcastIp, 1001);
			}
			break;
		case 0x03: //Write data to slot x
			if (deviceId == getDeviceId()) {
				unsigned char set = requestData[requestOffset];
				requestOffset++;
				Serial.println("Configuration write request received.");
				if (requestLength - requestOffset >= (int)CONFIG_DATA_SIZE) {
					Serial.print("Updating configuration...");
					unsigned char data[CONFIG_DATA_SIZE];
					readBytes(requestData, data, CONFIG_DATA_SIZE, &requestOffset);
					loadData(data);
					writeData(set);
					sendResponse(true);
				} else {
					sendResponse(false);
				}
			}
			else {
				Serial.print("Write request received for device ");
				Serial.println(deviceId);
			}
			break;
		case 0x04: //Reset EEPROM
			if (deviceId == getDeviceId()) {
				Serial.println("Configuration reset command received.");
				setFormatVersion(0);
				Serial.print("Reboot Arduino to complete.");
			}
			break;
		}

	}
}
void Configuration::sendResponse(bool success) {
	unsigned char responseData[3] = { 0, 0, success ? CONFIG_RESPONSE_ACK : CONFIG_RESPONSE_NCK };
	writeUInt16(responseData, getDeviceId(), 0);
	socket.sendPacket(responseData, 3, broadcastIp, 1001);
}
unsigned char Configuration::getFormatVersion() {
	return EEPROM.read(CONFIG_FORMAT_VERSION_OFFSET);
}
void Configuration::setFormatVersion(unsigned char value) {
	EEPROM.write(CONFIG_FORMAT_VERSION_OFFSET, value);
}
unsigned char Configuration::getSetCount() {
	return EEPROM.read(CONFIG_NUM_SETS_OFFSET);
}
void Configuration::setSetCount(unsigned char value) {
	EEPROM.write(CONFIG_NUM_SETS_OFFSET, value);
}
unsigned int Configuration::getDeviceId() {
	byte deviceIdBytes[2];
	for (int i = 0; i < 2; i++) {
		deviceIdBytes[i] = EEPROM.read(i + CONFIG_DEVICE_ID_OFFSET);
	}
	int temp = 0;
	return readUInt16(deviceIdBytes, &temp);
}
void Configuration::setDeviceId(unsigned int value) {
	byte deviceIdBytes[2];
	writeUInt16(deviceIdBytes, value, 0);
	for (int i = 0; i < 2; i++) {
		EEPROM.write(i + CONFIG_DEVICE_ID_OFFSET, deviceIdBytes[i]);
	}
}
unsigned int Configuration::getSetOffset(unsigned char set) {
	return CONFIG_SETS_OFFSET + CONFIG_DATA_SIZE * set;
}

unsigned char Configuration::getCurrentSet() {
	return 0;
}
