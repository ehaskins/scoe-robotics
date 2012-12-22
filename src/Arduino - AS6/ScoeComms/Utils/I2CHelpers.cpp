/*
 * I2CHelpers.cpp
 *
 * Created: 12/21/2012 7:54:28 PM
 *  Author: EHaskins
 */ 

#include <Wire\Wire.h>
#include "I2CHelpers.h"

void readI2CBytes(uint8_t device, uint8_t address, uint8_t *buffer, int offset, uint8_t count){
	Wire.beginTransmission(device); //start transmission to ACC
	Wire.write(address);        //sends address to read from
	Wire.endTransmission(); //end transmission
	
	Wire.beginTransmission(device); //start transmission to ACC
	Wire.requestFrom(device, count);    // request 6 bytes from ACC
	
	while(Wire.available())    //ACC may send less than requested (abnormal)
	{
		buffer[offset++] = Wire.read(); // receive a byte
	}
	Wire.endTransmission(); //end transmission
}

uint8_t writeI2CByte(uint8_t device, uint8_t address, uint8_t val){
	Wire.beginTransmission(device); //start transmission to ACC
	Wire.write(address);        // send register address
	Wire.write(val);        // send value to write
	Wire.endTransmission(); //end transmission
}